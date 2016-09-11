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
    }
}
