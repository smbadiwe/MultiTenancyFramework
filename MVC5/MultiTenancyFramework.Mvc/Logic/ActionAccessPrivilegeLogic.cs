using MultiTenancyFramework.Data;
using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using MultiTenancyFramework.Mvc.Logic;
using System.Collections.Generic;
using System.Linq;

namespace MultiTenancyFramework.Logic
{
    public class ActionAccessPrivilegeLogic : CoreLogic<ActionAccessPrivilege>
    {
        public ActionAccessPrivilegeLogic() : base(MyServiceLocator.GetInstance<IPrivilegeDAO<ActionAccessPrivilege>>())
        {
        }

        public override ActionAccessPrivilege Retrieve(long id)
        {
            ActionAccessPrivilege entity;
            if (DataCacheMVC.AllPrivileges.TryGetValue(id, out entity))
            {
                return entity;
            }
            return null;
        }
        
        public override IList<ActionAccessPrivilege> RetrieveAll()
        {
            return DataCacheMVC.AllPrivileges.Values.ToList();
        }

        public IList<string> GetAreas()
        {
           return  QueryProcessor.Process(new GetAreasQuery());
        }

        public override IList<ActionAccessPrivilege> RetrieveAllActive()
        {
            return DataCacheMVC.AllPrivileges.Values.Where(x => !x.IsDisabled).ToList();
        }

        public RetrievedData<ActionAccessPrivilege> Search(string name, AccessScope? scope, int page, int pageSize)
        {
            var query = new GetActionAccessPrivilegesByGridSearchParamsQuery
            {
                AccessScope = scope,
                Name = name,
                PageIndex = page,
                PageSize = pageSize,
            };
            return QueryProcessor.Process(query);
        }
        
        public override void OnAfterCommittingChanges(ActionAccessPrivilege e)
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
