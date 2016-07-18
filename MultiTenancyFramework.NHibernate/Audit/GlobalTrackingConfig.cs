using MultiTenancyFramework.Entities;
using System;
using System.Linq.Expressions;

namespace MultiTenancyFramework.NHibernate.Audit
{
    public static class GlobalTrackingConfig
    {
        public static bool Enabled { get; set; } = true;

        public static bool TrackEmptyPropertiesOnAdditionAndDeletion { get; set; } = false;

        public static bool DisconnectedContext { get; set; } = false;
        
        public static Type SoftDeletableType { get; set; } = typeof(IEntity);

        public static string SoftDeletablePropertyName { get; set; } = "IsDeleted";

        /// <summary>
        /// Set the property  to use for tracking deleted entities
        /// </summary>
        /// <typeparam name="TSoftDeletable"></typeparam>
        /// <param name="softDeletableProperty"></param>
        public static void SetSoftDeletableCriteria<TSoftDeletable>(Expression<Func<TSoftDeletable, bool>> softDeletableProperty)
        {
            SoftDeletableType = typeof(TSoftDeletable);
            SoftDeletablePropertyName = softDeletableProperty.GetPropertyInfo().Name;
        }
    }
}
