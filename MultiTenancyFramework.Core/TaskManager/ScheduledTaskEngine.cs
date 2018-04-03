using System.Collections.Generic;
using MultiTenancyFramework.Data;
using MultiTenancyFramework.Logic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System;

namespace MultiTenancyFramework.Core.TaskManager
{
    public class ScheduledTaskEngine : CoreLogic<ScheduledTask>
    {
        public ScheduledTaskEngine(string institutionCode) : base(MyServiceLocator.GetInstance<ICoreDAO<ScheduledTask>>(), institutionCode)
        {

        }

        public override IList<ScheduledTask> RetrieveAll(params string[] fields)
        {
            var list = base.RetrieveAll(fields);
            if (list == null || list.Count == 0)
            {
                list = AsyncHelper.RunSync(() => EnsureListExists(list));
            }
            return list;
        }

        public override async Task<IList<ScheduledTask>> RetrieveAllAsync(string[] fields, CancellationToken token = default(CancellationToken))
        {
            var list = await base.RetrieveAllAsync(fields, token);
            if (list == null || list.Count == 0)
            {
                list = await EnsureListExists(list);
            }
            return list;
        }

        private async Task<IList<ScheduledTask>> EnsureListExists(IList<ScheduledTask> list, CancellationToken token = default(CancellationToken))
        {
            var taskTypes = new AppDomainTypeFinder().FindClassesOfType<IRunnableTask>();
            if (taskTypes == null || !taskTypes.Any())
            {
                list = new List<ScheduledTask>();
                bool isCentralInst = string.IsNullOrWhiteSpace(InstitutionCode) || InstitutionCode.Equals(Utilities.INST_DEFAULT_CODE, StringComparison.OrdinalIgnoreCase);
                foreach (var type in taskTypes)
                {
                    var instance = Activator.CreateInstance(type) as IRunnableTask;
                    if (instance != null)
                    {
                        if (!isCentralInst && instance.OwnershipType == OwnershipType.CentralOnly)
                            continue;

                        list.Add(instance.DefaultTaskPlan);
                    }
                }

                await InsertAsync(list, token);
            }
            return list;
        }
    }
}
