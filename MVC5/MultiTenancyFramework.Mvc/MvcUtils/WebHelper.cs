using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using MultiTenancyFramework.Data;
using MultiTenancyFramework.Entities;
using MultiTenancyFramework.Logic;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;

namespace MultiTenancyFramework
{
    /// <summary>
    /// Represents a common helper for MVC. Best if used from controllers
    /// </summary>
    public class WebHelper : SimpleWebHelper
    {
        #region Ctor

        public WebHelper() : base()
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        public WebHelper(HttpContext httpContext) : base(httpContext)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        public WebHelper(HttpContextBase httpContext) : base(httpContext)
        {
        }

        #endregion

        #region Utilities

        #region From WebUtilities
        
        public virtual IdentityUser GetCurrentlyLoggedInUser()
        {
            try
            {
                var session = _httpContext.Session;
                if (session != null)
                {
                    IdentityUser user = session[SS_CURRENT_USER] as IdentityUser;
                    if (user == null)
                    {
                        if (_httpContext.User != null && _httpContext.User.Identity != null 
                            && _httpContext.User.Identity.IsAuthenticated)
                        {
                            var dao = MyServiceLocator.GetInstance<IAppUserDAO<IdentityUser>>();
                            dao.InstitutionCode = InstitutionCode;
                            var userId = _httpContext.User.Identity.GetUserId<long>();
                            if (userId > 0) user = dao.Retrieve(userId);
                            if (user == null) throw new LogOutUserException($"Called WebHelper.GetCurrentlyLoggedInUser(): Failed to get user [id: {userId}. instCode: {dao.InstitutionCode}]");

                            user.InstitutionCode = dao.InstitutionCode; //Needful? Maybe not.
                            _httpContext.Session[SS_CURRENT_USER] = user;
                        }
                    }
                    return user;
                }
                return null;
            }
            catch (Exception ex) when (!(ex is LogOutUserException))
            {
                Utilities.Logger.Log(ex);
                return null;
            }
        }

        /// <summary>
        /// Logs the use out, clears session and redirects the user to the login page.
        /// </summary>
        public virtual void LogOut(IAuthenticationManager auth = null, bool logThis = false)
        {
            try
            {
                if (logThis)
                {
                    try
                    {
                        var funMemUser = GetCurrentlyLoggedInUser();
                        if (funMemUser != null)
                        {
                            _httpContext.Session[SS_CURRENT_USER] = funMemUser;
                            MyServiceLocator.GetInstance<IAuditTrailLogger>().AuditLogin(EventType.Logout);
                        }
                    }
                    catch { }
                }
                _httpContext.Session.Clear();
                _httpContext.Session.Abandon();
                if (auth == null) auth = _httpContext.GetOwinContext().Authentication;
                auth.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            }
            catch (Exception ex)
            {
                Utilities.Logger.Log(ex);
            }
        }

        /// <summary>
        /// Key is ActionAccessPrivilege.Name, which is same as $"{Action}-{Controller}-{Area}";
        /// </summary>
        public virtual Dictionary<string, ActionAccessPrivilege> LoggedInUsersPrivilegesDict
        {
            get
            {
                try
                {
                    var privs = _httpContext.Session["::LoggedInUsersPrivileges::"] as Dictionary<string, ActionAccessPrivilege>;
                    if (privs == null) throw new GeneralException("Called WebUtilities.LoggedInUsersPrivilegesDict: Error getting LoggedInUsersPrivileges from session");

                    return privs;
                }
                catch (Exception)
                {
                    throw new LogOutUserException();
                }
            }
            set
            {
                try
                {
                    if (value == null) value = new Dictionary<string, ActionAccessPrivilege>();
                    _httpContext.Session["::LoggedInUsersPrivileges::"] = value;
                }
                catch (Exception) // paranoia
                {
                    throw new LogOutUserException();
                }
            }
        }

        #endregion

        #endregion
    }

}
