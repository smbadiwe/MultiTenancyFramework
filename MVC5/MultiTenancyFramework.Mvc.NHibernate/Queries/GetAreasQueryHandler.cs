using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using NHibernate.Criterion;
using System.Collections.Generic;

namespace MultiTenancyFramework.NHibernate.Queries
{
    public class GetAreasQueryHandler : CoreGeneralDAO, IDbQueryHandler<GetAreasQuery, IList<string>>
    {
        //public GetAreasQueryHandler()
        //{
        //    EntityName = NHManager.NHSessionManager.GetEntityNameToUseInNHSession(typeof(Privilege));
        //}

        public IList<string> Handle(GetAreasQuery theQuery)
        {
            var session = BuildSession();
            var query = session.QueryOver<ActionAccessPrivilege>()
                .Select(Projections.Distinct(Projections.Property<ActionAccessPrivilege>(x => x.Area)));
            return query.List<string>();
        }
    }
}
