using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MultiTenancyFramework.Entities;
using MultiTenancyFramework.NHibernate.NHManager.Listeners;
using System;

namespace MultiTenancyFramework.NHibernate.Maps
{
    /// <summary>
    /// NOTE: When overriding Override with this, remember to call base.Override(...)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class EntityAutoMap<T> : EntityAutoMap<T, long> where T : class, IBaseEntity
    {

    }

    /// <summary>
    /// NOTE: When overriding Override with this, remember to call base.Override(...)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class EntityAutoMap<T, idT> : IAutoMappingOverride<T> where T : class, IBaseEntity<idT> where idT : IEquatable<idT>
    {
        public virtual void Override(AutoMapping<T> mapping)
        {
            mapping.Map(x => x.InstitutionCode).Index("ind_InstitutionCode");
            mapping.ApplyFilter<AppFilterDefinition>();
        }
    }
}
