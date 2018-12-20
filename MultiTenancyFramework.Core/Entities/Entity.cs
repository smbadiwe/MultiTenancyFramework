using System;

namespace MultiTenancyFramework.Entities
{

    public abstract class Entity : Entity<long>, IEntity, IBaseEntity
    {
    }

    public abstract class Entity<idT> : BaseEntity<idT>, IEntity<idT>, IBaseEntity<idT> where idT : IEquatable<idT>
    {
        public virtual string Name { get; set; }
        public override string ToString()
        {
            return $"Name: {Name}; Id: {Id}; Institution Code: {InstitutionCode}";
        }
    }

    public abstract class BaseEntity : BaseEntity<long>, IBaseEntity
    {
    }

    public abstract class BaseEntity<idT> : IBaseEntity<idT> where idT : IEquatable<idT>
    {
        //[IgnoreInAuditLog]
        public virtual DateTime DateCreated { get; set; } = DateTime.Now.GetLocalTime();
        public virtual idT Id { get; set; }

        /// <summary>
        /// The user ID of the user that saved the entity
        /// </summary>
        //[IgnoreInAuditLog]
        public virtual idT CreatedBy { get; set; }

        /// <summary>
        /// In most cases, Institution Code is used as the discriminator where more than one institutions share database.
        /// If you need to add institution code as part of an entity but with meaning other that this, you're better off
        /// creating another property.
        /// </summary>
        //[IgnoreInAuditLog]
        public virtual string InstitutionCode { get; set; } = string.Empty; //This is important for our DB sake
        //[IgnoreInAuditLog]
        public virtual DateTime LastDateModified { get; set; } = DateTime.Now.GetLocalTime();
        public virtual bool IsDisabled { get; set; }
        public virtual bool IsDeleted { get; set; }
        /// <summary>
        /// Sometines we don't want to log the change. This is usually when the change was not done by a user;
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public virtual bool SkipAudit { get; set; }

        public virtual bool IsEnabled { get { return !IsDisabled; } }

        /// <summary>
        /// This default implementation returns the plural of the type name using the string extension method .ToPlural() in MultiTenancyFramework namespace
        /// </summary>
        /// <returns></returns>
        public virtual string GetTableName()
        {
            return GetType().Name.ToPlural();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;

            return IsSameAs((BaseEntity<idT>)obj);
        }

        public override int GetHashCode()
        {
            var code = Id.GetHashCode(); // + base.GetHashCode();
            if (string.IsNullOrWhiteSpace(InstitutionCode)) code += InstitutionCode.GetHashCode();
            return code;
        }

        public override string ToString()
        {
            return $"Id: {Id}; Institution Code: {InstitutionCode};";
        }

        private bool IsSameAs(BaseEntity<idT> other)
        {
            return Id.Equals(other.Id) && InstitutionCode == other.InstitutionCode;
        }

    }
}
