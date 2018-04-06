using Microsoft.Owin.Security;
using MultiTenancyFramework.Entities;
using System.Collections.Generic;
using System.Web;

namespace MultiTenancyFramework.Mvc
{
    /// <summary>
    /// If you're calling this from a controller, use this.WebHelper instead.
    /// </summary>
    public class WebUtilities
    {
        private static WebHelper _webHelper;
        protected static WebHelper WebHelper
        {
            get
            {
                if (_webHelper == null) _webHelper = new WebHelper(new HttpContextWrapper(HttpContext.Current));
                return _webHelper;
            }
        }

        /// <summary>
        /// Key is ActionAccessPrivilege.Name, which is same as $"{Action}-{Controller}-{Area}";
        /// </summary>
        public static Dictionary<string, ActionAccessPrivilege> LoggedInUsersPrivilegesDict
        {
            get
            {
                return WebHelper.LoggedInUsersPrivilegesDict;
            }
            set
            {
                WebHelper.LoggedInUsersPrivilegesDict = value;
            }
        }

        public static void SetCurrentlyLoggedInUser(IdentityUser user)
        {
            WebHelper.SetCurrentlyLoggedInUser(user);
        }

        public static IdentityUser GetCurrentlyLoggedInUser()
        {
            return WebHelper.GetCurrentlyLoggedInUser();
        }

        public static bool IsCentralInstitution
        {
            get
            {
                return WebHelper.IsCentralInstitution;
            }
        }

        /// <summary>
        /// The understanding is that for 'Central', the value returned is null
        /// </summary>
        public static string InstitutionCode
        {
            get
            {
                return WebHelper.InstitutionCode;
            }
            set
            {
                WebHelper.InstitutionCode = value;
            }
        }

        /// <summary>
        /// Logs the use out, clears session and redirects the user to the login page.
        /// </summary>
        public static void LogOut(IAuthenticationManager auth = null, bool logThis = false)
        {
            WebHelper.LogOut(auth, logThis);
        }
        
    }
}
