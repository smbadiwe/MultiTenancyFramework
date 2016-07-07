using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using System.Collections.Generic;

namespace MultiTenancyFramework.NHibernate.Queries
{
    public class GetUserClaimsByUserIdQueryHandler : CoreGeneralDAO, IDbQueryHandler<GetUserClaimsByUserIdQuery, IList<UserClaim>>
    {
        public IList<UserClaim> Handle(GetUserClaimsByUserIdQuery theQuery)
        {
            var session = BuildSession();
            var query = session.QueryOver<UserClaim>().Where(x => x.UserId == theQuery.UserId);
            return query.List();
        }
    }
}
