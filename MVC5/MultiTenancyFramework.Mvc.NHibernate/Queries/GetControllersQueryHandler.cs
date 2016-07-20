using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using NHibernate.Criterion;
using System.Collections.Generic;

namespace MultiTenancyFramework.NHibernate.Queries
{
    public class GetControllersQueryHandler : CoreGeneralDAO, IDbQueryHandler<GetControllersQuery, IList<string>>
    {
        public IList<string> Handle(GetControllersQuery theQuery)
        {
            var session = BuildSession();
            var query = session.QueryOver<ActionAccessPrivilege>()
                .Select(Projections.Distinct(Projections.Property<ActionAccessPrivilege>(x => x.Controller)));
            return query.List<string>();
        }
    }
}
