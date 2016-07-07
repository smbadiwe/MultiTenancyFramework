using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using MultiTenancyFramework;
using MultiTenancyFramework.Data;
using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using MultiTenancyFramework.Logic;
using MultiTenancyFramework.Mvc.Identity;
using System;
using System.Collections.Generic;
using System.Web;

namespace MultiTenancyFramework.Mvc
{
    public class WebUtilities
    {
        private static IInstitutionDAO<Institution> InstitutionEngine;
        private static IAppUserDAO<IdentityUser> IdentityUser;

        private const string SS_CODE = "::SS_INSTITUTION_CODE::";
        private const string SS_CURRENT_USER = "::SS_CURRENT_USER::";
        private const string SS_INST_SHORT_NAME = "::InstitutionShortName::";
        static WebUtilities()
        {
            InstitutionEngine = MyServiceLocator.GetInstance<IInstitutionDAO<Institution>>();
            IdentityUser = MyServiceLocator.GetInstance<IAppUserDAO<IdentityUser>>();
        }

        /// <summary>
        /// Key is Module Name, Value is list of RoleNames (as in, Privilege.Name)
        /// </summary>
        public static Dictionary<string, List<string>> LoggedInUsersPrivilegesDict
        {
            get
            {
                try
                {
                    return HttpContext.Current.Session["::LoggedInUsersPrivileges::"] as Dictionary<string, List<string>>;
                }
                catch
                {
                    throw new LogOutUserException();
                }
            }
            set
            {
                try
                {
                    HttpContext.Current.Session["::LoggedInUsersPrivileges::"] = value;
                }
                catch
                {
                    throw new LogOutUserException();
                }
            }
        }

        public const string LoginRedirectAction = "";
        public static List<string> LoggedInUsersPrivileges
        {
            get
            {
                try
                {
                    return HttpContext.Current.Session["::LoggedInUsersPrivileges::"] as List<string> ?? new List<string>();
                }
                catch
                {
                    throw new LogOutUserException();
                }
            }
            set
            {
                try
                {
                    if (value != null && !value.Contains(LoginRedirectAction))
                    {
                        value.Add(LoginRedirectAction);
                    }
                    HttpContext.Current.Session["::LoggedInUsersPrivileges::"] = value;
                }
                catch
                {
                    throw new LogOutUserException();
                }
            }
        }

        public static void SetCurrentlyLoggedInUser(AppUser user)
        {
            HttpContext.Current.Session[SS_CURRENT_USER] = user;
        }

        public static AppUser GetCurrentlyLoggedInUser()
        {
            try
            {
                if (HttpContext.Current != null && HttpContext.Current.User != null &&
                    HttpContext.Current.User.Identity != null && !string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name))
                {
                    try
                    {
                        // string key = SS_CURRENT_USER + InstitutionShortName;
                        var user = HttpContext.Current.Session[SS_CURRENT_USER] as AppUser;
                        if (user == null)
                        {
                            IdentityUser.InstitutionCode = InstitutionCode;
                            user = IdentityUser.Retrieve(HttpContext.Current.User.Identity.GetUserId<long>());
                        }
                        if (user == null) throw new LogOutUserException();
                        HttpContext.Current.Session[SS_CURRENT_USER] = user;
                        return user;
                    }
                    catch (Exception)
                    {
                        throw new LogOutUserException();
                    }
                }
                return null;
            }
            catch (LogOutUserException) { throw; }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool IsCentralInstitution
        {
            get
            {
                return string.IsNullOrWhiteSpace(InstitutionCode) || InstitutionCode == Utilities.INST_DEFAULT_CODE;
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
                    if (instCode == Utilities.INST_DEFAULT_CODE) return null;

                    if (HttpContext.Current.Session != null && HttpContext.Current.Session[SS_CODE] != null)
                    {
                        var codeInSession = Convert.ToString(HttpContext.Current.Session[SS_CODE]);
                        if (codeInSession != instCode) throw new LogOutUserException();
                        return codeInSession;
                    }
                    var query = new GetInstitutionByCodeQuery
                    {
                        Code = instCode,
                    };
                    var inst = Utilities.QueryProcessor.Process(query);
                    if (inst == null) throw new GeneralException($"The code: {instCode} does not belong to any institution on our system.", ExceptionType.UnidentifiedInstitutionCode);

                    return inst.Code;
                }
                throw new LogOutUserException(); // Code should never reach here.
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value)) value = Utilities.INST_DEFAULT_CODE;
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
            catch //(Exception ex)
            {
                //Logging.Logger.Log(ex);
            }
        }

    }
}
