using MultiTenancyFramework.Data;
using MultiTenancyFramework.Entities;
using MultiTenancyFramework.Mvc.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace MultiTenancyFramework.Mvc.Logic
{
    public class DataCacheMVC
    {
        private const string ALL_INSTITUTIONS = "::AllInstitutions::";
        private const string ALL_PRIVILEGES = "::AllPrivileges::";

        public static Dictionary<string, Institution> AllInstitutions
        {
            get
            {
                if (MemoryCache.Default != null)
                {
                    var allInstitutions = MemoryCache.Default[ALL_INSTITUTIONS] as Dictionary<string, Institution>;
                    if (allInstitutions == null || allInstitutions.Count == 0)
                    {
                        var instDAO = MyServiceLocator.GetInstance<IInstitutionDAO<Institution>>();
                        allInstitutions = instDAO.RetrieveAll()?.ToDictionary(x => x.Code);
                        MemoryCache.Default[ALL_INSTITUTIONS] = allInstitutions;
                    }
                    return allInstitutions;
                }
                return new Dictionary<string, Institution>();
            }
        }

        public static Dictionary<long, ActionAccessPrivilege> AllPrivileges
        {
            get
            {
                var allPrivileges = MemoryCache.Default[ALL_PRIVILEGES] as Dictionary<long, ActionAccessPrivilege>;
                if (allPrivileges == null || allPrivileges.Count == 0)
                {
                    var _dao = MyServiceLocator.GetInstance<IPrivilegeDAO<ActionAccessPrivilege>>();
                    allPrivileges = _dao.RetrieveAll()?.ToDictionary(x => x.Id);
                    MemoryCache.Default[ALL_PRIVILEGES] = allPrivileges;
                }
                return allPrivileges;
            }
            set
            {
                MemoryCache.Default[ALL_PRIVILEGES] = value;
            }
        }

        internal static MultiTenancyFrameworkSettings MultiTenancyFrameworkSettings
        {
            get
            {
                return MemoryCache.Default["::MultiTenancyFrameworkSettings::"] as MultiTenancyFrameworkSettings ?? new MultiTenancyFrameworkSettings();
            }
            set
            {
                MemoryCache.Default["::MultiTenancyFrameworkSettings::"] = value;
            }
        }
    }
}
