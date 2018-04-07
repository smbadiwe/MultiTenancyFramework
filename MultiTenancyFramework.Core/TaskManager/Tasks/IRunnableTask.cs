using MultiTenancyFramework.Core.TaskManager;
using System.Threading.Tasks;

namespace MultiTenancyFramework.Tasks
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
