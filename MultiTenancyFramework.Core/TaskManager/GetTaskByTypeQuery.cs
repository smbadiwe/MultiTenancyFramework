using MultiTenancyFramework.Data.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiTenancyFramework.Core.TaskManager
{
    public class GetTaskByTypeQuery : IDbQueryAsync<ScheduledTask>
    {
        public string Type { get; set; }
    }
}
