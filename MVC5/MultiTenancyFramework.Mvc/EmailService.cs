using Microsoft.AspNet.Identity;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MultiTenancyFramework.Mvc
{
    public class EmailService : IIdentityMessageService
    {
        public EmailService()
        {
            EmailAndSmtpSetting = MvcUtility.SystemSettings.EmailAndSmtpSetting;
        }

        public EmailService(EmailAndSmtpSetting emailAndSmtpSetting)
        {
            EmailAndSmtpSetting = emailAndSmtpSetting;
        }

        public EmailAndSmtpSetting EmailAndSmtpSetting { get; set; }

        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            try
            {
                if (string.IsNullOrWhiteSpace(message.Body)) return Task.FromResult(false);
                
                if (string.IsNullOrWhiteSpace(message.Destination)) message.Destination = EmailAndSmtpSetting.DefaultEmailReceiver;
                var toEmails = message.Destination.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (toEmails == null || toEmails.Length == 0) return Task.FromResult(false);

                // Construct MailMessage instance
                var msg = new MailMessage
                {
                    Body = message.Body,
                    Subject = message.Subject ?? EmailAndSmtpSetting.DefaultEmailSubject,
                };
                foreach (var toemail in toEmails)
                {
                    msg.To.Add(toemail);
                }
                EmailMessage emailMsg = message as EmailMessage;
                if (emailMsg != null)
                {
                    if (string.IsNullOrWhiteSpace(emailMsg.SenderEmail)) emailMsg.SenderEmail = EmailAndSmtpSetting.DefaultEmailSender ?? EmailAndSmtpSetting.SmtpUsername;
                    if (string.IsNullOrWhiteSpace(emailMsg.SenderDisplayName)) emailMsg.SenderDisplayName = EmailAndSmtpSetting.DefaultSenderDisplayName;
                    msg.Sender = msg.From = new MailAddress(emailMsg.SenderEmail, emailMsg.SenderDisplayName);
                    msg.IsBodyHtml = emailMsg.IsBodyHtml;

                    var ccEmails = emailMsg.CC.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                    if (ccEmails != null && ccEmails.Length > 0)
                    {
                        foreach (var email in ccEmails)
                        {
                            msg.CC.Add(new MailAddress(email));
                        }
                    }
                    var bccEmails = emailMsg.BCC.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                    if (bccEmails != null && bccEmails.Length > 0)
                    {
                        foreach (var email in bccEmails)
                        {
                            msg.Bcc.Add(new MailAddress(email));
                        }
                    }
                }

                // Construct SmtpClient
                SmtpClient client = new SmtpClient
                {
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = EmailAndSmtpSetting.EnableSSL,
                    Host = EmailAndSmtpSetting.SmtpHost,
                    Port = EmailAndSmtpSetting.SmtpPort,
                    Credentials = new NetworkCredential(EmailAndSmtpSetting.SmtpUsername, EmailAndSmtpSetting.SmtpPassword),
                };

                // Send
                Task.Factory.StartNew(() =>
                {
                    using (client)
                    {
                        using (msg)
                        {
                            ServicePointManager.ServerCertificateValidationCallback = (obj, cert, chain, policy) => true;
                            client.Send(msg);
                        }
                    }
                });
            }
            catch { }
            return Task.FromResult(false);
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
