using MultiTenancyFramework.Mvc;
using System;
using System.Security.Principal;
using System.Web;
using System.Web.Routing;

namespace MultiTenancyFramework
{
    public static class IIdentityExtensions
    {
        //private static UserStore<IdentityUser> userStore;
        private static RouteValueDictionary routeData { get { return HttpContext.Current.Request.RequestContext.RouteData.Values; } }
        //static IIdentityExtensions()
        //{
        //    userStore = new UserStore<IdentityUser>();
        //}

        // This is probably an irrelevant optimisation; but then, I detest unnecessary call stacks.
        // So please, leave these 'HasPrivilege' method overloads the way I wrote them
        // - Soma Mbadiwe

        public static bool HasPrivilege(this IPrincipal user)
        {
            return HasPrivilege(user, 
                Convert.ToString(routeData["action"]), 
                Convert.ToString(routeData["controller"]), 
                Convert.ToString(routeData["area"]));
        }

        public static bool HasPrivilege(this IPrincipal user, string action)
        {
            return HasPrivilege(user, action, 
                Convert.ToString(routeData["controller"]), 
                Convert.ToString(routeData["area"]));
        }

        public static bool HasPrivilege(this IPrincipal user, string action, string controller)
        {
            return HasPrivilege(user, action, controller, 
                Convert.ToString(routeData["area"]));
        }

        public static bool HasPrivilege(this IPrincipal user, string action, string controller, string area)
        {
            return WebUtilities.LoggedInUsersPrivilegesDict
                .ContainsKey($"{action}-{controller}-{area}");//NB: This key MUST conform with the pattern in Privilege.Name
        }
    }
}
