using System.Collections.Generic;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace MultiTenancyFramework
{
    public class EmailSender
    {
        public EmailAndSmtpSetting Settings { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailSender"/> class.
        /// Use <see cref="Settings"/> property to configure email subject and sender information.
        /// </summary>
        public EmailSender() : this(null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailSender"/> class.
        /// Use <see cref="settings"/> property to configure email subject and sender information.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public EmailSender(EmailAndSmtpSetting settings)
        {
            if (settings == null)
                settings = Utilities.SystemSettings?.EmailAndSmtpSetting ?? new EmailAndSmtpSetting();
            this.Settings = settings;
        }

        /// <summary>
        /// Sends the mail via mailgun (SMTP). Use <see cref="Settings"/> property to configure email subject and sender information.
        /// </summary>
        /// <param name="toEmails">To.</param>
        /// <param name="message">The message.</param>
        /// <param name="attachments">Attachments. Key is the file path; value is the content type.</param>
        public virtual async Task<bool> SendAsync(string toEmails, string message, IList<EmailAttachment> attachments = null, string ccEmails = null, string bccEmails = null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(toEmails))
            {
                toEmails = Settings.DefaultEmailReceiver;
            }
            try
            {
                var mailMsg = new MailMessage
                {
                    Subject = Settings.DefaultEmailSubject,
                    IsBodyHtml = true,
                    Body = message,
                    Sender = new MailAddress(Settings.DefaultEmailSender, Settings.DefaultSenderDisplayName),
                    From = new MailAddress(Settings.DefaultEmailSender, Settings.DefaultSenderDisplayName)
                };
                mailMsg.To.Add(toEmails);
                if (!string.IsNullOrWhiteSpace(ccEmails))
                {
                    mailMsg.CC.Add(ccEmails);
                }
                if (!string.IsNullOrWhiteSpace(bccEmails))
                {
                    mailMsg.Bcc.Add(bccEmails);
                }
                if (attachments != null && attachments.Count > 0)
                {
                    foreach (var attachment in attachments)
                    {
                        var att = new Attachment(attachment.FilePath, attachment.MediaType);
                        if (!string.IsNullOrWhiteSpace(attachment.ContentId))
                        {
                            att.ContentId = attachment.ContentId;
                            att.ContentDisposition.Inline = true;
                            att.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                        }
                        mailMsg.Attachments.Add(att);
                    }
                }

                // Send
                using (mailMsg)
                {
                    using (var client = Emailer.GetDefaultClient())
                    {
                        return await Emailer.SendEmail(mailMsg, client);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Utilities.Logger.Log(ex.GetFullExceptionMessage());
                return false;
            }
        }

        public virtual bool IsValidEmail(string email)
        {
            try
            {
                var mail = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }

    public class EmailAttachment
    {
        public string FilePath { get; set; }
        public string MediaType { get; set; }
        public string ContentId { get; set; }
    }

}
