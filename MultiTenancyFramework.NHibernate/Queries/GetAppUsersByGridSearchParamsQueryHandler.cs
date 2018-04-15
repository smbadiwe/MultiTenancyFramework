﻿using MultiTenancyFramework.Data;
using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using System.Linq;

namespace MultiTenancyFramework.NHibernate.Queries
{
    public class GetAppUsersByGridSearchParamsQueryHandler : CoreGeneralWithGridPagingDAO<AppUser>, IDbQueryHandler<GetAppUsersByGridSearchParamsQuery, RetrievedData<AppUser>>
    {
        public GetAppUsersByGridSearchParamsQueryHandler()
        {
            EntityName = NHManager.NHSessionManager.GetEntityNameToUseInNHSession(typeof(AppUser));
        }

        public RetrievedData<AppUser> Handle(GetAppUsersByGridSearchParamsQuery theQuery)
        {
            var session = BuildSession();
            var query = session.Query<AppUser>(EntityName);

            if (!string.IsNullOrWhiteSpace(theQuery.LastName))
            {
                query = query.Where(x => x.LastName == theQuery.LastName);
            }
            if (!string.IsNullOrWhiteSpace(theQuery.OtherNames))
            {
                query = query.Where(x => x.OtherNames == theQuery.OtherNames);
            }
            if (!string.IsNullOrWhiteSpace(theQuery.Username))
            {
                query = query.Where(x => x.UserName == theQuery.Username);
            }
            if (theQuery.UserRole > 0)
            {
                query = query.Where(x => x.UserRoles.Contains($"{theQuery.UserRole},"));
            }
            return RetrieveUsingPaging(query, theQuery.PageIndex, theQuery.PageSize);
        }
    }
}
