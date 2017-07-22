using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.NHibernate.Maps
{
    public class UserRoleMap : IAutoMappingOverride<UserRole>
    {
        public void Override(AutoMapping<UserRole> mapping)
        {
            mapping.Map(x => x.Privileges).VarCharMax();
        }
    }
}
