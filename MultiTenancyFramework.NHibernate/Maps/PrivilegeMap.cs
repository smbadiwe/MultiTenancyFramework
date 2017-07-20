using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.NHibernate.Maps
{
    public sealed class PrivilegeMap : PrivilegeMap<Privilege>
    {
    }

    public class PrivilegeMap<T> : EntityMap<T> where T : Privilege
    {
        public PrivilegeMap()
        {
            Table("Privileges");
            Map(x => x.Description);
            Map(x => x.Scope);
            Map(x => x.DisplayName);
            Map(x => x.IsDefault);
        }
    }
}
