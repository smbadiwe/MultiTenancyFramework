using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.NHibernate.Queries
{
    public sealed class GetIAppUserByEmailQueryHandler
        : CoreGeneralDAO, IDbQueryHandler<GetAppUserByEmailQuery, AppUser>
    {
        public GetIAppUserByEmailQueryHandler()
        {
            EntityName = NHManager.NHSessionManager.GetEntityNameToUseInNHSession(typeof(AppUser));
        }

        public AppUser Handle(GetAppUserByEmailQuery theQuery)
        {
            var session = BuildSession();
            var query = session.QueryOver<AppUser>(EntityName)
                .Where(x => x.Email == theQuery.Email);
            return query.SingleOrDefault();
        }
    }
}
