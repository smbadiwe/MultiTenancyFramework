using MultiTenancyFramework.Core.TaskManager;
using MultiTenancyFramework.Logic;
using System;
using System.Threading.Tasks;

namespace MultiTenancyFramework.Tasks
{
    public class ClearLogTask : IRunnableTask
    {
        public ScheduledTask DefaultTaskPlan
        {
            get
            {
                return new ScheduledTask
                {
                    Name = "Clear logs",
                    Seconds = 2 * 86400, // 2 day
                    Type = typeof(ClearLogTask).AssemblyQualifiedName,
                    IsDisabled = false,
                    StopOnError = false,
                };
            }
        }

        public OwnershipType OwnershipType
        {
            get
            {
                return OwnershipType.CentralOnly;
            }
        }

        public Task Execute(string institutionCode)
        {
            return new LogLogic().ClearLog();
        }
    }
}
