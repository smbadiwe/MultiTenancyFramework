using System;
using System.Threading.Tasks;
using MultiTenancyFramework.Core.TaskManager;
using MultiTenancyFramework.Core.TaskManager.Tasks;

namespace MultiTenancyFramework.Tasks
{
    public class QueuedMessagesSendTask : IRunnableTask
    {
        public ScheduledTask DefaultTaskPlan
        {
            get
            {
                return new ScheduledTask
                {
                    Name = "Send emails",
                    Seconds = 60,
                    Type = typeof(QueuedMessagesSendTask).AssemblyQualifiedName,
                    IsDisabled = false,
                    StopOnError = false,
                };
            }
        }

        public OwnershipType OwnershipType
        {
            get
            {
                return OwnershipType.PerInstitution;
            }
        }

        public async Task Execute(string institutionCode)
        {
            var logger = Utilities.Logger;
            logger.SetNLogLogger("QueuedMessagesSendTask");
            logger.Trace("QueuedMessagesSendTask: Executing now...");
            try
            {
                var _queuedEmailService = new QueuedEmailLogic(institutionCode);
                var query = new SearchQueuedEmailsQuery
                {
                    LoadNotSentItemsOnly = true,
                    LoadOnlyItemsToBeSent = true,
                    MaxSendTries = 3,
                    PageSize = 500
                };
                var processor = Utilities.QueryProcessor;
                processor.InstitutionCode = institutionCode;
                var queuedEmails = processor.Process(query);
                logger.Trace($"QueuedMessagesSendTask: {queuedEmails.DataBatch.Count} queued emails to be processed now");
                if (queuedEmails.DataBatch.Count == 0) return;

                var emailSender = new EmailSender();
                foreach (var queuedEmail in queuedEmails.DataBatch)
                {
                    var bcc = string.IsNullOrWhiteSpace(queuedEmail.Bcc)
                                ? null
                                : queuedEmail.Bcc.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                    var cc = string.IsNullOrWhiteSpace(queuedEmail.CC)
                                ? null
                                : queuedEmail.CC.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                    var attachment = string.IsNullOrWhiteSpace(queuedEmail.AttachmentFilePath)
                                        ? null
                                        : new[] { new EmailAttachment {
                                        FilePath = queuedEmail.AttachmentFilePath,
                                        MediaType = Utilities.GetMimeType(queuedEmail.AttachmentFilePath),
                                    } };
                    try
                    {

                        await emailSender.SendEmail(queuedEmail.EmailAccount,
                                queuedEmail.Subject,
                                queuedEmail.Body,
                                queuedEmail.Sender,
                                queuedEmail.SenderName,
                                queuedEmail.Receivers,
                                queuedEmail.ReceiverName,
                                queuedEmail.ReplyTo,
                                queuedEmail.ReplyToName,
                                bcc,
                                cc);

                        queuedEmail.SentOnUtc = DateTime.UtcNow;
                    }
                    catch (Exception exc)
                    {
                        logger.Error($"QueuedMessagesSendTask: Error sending e-mail. {exc.Message}.\n{exc.GetFullExceptionMessage()}");
                    }
                    finally
                    {
                        queuedEmail.SentTries++;
                        _queuedEmailService.Update(queuedEmail);
                    }
                }
            }
            finally
            {
                logger.Trace("QueuedMessagesSendTask: All done. Winding up now...");
                logger.SetNLogLogger(null);
            }
        }
    }
}
