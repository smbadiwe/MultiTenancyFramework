using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace MultiTenancyFramework.Mvc
{
    public class EmailService : IIdentityMessageService
    {
        private EmailAndSmtpSetting _settings;

        public EmailService() : this(null) { }

        public EmailService(EmailAndSmtpSetting settings)
        {
            if (settings == null)
                settings = Utilities.SystemSettings?.EmailAndSmtpSetting ?? new EmailAndSmtpSetting();
            this._settings = settings;
        }

        public virtual Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            if (string.IsNullOrWhiteSpace(message.Body)) return Task.FromResult(false);

            if (string.IsNullOrWhiteSpace(message.Destination)) message.Destination = _settings.DefaultEmailReceiver;

            if (!string.IsNullOrWhiteSpace(message.Subject))
                _settings.DefaultEmailSubject = message.Subject;

            EmailMessage emailMsg = message as EmailMessage;
            if (emailMsg != null)
            {
                if (!string.IsNullOrWhiteSpace(emailMsg.SenderEmail))
                    _settings.DefaultEmailSender = emailMsg.SenderEmail;
                if (!string.IsNullOrWhiteSpace(emailMsg.SenderDisplayName))
                    _settings.DefaultSenderDisplayName = emailMsg.SenderDisplayName;
            }

            return new EmailSender(_settings)
                .SendEmail(message.Destination, message.Body, emailMsg?.EmailAttachments, ccEmails: emailMsg?.CC, bccEmails: emailMsg?.BCC);
        }
    }

}
