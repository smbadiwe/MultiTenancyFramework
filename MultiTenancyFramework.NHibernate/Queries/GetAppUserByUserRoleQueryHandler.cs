using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using NHibernate.Criterion;
using System.Collections.Generic;

namespace MultiTenancyFramework.NHibernate.Queries
{
    public sealed class GetAppUserByUserRoleQueryHandler
        : CoreGeneralDAO, IDbQueryHandler<GetAppUserByUserRoleQuery, IList<AppUser>>
    {
        public GetAppUserByUserRoleQueryHandler()
        {
            EntityName = NHManager.NHSessionManager.GetEntityNameToUseInNHSession(typeof(AppUser));
        }

        public IList<AppUser> Handle(GetAppUserByUserRoleQuery theQuery)
        {
            var session = BuildSession();
            var query = session.QueryOver<AppUser>(EntityName)
                .Where(x => x.UserRoles.IsInsensitiveLike($"{theQuery.UserRoleId},", MatchMode.Anywhere));
            return query.List();
        }
    }
}
