using MultiTenancyFramework.Data;
using MultiTenancyFramework.NHibernate.NHManager;

namespace MultiTenancyFramework.NHibernate
{
    public class DataInitializer : IDataInitializer
    {
        public virtual void Init(bool isWeb = true)
        {
            ISessionStorage sessionStorage;
            if (isWeb)
            {
                sessionStorage = new WebSessionStorage();
            }
            else
            {
                sessionStorage = new NonWebSessionStorage();
            }
            //Sesson Factory
            NHSessionManager.Init(sessionStorage, null, NHSessionManager.GetSessionKey(isWebSession: isWeb));
            
        }

        public virtual void Terminate(bool isWeb = true)
        {
            NHSessionManager.CloseStorage(isWebSession: isWeb);
        }
    }
}
