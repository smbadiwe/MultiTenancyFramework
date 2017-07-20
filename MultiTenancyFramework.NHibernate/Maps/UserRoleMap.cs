using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.NHibernate.Maps
{
    public class UserRoleMap : EntityMap<UserRole>
    {
        public UserRoleMap()
        {
            Map(x => x.Description);
            Map(x => x.IsSystemProvided);
            Map(x => x.Privileges).VarCharMax();
        }
    }
}
