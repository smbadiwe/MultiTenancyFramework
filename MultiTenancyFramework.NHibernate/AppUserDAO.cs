using MultiTenancyFramework.Data;
using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.NHibernate
{
    public class AppUserDAO : AppUserDAO<AppUser>, IAppUserDAO
    {
    }

    public class AppUserDAO<T> : CoreDAO<T>, IAppUserDAO<T> where T : AppUser
    {
        public AppUserDAO()
        {
            EntityName = NHManager.NHSessionManager.GetEntityNameToUseInNHSession(typeof(AppUser));
        }
    }
}
