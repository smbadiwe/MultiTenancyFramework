using Microsoft.AspNet.Identity;
using System.Collections.Generic;

namespace MultiTenancyFramework.Mvc
{
    public class EmailMessage : IdentityMessage
    {
        public string SenderDisplayName { get; set; }
        public string SenderEmail { get; set; }
        public string CC { get; set; } = string.Empty;
        public string BCC { get; set; } = string.Empty;
        /// <summary>
        /// Default is true.
        /// </summary>
        public bool IsBodyHtml { get; set; } = true;
        public IList<EmailAttachment> EmailAttachments { get; set; } = new List<EmailAttachment>();
    }
}
