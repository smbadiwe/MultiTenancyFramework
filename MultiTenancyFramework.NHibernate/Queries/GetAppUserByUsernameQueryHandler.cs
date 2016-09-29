using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;

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
            var query = session.QueryOver<AppUser>(EntityName)
                .Where(x => x.UserName == theQuery.Username);
            return query.SingleOrDefault();
        }
    }
}
