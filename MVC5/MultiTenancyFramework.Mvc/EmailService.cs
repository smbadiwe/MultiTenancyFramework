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

            var settings = Utilities.SystemSettings?.EmailAndSmtpSetting ?? new EmailAndSmtpSetting();
            if (!string.IsNullOrWhiteSpace(message.Subject))
                settings.DefaultEmailSubject = message.Subject;

            EmailMessage emailMsg = message as EmailMessage;
            if (emailMsg != null)
            {
                if (!string.IsNullOrWhiteSpace(emailMsg.SenderEmail))
                    settings.DefaultEmailSender = emailMsg.SenderEmail;
                if (!string.IsNullOrWhiteSpace(emailMsg.SenderDisplayName))
                    settings.DefaultSenderDisplayName = emailMsg.SenderDisplayName;
            }

            return new EmailSender(settings)
                .SendAsync(message.Destination, message.Body, ccEmails: emailMsg.CC, bccEmails: emailMsg.BCC);
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
