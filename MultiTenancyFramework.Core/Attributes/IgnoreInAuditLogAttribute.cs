using System;

namespace MultiTenancyFramework
{
    /// <summary>
    /// When used, it specifies that the entity property should be ignored by audit trail.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class IgnoreInAuditLogAttribute : Attribute
    {

    }
}
