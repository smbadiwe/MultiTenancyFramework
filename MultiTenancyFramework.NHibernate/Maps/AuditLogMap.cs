using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.NHibernate.Maps
{
    public class AuditLogMap : EntityMap<AuditLog>
    {
        public AuditLogMap()
        {
            Map(x => x.EventDate);
            Map(x => x.EventType);
            Map(x => x.Entity);
            Map(x => x.EntityId);
            Map(x => x.AuditData).VarCharMax();
            Map(x => x.UserId);
            Map(x => x.UserName);
            Map(x => x.ClientIpAddress);
            Map(x => x.ClientName);
            Map(x => x.ApplicationName);
        }
    }
}
