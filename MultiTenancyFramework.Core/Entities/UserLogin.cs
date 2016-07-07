namespace MultiTenancyFramework.Entities
{
    /// <summary>
    /// Mirrows Microsoft.AspNet.Identity.UserLoginInfo. Used to track external logins
    /// </summary>
    public class UserLogin : Entity, IDoNotNeedAudit
    {
        //
        // Summary:
        //     Provider for the linked login, i.e. Facebook, Google, etc.
        public virtual string LoginProvider { get; set; }
        //
        // Summary:
        //     User specific key for the login provider
        public virtual string ProviderKey { get; set; }

        public virtual long UserId { get; set; }
    }
}
