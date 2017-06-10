using MultiTenancyFramework.Mvc;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace MultiTenancyFramework.WebAPI
{
    public class GlobalAuthorizeAttribute : AuthorizationFilterAttribute // AuthorizeAttribute // 
    {
        public override void OnAuthorization(HttpActionContext filterContext)
        {
            var actionDescriptor = filterContext.ActionDescriptor;

            #region Check whether it's an anonymous action

            // check if AllowAnonymous is on the controller
            var anonymous = actionDescriptor.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>(true)
                    .Cast<AllowAnonymousAttribute>();
            if (anonymous.Any())
            {
                //Allow Anonymous
                anonymous = null;
                return;
            }

            // It's not; so check if AllowAnonymous is on the action
            anonymous = actionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>(true)
                    .Cast<AllowAnonymousAttribute>();
            if (anonymous.Any())
            {
                //Allow Anonymous
                anonymous = null;
                return;
            }
            anonymous = null;

            #endregion

            // If user is not logged in (authenticated) yet, 
            var IdentityUser = WebUtilities.GetCurrentlyLoggedInUser();
            if (IdentityUser == null)
            {
                // It's not anonymous, so force user to login
                WebUtilities.LogOut();
                filterContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                return;
            }

            // At this point, we have established that we have a logged-in user. So...
            #region Authorize at Privilege level

            var userPrivList = WebUtilities.LoggedInUsersPrivilegesDict;
            //This should never be true under normal circumstances, 'cos a properly logged-in user
            // should have at least one user privllege
            if (userPrivList == null)
            {
                WebUtilities.LogOut();
                filterContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                return;
            }

            // OK. So the user has some privileges. So...
            var area = filterContext.RequestContext.RouteData.Values["area"];
            string privilegeName = string.Format("{0}-{1}-{2}",
                   actionDescriptor.ActionName,
                   actionDescriptor.ControllerDescriptor.ControllerName,
                   area);

            if (!userPrivList.ContainsKey(privilegeName))
            {
                //The generalized case of the above 'GetData' trick 
                var point = actionDescriptor.GetCustomAttributes<ValidateUsingPrivilegeForActionAttribute>(true).FirstOrDefault();
                if (point != null)
                {
                    foreach (var actionName in point.ActionNames)
                    {
                        if (userPrivList.ContainsKey(string.Format("{0}-{1}-{2}",
                            actionName, actionDescriptor.ControllerDescriptor.ControllerName, area)))
                        {
                            return; //Good!
                        }
                    }
                }

                filterContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                return;
            }

            #endregion

            // fall back to base
            base.OnAuthorization(filterContext);
        }
    }

}
