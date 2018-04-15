using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using System.Linq;

namespace MultiTenancyFramework.NHibernate.Queries
{
    public sealed class GetUserRoleByNameQueryHandler
        : CoreGeneralDAO, IDbQueryHandler<GetUserRoleByNameQuery, UserRole>
    {
        public UserRole Handle(GetUserRoleByNameQuery theQuery)
        {
            var session = BuildSession();
            var query = session.Query<UserRole>()
                .Where(x => x.Name == theQuery.Name);
            return query.SingleOrDefault();
        }
    }
}
