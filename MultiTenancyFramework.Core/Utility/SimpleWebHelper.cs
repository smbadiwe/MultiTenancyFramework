using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiTenancyFramework.Data.Queries;
using System.Web;
using System.IO;
using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework
{
    public class SimpleWebHelper
    {
        #region Const 

        private const string SS_CODE = "::SS_INSTITUTION_CODE::";
        protected const string SS_CURRENT_USER = "::SS_CURRENT_USER::";
        /// <summary>
        /// This key represents a flag to tell us if HTTP request is available.
        /// Set it in application_beginrequest in httpContext.Items.
        /// </summary>
        public const string HttpRequestIsAvailable = "ReqAvailable";

        #endregion

        #region Fields 

        protected readonly HttpContextBase _httpContext;

        #endregion

        #region Ctor

        public SimpleWebHelper() : this(new HttpContextWrapper(HttpContext.Current))
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        public SimpleWebHelper(HttpContext httpContext) : this(new HttpContextWrapper(httpContext))
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        public SimpleWebHelper(HttpContextBase httpContext)
        {
            this._httpContext = httpContext;
        }

        #endregion

        #region Utilities

        #region From WebUtilities

        public virtual void SetCurrentlyLoggedInUser(AppUser user)
        {
            _httpContext.Session[SS_CURRENT_USER] = user;
        }

        /// <summary>
        /// Gets the institution code. This returns null IF HTTP context or request or session is not available.
        /// If we return null, then requestAvailable is false. If central, we return the default code
        /// </summary>
        /// <returns></returns>
        /// <exception cref="GeneralException"></exception>
        public virtual string GetInstitutionCode(out bool requestAvailable)
        {
            if (IsRequestAvailable(_httpContext))
            {
                string core = Utilities.INST_DEFAULT_CODE;
                requestAvailable = true;
                string instCode;
                var httpSession = _httpContext.Session;
                var routeData = _httpContext.Request.RequestContext?.RouteData?.Values;
                if (routeData == null || routeData.Count < 2 || httpSession == null)
                {
                    // This can happen when we invoke this before MVC is activated.
                    #region Pre-MVC 
                    if (routeData != null && routeData.ContainsKey("institution"))
                    {
                        return Convert.ToString(routeData["institution"]);
                    }

                    var url = GetThisPageUrl(false, false).ToLowerInvariant();
                    var siteUrl = ConfigurationHelper.GetSiteUrl().ToLowerInvariant();
                    url = url.Replace(siteUrl, string.Empty);
                    if (string.IsNullOrWhiteSpace(url)) return core;

                    var segments = url.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    switch (segments.Length)
                    {
                        // This code HEAVILY relies on the url templates setup in
                        // MultiTenancyFramework.Mvc.InstitutionRouteConfig.RegisterRoutes
                        case 1:
                            if (new[] { "home", "error" }.Contains(segments[0]))
                                instCode = core;
                            else
                                instCode = segments[0];
                            break;
                        case 2:
                            if (new[] { "home", "error" }.Contains(segments[1]))
                                instCode = segments[0];
                            else
                                instCode = core;
                            break;
                        case 3:
                            int id;
                            if (int.TryParse(segments[2], out id))
                                instCode = core;
                            else
                                instCode = segments[0];
                            break;
                        case 4:
                        default:
                            instCode = segments[0];
                            break;
                    }
                    #endregion
                }
                else
                {
                    #region MVC-Specific
                    instCode = Convert.ToString(routeData["institution"]) ?? core;
                    if (string.IsNullOrWhiteSpace(instCode) || instCode.Equals(core, StringComparison.OrdinalIgnoreCase)) return core;

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
                    #endregion
                }

                // if it's a tenant
                if (!string.IsNullOrWhiteSpace(instCode) && !instCode.Equals(core, StringComparison.OrdinalIgnoreCase))
                {
                    var query = new GetInstitutionByCodeQuery
                    {
                        Code = instCode,
                    };
                    var inst = Utilities.QueryProcessor.Process(query);
                    if (inst == null)
                    {
                        if (IsStaticResource())
                            instCode = core;
                        else
                            throw new GeneralException($"The code: {instCode} does not belong to any institution on our system.", ExceptionType.UnidentifiedInstitutionCode);
                    }
                }
                _httpContext.Request.RequestContext.RouteData.Values["institution"] = instCode;
                if (_httpContext.Session != null)
                    _httpContext.Session[SS_CODE] = instCode;

                return instCode; // return inst.Code; 
            }

            requestAvailable = false;
            return null;
        }

        /// <summary>
        /// Gets or sets the institution code. Use this only from a web context It throws logoutexception if request is not available.
        /// </summary>
        /// <value>
        /// The institution code.
        /// </value>
        /// <exception cref="LogOutUserException">
        /// </exception>
        /// <exception cref="GeneralException"></exception>
        public virtual string InstitutionCode
        {
            get
            {
                bool requestAvailable;
                var instCode = GetInstitutionCode(out requestAvailable);
                if (!requestAvailable) throw new LogOutUserException();
                return instCode;
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

        #endregion

        /// <summary>
        /// Determines whether request is available in the specified HTTP context.
        /// To avoid throwing exceptions, set key defined in HttpRequestIsAvailable to true in httpContext.Items
        /// We check httpContext.Items for key defined in <see cref="HttpRequestIsAvailable"/> key
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>
        ///   <c>true</c> if request is available in the specified HTTP context; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsRequestAvailable(HttpContextBase httpContext)
        {
            if (httpContext == null)
                return false;

            try
            {
                // Because I add this item on Application_BeginRequest
                if (!httpContext.Items.Contains(HttpRequestIsAvailable))
                    return false;

                return Convert.ToBoolean(httpContext.Items[HttpRequestIsAvailable]);

                // This crashes where Request is not available.
                //if (httpContext.Request == null)
                //    return false;
            }
            catch (HttpException)
            {
                return false;
            }
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
        public virtual string GetSessionId()
        {
            return _httpContext.Session?.SessionID;
        }

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
        public virtual string GetThisPageUrl(bool includeQueryString = true, bool addHttpMethod = false)
        {
            if (!IsRequestAvailable(_httpContext))
                return string.Empty;

            var url = _httpContext.Request.Url.AbsoluteUri;
            if (addHttpMethod)
                url = _httpContext.Request.HttpMethod + " " + url;

            if (!includeQueryString)
            {
                var qsStart = url.IndexOf('?');
                if (qsStart > -1)
                    url = url.Substring(0, qsStart);

                // append the final backslash
                if (!url.EndsWith("/"))
                    url = url + "/";
            }

            return url;
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
        /// Returns true if the requested resource is a static file.
        /// The key is that from an MVC point of view, if the Physical Path 
        /// has an extension, then it's a physical resource
        /// </summary>
        /// <param name="request">HTTP Request</param>
        /// <returns>True if the request targets a static resource file.</returns>
        public virtual bool IsStaticResource()
        {
            if (_httpContext.Request == null)
                throw new ArgumentNullException("_httpContext.Request");

            string extension = VirtualPathUtility.GetExtension(_httpContext.Request.Path);

            return (!string.IsNullOrWhiteSpace(extension));
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
                    redirectUrl = GetThisPageUrl();
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
