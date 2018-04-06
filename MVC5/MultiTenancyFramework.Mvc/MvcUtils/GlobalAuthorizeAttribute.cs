using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MultiTenancyFramework.Mvc
{
    /// <summary>
    /// Our custom Authorize attribute
    /// </summary>
    public class GlobalAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            var actionDescriptor = filterContext.ActionDescriptor;
            
            #region Check whether it's an anonymous action

            if (actionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) 
                || actionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            {
                //Allow Anonymous
                return;
            }

            #endregion
            
            var values = filterContext.RouteData.Values;
            var instCode = Convert.ToString(values["institution"]);

            var webHelper = new WebHelper(filterContext.HttpContext);
            // If user is not logged in (authenticated) yet, 
            var IdentityUser = webHelper.GetCurrentlyLoggedInUser();
            //if (!filterContext.HttpContext.Request.IsAuthenticated)
            if (IdentityUser == null || filterContext.HttpContext.Session?.IsNewSession == true)
            {
                // It's not anonymous, so force user to login
                webHelper.LogOut();
                filterContext.Result = MvcUtility.GetLoginPageResult(instCode);
                return;
            }

            var area = values["area"];
            string privilegeName = string.Format("{0}-{1}-{2}",
                   actionDescriptor.ActionName,
                   actionDescriptor.ControllerDescriptor.ControllerName,
                   area);
            if (Utilities.INST_DEFAULT_CODE.Equals(instCode, StringComparison.OrdinalIgnoreCase))
            {
                #region Check whether to allow Core access the action
                
                if (!actionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAccessToParentAttribute), true)
                    && !actionDescriptor.IsDefined(typeof(AllowAccessToParentAttribute), true))
                {
                    // bounce
                    filterContext.Result = MvcUtility.GetPageResult("TenantsOnlyAllowed", "Error", "", instCode, new Dictionary<string, object> { { "actionAttempted", filterContext.HttpContext.Request.Url.AbsoluteUri } });
                    return;
                }

                #endregion
            }

            // At this point, we have established that we have a logged-in user. So...
            #region Authorize at Privilege level

            var userPrivList = webHelper.LoggedInUsersPrivilegesDict;
            //This should never be true under normal circumstances, 'cos a properly logged-in user
            // should have at least one user privllege
            if (userPrivList == null)
            {
                webHelper.LogOut();
                filterContext.Result = MvcUtility.GetLoginPageResult(instCode);
                return;
            }

            // OK. So the user has some privileges. So...

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
                filterContext.Result = MvcUtility.GetPageResult("DenyAccess", "Error", "", instCode, new System.Collections.Generic.Dictionary<string, object> { { "actionAttempted", privilegeName } });
                return;
            }

            #endregion

            // If we get to this point, then the user authorized to access this action
        }
    }

}
