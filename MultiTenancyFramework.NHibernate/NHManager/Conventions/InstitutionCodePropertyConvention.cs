using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace MultiTenancyFramework.NHibernate.NHManager.Conventions
{
    public class InstitutionCodePropertyConvention : IPropertyConvention
    {
        public void Apply(IPropertyInstance instance)
        {
            Entities.BaseEntity e;
            if (instance.Name == nameof(e.InstitutionCode) && instance.Property.PropertyType == typeof(string))
            {
                instance.Index("ind_InstitutionCode");
            }
        }

    }
}
