using System;

namespace MultiTenancyFramework.Entities
{
    public interface IEntity : IEntity<long>
    {
    }

    public interface IEntity<idT> where idT : IEquatable<idT>
    {
        idT Id { get; set; }
        /// <summary>
        /// The tenant discriminator
        /// </summary>
        string InstitutionCode { get; set; }
        /// <summary>
        /// Most entities will have a name;so this is provided for convenience
        /// </summary>
        string Name { get; set; }
        bool IsDisabled { get; set; }
        bool IsDeleted { get; set; }
        bool IsEnabled { get; }
        DateTime DateCreated { get; set; }
        DateTime LastDateModified { get; set; }
        /// <summary>
        /// Sometimes it may be necessary so skip audit for a particular operation. This is usually for occasions when action is not user-initiated
        /// </summary>
        bool SkipAudit { get; set; }
        /// <summary>
        /// The user ID of the user that saved the entity
        /// </summary>
        idT CreatedBy { get; set; }
    }
}
