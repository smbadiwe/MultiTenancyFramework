using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using System.Collections.Generic;
using System.Linq;

namespace MultiTenancyFramework.NHibernate.Queries
{
    public class GetUserClaimsByUserIdQueryHandler : CoreGeneralDAO, IDbQueryHandler<GetUserClaimsByUserIdQuery, IList<UserClaim>>
    {
        public IList<UserClaim> Handle(GetUserClaimsByUserIdQuery theQuery)
        {
            var session = BuildSession();
            var query = session.Query<UserClaim>().Where(x => x.UserId == theQuery.UserId);
            return query.ToList();
        }
    }
}
