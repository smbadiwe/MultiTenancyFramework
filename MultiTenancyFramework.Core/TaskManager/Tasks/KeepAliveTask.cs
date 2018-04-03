using System;
using System.Net;
using System.Threading.Tasks;

namespace MultiTenancyFramework.Core.TaskManager.Tasks
{
    public class KeepAliveTask : IRunnableTask
    {
        public ScheduledTask DefaultTaskPlan
        {
            get
            {
                return new ScheduledTask
                {
                    Name = "Keep alive",
                    Seconds = 300,
                    Type = typeof(KeepAliveTask).AssemblyQualifiedName,
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
            using (var client = new WebClient())
            {
                client.DownloadStringAsync(new Uri(ConfigurationHelper.GetSiteUrl() + "keepalive/index"));
            }
            return Task.CompletedTask;
        }
    }
}
