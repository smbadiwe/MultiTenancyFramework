using MultiTenancyFramework.Data;
using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.NHibernate
{
    public class InstitutionDAO : InstitutionDAO<Institution>, IInstitutionDAO
    {
    }

    public class InstitutionDAO<T> : CoreDAO<T>, IInstitutionDAO<T> where T : Institution
    {
        public InstitutionDAO()
        {
            EntityName = NHManager.NHSessionManager.GetEntityNameToUseInNHSession(typeof(T));
        }
    }
}
