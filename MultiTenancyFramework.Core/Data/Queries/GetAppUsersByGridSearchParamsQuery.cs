using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.Data.Queries
{
    public class GetAppUsersByGridSearchParamsQuery : DbPagingQuery, IDbQuery<RetrievedData<AppUser>>
    {
        public string LastName { get; set; }
        public string OtherNames { get; set; }
        public string Username { get; set; }
        public long UserRole { get; set; }
    }
}
