using MultiTenancyFramework.Data;
using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using System.Linq;

namespace MultiTenancyFramework.NHibernate.Queries
{
    public class GetPrivilegesByGridSearchParamsQueryHandler : CoreGeneralWithGridPagingDAO<Privilege>, IDbQueryHandler<GetPrivilegesByGridSearchParamsQuery, RetrievedData<Privilege>>
    {
        public GetPrivilegesByGridSearchParamsQueryHandler()
        {
            EntityName = NHManager.NHSessionManager.GetEntityNameToUseInNHSession(typeof(Privilege));
        }

        public RetrievedData<Privilege> Handle(GetPrivilegesByGridSearchParamsQuery theQuery)
        {
            var session = BuildSession();
            var query = session.Query<Privilege>(EntityName);
            if (!string.IsNullOrWhiteSpace(theQuery.Name))
            {
                query = query.Where(x => x.Name.Contains(theQuery.Name) || x.DisplayName.Contains(theQuery.Name));
            }
            if (theQuery.AccessScope.HasValue && theQuery.AccessScope.Value > 0)
            {
                query = query.Where(x => x.Scope == theQuery.AccessScope.Value);
            }
            return RetrieveUsingPaging(query, theQuery.PageIndex, theQuery.PageSize);
        }
    }
}
