using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.Data.Queries
{
    public class GetUserLoginByLoginProviderKeyAndUserIdQuery : IDbQuery<UserLogin>
    {
        public long UserID { get; set; }
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
    }
}
