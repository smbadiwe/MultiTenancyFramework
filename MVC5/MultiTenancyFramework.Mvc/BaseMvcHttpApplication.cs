using System.Text.RegularExpressions;

namespace MultiTenancyFramework.Mvc
{
    public class BaseMvcHttpApplication : System.Web.HttpApplication
    {
        protected virtual void Application_PreSendRequestHeaders()
        {
            Context.Response.Headers.Remove("Server");
            Context.Response.Headers.Remove("X-Powered-By");
            Context.Response.Headers.Remove("X-AspNet-Version");
            Context.Response.Headers.Remove("X-AspNetMvc-Version");
            Context.Response.Headers.Remove("X-Powered-By-Plesk");
        }

        protected virtual void Application_BeginRequest()
        {
            if (Request.HttpMethod == "GET")
            {
                // If upper case letters are found in the URL, redirect to lower case URL (keep querystring the same).
                string requestURL = (Request.Url.Scheme + "://" + Request.Url.Authority + Request.Url.AbsolutePath);
                if (!Regex.IsMatch(requestURL, InstitutionRouteConfig.StaticFileExtensionsRegex) && Regex.IsMatch(requestURL, @"[A-Z]"))
                {
                    requestURL = requestURL.ToLower() + Request.Url.Query;

                    Response.Clear();
                    Response.StatusCode = 301;
                    Response.Status = "301 Moved Permanently";
                    Response.AddHeader("Location", requestURL);
                    Response.End();
                }
            }

            Context.Items.Add(SimpleWebHelper.HttpRequestIsAvailable, true);
        }

    }
}
