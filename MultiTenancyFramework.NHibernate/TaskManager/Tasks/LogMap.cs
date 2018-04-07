using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MultiTenancyFramework.NHibernate.Maps;
using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.NHibernate.TaskManager.Tasks
{
    public class LogMap : IAutoMappingOverride<Log>
    {
        public void Override(AutoMapping<Log> mapping)
        {
            mapping.Map(x => x.ShortMessage).VarCharMax();
            mapping.Map(x => x.FullMessage).VarCharMax();
        }
    }
}
