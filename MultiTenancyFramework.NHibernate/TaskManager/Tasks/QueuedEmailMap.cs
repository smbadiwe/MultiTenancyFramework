using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MultiTenancyFramework.Core.TaskManager.Tasks;
using MultiTenancyFramework.NHibernate.Maps;

namespace MultiTenancyFramework.NHibernate.TaskManager.Tasks
{
    public class QueuedEmailMap : IAutoMappingOverride<QueuedEmail>
    {
        public void Override(AutoMapping<QueuedEmail> mapping)
        {
            mapping.Map(x => x.Receivers).Length(1000);
            mapping.Map(x => x.Subject).Length(1000);
            mapping.Map(x => x.Body).VarCharMax();
            //mapping.References(x => x.EmailAccount).Fetch.Join();
        }
    }
}
