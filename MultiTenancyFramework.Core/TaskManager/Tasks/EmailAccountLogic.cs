using MultiTenancyFramework.Data;
using MultiTenancyFramework.Logic;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System;

namespace MultiTenancyFramework.Core.TaskManager.Tasks
{
    public class EmailAccountLogic : CoreLogic<EmailAccount>
    {
        public EmailAccountLogic()
            : base(MyServiceLocator.GetInstance<ICoreDAO<EmailAccount>>())
        {

        }

        public async Task SendTestEmail(EmailAccount account, string email)
        {
            var sender = new EmailSender();
            await sender.SendEmail(account, "Test Email", "This is a test email. If you received it, then the email account setup is working fine.", email, string.Empty);
        }

        // This list is not usually long; usually less than 10.
        // So it's OK with me to sort it in memory.
        public override IList<EmailAccount> RetrieveAllActive(params string[] fields)
        {
            return RetrieveAll(fields).Where(x => x.IsEnabled).ToList();
        }

        public override async Task<IList<EmailAccount>> RetrieveAllActiveAsync(string[] fields = null, CancellationToken token = default(CancellationToken))
        {
            return (await RetrieveAllAsync(fields, token)).Where(x => x.IsEnabled).ToList();
        }

        public override async Task<IList<EmailAccount>> RetrieveAllAsync(string[] fields = null, CancellationToken token = default(CancellationToken))
        {
            var list = await base.RetrieveAllAsync(fields, token);
            if (list == null || list.Count == 0)
            {
                list = await EnsureListExistsAsync(list);
            }
            return list;
        }

        public override IList<EmailAccount> RetrieveAll(params string[] fields)
        {
            var list = base.RetrieveAll(fields);
            if (list == null || list.Count == 0)
            {
                list = EnsureListExists(list);
            }
            return list;
        }

        public async Task<EmailAccount> GetPaymentsAccount()
        {
            var accounts = await RetrieveAllActiveAsync();

            return accounts?.FirstOrDefault(x =>
                x.Scope == EmailAccountScope.Payments
                || x.Scope == EmailAccountScope.Billings
                || x.Scope == EmailAccountScope.Default);
        }

        public async Task<EmailAccount> GetBillingsAccount()
        {
            var accounts = await RetrieveAllActiveAsync();

            return accounts?.FirstOrDefault(x =>
                x.Scope == EmailAccountScope.Billings
                || x.Scope == EmailAccountScope.Default);
        }

        /// <summary>
        /// Gets the logging account. This is guaranteed to never crash.
        /// </summary>
        /// <returns></returns>
        public EmailAccount GetLoggingAccount()
        {
            try
            {
                var accounts = RetrieveAllActive();

                return accounts?.FirstOrDefault(x =>
                    x.Scope == EmailAccountScope.Logging
                    || x.Scope == EmailAccountScope.Default);
            }
            catch (System.Exception)
            {
                var config = Utilities.SystemSettings.EmailAndSmtpSetting;
                return config.ToEmailAccount();
            }
        }

        public async Task<EmailAccount> GetPromoAccount()
        {
            var accounts = await RetrieveAllActiveAsync();

            return accounts?.FirstOrDefault(x =>
                x.Scope == EmailAccountScope.Promo
                || x.Scope == EmailAccountScope.Default);
        }

        public EmailAccount GetDefaultAccount()
        {
            var accounts = RetrieveAllActive();

            return accounts?.FirstOrDefault(x =>
                x.Scope == EmailAccountScope.Default);
        }

        public async Task<EmailAccount> GetDefaultAccountAsync()
        {
            var accounts = await RetrieveAllActiveAsync();

            return accounts?.FirstOrDefault(x =>
                x.Scope == EmailAccountScope.Default);
        }

        public async Task<EmailAccount> GetInfoAccount()
        {
            var accounts = await RetrieveAllActiveAsync();

            return accounts?.FirstOrDefault(x =>
                x.Scope == EmailAccountScope.Default);
        }

        public async Task<EmailAccount> GetByScope(EmailAccountScope scope)
        {
            var accounts = await RetrieveAllActiveAsync();

            Func<EmailAccount, bool> query;
            switch (scope)
            {
                case EmailAccountScope.Payments:
                    query = x => x.Scope == scope || x.Scope == EmailAccountScope.Billings || x.Scope == EmailAccountScope.Default;
                    break;
                case EmailAccountScope.Promo:
                case EmailAccountScope.Logging:
                case EmailAccountScope.Billings:
                    query = x => x.Scope == scope || x.Scope == EmailAccountScope.Default;
                    break;
                default:
                    query = x => x.Scope == scope;
                    break;
            }
            return accounts?.FirstOrDefault(query);
        }

        public override void OnAfterCommittingChanges(EmailAccount e)
        {
            if (e.Scope == EmailAccountScope.Logging)
            {
                Emailer.LoggerEmailAccount = null;
            }
            base.OnAfterCommittingChanges(e);
        }

        public override void OnAfterCommittingListChanges(IList<EmailAccount> e)
        {
            if (e.Any(s => s.Scope == EmailAccountScope.Logging))
            {
                Emailer.LoggerEmailAccount = null;
            }
            base.OnAfterCommittingListChanges(e);
        }

        private async Task<IList<EmailAccount>> EnsureListExistsAsync(IList<EmailAccount> list)
        {
            if (list == null)
                list = new List<EmailAccount>(1);
            if (list.Count > 0)
                return list;

            var config = Utilities.SystemSettings.EmailAndSmtpSetting;
            var newAccount = config.ToEmailAccount();
            await InsertAsync(newAccount);
            list.Add(newAccount);

            return list;
        }

        private IList<EmailAccount> EnsureListExists(IList<EmailAccount> list)
        {
            if (list == null)
                list = new List<EmailAccount>(1);
            if (list.Count > 0)
                return list;

            var config = Utilities.SystemSettings.EmailAndSmtpSetting;
            var newAccount = config.ToEmailAccount();
            Insert(newAccount);
            list.Add(newAccount);

            return list;
        }
    }
}
