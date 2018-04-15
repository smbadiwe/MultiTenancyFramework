using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using System.Linq;

namespace MultiTenancyFramework.NHibernate.Queries
{
    public class GetUserLoginByLoginProviderKeyAndUserIdQueryHandler : CoreGeneralDAO, IDbQueryHandler<GetUserLoginByLoginProviderKeyAndUserIdQuery, UserLogin>
    {
        public UserLogin Handle(GetUserLoginByLoginProviderKeyAndUserIdQuery theQuery)
        {
            var session = BuildSession();
            var query = session.Query<UserLogin>()
                .Where(x => x.LoginProvider == theQuery.LoginProvider && x.ProviderKey == theQuery.ProviderKey);
            if (theQuery.UserID > 0)
            {
                query = query.Where(x => x.UserId == theQuery.UserID);
            }
            return query.SingleOrDefault();
        }
    }
}
