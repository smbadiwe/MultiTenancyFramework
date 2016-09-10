using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
