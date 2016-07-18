using FluentNHibernate.Mapping;
using NHibernate;

namespace MultiTenancyFramework.NHibernate.NHManager.Listeners
{
    internal class AppFilterDefinition : FilterDefinition
    {
        public AppFilterDefinition()
        {
            //Where IsDeleted != 1 AND InstitutionCode = :instCode
            WithName(Utilities.InstitutionFilterName)
                .WithCondition($"{Utilities.SoftDeletePropertyName} != 1 AND {Utilities.InstitutionCodePropertyName} = :{Utilities.InstitutionCodeQueryParamName}")
                .AddParameter(Utilities.InstitutionCodeQueryParamName, NHibernateUtil.String);
        }
    }
}
