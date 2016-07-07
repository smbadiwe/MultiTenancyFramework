using MultiTenancyFramework.Entities;
using System.Collections.Generic;

namespace MultiTenancyFramework.Data.Queries
{
    public class GetAppUserByUserRoleQuery : IDbQuery<IList<AppUser>>
    {
        public long UserRoleId { get; set; }
    }
}
