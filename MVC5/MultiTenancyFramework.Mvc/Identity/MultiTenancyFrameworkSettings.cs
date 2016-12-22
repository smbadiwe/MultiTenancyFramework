using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Default: 30. Time in minutes for CookieAuthenticationOptions.ExpireTimeSpan
        /// </summary>
        public double CookieAuthExpireTime { get; set; } = 30;
        /// <summary>
        /// Default: 30. Time in minutes for CookieAuthenticationOptions.Provider's OnValidateIdentity (=== (SecurityStampValidator.OnValidateIdentity)'s validateInterval
        /// </summary>
        public double SecurityStampValidateInterval { get; set; } = 30;
        /// <summary>
        /// Default: 5. Time in minutes for app.UseTwoFactorSignInCookie's expire parameter
        /// </summary>
        public double TwoFactorSignInCookieExpireTime { get; set; } = 5;
    }
}
