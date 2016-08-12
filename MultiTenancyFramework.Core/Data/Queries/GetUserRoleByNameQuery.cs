using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.Data.Queries
{
    public class GetUserRoleByNameQuery : IDbQuery<UserRole>
    {
        public string Name { get; set; }
    }
}
