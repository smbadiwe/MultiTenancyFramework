namespace MultiTenancyFramework
{
    [System.ComponentModel.DataAnnotations.Schema.ComplexType]
    public class UsernameAndPasswordRule
    {
        /// <summary>
        /// Only allow [A-Za-z0-9@_] in UserNames
        /// </summary>
        [DisplayHint("Allow Only alphanumeric characters and a few specials for username (i.e. A-Za-z0-9@_)")]
        public virtual bool AllowOnlyAlphanumericUserNames { get; set; }
        /// <summary>
        /// If true, enforces that emails are non empty, valid, and unique
        /// </summary>
        [DisplayHint("Require a unique email for every user. If true, this email will be verified and used as login username")]
        public virtual bool RequireUniqueEmail { get; set; } = true;
        [DisplayHint("If true, users who enter wrong credentials repeatedly during login will be locked out of the system for a while.")]
        public virtual bool UserLockoutEnabledByDefault { get; set; } = true;
        [DisplayHint("Max failed access attempts before lockout")]
        public virtual int MaxFailedAccessAttemptsBeforeLockout { get; set; } = 5;
        [DisplayHint("How long in minutes we will lock an erring user out of the system")]
        public virtual int DefaultAccountLockoutTimeSpanInMinutes { get; set; } = 5;
        [DisplayHint("Minimum number of characters a user's password should contain.")]
        public virtual int PasswordRequiredLength { get; set; } = 8;

        [DisplayHint("If true, all passwords must contain at least one special characted")]
        public virtual bool PasswordRequireNonLetterOrDigit { get; set; } = true;
        [DisplayHint("If true, all passwords must contain at least one digit: (0-9)")]
        public virtual bool PasswordRequireDigit { get; set; } = true;
        [DisplayHint("If true, all passwords must contain at least one lower-case letter: (a-z)")]
        public virtual bool PasswordRequireLowercase { get; set; } = true;
        [DisplayHint("If true, all passwords must contain at least one upper-case letter: (A-Z)")]
        public virtual bool PasswordRequireUppercase { get; set; } = true;
    }
}
