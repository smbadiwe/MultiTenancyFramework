using MultiTenancyFramework.Data;
using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using MultiTenancyFramework.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web;

namespace MultiTenancyFramework.Logic
{
    public class AuditLogLogic : CoreLogic<AuditLog>
    {
        private const string SYSTEM_CHANGE_TEXT = "[[SYSTEM CHANGE]]";
        public AuditLogLogic(string institutionCode) : base(MyServiceLocator.GetInstance<ICoreDAO<AuditLog>>(), institutionCode)
        {
        }

        public override string InstitutionCode
        {
            get
            {
                return base.InstitutionCode;
            }
            set
            {
                _dao.InstitutionCode = base.InstitutionCode = value;
            }
        }
        
        public List<TrailItem> GetData(long id)
        {
            var auditTrail = _dao.Retrieve(id);

            if (auditTrail != null && !string.IsNullOrWhiteSpace(auditTrail.AuditData))
            {
                return JsonConvert.DeserializeObject<List<TrailItem>>(auditTrail.AuditData);
            }
            return null;
        }

        public IList<string> RetrieveEntities()
        {
            return QueryProcessor.Process(new GetEntitiesAppearingInAuditLogsQuery());
        }
        
        /// <summary>
        /// Audit Trail at every login or logout attempt. Do not set the second parameter 
        /// if it was possible at all to retrieve the user. In that case, just bundle the user in session.
        /// We'll pick it up from there.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="remarks">Remarks.</param>
        public virtual void AuditLogin(EventType action, string userName = null, string remarks = null, string institutionCode = null)
        {
            //NOTE: A' + B' = (AB)' in boolean algebra
            if (!(ConfigurationHelper.AppSettingsItem<bool>("EnableAuditTrail") && ConfigurationHelper.AppSettingsItem<bool>("TrailLogins")))
            {
                return;
            }
            if (action != EventType.Login && action != EventType.Logout && action != EventType.FailedLogin)
            {
                return;
            }

            AuditLog auditTrail = new AuditLog
            {
                EventType = action,
                EventDate = DateTime.Now.GetLocalTime(),
                Remark = remarks,
                InstitutionCode = institutionCode,
            };
            try
            {
                if (HttpContext.Current != null)
                {
                    if (!string.IsNullOrWhiteSpace(userName)) // usuall when the user fails to type in the right username (action == AuditAction.FAILEDLOGIN)
                    {
                        auditTrail.UserName = userName;
                    }
                    else
                    {
                        var user = WebUtilities.GetCurrentlyLoggedInUser();
                        if (user != null)
                        {
                            auditTrail.UserName = user.UserName;
                            auditTrail.UserId = user.Id;
                        }
                    }
                    auditTrail.ClientIpAddress = IPResolver.GetIP4Address();
                    auditTrail.ClientName = HttpContext.Current.Request.UserHostName;
                }
            }
            catch
            {
                auditTrail.ClientName = "[Could not resolve Client Name]";
            }
        }

    }
}
