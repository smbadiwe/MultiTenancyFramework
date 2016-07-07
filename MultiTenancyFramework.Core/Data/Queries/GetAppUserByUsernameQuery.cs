using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.Data.Queries
{
    public class GetAppUserByUsernameQuery : IDbQuery<AppUser>
    {
        public string Username { get; set; }
    }
}
