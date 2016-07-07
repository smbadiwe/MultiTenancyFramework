using System.Collections.Concurrent;

namespace MultiTenancyFramework.NHibernate.Audit
{
    internal static class TrackingDataStore
    {
        /// <summary>
        /// This tells us whether or not a given property (the key) in an entity should be skipped in AuditLog.
        /// <para>Value = false means it SHOULD NOT be skipped</para>
        /// </summary>
        internal static ConcurrentDictionary<PropertyConfigKey, bool> PropertyConfigStore = new ConcurrentDictionary<PropertyConfigKey, bool>();
    }
}
