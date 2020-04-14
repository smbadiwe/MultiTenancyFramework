using System.Web.Routing;
using System.Linq;

namespace System.Web.Mvc
{
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// This incorporates the institution code in the generated Url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string MyAction(this UrlHelper url)
        {
            return url.MyAction(null, null, null, null);
        }

        /// <summary>
        /// This incorporates the institution code in the generated Url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public static string MyAction(this UrlHelper url, string actionName)
        {
            return url.MyAction(actionName, null, null, null);
        }

        /// <summary>
        /// This incorporates the institution code in the generated Url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <returns></returns>
        public static string MyAction(this UrlHelper url, string actionName, string controllerName)
        {
            return url.MyAction(actionName, controllerName, null, null);
        }

        /// <summary>
        /// This incorporates the institution code in the generated Url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="areaName"></param>
        /// <returns></returns>
        public static string MyAction(this UrlHelper url, string actionName, string controllerName, string areaName)
        {
            return url.MyAction(actionName, controllerName, areaName, null);
        }

        /// <summary>
        /// This incorporates the institution code in the generated Url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="areaName"></param>
        /// <param name="routeValues"></param>
        /// <returns></returns>
        public static string MyAction(this UrlHelper url, string actionName, string controllerName, string areaName, object routeValues)
        {
            RouteValueDictionary routes = new RouteValueDictionary(HttpContext.Current.Request.RequestContext.RouteData.Values);
            if (routeValues != null)
            {
                routes = new RouteValueDictionary(routes.Union(new RouteValueDictionary(routeValues)).ToDictionary(k => k.Key, k => k.Value));
            }
            if (areaName != null) // don't do 'if (!string.IsNullOrWhiteSpace(areaName))'
            {
                routes["area"] = areaName;
            }
            if (!string.IsNullOrWhiteSpace(actionName))
            {
                routes["action"] = actionName;
            }
            if (!string.IsNullOrWhiteSpace(controllerName))
            {
                routes["controller"] = controllerName;
            }

            //TODO: Make this hard-coding go away to allow users not need to follow this convention
            //// Some performance optimization here: .RouteUrl instead of .Action
            //if (!string.IsNullOrWhiteSpace(areaName))
            //{
            //    //return url.RouteUrl(MultiTenancyFramework.Mvc.MvcUtility.GetRouteNameForArea(areaName), routes);

            //    //TODO: Make this hard-coding go away to allow users not need to follow this convention
            //    return url.RouteUrl($"{areaName}_MultiTenant", routes);
            //}
            return url.RouteUrl(routes);
        }

    }
}
