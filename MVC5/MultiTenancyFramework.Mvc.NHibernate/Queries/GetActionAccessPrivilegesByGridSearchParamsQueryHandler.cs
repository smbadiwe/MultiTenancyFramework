using MultiTenancyFramework.Data;
using MultiTenancyFramework.Entities;
using MultiTenancyFramework.NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;
using MultiTenancyFramework.Data.Queries;

namespace MultiTenancyFramework.NHibernate.Queries
{
    public class GetActionAccessPrivilegesByGridSearchParamsQueryHandler : CoreGeneralWithGridPagingDAO<Privilege>, IDbQueryHandler<GetActionAccessPrivilegesByGridSearchParamsQuery, RetrievedData<ActionAccessPrivilege>>
    {
        //public GetActionAccessPrivilegesByGridSearchParamsQueryHandler()
        //{
        //    EntityName = NHManager.NHSessionManager.GetEntityNameToUseInNHSession(typeof(Privilege));
        //}

        public RetrievedData<ActionAccessPrivilege> Handle(GetActionAccessPrivilegesByGridSearchParamsQuery theQuery)
        {
            var session = BuildSession();
            var query = session.QueryOver<ActionAccessPrivilege>();
            if (!string.IsNullOrWhiteSpace(theQuery.Name))
            {
                query = query.Where(x => x.Name.IsInsensitiveLike(theQuery.Name) || x.DisplayName.IsInsensitiveLike(theQuery.Name));
            }
            if (theQuery.AccessScope.HasValue && theQuery.AccessScope.Value > 0)
            {
                query = query.Where(x => x.Scope == theQuery.AccessScope.Value);
            }
            return RetrieveUsingPaging(query, theQuery.PageIndex, theQuery.PageSize);
        }
    }
}
