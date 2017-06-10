using MultiTenancyFramework.Mvc;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web.Routing;

namespace System.Web.Mvc
{
    public static class RouteCollectionExtensions
    {
        /// <summary>
        /// If you need your urls to render as lowercase, use this; otherwise, use <code>.MapRoute</code> extension.
        /// </summary>
        /// <param name="routes"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#", Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapRouteLowerCase(this RouteCollection routes, string name, string url)
        {
            return MapRouteLowerCase(routes, name, url, null /* defaults */, (object)null /* constraints */);
        }

        /// <summary>
        /// If you need your urls to render as lowercase, use this; otherwise, use <code>.MapRoute</code> extension.
        /// </summary>
        /// <param name="routes"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="defaults"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#", Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapRouteLowerCase(this RouteCollection routes, string name, string url, object defaults)
        {
            return MapRouteLowerCase(routes, name, url, defaults, (object)null /* constraints */);
        }

        /// <summary>
        /// If you need your urls to render as lowercase, use this; otherwise, use <code>.MapRoute</code> extension.
        /// </summary>
        /// <param name="routes"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="defaults"></param>
        /// <param name="constraints"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#", Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapRouteLowerCase(this RouteCollection routes, string name, string url, object defaults, object constraints)
        {
            return MapRouteLowerCase(routes, name, url, defaults, constraints, null /* namespaces */);
        }

        /// <summary>
        /// If you need your urls to render as lowercase, use this; otherwise, use <code>.MapRoute</code> extension.
        /// </summary>
        /// <param name="routes"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="namespaces"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#", Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapRouteLowerCase(this RouteCollection routes, string name, string url, string[] namespaces)
        {
            return MapRouteLowerCase(routes, name, url, null /* defaults */, null /* constraints */, namespaces);
        }

        /// <summary>
        /// If you need your urls to render as lowercase, use this; otherwise, use <code>.MapRoute</code> extension.
        /// </summary>
        /// <param name="routes"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="defaults"></param>
        /// <param name="namespaces"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#", Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapRouteLowerCase(this RouteCollection routes, string name, string url, object defaults, string[] namespaces)
        {
            return MapRouteLowerCase(routes, name, url, defaults, null /* constraints */, namespaces);
        }
        
        /// <summary>
        /// If you need your urls to render as lowercase, use this; otherwise, use <code>.MapRoute</code> extension.
        /// </summary>
        /// <param name="routes"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="defaults"></param>
        /// <param name="constraints"></param>
        /// <param name="namespaces"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#", Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapRouteLowerCase(this RouteCollection routes, string name, string url, object defaults, object constraints, string[] namespaces)
        {
            if (routes == null)
            {
                throw new ArgumentNullException("routes");
            }
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            LowerCaseRoute route = new LowerCaseRoute(url, new MvcRouteHandler())
            {
                Defaults = CreateRouteValueDictionary(defaults),
                Constraints = CreateRouteValueDictionary(constraints),
                DataTokens = new RouteValueDictionary()
            };

            if ((namespaces != null) && (namespaces.Length > 0))
            {
                route.DataTokens["Namespaces"] = namespaces;
            }

            routes.Add(name, route);

            return route;
        }

        private static RouteValueDictionary CreateRouteValueDictionary(object values)
        {
            var dictionary = values as IDictionary<string, object>;
            if (dictionary != null)
            {
                return new RouteValueDictionary(dictionary);
            }

            return new RouteValueDictionary(values);
        }
        
    }
}
