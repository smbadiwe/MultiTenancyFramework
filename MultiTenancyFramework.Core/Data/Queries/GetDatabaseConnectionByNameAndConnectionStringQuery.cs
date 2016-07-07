using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.Data.Queries
{
    public class GetDatabaseConnectionByNameAndConnectionStringQuery : IDbQuery<DatabaseConnection>
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
    }
}
