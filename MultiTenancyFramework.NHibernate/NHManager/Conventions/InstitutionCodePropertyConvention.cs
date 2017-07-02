using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace MultiTenancyFramework.NHibernate.NHManager.Conventions
{
    public class InstitutionCodePropertyConvention : IPropertyConvention, IPropertyConventionAcceptance
    {
        public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
        {
            Entities.Entity e;
            criteria.Expect(x => x.Property.PropertyType == typeof(string) &&
                                 x.Property.PropertyType.Name == nameof(e.InstitutionCode)
                            );
        }

        public void Apply(IPropertyInstance instance)
        {
            instance.Index("ind_InstitutionCode");
        }

    }
}
