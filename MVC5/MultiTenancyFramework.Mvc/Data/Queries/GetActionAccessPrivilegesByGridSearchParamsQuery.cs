using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.Data.Queries
{
    public class GetActionAccessPrivilegesByGridSearchParamsQuery : DbPagingQuery, IDbQuery<RetrievedData<ActionAccessPrivilege>>
    {
        public string Name { get; set; }
        public AccessScope? AccessScope { get; set; }
    }
}
