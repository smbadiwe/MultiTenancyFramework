using System.Web.Routing;

namespace System.Web.Mvc
{
    public static class MvcExtensions2
    {
        /// <summary>
        /// This incorporates the institution code in the generated Url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public static string MyAction(this UrlHelper url, string actionName)
        {
            return url.MyAction(actionName, null, (object)null);
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
            return url.MyAction(actionName, controllerName, (object)null);
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
            return url.MyAction(actionName, controllerName, new { area = areaName });
        }

        /// <summary>
        /// This incorporates the institution code in the generated Url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="routeValues"></param>
        /// <returns></returns>
        public static string MyAction(this UrlHelper url, string actionName, string controllerName, object routeValues)
        {
            RouteValueDictionary routes;
            if (routeValues == null)
            {
                routes = new RouteValueDictionary();
            }
            else
            {
                routes = new RouteValueDictionary(routeValues);
            }
            routes["institution"] = System.Web.HttpContext.Current.Request.RequestContext.RouteData.Values["institution"];

            if (string.IsNullOrWhiteSpace(controllerName))
            {
                return url.Action(actionName, routes);
            }
            return url.Action(actionName, controllerName, routes);
        }

    }
}
