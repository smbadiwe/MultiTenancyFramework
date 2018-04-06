using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using MultiTenancyFramework.Data;
using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using MultiTenancyFramework.Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace MultiTenancyFramework
{
    /// <summary>
    /// Represents a common helper for MVC. Best if used from controllers
    /// </summary>
    public class WebHelper
    {
        #region Const 

        private const string SS_CODE = "::SS_INSTITUTION_CODE::";
        private const string SS_CURRENT_USER = "::SS_CURRENT_USER::";

        #endregion

        #region Fields 

        private readonly HttpContextBase _httpContext;
        private readonly string[] _staticFileExtensions;

        #endregion

        #region Constructor

        public WebHelper() : this(new HttpContextWrapper(HttpContext.Current))
        {

        }

        public WebHelper(HttpContext httpContext) : this(new HttpContextWrapper(httpContext))
        {

        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        public WebHelper(HttpContextBase httpContext)
        {
            this._httpContext = httpContext;
            this._staticFileExtensions = new[] { ".axd", ".ashx", ".bmp", ".css", ".gif", ".htm", ".html", ".ico", ".jpeg", ".jpg", ".js", ".png", ".rar", ".zip" };
        }

        #endregion

        #region Utilities

        #region From WebUtilities

        /// <summary>
        /// The understanding is that for 'Central', the value returned is null
        /// </summary>
        public virtual string InstitutionCode
        {
            get
            {
                if (_httpContext != null && _httpContext.Request != null
                    && _httpContext.Request.RequestContext != null)
                {
                    var instCode = Convert.ToString(_httpContext.Request.RequestContext.RouteData.Values["institution"]) ?? Utilities.INST_DEFAULT_CODE;
                    if (string.IsNullOrWhiteSpace(instCode) || instCode.Equals(Utilities.INST_DEFAULT_CODE, StringComparison.OrdinalIgnoreCase)) return null;

                    var httpSession = _httpContext.Session;
                    // Should never happen; but yeah...
                    if (httpSession == null) throw new LogOutUserException();

                    // The request may be from a non-existent institution, so we check if we have it in session
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
                _httpContext.Request.RequestContext.RouteData.Values["institution"] = value;
                _httpContext.Session[SS_CODE] = value;
            }
        }

        public virtual bool IsCentralInstitution
        {
            get
            {
                var instCode = InstitutionCode;
                return string.IsNullOrWhiteSpace(instCode) || Utilities.INST_DEFAULT_CODE.Equals(instCode, StringComparison.OrdinalIgnoreCase);
            }
        }

        public virtual void SetCurrentlyLoggedInUser(IdentityUser user)
        {
            _httpContext.Session[SS_CURRENT_USER] = user;
        }

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
                        var principal = _httpContext.User as ClaimsPrincipal;
                        if (principal != null && principal.Identity.IsAuthenticated)
                        {
                            var dao = MyServiceLocator.GetInstance<IAppUserDAO<IdentityUser>>();
                            dao.InstitutionCode = InstitutionCode;
                            var userId = _httpContext.User.Identity.GetUserId<long>();
                            if (userId > 0) user = dao.Retrieve(userId);
                            if (user == null) throw new LogOutUserException($"Called WebUtilities.GetCurrentlyLoggedInUser(): Failed to get user [id: {userId}. instCode: {dao.InstitutionCode}]");

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

        public virtual bool IsRequestAvailable(HttpContextBase httpContext)
        {
            if (httpContext == null)
                return false;

            try
            {
                if (httpContext.Request == null)
                    return false;
            }
            catch (HttpException)
            {
                return false;
            }

            return true;
        }

        public virtual bool TryWriteWebConfig()
        {
            try
            {
                // In medium trust, "UnloadAppDomain" is not supported. Touch web.config
                // to force an AppDomain restart.
                File.SetLastWriteTimeUtc(CommonHelper.MapPath("~/web.config"), DateTime.UtcNow);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual bool TryWriteGlobalAsax()
        {
            try
            {
                //When a new plugin is dropped in the Plugins folder and is installed into nopCommerce, 
                //even if the plugin has registered routes for its controllers, 
                //these routes will not be working as the MVC framework couldn't 
                //find the new controller types and couldn't instantiate the requested controller. 
                //That's why you get these nasty errors 
                //i.e "Controller does not implement IController".
                //The issue is described here: http://www.nopcommerce.com/boards/t/10969/nop-20-plugin.aspx?p=4#51318
                //The solution is to touch global.asax file
                File.SetLastWriteTimeUtc(CommonHelper.MapPath("~/global.asax"), DateTime.UtcNow);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get URL referrer
        /// </summary>
        /// <returns>URL referrer</returns>
        public virtual string GetUrlReferrer()
        {
            string referrerUrl = string.Empty;

            //URL referrer is null in some case (for example, in IE 8)
            if (IsRequestAvailable(_httpContext) && _httpContext.Request.UrlReferrer != null)
                referrerUrl = _httpContext.Request.UrlReferrer.PathAndQuery;

            return referrerUrl;
        }

        /// <summary>
        /// Get context IP address
        /// </summary>
        /// <returns>URL referrer</returns>
        public virtual string GetCurrentIpAddress()
        {
            if (!IsRequestAvailable(_httpContext))
                return string.Empty;

            var result = "";
            try
            {
                if (_httpContext.Request.Headers != null)
                {
                    //The X-Forwarded-For (XFF) HTTP header field is a de facto standard
                    //for identifying the originating IP address of a client
                    //connecting to a web server through an HTTP proxy or load balancer.
                    var forwardedHttpHeader = "X-FORWARDED-FOR";
                    if (!string.IsNullOrWhiteSpace(ConfigurationHelper.AppSettingsItem<string>("ForwardedHTTPheader")))
                    {
                        //but in some cases server use other HTTP header
                        //in these cases an administrator can specify a custom Forwarded HTTP header
                        //e.g. CF-Connecting-IP, X-FORWARDED-PROTO, etc
                        forwardedHttpHeader = ConfigurationHelper.AppSettingsItem<string>("ForwardedHTTPheader");
                    }

                    //it's used for identifying the originating IP address of a client connecting to a web server
                    //through an HTTP proxy or load balancer. 
                    string xff = _httpContext.Request.Headers.AllKeys
                        .Where(x => forwardedHttpHeader.Equals(x, StringComparison.InvariantCultureIgnoreCase))
                        .Select(k => _httpContext.Request.Headers[k])
                        .FirstOrDefault();

                    //if you want to exclude private IP addresses, then see http://stackoverflow.com/questions/2577496/how-can-i-get-the-clients-ip-address-in-asp-net-mvc
                    if (!string.IsNullOrWhiteSpace(xff))
                    {
                        string lastIp = xff.Split(',')[0];
                        result = lastIp;
                    }
                }

                if (string.IsNullOrWhiteSpace(result))
                {
                    result = _httpContext.Request.UserHostAddress;
                }
            }
            catch { }

            if (string.IsNullOrWhiteSpace(result)) // should never happen
            {
                result = IPResolver.GetIP4Address();
            }

            //some validation
            if (result == "::1")
                result = "127.0.0.1";
            //remove port
            if (!string.IsNullOrWhiteSpace(result))
            {
                int index = result.IndexOf(":", StringComparison.InvariantCultureIgnoreCase);
                if (index > 0)
                    result = result.Substring(0, index);
            }
            return result;

        }

        /// <summary>
        /// Gets this page name
        /// </summary>
        /// <param name="includeQueryString">Value indicating whether to include query strings</param>
        /// <returns>Page name</returns>
        public virtual string GetThisPageUrl(bool includeQueryString)
        {
            bool useSsl = IsCurrentConnectionSecured();
            return GetThisPageUrl(includeQueryString, useSsl);
        }

        /// <summary>
        /// Gets this page name
        /// </summary>
        /// <param name="includeQueryString">Value indicating whether to include query strings</param>
        /// <param name="useSsl">Value indicating whether to get SSL public page</param>
        /// <returns>Page name</returns>
        public virtual string GetThisPageUrl(bool includeQueryString, bool useSsl)
        {
            if (!IsRequestAvailable(_httpContext))
                return string.Empty;

            //get the host considering using SSL
            var url = GetInstitutionHost(useSsl).TrimEnd('/');

            //get full URL with or without query string
            url += includeQueryString ? _httpContext.Request.RawUrl : _httpContext.Request.Path;

            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets a value indicating whether current connection is secured
        /// </summary>
        /// <returns>true - secured, false - not secured</returns>
        public virtual bool IsCurrentConnectionSecured()
        {
            bool useSsl = false;
            if (IsRequestAvailable(_httpContext))
            {
                //when your hosting uses a load balancer on their server then the Request.IsSecureConnection is never got set to true

                //1. use HTTP_CLUSTER_HTTPS?
                if (ConfigurationHelper.AppSettingsItem<bool>("HTTP_CLUSTER_HTTPS"))
                {
                    useSsl = ServerVariables("HTTP_CLUSTER_HTTPS") == "on";
                }
                //2. use HTTP_X_FORWARDED_PROTO?
                else if (ConfigurationHelper.AppSettingsItem<bool>("HTTP_X_FORWARDED_PROTO"))
                {
                    useSsl = string.Equals(ServerVariables("HTTP_X_FORWARDED_PROTO"), "https", StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    useSsl = _httpContext.Request.IsSecureConnection;
                }
            }

            return useSsl;
        }

        /// <summary>
        /// Gets server variable by name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Server variable</returns>
        public virtual string ServerVariables(string name)
        {
            string result = string.Empty;

            try
            {
                if (!IsRequestAvailable(_httpContext))
                    return result;

                //put this method is try-catch 
                //as described here http://www.nopcommerce.com/boards/t/21356/multi-institution-roadmap-lets-discuss-update-done.aspx?p=6#90196
                if (_httpContext.Request.ServerVariables[name] != null)
                {
                    result = _httpContext.Request.ServerVariables[name];
                }
            }
            catch
            {
                result = string.Empty;
            }
            return result;
        }

        /// <summary>
        /// Gets institution host location
        /// </summary>
        /// <param name="useSsl">Use SSL</param>
        /// <returns>Institution host location</returns>
        public virtual string GetInstitutionHost(bool useSsl)
        {
            return ConfigurationHelper.GetSiteUrl();
        }
        
        /// <summary>
        /// Returns true if the requested resource is one of the typical resources that needn't be processed by the cms engine.
        /// </summary>
        /// <param name="request">HTTP Request</param>
        /// <returns>True if the request targets a static resource file.</returns>
        /// <remarks>
        /// These are the file extensions considered to be static resources:
        /// .css
        ///	.gif
        /// .png 
        /// .jpg
        /// .jpeg
        /// .js
        /// .axd
        /// .ashx
        /// </remarks>
        public virtual bool IsStaticResource(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            string path = request.Path;
            string extension = VirtualPathUtility.GetExtension(path);

            if (extension == null) return false;

            return _staticFileExtensions.Contains(extension);
        }

        /// <summary>
        /// Modifies query string
        /// </summary>
        /// <param name="url">Url to modify</param>
        /// <param name="queryStringModification">Query string modification</param>
        /// <param name="anchor">Anchor</param>
        /// <returns>New url</returns>
        public virtual string ModifyQueryString(string url, string queryStringModification, string anchor)
        {
            if (url == null)
                url = string.Empty;
            url = url.ToLowerInvariant();

            if (queryStringModification == null)
                queryStringModification = string.Empty;
            queryStringModification = queryStringModification.ToLowerInvariant();

            if (anchor == null)
                anchor = string.Empty;
            anchor = anchor.ToLowerInvariant();


            string str = string.Empty;
            string str2 = string.Empty;
            if (url.Contains("#"))
            {
                str2 = url.Substring(url.IndexOf("#") + 1);
                url = url.Substring(0, url.IndexOf("#"));
            }
            if (url.Contains("?"))
            {
                str = url.Substring(url.IndexOf("?") + 1);
                url = url.Substring(0, url.IndexOf("?"));
            }
            if (!string.IsNullOrWhiteSpace(queryStringModification))
            {
                if (!string.IsNullOrWhiteSpace(str))
                {
                    var dictionary = new Dictionary<string, string>();
                    foreach (string str3 in str.Split(new[] { '&' }))
                    {
                        if (!string.IsNullOrWhiteSpace(str3))
                        {
                            string[] strArray = str3.Split(new[] { '=' });
                            if (strArray.Length == 2)
                            {
                                if (!dictionary.ContainsKey(strArray[0]))
                                {
                                    //do not add value if it already exists
                                    //two the same query parameters? theoretically it's not possible.
                                    //but MVC has some ugly implementation for checkboxes and we can have two values
                                    //find more info here: http://www.mindstorminteractive.com/topics/jquery-fix-asp-net-mvc-checkbox-truefalse-value/
                                    //we do this validation just to ensure that the first one is not overridden
                                    dictionary[strArray[0]] = strArray[1];
                                }
                            }
                            else
                            {
                                dictionary[str3] = null;
                            }
                        }
                    }
                    foreach (string str4 in queryStringModification.Split(new[] { '&' }))
                    {
                        if (!string.IsNullOrWhiteSpace(str4))
                        {
                            string[] strArray2 = str4.Split(new[] { '=' });
                            if (strArray2.Length == 2)
                            {
                                dictionary[strArray2[0]] = strArray2[1];
                            }
                            else
                            {
                                dictionary[str4] = null;
                            }
                        }
                    }
                    var builder = new StringBuilder();
                    foreach (string str5 in dictionary.Keys)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append("&");
                        }
                        builder.Append(str5);
                        if (dictionary[str5] != null)
                        {
                            builder.Append("=");
                            builder.Append(dictionary[str5]);
                        }
                    }
                    str = builder.ToString();
                }
                else
                {
                    str = queryStringModification;
                }
            }
            if (!string.IsNullOrWhiteSpace(anchor))
            {
                str2 = anchor;
            }
            return (url + (string.IsNullOrWhiteSpace(str) ? "" : ("?" + str)) + (string.IsNullOrWhiteSpace(str2) ? "" : ("#" + str2))).ToLowerInvariant();
        }

        /// <summary>
        /// Remove query string from url
        /// </summary>
        /// <param name="url">Url to modify</param>
        /// <param name="queryString">Query string to remove</param>
        /// <returns>New url</returns>
        public virtual string RemoveQueryString(string url, string queryString)
        {
            if (url == null)
                url = string.Empty;
            url = url.ToLowerInvariant();

            if (queryString == null)
                queryString = string.Empty;
            queryString = queryString.ToLowerInvariant();


            string str = string.Empty;
            if (url.Contains("?"))
            {
                str = url.Substring(url.IndexOf("?") + 1);
                url = url.Substring(0, url.IndexOf("?"));
            }
            if (!string.IsNullOrWhiteSpace(queryString))
            {
                if (!string.IsNullOrWhiteSpace(str))
                {
                    var dictionary = new Dictionary<string, string>();
                    foreach (string str3 in str.Split(new[] { '&' }))
                    {
                        if (!string.IsNullOrWhiteSpace(str3))
                        {
                            string[] strArray = str3.Split(new[] { '=' });
                            if (strArray.Length == 2)
                            {
                                dictionary[strArray[0]] = strArray[1];
                            }
                            else
                            {
                                dictionary[str3] = null;
                            }
                        }
                    }
                    dictionary.Remove(queryString);

                    var builder = new StringBuilder();
                    foreach (string str5 in dictionary.Keys)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append("&");
                        }
                        builder.Append(str5);
                        if (dictionary[str5] != null)
                        {
                            builder.Append("=");
                            builder.Append(dictionary[str5]);
                        }
                    }
                    str = builder.ToString();
                }
            }
            return (url + (string.IsNullOrWhiteSpace(str) ? "" : ("?" + str)));
        }

        /// <summary>
        /// Gets query string value by name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">Parameter name</param>
        /// <returns>Query string value</returns>
        public virtual T QueryString<T>(string name)
        {
            string queryParam = null;
            if (IsRequestAvailable(_httpContext) && _httpContext.Request.QueryString[name] != null)
                queryParam = _httpContext.Request.QueryString[name];

            if (!string.IsNullOrWhiteSpace(queryParam))
                return CommonHelper.To<T>(queryParam);

            return default(T);
        }

        /// <summary>
        /// Restart application domain
        /// </summary>
        /// <param name="makeRedirect">A value indicating whether we should made redirection after restart</param>
        /// <param name="redirectUrl">Redirect URL; empty string if you want to redirect to the current page URL</param>
        public virtual void RestartAppDomain(bool makeRedirect = false, string redirectUrl = "")
        {
            if (CommonHelper.GetTrustLevel() > AspNetHostingPermissionLevel.Medium)
            {
                //full trust
                HttpRuntime.UnloadAppDomain();

                TryWriteGlobalAsax();
            }
            else
            {
                //medium trust
                bool success = TryWriteWebConfig();
                if (!success)
                {
                    throw new GeneralException("nopCommerce needs to be restarted due to a configuration change, but was unable to do so." + Environment.NewLine +
                        "To prevent this issue in the future, a change to the web server configuration is required:" + Environment.NewLine +
                        "- run the application in a full trust environment, or" + Environment.NewLine +
                        "- give the application write access to the 'web.config' file.");
                }
                success = TryWriteGlobalAsax();

                if (!success)
                {
                    throw new GeneralException("nopCommerce needs to be restarted due to a configuration change, but was unable to do so." + Environment.NewLine +
                        "To prevent this issue in the future, a change to the web server configuration is required:" + Environment.NewLine +
                        "- run the application in a full trust environment, or" + Environment.NewLine +
                        "- give the application write access to the 'Global.asax' file.");
                }
            }

            // If setting up extensions/modules requires an AppDomain restart, it's very unlikely the
            // current request can be processed correctly.  So, we redirect to the same URL, so that the
            // new request will come to the newly started AppDomain.
            if (_httpContext != null && makeRedirect)
            {
                if (string.IsNullOrWhiteSpace(redirectUrl))
                    redirectUrl = GetThisPageUrl(true);
                _httpContext.Response.Redirect(redirectUrl, true /*endResponse*/);
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the client is being redirected to a new location
        /// </summary>
        public virtual bool IsRequestBeingRedirected
        {
            get
            {
                var response = _httpContext.Response;
                return response.IsRequestBeingRedirected;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the client is being redirected to a new location using POST
        /// </summary>
        public virtual bool IsPostBeingDone
        {
            get
            {
                if (_httpContext.Items["Cauchy.IsPOSTBeingDone"] == null)
                    return false;
                return Convert.ToBoolean(_httpContext.Items["Cauchy.IsPOSTBeingDone"]);
            }
            set
            {
                _httpContext.Items["Cauchy.IsPOSTBeingDone"] = value;
            }
        }

        #endregion
    }

}
