using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.Core.TaskManager.Tasks
{
    /// <summary>
    /// Represents an email account. It's hosted centrally.
    /// </summary>
    public class EmailAccount : BaseEntity, IAmHostedCentrally
    {
        /// <summary>
        /// Gets or sets an email address
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Gets or sets an email display name
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        public virtual EmailAccountScope Scope { get; set; }

        /// <summary>
        /// Gets or sets the type of the account.
        /// </summary>
        public virtual EmailAccountType AccountType { get; set; }

        /// <summary>
        /// Gets or sets the default email address to BCC.
        /// </summary>
        public virtual string DefaultBcc { get; set; }

        /// <summary>
        /// Gets or sets an email host
        /// Default is smtp.gmail.com
        /// </summary>
        public virtual string Host { get; set; } = "smtp.gmail.com";

        /// <summary>
        /// Gets or sets an email port
        /// Default is 587
        /// </summary>
        public virtual int Port { get; set; } = 587;

        /// <summary>
        /// Gets or sets an email user name
        /// </summary>
        public virtual string Username { get; set; }

        /// <summary>
        /// Gets or sets an email password
        /// </summary>
        public virtual string Password { get; set; }

        /// <summary>
        /// Gets or sets a value that controls whether the SmtpClient uses Secure Sockets Layer (SSL) to encrypt the connection. 
        /// Default is true
        /// </summary>
        public virtual bool EnableSsl { get; set; } = true;

        /// <summary>
        /// Gets or sets a value that controls whether the default system credentials of the application are sent with requests.
        /// </summary>
        public virtual bool UseDefaultCredentials { get; set; }

        // for API
        public virtual string ApiKey { get; set; }
        public virtual string ApiBaseUrl { get; set; }
        public virtual string ApiRequestUrl { get; set; }

        /// <summary>
        /// Gets a friendly email account name
        /// </summary>
        public virtual string FriendlyName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.DisplayName))
                    return this.Email + " (" + this.DisplayName + ")";
                return this.Email;
            }
        }
    }

    public enum EmailAccountType
    {
        [EnumDescription("SMTP-Based")]
        SmtpBased,
        [EnumDescription("API-Based")]
        ApiBased
    }

    public enum EmailAccountScope
    {
        /// <summary>
        /// The default. could be info@...
        /// </summary>
        Default,
        Billings,
        Payments,
        Logging,
        Promo,
    }
}
