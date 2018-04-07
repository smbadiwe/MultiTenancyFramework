using MultiTenancyFramework.NHibernate.NHManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MultiTenancyFramework.Mvc
{
    public class EndRequestFilter : IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var x = filterContext.HttpContext;
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
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
        }
    }
}
