using MultiTenancyFramework.Data;
using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using MultiTenancyFramework.Mvc.Logic;
using System.Collections.Generic;
using System.Linq;

namespace MultiTenancyFramework.Logic
{
    public class PrivilegeLogic : CoreLogic<Privilege>
    {
        public PrivilegeLogic() : base(MyServiceLocator.GetInstance<IPrivilegeDAO<Privilege>>())
        {
        }

        public override Privilege Retrieve(long id)
        {
            Privilege entity;
            if (DataCacheMVC.AllPrivileges.TryGetValue(id, out entity))
            {
                return entity;
            }
            return null;
        }
        
        public override IList<Privilege> RetrieveAll()
        {
            return DataCacheMVC.AllPrivileges.Values.ToList();
        }

        public IList<string> GetAreas()
        {
           return  QueryProcessor.Process(new GetAreasQuery());
        }

        public override IList<Privilege> RetrieveAllActive()
        {
            return DataCacheMVC.AllPrivileges.Values.Where(x => !x.IsDisabled).ToList();
        }

        public RetrievedData<Privilege> Search(string name, AccessScope? scope, int page, int pageSize)
        {
            var query = new GetPrivilegesByGridSearchParamsQuery
            {
                AccessScope = scope,
                Name = name,
                PageIndex = page,
                PageSize = pageSize,
            };
            return QueryProcessor.Process(query);
        }
        
        public override void OnAfterCommittingChanges(Privilege e)
        {
            if (e.IsDeleted)
            {
                DataCacheMVC.AllPrivileges.Remove(e.Id);
            }
            else
            {
                DataCacheMVC.AllPrivileges[e.Id] = e;
            }
            base.OnAfterCommittingChanges(e);
        }

    }
}
