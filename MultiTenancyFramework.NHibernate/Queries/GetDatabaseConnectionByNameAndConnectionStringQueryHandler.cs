using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using System.Linq;

namespace MultiTenancyFramework.NHibernate.Queries
{
    public sealed class GetDatabaseConnectionByNameAndConnectionStringQueryHandler
        : CoreGeneralDAO, IDbQueryHandler<GetDatabaseConnectionByNameAndConnectionStringQuery, DatabaseConnection>
    {
        public DatabaseConnection Handle(GetDatabaseConnectionByNameAndConnectionStringQuery theQuery)
        {
            var session = BuildSession();
            var query = session.Query<DatabaseConnection>()
                .Where(x => x.Name == theQuery.Name && x.ConnectionString == theQuery.ConnectionString);
            return query.SingleOrDefault();
        }
    }
}
