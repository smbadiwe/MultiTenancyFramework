using System;

namespace MultiTenancyFramework.Entities
{
    public class Log : BaseEntity, IDoNotNeedAudit, IAmHostedCentrally
    {
        /// <summary>
        /// Gets or sets the short message
        /// </summary>
        public virtual string ShortMessage { get; set; }

        /// <summary>
        /// Gets or sets the full exception
        /// </summary>
        public virtual string FullMessage { get; set; }

        /// <summary>
        /// Gets or sets the IP address
        /// </summary>
        public virtual string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the session id
        /// </summary>
        public virtual string SessionId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier
        /// </summary>
        public virtual string UserId { get; set; }

        /// <summary>
        /// Gets or sets the institution code to which this log record belong.
        /// This MUST be set: if it's central, set it to Utilities.INST_DEFAULT_CODE.
        /// </summary>
        public virtual string InstitutionId { get; set; }

        /// <summary>
        /// Gets or sets the page URL
        /// </summary>
        public virtual string PageUrl { get; set; }

        /// <summary>
        /// Gets or sets the referrer URL
        /// </summary>
        public virtual string ReferrerUrl { get; set; }

        /// <summary>
        /// Gets or sets the logger name.
        /// </summary>
        public virtual string Logger { get; set; }

        /// <summary>
        /// Gets or sets the log level
        /// </summary>
        public virtual LoggingLevel LoggingLevel { get; set; }

    }
}
