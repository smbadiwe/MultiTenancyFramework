using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.NHibernate.Queries
{
    public sealed class GetUserRoleByNameQueryHandler
        : CoreGeneralDAO, IDbQueryHandler<GetUserRoleByNameQuery, UserRole>
    {
        public UserRole Handle(GetUserRoleByNameQuery theQuery)
        {
            var session = BuildSession();
            var query = session.QueryOver<UserRole>()
                .Where(x => x.Name == theQuery.Name);
            return query.SingleOrDefault();
        }
    }
}
