using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using MultiTenancyFramework.Data;
using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using MultiTenancyFramework.Logic;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;

namespace MultiTenancyFramework.Mvc
{
    public class WebUtilities
    {
        private static IInstitutionDAO<Institution> InstitutionEngine;
        private static IAppUserDAO<IdentityUser> IdentityUserDAO;

        private const string SS_CODE = "::SS_INSTITUTION_CODE::";
        private const string SS_CURRENT_USER = "::SS_CURRENT_USER::";
        private const string SS_INST_SHORT_NAME = "::InstitutionShortName::";
        static WebUtilities()
        {
            InstitutionEngine = MyServiceLocator.GetInstance<IInstitutionDAO<Institution>>();
            IdentityUserDAO = MyServiceLocator.GetInstance<IAppUserDAO<IdentityUser>>();
        }

        /// <summary>
        /// Key is ActionAccessPrivilege.Name, which is same as $"{Action}-{Controller}-{Area}";
        /// </summary>
        public static Dictionary<string, ActionAccessPrivilege> LoggedInUsersPrivilegesDict
        {
            get
            {
                try
                {
                    var privs = HttpContext.Current.Session["::LoggedInUsersPrivileges::"] as Dictionary<string, ActionAccessPrivilege>;
                    if (privs == null) throw new GeneralException("Called WebUtilities.LoggedInUsersPrivilegesDict: Error getting LoggedInUsersPrivileges from session");

                    return privs;
                }
                catch (Exception ex)
                {
                    throw new LogOutUserException();
                }
            }
            set
            {
                try
                {
                    if (value == null) value = new Dictionary<string, ActionAccessPrivilege>();
                    HttpContext.Current.Session["::LoggedInUsersPrivileges::"] = value;
                }
                catch (Exception ex) // paranoia
                {
                    throw new LogOutUserException();
                }
            }
        }

        public static void SetCurrentlyLoggedInUser(IdentityUser user)
        {
            HttpContext.Current.Session[SS_CURRENT_USER] = user;
        }

        public static IdentityUser GetCurrentlyLoggedInUser(HttpSessionStateBase session = null)
        {
            try
            {
                if (session == null && HttpContext.Current != null && HttpContext.Current.Session != null)
                    session = new HttpSessionStateWrapper(HttpContext.Current.Session);
                if (session != null)
                {
                    IdentityUser user = session[SS_CURRENT_USER] as IdentityUser;
                    if (user == null)
                    {
                        var principal = HttpContext.Current.User as ClaimsPrincipal;
                        if (principal != null && principal.Identity.IsAuthenticated)
                        {
                            IdentityUserDAO.InstitutionCode = InstitutionCode;
                            var userId = HttpContext.Current.User.Identity.GetUserId<long>();
                            if (userId > 0) user = IdentityUserDAO.Retrieve(userId);
                            if (user == null) throw new LogOutUserException($"Called WebUtilities.GetCurrentlyLoggedInUser(): Failed to get user [id: {userId}. instCode: {IdentityUserDAO.InstitutionCode}]");

                            user.InstitutionCode = IdentityUserDAO.InstitutionCode; //Needful? Maybe not.
                            HttpContext.Current.Session[SS_CURRENT_USER] = user;
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

        public static bool IsCentralInstitution
        {
            get
            {
                var instCode = InstitutionCode;
                return string.IsNullOrWhiteSpace(instCode) || Utilities.INST_DEFAULT_CODE.Equals(instCode, StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// The understanding is that for 'Central', the value returned is null
        /// </summary>
        public static string InstitutionCode
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Request != null
                    && HttpContext.Current.Request.RequestContext != null)
                {
                    var instCode = Convert.ToString(HttpContext.Current.Request.RequestContext.RouteData.Values["institution"]) ?? Utilities.INST_DEFAULT_CODE;
                    if (string.IsNullOrWhiteSpace(instCode) || instCode.Equals(Utilities.INST_DEFAULT_CODE, StringComparison.OrdinalIgnoreCase)) return null;

                    // Thr request may be from a non-existent institution, so we check if we have it in session
                    var httpSession = HttpContext.Current.Session;
                    if (httpSession != null && httpSession[SS_CODE] != null)
                    {
                        var codeInSession = Convert.ToString(httpSession[SS_CODE]); //ClaimsPrincipal.Current.FindFirst("ic")?.Value; // 
                        if (!instCode.Equals(codeInSession, StringComparison.OrdinalIgnoreCase))
                        {
                            var error = $"codeInSession ({codeInSession}) != instCode ({instCode}).";
                            var ex = new LogOutUserException(error);
                            throw ex;
                        }
                        return codeInSession;
                    }
                    var query = new GetInstitutionByCodeQuery
                    {
                        Code = instCode,
                    };
                    var inst = Utilities.QueryProcessor.Process(query);
                    if (inst == null) throw new GeneralException($"The code: {instCode} does not belong to any institution on our system.", ExceptionType.UnidentifiedInstitutionCode);

                    return instCode; // return inst.Code;
                }
                else
                {
                    throw new LogOutUserException(); // Code should never reach here.
                }
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value)) value = Utilities.INST_DEFAULT_CODE;
                value = value.ToLower();
                HttpContext.Current.Request.RequestContext.RouteData.Values["institution"] = value;
                HttpContext.Current.Session[SS_CODE] = value;
            }
        }

        /// <summary>
        /// Logs the use out, clears session and redirects the user to the login page.
        /// </summary>
        public static void LogOut(IAuthenticationManager auth = null, bool logThis = false)
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
                            HttpContext.Current.Session[SS_CURRENT_USER] = funMemUser;
                            MyServiceLocator.GetInstance<IAuditTrailLogger>().AuditLogin(EventType.Logout);
                        }
                    }
                    catch { }
                }
                HttpContext.Current.Session.Clear();
                HttpContext.Current.Session.Abandon();
                if (auth == null) auth = HttpContext.Current.GetOwinContext().Authentication;
                auth.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            }
            catch (Exception ex)
            {
                Utilities.Logger.Log(ex);
            }
        }

    }
}
