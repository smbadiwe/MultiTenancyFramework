namespace MultiTenancyFramework
{
    [System.ComponentModel.DataAnnotations.Schema.ComplexType]
    public class EmailAndSmtpSetting
    {
        public virtual string DefaultEmailSubject { get; set; } = "Alert";
        public virtual string DefaultEmailReceiver { get; set; }
        public virtual string DefaultEmailSender { get; set; }
        public virtual string DefaultSenderDisplayName { get; set; }
        public virtual string SmtpUsername { get; set; }
        public virtual string SmtpPassword { get; set; }
        public virtual string SmtpHost { get; set; } = "smtp.gmail.com";
        public virtual int SmtpPort { get; set; } = 587;
        public virtual bool EnableSSL { get; set; } = true;

        // for API
        public virtual string ApiKey { get; set; }
        public virtual string ApiBaseUrl { get; set; }
        public virtual string ApiRequestUrl { get; set; }
    }
}
