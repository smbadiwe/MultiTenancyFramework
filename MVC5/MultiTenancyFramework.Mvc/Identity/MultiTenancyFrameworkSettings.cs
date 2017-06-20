using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using MultiTenancyFramework.Entities;
using System;

namespace MultiTenancyFramework.Mvc.Identity
{
    /// <summary>
    /// Settings the framework will use for execution
    /// </summary>
    public sealed class MultiTenancyFrameworkSettings
    {
        /// <summary>
        /// Get or set the path to redirect to when the user is signed out. Default is "/Account/Login".
        /// </summary>
        public string LoginPath { get; set; } = "/Account/Login";
        /// <summary>
        /// Default: 14. number of days for CookieAuthenticationOptions.ExpireTimeSpan
        /// </summary>
        public double CookieAuthExpireTime { get; set; } = 14;
        /// <summary>
        /// Default: 30. Time in minutes for CookieAuthenticationOptions.Provider's OnValidateIdentity (=== (SecurityStampValidator.OnValidateIdentity)'s validateInterval
        /// </summary>
        public double SecurityStampValidateInterval { get; set; } = 30;
        /// <summary>
        /// Default: 5. Time in minutes for app.UseTwoFactorSignInCookie's expire parameter
        /// </summary>
        public double TwoFactorSignInCookieExpireTime { get; set; } = 5;

        public CookieAuthenticationOptions GetCookieAuthenticationOptions()
        {
            return new CookieAuthenticationOptions
            {
                ExpireTimeSpan = TimeSpan.FromDays(this.CookieAuthExpireTime),
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString(this.LoginPath),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, IdentityUser, long>(
                                    validateInterval: TimeSpan.FromMinutes(this.SecurityStampValidateInterval),
                                    regenerateIdentityCallback: (manager, user) => user.GenerateUserIdentityAsync(manager),
                                    getUserIdCallback: (claimIdentity) => claimIdentity.GetUserId<long>()
                                    )
                },
                CookieManager = new SystemWebCookieManager() // Microsoft.Owin.Host.SystemWeb.SystemWebCookieManager()
            };
        }
    }
}
