using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.NHibernate.Maps
{
    public class AuditLogMap : IAutoMappingOverride<AuditLog>
    {
        public void Override(AutoMapping<AuditLog> mapping)
        {
            mapping.Map(x => x.AuditData).VarCharMax();
        }
    }
}
