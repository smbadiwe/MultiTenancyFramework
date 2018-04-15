using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using System.Collections.Generic;
using System.Linq;

namespace MultiTenancyFramework.NHibernate.Queries
{
    public class GetEntitiesAppearingInAuditLogsQueryHandler : CoreGeneralDAO, IDbQueryHandler<GetEntitiesAppearingInAuditLogsQuery, IList<string>>
    {
        public IList<string> Handle(GetEntitiesAppearingInAuditLogsQuery theQuery)
        {
            var session = BuildSession();

            var query = session.Query<AuditLog>()
                .Where(x => x.Entity != null)
                .OrderBy(t => t.Entity).Select(s => s.Entity).Distinct();
            return query.ToList();
        }
    }
}
