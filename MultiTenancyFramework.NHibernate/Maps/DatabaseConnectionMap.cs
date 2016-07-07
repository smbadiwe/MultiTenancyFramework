using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.NHibernate.Maps
{
    public class DatabaseConnectionMap : EntityMap<DatabaseConnection>
    {
        public DatabaseConnectionMap()
        {
            Map(x => x.Name);
            Map(x => x.ConnectionString);
            Map(x => x.NumberOfInstitutionsCurrentlyHosted);
            Map(x => x.MaximumNumberOfInstitutionsHosted);
            Map(x => x.TenantsOnIt);
        }
    }
}
