namespace MultiTenancyFramework.Entities
{
    /// <summary>
    /// Captures settings that apply globally, i.e. independent of the institutions using the system
    /// </summary>
    public class SystemSetting : BaseEntity, IAmHostedCentrally
    {
        public virtual string ApplicationName { get; set; }
        public virtual bool EmailLogMessages { get; set; }
        public virtual UsernameAndPasswordRule UsernameAndPasswordRule { get; set; } = new UsernameAndPasswordRule();
        public virtual EmailAndSmtpSetting EmailAndSmtpSetting { get; set; } = new EmailAndSmtpSetting();

        /// <summary>
        /// The folder where institutions' logo images will be stored
        /// </summary>
        public virtual string LogoImageFolder { get; set; }
    }
}
