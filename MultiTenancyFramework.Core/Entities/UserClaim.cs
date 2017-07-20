using System;
using System.Security.Claims;

namespace MultiTenancyFramework.Entities
{
    public class UserClaim : Claim, IBaseEntity, IDoNotNeedAudit
    {
        public UserClaim() : this("default", "null") { }
        public UserClaim(string type, string value) : base(type, value)
        {
            DateCreated = DateTime.Now;
        }
        
        public virtual long UserId { get; set; } //May need to use User object

        public virtual string ClaimType { get; set; }

        public virtual string ClaimValue { get; set; }
        public virtual bool SkipAudit { get; set; }


        //IBaseEntity
        public virtual DateTime DateCreated { get; set; }
        public virtual long Id { get; set; }
        public virtual string InstitutionCode { get; set; }
        public virtual DateTime LastDateModified { get; set; }
        public virtual bool IsDisabled { get; set; }
        public virtual bool IsDeleted { get; set; }
        public virtual bool IsEnabled { get { return !IsDisabled; } }

        public virtual long CreatedBy { get; set; }

        public virtual string GetTableName()
        {
            return GetType().Name.ToPlural();
        }
    }
}
