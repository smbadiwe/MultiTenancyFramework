using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiTenancyFramework.Core.TaskManager
{
    public interface IRunnableTask
    {
        OwnershipType OwnershipType { get; }
        ScheduledTask DefaultTaskPlan { get; }
        Task Execute(string institutionCode);
    }

    public enum OwnershipType
    {
        CentralOnly,
        PerInstitution
    }
}
