using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MultiTenancyFramework.Core.TaskManager;

namespace MultiTenancyFramework.NHibernate.TaskManager
{
    public class ScheduleTaskMap : IAutoMappingOverride<ScheduledTask>
    {
        public void Override(AutoMapping<ScheduledTask> mapping)
        {
            mapping.Map(x => x.Name).Not.Nullable();
            mapping.Map(x => x.Type).Not.Nullable();
        }
    }
}
