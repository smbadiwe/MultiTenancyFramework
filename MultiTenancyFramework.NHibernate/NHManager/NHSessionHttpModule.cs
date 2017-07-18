using System;
using System.Collections.Generic;
using System.Web;

namespace MultiTenancyFramework.NHibernate.NHManager
{
    public class NHSessionHttpModule : IHttpModule
    {
        /// <summary>
        /// Constant key for storing the session in the HttpContext
        /// </summary>
        private HttpApplication _context;
        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += Application_BeginRequest;
            context.Error += Application_Error;
            _context = context;
        }
        
        private void Application_BeginRequest(object sender, EventArgs e)
        {
            _context.Context.AddOnRequestCompleted(x =>
            {
                // exclude static resources
                //  The key is that from an MVC point of view, if the Physical Path has an extension, then it's a physical resource
                if (string.IsNullOrWhiteSpace(x.Request.PhysicalPath)) return;
                var ext = System.IO.Path.GetExtension(x.Request.PhysicalPath);
                if (!string.IsNullOrWhiteSpace(ext)) return;

                if (x.Items.Contains(WebSessionStorage.CurrentSessionKey))
                {
                    var storageSet = new Dictionary<string, ISessionStorage>(NHSessionManager.SessionStorages);

                    if (storageSet != null && storageSet.Count > 0)
                    {
                        foreach (var storage in storageSet.Values)
                        {
                            //Closes the session if there's any open session
                            if (storage != null && storage.Session != null)
                            {
                                NHSessionManager.CloseStorage(((WebSessionStorage)storage)?.InstitutionCode);
                            }
                        }
                        storageSet.Clear();
                        x.Items.Remove(WebSessionStorage.CurrentSessionKey);
                    }
                }
            });
        }

        private void Application_Error(object sender, EventArgs e)
        {
            var lastError = _context.Server.GetLastError();
            var logger = Utilities.Logger;
            try
            {
                logger.Log(lastError);

                NHSessionManager.CloseStorage();
                NHSessionManager.SessionFactories.Clear();
            }
            catch (Exception ex)
            {
                logger.Log(ex);
            }
            finally
            {
                if (_context.Context != null && _context.Context.Response != null)
                {
                    var baseUrl = ConfigurationHelper.GetSiteUrl() ?? _context.Context.Request.Url.Authority;

                    var errorUrl = $"{baseUrl}/{_context.Context.Request.RequestContext.RouteData.Values["institution"]}/Error/?gl=1&ErrorMessage={lastError.Message}";
                    logger.Log($"A.._Error (NHSessionHttpModule) called. Redirecting to {errorUrl}");
                    _context.Context.Response.Redirect(errorUrl);
                }
            }
        }

    }
}
