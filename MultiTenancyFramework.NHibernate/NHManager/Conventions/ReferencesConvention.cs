using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace MultiTenancyFramework.NHibernate.NHManager.Conventions
{
    public class ReferencesConvention : IReferenceConvention
    {
        public void Apply(IManyToOneInstance instance)
        {
            instance.Column((instance.Property.Name.StartsWith("The") 
                ? instance.Property.Name.Remove(0, 3) 
                : instance.Property.Name) + "Id");
        }
    }
}
