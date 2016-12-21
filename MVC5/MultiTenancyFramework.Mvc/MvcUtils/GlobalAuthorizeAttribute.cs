using System.Web.Mvc;
using System.Linq;
using System;

namespace MultiTenancyFramework.Mvc
{
    /// <summary>
    /// Our custom Authorize attribute
    /// </summary>
    public class GlobalAuthorizeAttribute : AuthorizeAttribute
    {
        //private readonly ILogger Logger;
        //public GlobalAuthorizeAttribute()
        //{
        //    Logger = Utilities.Logger;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var actionDescriptor = filterContext.ActionDescriptor;

            #region Check whether it's an anonymous action

            // check if AllowAnonymous is on the controller
            var anonymous = actionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true)
                    .Cast<AllowAnonymousAttribute>();
            if (anonymous.Any())
            {
                //Allow Anonymous
                anonymous = null;
                return;
            }

            // It's not; so check if AllowAnonymous is on the action
            anonymous = actionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true)
                    .Cast<AllowAnonymousAttribute>();
            if (anonymous.Any())
            {
                //Allow Anonymous
                anonymous = null;
                return;
            }
            anonymous = null;

            #endregion

            var values = filterContext.RouteData.Values;
            var instCode = Convert.ToString(values["institution"]);
            // If user is not logged in (authenticated) yet, 
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                // It's not anonymous, so force user to login
                WebUtilities.LogOut();
                filterContext.Result = MvcUtility.GetLoginPageResult(instCode);
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
                filterContext.Result = MvcUtility.GetLoginPageResult(instCode);
                return;
            }

            // OK. So the user has some privileges. So...
            var area = values["area"];
            string privilegeName = string.Format("{0}-{1}-{2}",
                   actionDescriptor.ActionName,
                   actionDescriptor.ControllerDescriptor.ControllerName,
                   area);

            if (!userPrivList.ContainsKey(privilegeName))
            {
                //The generalized case of the above 'GetData' trick 
                var point = actionDescriptor.GetCustomAttributes(typeof(ValidateUsingPrivilegeForActionAttribute), true)
                        .Cast<ValidateUsingPrivilegeForActionAttribute>().FirstOrDefault();
                if (point != null)
                {
                    foreach (var actionName in point.ActionNames)
                    {
                        if (userPrivList.ContainsKey(string.Format("{0}-{1}-{2}",
                            actionName, actionDescriptor.ControllerDescriptor.ControllerName, area)))
                        {
                            return;
                        }
                    }
                }
                filterContext.Result = MvcUtility.GetPageResult("DenyAccess", "Error", "", instCode);
                return;
            }

            #endregion

            // fall back to base
            base.OnAuthorization(filterContext);
        }
    }

}
