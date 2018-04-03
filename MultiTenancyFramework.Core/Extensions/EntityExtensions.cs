using MultiTenancyFramework.Core.TaskManager.Tasks;
using MultiTenancyFramework.Data;
using System.Linq;

namespace MultiTenancyFramework
{
    public static class EntityExtensions
    {
        public static RetrievedData<TSub> Cast<T, TSub>(this RetrievedData<T> list)
        {
            return new RetrievedData<TSub>
            {
                DataBatch = list.DataBatch.Cast<TSub>().ToList(),
                TotalCount = list.TotalCount
            };
        }

        /// <summary>
        /// Gets an email account instance. A new one is created; as opposed to reading from DB
        /// </summary>
        /// <param name="Settings">The settings.</param>
        /// <returns></returns>
        public static EmailAccount ToEmailAccount(this EmailAndSmtpSetting Settings)
        {
            return new EmailAccount
            {
                DisplayName = Settings.DefaultSenderDisplayName,
                Username = Settings.SmtpUsername,
                Password = Settings.SmtpPassword,
                Host = Settings.SmtpHost,
                Port = Settings.SmtpPort,
                EnableSsl = Settings.EnableSSL,
                UseDefaultCredentials = false,
                Email = Settings.DefaultEmailSender
            };
        }
    }
}
