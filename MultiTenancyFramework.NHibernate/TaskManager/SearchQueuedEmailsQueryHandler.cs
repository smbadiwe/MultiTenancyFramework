using MultiTenancyFramework.Core.TaskManager.Tasks;
using MultiTenancyFramework.Data;
using MultiTenancyFramework.Data.Queries;
using System;
using NHibernate.Criterion;
using System.Threading.Tasks;
using System.Threading;

namespace MultiTenancyFramework.NHibernate.TaskManager
{
    public class SearchQueuedEmailsQueryHandler : CoreGeneralWithGridPagingDAO<QueuedEmail>, IDbQueryHandlerAsync<SearchQueuedEmailsQuery, RetrievedData<QueuedEmail>>
    {
        public Task<RetrievedData<QueuedEmail>> Handle(SearchQueuedEmailsQuery theQuery, CancellationToken token = default(CancellationToken))
        {
            theQuery.FromEmail = (theQuery.FromEmail ?? "").Trim();
            theQuery.ToEmail = (theQuery.ToEmail ?? "").Trim();

            var session = BuildSession();
            var query = session.QueryOver<QueuedEmail>();
            if (!string.IsNullOrWhiteSpace(theQuery.FromEmail))
                query = query.Where(qe => qe.From.IsInsensitiveLike(theQuery.FromEmail, MatchMode.Anywhere));
            
            if (!string.IsNullOrWhiteSpace(theQuery.ToEmail))
                query = query.Where(qe => qe.To.IsInsensitiveLike(theQuery.ToEmail, MatchMode.Anywhere));
            
            if (theQuery.CreatedFromUtc.HasValue)
                query = query.Where(qe => qe.CreatedOnUtc >= theQuery.CreatedFromUtc);

            if (theQuery.CreatedToUtc.HasValue)
                query = query.Where(qe => qe.CreatedOnUtc <= theQuery.CreatedToUtc);

            if (theQuery.LoadNotSentItemsOnly)
                query = query.Where(qe => !qe.SentOnUtc.HasValue);

            if (theQuery.LoadOnlyItemsToBeSent)
            {
                var nowUtc = DateTime.UtcNow;
                query = query.Where(qe => !qe.DontSendBeforeDateUtc.HasValue || qe.DontSendBeforeDateUtc.Value <= nowUtc);
            }

            query = query.Where(qe => qe.SentTries <= theQuery.MaxSendTries);
            query = theQuery.LoadNewest ?
                //load the newest records
                query.OrderBy(qe => qe.CreatedOnUtc).Desc :
                //else, load by priority
                query.OrderBy(qe => qe.Priority).Desc
                .OrderBy(qe => qe.CreatedOnUtc).Asc;

            var result = RetrieveUsingPaging(query, theQuery.PageIndex, theQuery.PageSize, true);
            return Task.FromResult(result);
        }
    }
}
