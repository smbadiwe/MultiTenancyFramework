using MultiTenancyFramework.Data;
using MultiTenancyFramework.Logic;
using System.Linq;
using System.Collections.Generic;
using System;

namespace MultiTenancyFramework.Core.TaskManager.Tasks
{
    public class QueuedEmailLogic : CoreLogic<QueuedEmail>
    {
        public QueuedEmailLogic(string institutionCode)
            : base(MyServiceLocator.GetInstance<ICoreDAO<QueuedEmail>>(), institutionCode)
        {

        }

        public override void OnBeforeSaving(QueuedEmail e)
        {
            CheckNonNullables(e);
            base.OnBeforeSaving(e);
        }


        public override void OnBeforeUpdating(QueuedEmail e)
        {
            CheckNonNullables(e);
            base.OnBeforeUpdating(e);
        }

        public override void OnBeforeSavingList(IList<QueuedEmail> e)
        {
            CheckNonNullables(e);
            base.OnBeforeSavingList(e);
        }

        public override void OnBeforeUpdatingList(IList<QueuedEmail> e)
        {
            CheckNonNullables(e);
            base.OnBeforeUpdatingList(e);
        }

        private void CheckNonNullables(IList<QueuedEmail> e)
        {
            foreach (var em in e)
            {
                if (string.IsNullOrWhiteSpace(em.Receivers))
                    throw new ArgumentNullException("To", "The receiver(s) (to) email cannot be null or empty");

                if (string.IsNullOrWhiteSpace(em.Sender))
                {
                    if (em.EmailAccount == null)
                        throw new ArgumentNullException("From", "The sender (from) email cannot be null or empty");
                    else
                        em.Sender = em.EmailAccount.Email;
                }

                if (string.IsNullOrWhiteSpace(em.SenderName) && em.EmailAccount != null)
                    em.SenderName = em.EmailAccount.DisplayName;

                em.SkipAudit = true;
            }
        }

        private void CheckNonNullables(QueuedEmail e)
        {
            if (string.IsNullOrWhiteSpace(e.Receivers))
                throw new ArgumentNullException("To", "The receiver(s) (to) email cannot be null or empty");
            if (string.IsNullOrWhiteSpace(e.Sender))
            {
                if (e.EmailAccount == null)
                    throw new ArgumentNullException("From", "The sender (from) email cannot be null or empty");
                else
                    e.Sender = e.EmailAccount.Email;
            }

            if (string.IsNullOrWhiteSpace(e.SenderName) && e.EmailAccount != null)
                e.SenderName = e.EmailAccount.DisplayName;

            e.SkipAudit = true;
        }
    }
}
