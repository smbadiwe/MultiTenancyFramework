using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using NHibernate.Criterion;
using System.Collections.Generic;

namespace MultiTenancyFramework.NHibernate.Queries
{
    public class GetActionsQueryHandler : CoreGeneralDAO, IDbQueryHandler<GetActionsQuery, IList<string>>
    {
        public IList<string> Handle(GetActionsQuery theQuery)
        {
            var session = BuildSession();
            var query = session.QueryOver<ActionAccessPrivilege>()
                .Select(Projections.Distinct(Projections.Property<ActionAccessPrivilege>(x => x.Action)));
            return query.List<string>();
        }
    }
}
