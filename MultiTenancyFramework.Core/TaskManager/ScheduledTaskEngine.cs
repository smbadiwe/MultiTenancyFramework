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

        public override IList<ScheduledTask> RetrieveAllActive(params string[] fields)
        {
            var list = base.RetrieveAllActive(fields);
            if (list == null || list.Count == 0)
            {
                list = RetrieveAll().Where(x => x.IsEnabled).ToList();
            }
            return list;
        }

        public override async Task<IList<ScheduledTask>> RetrieveAllActiveAsync(string[] fields = null, CancellationToken token = default(CancellationToken))
        {
            var list = await base.RetrieveAllActiveAsync(fields, token);
            if (list == null || list.Count == 0)
            {
                list = (await RetrieveAllAsync(fields, token)).Where(x => x.IsEnabled).ToList();
            }
            return list;
        }

        public override IList<ScheduledTask> RetrieveAll(params string[] fields)
        {
            var list = base.RetrieveAll(fields);
            if (list == null || list.Count == 0)
            {
                list = EnsureListExists(list);
            }
            return list;
        }

        public override async Task<IList<ScheduledTask>> RetrieveAllAsync(string[] fields = null, CancellationToken token = default(CancellationToken))
        {
            var list = await base.RetrieveAllAsync(fields, token);
            if (list == null || list.Count == 0)
            {
                list = await EnsureListExistsAsync(list);
            }
            return list;
        }

        private async Task<IList<ScheduledTask>> EnsureListExistsAsync(IList<ScheduledTask> list, CancellationToken token = default(CancellationToken))
        {
            if (list == null) list = new List<ScheduledTask>();
            PrePeocess(list);

            if (list.Count > 0)
                await InsertAsync(list, token);
            return list;
        }

        private IList<ScheduledTask> EnsureListExists(IList<ScheduledTask> list)
        {
            if (list == null) list = new List<ScheduledTask>();
            PrePeocess(list);

            if (list.Count > 0)
                Insert(list);
            return list;
        }

        private void PrePeocess(IList<ScheduledTask> list)
        {
            if (list.Count > 0) return;

            var taskTypes = new AppDomainTypeFinder().FindClassesOfType<IRunnableTask>();
            if (taskTypes != null)
            {
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

            }

        }
    }
}
