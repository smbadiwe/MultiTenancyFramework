using MultiTenancyFramework.Data;
using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiTenancyFramework.Data.Queries
{
    public class GetActionAccessPrivilegesByGridSearchParamsQuery : DbPagingQuery, IDbQuery<RetrievedData<ActionAccessPrivilege>>
    {
        public string Name { get; set; }
        public AccessScope? AccessScope { get; set; }
    }
}
