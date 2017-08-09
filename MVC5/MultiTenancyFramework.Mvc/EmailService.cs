using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;

namespace MultiTenancyFramework.Mvc
{
    public class EmailService : IIdentityMessageService
    {
        public EmailService()
        {
            EmailAndSmtpSetting = Utilities.SystemSettings.EmailAndSmtpSetting;
        }

        public EmailService(EmailAndSmtpSetting emailAndSmtpSetting)
        {
            EmailAndSmtpSetting = emailAndSmtpSetting;
        }

        public EmailAndSmtpSetting EmailAndSmtpSetting { get; set; }

        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            if (string.IsNullOrWhiteSpace(message.Body)) return Task.FromResult(false);

            if (string.IsNullOrWhiteSpace(message.Destination)) message.Destination = EmailAndSmtpSetting.DefaultEmailReceiver;
            var toEmails = message.Destination?.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (toEmails == null || toEmails.Length == 0) return Task.FromResult(false);

            string[] ccEmails, bccEmails;
            bool isBodyHtml = false;
            string fromEmail, displayName;
            EmailMessage emailMsg = message as EmailMessage;
            if (emailMsg != null)
            {
                fromEmail = emailMsg.SenderEmail;
                displayName = emailMsg.SenderDisplayName;
                isBodyHtml = emailMsg.IsBodyHtml;

                ccEmails = emailMsg.CC.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                bccEmails = emailMsg.BCC.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                ccEmails = null;
                bccEmails = null;
                fromEmail = null; // we'll use the default
                displayName = null; // we'll use the default
            }
            var sent = Emailer.SendEmail(fromEmail, message.Body, message.Subject, null, toEmails, ccEmails, bccEmails, isBodyHtml, displayName);
            
            return Task.FromResult(sent);
        }
    }

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
    }
}
