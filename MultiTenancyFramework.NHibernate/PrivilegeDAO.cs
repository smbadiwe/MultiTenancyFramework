using MultiTenancyFramework.Data;
using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.NHibernate
{
    public class PrivilegeDAO : PrivilegeDAO<Privilege>, IPrivilegeDAO
    {
    }

    public class PrivilegeDAO<T> : CoreDAO<T>, IPrivilegeDAO<T> where T : Privilege
    {
        public PrivilegeDAO()
        {
            EntityName = NHManager.NHSessionManager.GetEntityNameToUseInNHSession(typeof(Privilege));
        }
    }
}
