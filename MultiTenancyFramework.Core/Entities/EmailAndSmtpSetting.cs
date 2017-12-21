namespace MultiTenancyFramework
{
    [System.ComponentModel.DataAnnotations.Schema.ComplexType]
    public class EmailAndSmtpSetting
    {
        public virtual string DefaultEmailSubject { get; set; } = "Attention Please!";
        /// <summary>
        /// Gets or sets the default email receiver. If more than one, separate by comma
        /// </summary>
        /// <value>
        /// The default email receiver.
        /// </value>
        public virtual string DefaultEmailReceiver { get; set; }
        /// <summary>
        /// Gets or sets the default BCC email receiver. If more than one, separate by comma. This is mailny used for testing that email was actually delivered.
        /// </summary>
        /// <value>
        /// The default BCC email receiver.
        /// </value>
        public virtual string DefaultBccEmailReceiver { get; set; }
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
