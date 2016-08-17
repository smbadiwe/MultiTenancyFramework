using MultiTenancyFramework.Data;
using MultiTenancyFramework.NHibernate.NHManager;

namespace MultiTenancyFramework.NHibernate
{
    public class DataInitializer : IDataInitializer
    {
        public virtual void Init(bool isWeb = true)
        {
            //Sesson Factory
            NHSessionManager.Init(null, NHSessionManager.GetSessionKey(isWebSession: isWeb));
        }

        public virtual void Terminate(bool isWeb = true)
        {
            NHSessionManager.CloseStorage(isWebSession: isWeb);
        }
    }
}
