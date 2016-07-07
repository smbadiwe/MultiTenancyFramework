using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using MultiTenancyFramework;
using System;

namespace MultiTenancyFramework.NHibernate.NHManager.Conventions
{
    public class EnumMappingConvention : IPropertyConvention, IPropertyConventionAcceptance
    {
        public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
        {
            criteria.Expect(x => x.Property.PropertyType.IsEnum ||
                                    (x.Property.PropertyType.IsNullable() &&
                                     Nullable.GetUnderlyingType(x.Property.PropertyType).IsEnum)
                            );
        }

        public void Apply(IPropertyInstance instance)
        {
            instance.CustomType(instance.Property.PropertyType);
        }

    }
}
