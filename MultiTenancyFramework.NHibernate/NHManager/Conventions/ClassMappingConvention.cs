using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace MultiTenancyFramework.NHibernate.NHManager.Conventions
{
    public class ClassMappingConvention : IClassConvention
    {
        public void Apply(IClassInstance instance)
        {
            instance.Table(instance.EntityType.Name.ToPlural());
        }
    }
}
