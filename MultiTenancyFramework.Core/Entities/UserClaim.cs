using System;
using System.Security.Claims;

namespace MultiTenancyFramework.Entities
{
    public class UserClaim : BaseEntity, IDoNotNeedAudit //Claim, 
    {
        public UserClaim(Claim claim) : this(claim.Type, claim.Value, claim.ValueType) { }
        public UserClaim() : this("default", "null", "http://www.w3.org/2001/XMLSchema#string") { }
        public UserClaim(string type, string value, string valueType) //: base(type, value)
        {
            DateCreated = DateTime.Now.GetLocalTime();
            ClaimType = type;
            ClaimValue = value;
            ClaimValueType = valueType;
        }
        
        public virtual long UserId { get; set; } //May need to use User object

        public virtual string ClaimType { get; set; }

        public virtual string ClaimValue { get; set; }

        public virtual string ClaimValueType { get; set; }
        //[System.ComponentModel.DataAnnotations.Schema.NotMapped]
        //public virtual bool SkipAudit { get; set; }


        ////IBaseEntity
        //public virtual DateTime DateCreated { get; set; }
        //public virtual long Id { get; set; }
        //public virtual string InstitutionCode { get; set; }
        //public virtual DateTime LastDateModified { get; set; }
        //public virtual bool IsDisabled { get; set; }
        //public virtual bool IsDeleted { get; set; }
        //public virtual bool IsEnabled { get { return !IsDisabled; } }

        //public virtual long CreatedBy { get; set; }

        //public virtual string GetTableName()
        //{
        //    return GetType().Name.ToPlural();
        //}
    }
}
