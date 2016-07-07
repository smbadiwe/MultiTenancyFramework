using MultiTenancyFramework;
using MultiTenancyFramework.Data;
using MultiTenancyFramework.Entities;
using System.Collections.Generic;
using System.Linq;
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
                if (HttpRuntime.Cache != null)
                {
                    var allInstitutions = HttpRuntime.Cache[ALL_INSTITUTIONS] as Dictionary<string, Institution>;
                    if (allInstitutions == null || allInstitutions.Count == 0)
                    {
                        var instDAO = MyServiceLocator.GetInstance<IInstitutionDAO<Institution>>();
                        allInstitutions = instDAO.RetrieveAll()?.ToDictionary(x => x.Code);
                        HttpRuntime.Cache[ALL_INSTITUTIONS] = allInstitutions;
                    }
                    return allInstitutions;
                }
                return new Dictionary<string, Institution>();
            }
        }

        public static Dictionary<long, Privilege> AllPrivileges
        {
            get
            {
                var allPrivileges = HttpRuntime.Cache[ALL_PRIVILEGES] as Dictionary<long, Privilege>;
                if (allPrivileges == null || allPrivileges.Count == 0)
                {
                    var _dao = MyServiceLocator.GetInstance<IPrivilegeDAO<Privilege>>();
                    allPrivileges = _dao.RetrieveAll()?.ToDictionary(x => x.Id);
                    HttpRuntime.Cache[ALL_PRIVILEGES] = allPrivileges;
                }
                return allPrivileges;
            }
            set
            {
                HttpRuntime.Cache[ALL_PRIVILEGES] = value;
            }
        }

    }
}
