using MultiTenancyFramework.Entities;
using System.Collections.Generic;

namespace MultiTenancyFramework.Data.Queries
{
    public class GetUserClaimsByUserIdQuery : IDbQuery<IList<UserClaim>>
    {
        public long UserId { get; set; }
    }
}
