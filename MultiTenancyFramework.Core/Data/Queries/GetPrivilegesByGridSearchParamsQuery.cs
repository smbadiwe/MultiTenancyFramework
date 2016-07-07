using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.Data.Queries
{
    /// <summary>
    /// Seaches params: Name and Access Scope
    /// </summary>
    public class GetPrivilegesByGridSearchParamsQuery : DbPagingQuery, IDbQuery<RetrievedData<Privilege>>
    {
        public string Name { get; set; }
        public AccessScope? AccessScope { get; set; }
    }
}
