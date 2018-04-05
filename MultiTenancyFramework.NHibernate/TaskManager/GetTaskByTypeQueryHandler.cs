using MultiTenancyFramework.Core.TaskManager;
using MultiTenancyFramework.Data.Queries;
using System.Linq;
using NHibernate.Linq;
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
            var query = session.Query<ScheduledTask>()
                .Where(x => x.Type == theQuery.Type)
                .OrderByDescending(x => x.Id);

            return await query.FirstOrDefaultAsync();
        }
    }
}
