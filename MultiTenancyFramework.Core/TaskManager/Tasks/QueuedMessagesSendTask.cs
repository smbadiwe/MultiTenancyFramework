using System;
using System.Threading.Tasks;

namespace MultiTenancyFramework.Core.TaskManager.Tasks
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
            var queuedEmails = await processor.ProcessAsync(query);

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
                            queuedEmail.From,
                            queuedEmail.FromName,
                            queuedEmail.To,
                            queuedEmail.ToName,
                            queuedEmail.ReplyTo,
                            queuedEmail.ReplyToName,
                            bcc,
                            cc);

                    queuedEmail.SentOnUtc = DateTime.UtcNow;
                }
                catch (Exception exc)
                {
                    Utilities.Logger.Log($"QueuedMessagesSendTask: Error sending e-mail. {exc.Message}.\n{exc.GetFullExceptionMessage()}");
                }
                finally
                {
                    queuedEmail.SentTries++;
                    _queuedEmailService.Update(queuedEmail);
                }
            }
        }
    }
}
