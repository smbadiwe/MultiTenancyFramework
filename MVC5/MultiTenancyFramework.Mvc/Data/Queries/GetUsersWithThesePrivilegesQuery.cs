using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using System.Collections.Generic;

namespace MultiTenancyFramework.Mvc.Data.Queries
{
    public class GetUsersWithThesePrivilegesQuery : IDbQuery<IList<AppUser>>
    {
        public string[] Privileges { get; set; }
    }
}
