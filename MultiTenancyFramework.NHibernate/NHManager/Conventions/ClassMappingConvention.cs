using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using MultiTenancyFramework.NHibernate.NHManager.Listeners;

namespace MultiTenancyFramework.NHibernate.NHManager.Conventions
{
    public class ClassMappingConvention : IClassConvention
    {
        public void Apply(IClassInstance instance)
        {
            //Table name rule
            instance.Table(instance.EntityType.Name.ToPlural());
            //To filter queries based on what I've defined in the Tenant filter definition
            instance.ApplyFilter<AppFilterDefinition>();
        }
    }
}
