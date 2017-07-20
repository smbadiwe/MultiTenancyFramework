using FluentNHibernate.Mapping;
using MultiTenancyFramework.Entities;
using MultiTenancyFramework.NHibernate.NHManager.Listeners;
using System;

namespace MultiTenancyFramework.NHibernate.Maps
{
    public class EntityMap<T> : EntityMap<T, long> where T : class, IEntity
    {

    }

    public class EntityMap<T, idT> : BaseEntityMap<T, idT> where T : class, IEntity<idT> where idT : IEquatable<idT>
    {
        public EntityMap()
        {
            Map(x => x.Name);
        }
    }

    public class BaseEntityMap<T> : BaseEntityMap<T, long> where T : class, IBaseEntity
    {

    }

    public class BaseEntityMap<T, idT> : ClassMap<T> where T : class, IBaseEntity<idT> where idT : IEquatable<idT>
    {
        public BaseEntityMap()
        {
            Id(x => x.Id);
            Map(x => x.InstitutionCode).Index("ind_InstitutionCode");
            Map(x => x.IsDeleted);
            Map(x => x.IsDisabled);
            Map(x => x.DateCreated);
            Map(x => x.CreatedBy);
            Map(x => x.LastDateModified);

            //To filter queries based on what I've defined in the Tenant filter definition
            ApplyFilter<AppFilterDefinition>();
        }

    }
}
