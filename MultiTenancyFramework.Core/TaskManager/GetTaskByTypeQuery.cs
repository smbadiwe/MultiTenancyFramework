using MultiTenancyFramework.Data.Queries;

namespace MultiTenancyFramework.Core.TaskManager
{
    public class GetTaskByTypeQuery : IDbQueryAsync<ScheduledTask>
    {
        public string Type { get; set; }
    }
}
