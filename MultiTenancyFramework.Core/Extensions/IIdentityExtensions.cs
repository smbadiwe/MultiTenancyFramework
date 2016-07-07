using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web;
using System.Security.Principal;

namespace MultiTenancyFramework.Core
{
    public static class IIdentityExtensions
    {
        private static UserStore<IdentityUser> userStore;
        static RouteValueDictionary routeData { get { return HttpContext.Current.Request.RequestContext.RouteData.Values; } }
        static IIdentityExtensions()
        {
            userStore = new UserStore<IdentityUser>();
        }

        // This is probably an irrelevant optimisation; but then, I detest unnecessary call stacks.
        // So please, leave these 'HasPrivilege' method overloads the way I wrote them
        // - MultiTenancyFramework.Coredina Mbadiwe

        public static bool HasPrivilege(this IPrincipal user)
        {
            var action = Convert.ToString(routeData["action"]);
            var controller = Convert.ToString(routeData["controller"]);
            var area = Convert.ToString(routeData["area"]);
            return HasPrivilege(user, action, controller, area);
        }

        public static bool HasPrivilege(this IPrincipal user, string action)
        {
            var controller = Convert.ToString(routeData["controller"]);
            var area = Convert.ToString(routeData["area"]);
            return HasPrivilege(user, action, controller, area);
        }
        
        public static bool HasPrivilege(this IPrincipal user, string action, string controller)
        {
            var area = Convert.ToString(routeData["area"]);
            return HasPrivilege(user, action, controller, area);
        }

        public static bool HasPrivilege(this IPrincipal user, string action, string controller, string area)
        {
            var currentAppUser = WebUtilities.GetCurrentlyLoggedInUser();
            //TODO: I suspect I might not need this line.
            if ((
                currentAppUser.UserName == user.Identity.Name 
                    && currentAppUser.Id == user.Identity.GetUserId<long>()
                ) == false)
            {
                return false;
            }

            string role = $"{action}-{controller}-{area}"; //NB: This MUST conform with the pattern in Privilege.Name
            return userStore.IsInRoleAsync(currentAppUser, role).Result;
        }
    }
}
