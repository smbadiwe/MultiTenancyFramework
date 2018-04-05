using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using System.Linq;

namespace MultiTenancyFramework.NHibernate.Queries
{
    public sealed class GetAppUserByUsernameQueryHandler
        : CoreGeneralDAO, IDbQueryHandler<GetAppUserByUsernameQuery, AppUser>
    {
        public GetAppUserByUsernameQueryHandler()
        {
            EntityName = NHManager.NHSessionManager.GetEntityNameToUseInNHSession(typeof(AppUser));
        }

        public AppUser Handle(GetAppUserByUsernameQuery theQuery)
        {
            var session = BuildSession();
            var query = session.Query<AppUser>(EntityName)
                .Where(x => x.UserName == theQuery.Username);
            return query.SingleOrDefault();
        }
    }
}
