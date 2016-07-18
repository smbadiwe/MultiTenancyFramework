using System;
using System.Reflection;

namespace MultiTenancyFramework.NHibernate.Audit
{
    public static class PropertyTracking
    {
        public static bool IsTrackingEnabled(PropertyConfigKey property, Type entityType)
        {
            bool isEnabled;
            if (!TrackingDataStore.PropertyConfigStore.TryGetValue(property, out isEnabled))
            {
                isEnabled = entityType.GetProperty(property.PropertyName).GetCustomAttribute<IgnoreInAuditLogAttribute>() == null;
                TrackingDataStore.PropertyConfigStore.TryAdd(property, isEnabled);
            }
            return isEnabled;
        }
    }
}
