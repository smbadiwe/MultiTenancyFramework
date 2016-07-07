using MultiTenancyFramework.Entities;
using System.Collections.Generic;

namespace MultiTenancyFramework.Data.Queries
{
    public class GetUserLoginsByUserIdQuery : IDbQuery<IList<UserLogin>>
    {
        public long UserId { get; set; }
    }
}
