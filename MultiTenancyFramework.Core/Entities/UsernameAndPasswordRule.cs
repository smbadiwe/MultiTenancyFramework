
namespace MultiTenancyFramework.Entities
{
    // Maintain exact copy with .Mvc.UsernameAndPasswordRule 
    // w.r.t property names and types
    [System.ComponentModel.DataAnnotations.Schema.ComplexType]
    public class UsernameAndPasswordRule
    {
        /// <summary>
        /// Only allow [A-Za-z0-9@_] in UserNames
        /// </summary>
        public virtual bool AllowOnlyAlphanumericUserNames { get; set; }
        /// <summary>
        /// If true, enforces that emails are non empty, valid, and unique
        /// </summary>
        public virtual bool RequireUniqueEmail { get; set; } = true;
        public virtual bool UserLockoutEnabledByDefault { get; set; } = true;
        public virtual int MaxFailedAccessAttemptsBeforeLockout { get; set; } = 5;
        public virtual int DefaultAccountLockoutTimeSpanInMinutes { get; set; } = 5;
        public virtual int PasswordRequiredLength { get; set; } = 8;

        public virtual bool PasswordRequireNonLetterOrDigit { get; set; } = true;
        public virtual bool PasswordRequireDigit { get; set; } = true;
        public virtual bool PasswordRequireLowercase { get; set; } = true;
        public virtual bool PasswordRequireUppercase { get; set; } = true;
    }
}
