using FluentNHibernate.Mapping;
using NHibernate;

namespace MultiTenancyFramework.NHibernate.NHManager.Listeners
{
    internal class AppFilterDefinition : FilterDefinition
    {
        public AppFilterDefinition()
        {
            if (!ConfigurationHelper.AppSettingsItem<bool>("SingleTenant"))
            {
                //Where IsDeleted != true AND InstitutionCode = :instCode
                WithName(Utilities.InstitutionFilterName)
                .WithCondition($"{Utilities.SoftDeletePropertyName} != :{Utilities.SoftDeleteParamName} AND {Utilities.InstitutionCodePropertyName} = :{Utilities.InstitutionCodeQueryParamName}")
                .AddParameter(Utilities.InstitutionCodeQueryParamName, NHibernateUtil.String)
                .AddParameter(Utilities.SoftDeleteParamName, NHibernateUtil.Boolean);
            }
        }
    }
}
