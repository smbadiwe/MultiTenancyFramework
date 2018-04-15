using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using System.Collections.Generic;
using System.Linq;

namespace MultiTenancyFramework.NHibernate.Queries
{
    public class GetUserLoginssByUserIdQueryHandler : CoreGeneralDAO, IDbQueryHandler<GetUserLoginsByUserIdQuery, IList<UserLogin>>
    {
        public IList<UserLogin> Handle(GetUserLoginsByUserIdQuery theQuery)
        {
            var session = BuildSession();
            var query = session.Query<UserLogin>().Where(x => x.UserId == theQuery.UserId);
            return query.ToList();
        }
    }
}
