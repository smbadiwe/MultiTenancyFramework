using MultiTenancyFramework.Data;
using System.Collections.Generic;

namespace MultiTenancyFramework.NHibernate.NHManager
{
    public class DbSessionCleanup : IDbSessionCleanup
    {
        public void CloseDbConnections()
        {
            var storageSet = new Dictionary<string, ISessionStorage>(NHSessionManager.SessionStorages);
            if (storageSet != null && storageSet.Count > 0)
            {
                foreach (var storage in storageSet.Values)
                {
                    //Closes the session if there's any open session
                    if (storage != null && storage.Session != null)
                    {
                        NHSessionManager.CloseStorage(((WebSessionStorage)storage)?.InstitutionCode);
                    }
                }
                storageSet.Clear();
            }
        }
    }
}
