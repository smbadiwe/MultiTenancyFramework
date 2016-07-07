using System.Collections.Generic;

namespace MultiTenancyFramework.Data.Queries
{
    /// <summary>
    /// This query, when run, returns list of all the entities that has been logged at some point in time in the audit logs table
    /// </summary>
    public class GetEntitiesAppearingInAuditLogsQuery : IDbQuery<IList<string>>
    {
    }
}
