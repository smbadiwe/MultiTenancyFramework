using System.Web.Routing;

namespace MultiTenancyFramework.Mvc
{
    /// <summary>
    /// This converts urls to lowercase. Ideally, the code "routes.LowercaseUrls = true;" should have done it, but for some reason, it causes unexpected behaviours
    /// when a View which has Layout is loaded.
    /// </summary>
    public sealed class LowerCaseRoute : Route
    {
        public LowerCaseRoute(string url, IRouteHandler routeHandler)
            : base(url, routeHandler) { }
        public LowerCaseRoute(string url, RouteValueDictionary defaults, IRouteHandler routeHandler) 
            : base(url, defaults, routeHandler) { }
        public LowerCaseRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler routeHandler) 
            : base(url, defaults, constraints, routeHandler) { }
        public LowerCaseRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler) 
            : base(url, defaults, constraints, dataTokens, routeHandler) { }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            VirtualPathData path = base.GetVirtualPath(requestContext, values);
            if (path != null)
            {
                path.VirtualPath = path.VirtualPath.ToLowerInvariant();
            }
            return path;
        }
    }
}
