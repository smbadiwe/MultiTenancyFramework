using MultiTenancyFramework.Data;
using MultiTenancyFramework.Logic;
using System.Linq;
using System.Collections.Generic;
using System;

namespace MultiTenancyFramework.Core.TaskManager.Tasks
{
    public class QueuedEmailLogic : CoreLogic<QueuedEmail>
    {
        private EmailAccountLogic emailAccountLogic;
        public QueuedEmailLogic(string institutionCode)
            : base(MyServiceLocator.GetInstance<ICoreDAO<QueuedEmail>>(), institutionCode)
        {
            emailAccountLogic = new EmailAccountLogic();
        }

        public override void OnBeforeSaving(QueuedEmail e)
        {
            if (e == null) return;

            CheckNonNullables(e);
            base.OnBeforeSaving(e);
        }


        public override void OnBeforeUpdating(QueuedEmail e)
        {
            if (e == null) return;

            CheckNonNullables(e);
            base.OnBeforeUpdating(e);
        }

        public override void OnBeforeSavingList(IList<QueuedEmail> e)
        {
            if (e == null || e.Count == 0) return;

            CheckNonNullables(e);
            base.OnBeforeSavingList(e);
        }

        public override void OnBeforeUpdatingList(IList<QueuedEmail> e)
        {
            if (e == null || e.Count == 0) return;

            CheckNonNullables(e);
            base.OnBeforeUpdatingList(e);
        }

        private void CheckNonNullables(IList<QueuedEmail> e)
        {
            var accounts = emailAccountLogic.RetrieveAllActive();
            var defaultAccount = accounts.FirstOrDefault(x =>
                x.Scope == EmailAccountScope.Default);
            foreach (var em in e)
            {
                if (string.IsNullOrWhiteSpace(em.Receivers))
                    throw new ArgumentNullException("To", "The receiver(s) (to) email cannot be null or empty");

                if (em.EmailAccountId > 0)
                    em.SetEmailAccount(accounts.FirstOrDefault(x => x.Id == em.EmailAccountId));
                else
                    em.SetEmailAccount(defaultAccount);

                if (string.IsNullOrWhiteSpace(em.Sender))
                {
                    if (em.GetEmailAccount() == null)
                        throw new ArgumentNullException("From", "The sender (from) email cannot be null or empty");
                    else
                        em.Sender = em.GetEmailAccount().Email;
                }

                if (string.IsNullOrWhiteSpace(em.SenderName) && em.GetEmailAccount() != null)
                    em.SenderName = em.GetEmailAccount().DisplayName;

                em.SkipAudit = true;
            }
        }

        private void CheckNonNullables(QueuedEmail e)
        {
            if (e == null) return;

            if (string.IsNullOrWhiteSpace(e.Receivers))
                throw new ArgumentNullException("To", "The receiver(s) (to) email cannot be null or empty");

            if (e.EmailAccountId > 0)
                e.SetEmailAccount(emailAccountLogic.Retrieve(e.EmailAccountId));
            else
                e.SetEmailAccount(emailAccountLogic.GetDefaultAccount());

            if (string.IsNullOrWhiteSpace(e.Sender))
            {
                if (e.GetEmailAccount() == null)
                    throw new ArgumentNullException("From", "The sender (from) email cannot be null or empty");
                else
                    e.Sender = e.GetEmailAccount().Email;
            }

            if (string.IsNullOrWhiteSpace(e.SenderName) && e.GetEmailAccount() != null)
                e.SenderName = e.GetEmailAccount().DisplayName;

            e.SkipAudit = true;
        }
    }
}
