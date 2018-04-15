using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using System.Collections.Generic;
using System.Linq;

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
            var query = session.Query<AppUser>(EntityName)
                .Where(x => x.UserRoles.Contains($"{theQuery.UserRoleId},"));
            return query.ToList();
        }
    }
}
