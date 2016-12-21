using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using System.Web.Http.ExceptionHandling;

namespace MultiTenancyFramework.WebAPI
{
    public static class GlobalWebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            
            config.Filters.Add(new GlobalAuthorizeAttribute());
            config.Filters.Add(new GlobalExceptionFilterAttribute());
            config.Services.Replace(typeof(IExceptionLogger), new GlobalExceptionLogger());
            config.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());

            // Web API routes
            config.MapHttpAttributeRoutes();

            GlobalWebApiRoutesConfig.RegisterRoutes(config.Routes);
        }
    }
}
