using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.Data.Queries
{
    public class GetAppUserByEmailQuery : IDbQuery<AppUser>
    {
        public string Email { get; set; }
    }
}
