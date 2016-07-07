using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.NHibernate.Queries
{
    public class GetUserLoginByLoginProviderKeyAndUserIdQueryHandler : CoreGeneralDAO, IDbQueryHandler<GetUserLoginByLoginProviderKeyAndUserIdQuery, UserLogin>
    {
        public UserLogin Handle(GetUserLoginByLoginProviderKeyAndUserIdQuery theQuery)
        {
            var session = BuildSession();
            var query = session.QueryOver<UserLogin>()
                .Where(x => x.LoginProvider == theQuery.LoginProvider)
                .And(x => x.ProviderKey == theQuery.ProviderKey);
            if (theQuery.UserID > 0)
            {
                query = query.And(x => x.UserId == theQuery.UserID);
            }
            return query.SingleOrDefault();
        }
    }
}
