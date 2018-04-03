using MultiTenancyFramework.Core.TaskManager;
using MultiTenancyFramework.Data.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MultiTenancyFramework.NHibernate.TaskManager
{
    public class GetTaskByTypeQueryHandler : CoreGeneralDAO, IDbQueryHandlerAsync<GetTaskByTypeQuery, ScheduledTask>
    {
        public async Task<ScheduledTask> Handle(GetTaskByTypeQuery theQuery, CancellationToken token = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(theQuery.Type))
                return null;

            var session = BuildSession();
            var query = session.QueryOver<ScheduledTask>()
                .Where(x => x.Type == theQuery.Type)
                .OrderBy(x => x.Id).Desc
                .Take(1);

            return await query.SingleOrDefaultAsync(token);
        }
    }
}
