using MultiTenancyFramework.Core.TaskManager.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Linq;
using System;

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
        public virtual async Task<bool> SendEmail(string toEmails, string message, IList<EmailAttachment> attachments = null, string ccEmails = null, string bccEmails = null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(toEmails))
            {
                toEmails = Settings.DefaultEmailReceiver;
            }
            if (string.IsNullOrWhiteSpace(bccEmails))
            {
                bccEmails = Settings.DefaultBccEmailReceiver;
            }
            else if (!string.IsNullOrWhiteSpace(Settings.DefaultBccEmailReceiver))
            {
                bccEmails = string.Format("{0},{1}", Settings.DefaultBccEmailReceiver, bccEmails);
            }

            try
            {
                await SendEmail(
                        emailAccount: null,
                        subject: Settings.DefaultEmailSubject,
                        body: message,
                        fromAddress: Settings.DefaultEmailSender,
                        fromName: Settings.DefaultSenderDisplayName,
                        toAddress: toEmails,
                        toName: null,
                        replyTo: null,
                        replyToName: null,
                        cc: ccEmails?.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries),
                        bcc: bccEmails?.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries),
                        attachments: attachments,
                        attachedDownloadId: 0,
                        headers: null);
            }
            catch (Exception ex)
            {
                Utilities.Logger.Log(ex.GetFullExceptionMessage());
                return false;
            }
            return true;
        }

        /// <summary>
        /// Sends an email. This will crash if anything goes wrong, so if you don't need a crash, swallow it
        /// </summary>
        /// <param name="emailAccount">Email account to use</param>
        /// <param name="subject">Subject</param>
        /// <param name="body">Body</param>
        /// <param name="fromAddress">From address</param>
        /// <param name="fromName">From display name</param>
        /// <param name="toAddress">To address</param>
        /// <param name="toName">To display name</param>
        /// <param name="replyTo">ReplyTo address</param>
        /// <param name="replyToName">ReplyTo display name</param>
        /// <param name="bcc">BCC addresses list</param>
        /// <param name="cc">CC addresses list</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
        /// <param name="attachedDownloadId">Attachment download ID (another attachedment)</param>
        /// <param name="headers">Headers</param>
        public virtual async Task SendEmail(EmailAccount emailAccount, string subject, string body,
            string toAddress, string toName,
             string replyTo = null, string replyToName = null,
            IEnumerable<string> bcc = null, IEnumerable<string> cc = null,
            IEnumerable<EmailAttachment> attachments = null,
            int attachedDownloadId = 0, IDictionary<string, string> headers = null)
        {
            await SendEmail(emailAccount, subject, body, emailAccount.Email, emailAccount.DisplayName, toAddress, toName
                , replyTo, replyToName, bcc, cc, attachments, attachedDownloadId, headers);
        }

        /// <summary>
        /// Sends an email. This will crash if anything goes wrong, so if you don't need a crash, swallow it
        /// </summary>
        /// <param name="emailAccount">Email account to use</param>
        /// <param name="subject">Subject</param>
        /// <param name="body">Body</param>
        /// <param name="fromAddress">From address</param>
        /// <param name="fromName">From display name</param>
        /// <param name="toAddress">To address</param>
        /// <param name="toName">To display name</param>
        /// <param name="replyTo">ReplyTo address</param>
        /// <param name="replyToName">ReplyTo display name</param>
        /// <param name="bcc">BCC addresses list</param>
        /// <param name="cc">CC addresses list</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
        /// <param name="attachedDownloadId">Attachment download ID (another attachedment)</param>
        /// <param name="headers">Headers</param>
        public virtual async Task SendEmail(EmailAccount emailAccount, string subject, string body,
            string fromAddress, string fromName, string toAddress, string toName,
             string replyTo = null, string replyToName = null,
            IEnumerable<string> bcc = null, IEnumerable<string> cc = null,
            IEnumerable<EmailAttachment> attachments = null,
            int attachedDownloadId = 0, IDictionary<string, string> headers = null)
        {

            if (emailAccount == null)
            {
                emailAccount = await new EmailAccountLogic().GetDefaultAccount();
            }
            if (string.IsNullOrWhiteSpace(fromAddress))
            {
                fromAddress = emailAccount.Email;
            }
            if (string.IsNullOrWhiteSpace(fromName))
            {
                fromName = emailAccount.DisplayName;
            }

            //from, to, reply to
            var message = new MailMessage
            {
                From = new MailAddress(fromAddress, fromName)
            };
            message.To.Add(new MailAddress(toAddress, toName));
            if (!string.IsNullOrWhiteSpace(replyTo))
            {
                message.ReplyToList.Add(new MailAddress(replyTo, replyToName));
            }

            //BCC
            if (bcc != null)
            {
                foreach (var address in bcc.Where(bccValue => !string.IsNullOrWhiteSpace(bccValue)))
                {
                    message.Bcc.Add(address.Trim());
                }
            }

            //CC
            if (cc != null)
            {
                foreach (var address in cc.Where(ccValue => !string.IsNullOrWhiteSpace(ccValue)))
                {
                    message.CC.Add(address.Trim());
                }
            }

            //content
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            //headers
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    message.Headers.Add(header.Key, header.Value);
                }
            }
            //create the file attachment for this e-mail message
            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    try
                    {
                        var att = new Attachment(attachment.FilePath);
                        if (!string.IsNullOrWhiteSpace(attachment.MediaType))
                        {
                            att.ContentType = new ContentType(attachment.MediaType);
                        }
                        if (!string.IsNullOrWhiteSpace(attachment.ContentId))
                        {
                            att.ContentId = attachment.ContentId;
                            att.ContentDisposition.Inline = true;
                            att.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                        }
                        //att.ContentDisposition.CreationDate = File.GetCreationTime(attachment.FilePath);
                        //att.ContentDisposition.ModificationDate = File.GetLastWriteTime(attachment.FilePath);
                        //att.ContentDisposition.ReadDate = File.GetLastAccessTime(attachment.FilePath);

                        message.Attachments.Add(att);
                    }
                    catch (Exception)
                    {
                        // We can afford to let attachment not attach. The main message will still be sent
                        //throw;
                    }
                }
            }

            #region if (attachedDownloadId > 0)
            ////another attachment?
            //if (attachedDownloadId > 0)
            //{
            //    var download = _downloadService.GetDownloadById(attachedDownloadId);
            //    if (download != null)
            //    {
            //        //we do not support URLs as attachments
            //        if (!download.UseDownloadUrl)
            //        {
            //            var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : download.Id.ToString();
            //            fileName += download.Extension;


            //            var ms = new MemoryStream(download.DownloadBinary);
            //            var attachment = new Attachment(ms, fileName);
            //            //string contentType = !string.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : "application/octet-stream";
            //            //var attachment = new Attachment(ms, fileName, contentType);
            //            attachment.ContentDisposition.CreationDate = DateTime.UtcNow;
            //            attachment.ContentDisposition.ModificationDate = DateTime.UtcNow;
            //            attachment.ContentDisposition.ReadDate = DateTime.UtcNow;
            //            message.Attachments.Add(attachment);
            //        }
            //    }
            //} 
            #endregion

            //send email
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.UseDefaultCredentials = emailAccount.UseDefaultCredentials;
                smtpClient.Host = emailAccount.Host;
                smtpClient.Port = emailAccount.Port;
                smtpClient.EnableSsl = emailAccount.EnableSsl;
                smtpClient.Credentials = emailAccount.UseDefaultCredentials ?
                    CredentialCache.DefaultNetworkCredentials :
                    new NetworkCredential(emailAccount.Username, emailAccount.Password);

                ServicePointManager.ServerCertificateValidationCallback = (obj, cert, chain, policy) => true;

                await smtpClient.SendMailAsync(message);
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
