using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.NHibernate.Queries
{
    public sealed class GetDatabaseConnectionByNameAndConnectionStringQueryHandler
        : CoreGeneralDAO, IDbQueryHandler<GetDatabaseConnectionByNameAndConnectionStringQuery, DatabaseConnection>
    {
        public DatabaseConnection Handle(GetDatabaseConnectionByNameAndConnectionStringQuery theQuery)
        {
            var session = BuildSession();
            var query = session.QueryOver<DatabaseConnection>()
                .Where(x => x.Name == theQuery.Name).And(x => x.ConnectionString == theQuery.ConnectionString);
            return query.SingleOrDefault();
        }
    }
}
