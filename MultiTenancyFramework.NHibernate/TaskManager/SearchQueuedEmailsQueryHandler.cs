using MultiTenancyFramework.Core.TaskManager.Tasks;
using MultiTenancyFramework.Data;
using MultiTenancyFramework.Data.Queries;
using System;
using System.Linq;

namespace MultiTenancyFramework.NHibernate.TaskManager
{
    public class SearchQueuedEmailsQueryHandler : CoreGeneralWithGridPagingDAO<QueuedEmail>, IDbQueryHandler<SearchQueuedEmailsQuery, RetrievedData<QueuedEmail>>
    {
        public RetrievedData<QueuedEmail> Handle(SearchQueuedEmailsQuery theQuery)
        {
            theQuery.FromEmail = (theQuery.FromEmail ?? "").Trim();
            theQuery.ToEmail = (theQuery.ToEmail ?? "").Trim();

            var session = BuildSession();
            var query = session.Query<QueuedEmail>();
            if (!string.IsNullOrWhiteSpace(theQuery.FromEmail))
                query = query.Where(qe => qe.Sender.Contains(theQuery.FromEmail));

            if (!string.IsNullOrWhiteSpace(theQuery.ToEmail))
                query = query.Where(qe => qe.Receivers.Contains(theQuery.ToEmail));

            if (theQuery.CreatedFromUtc.HasValue && theQuery.CreatedFromUtc.Value > DateTime.MinValue)
                query = query.Where(qe => qe.CreatedOnUtc >= theQuery.CreatedFromUtc);

            if (theQuery.CreatedToUtc.HasValue && theQuery.CreatedToUtc.Value > DateTime.MinValue)
                query = query.Where(qe => qe.CreatedOnUtc <= theQuery.CreatedToUtc);

            if (theQuery.LoadNotSentItemsOnly)
                query = query.Where(qe => qe.SentOnUtc == null || qe.SentOnUtc == DateTime.MinValue);

            if (theQuery.LoadOnlyItemsToBeSent)
            {
                var nowUtc = DateTime.UtcNow;
                query = query.Where(qe => qe.DontSendBeforeDateUtc == null || qe.DontSendBeforeDateUtc == DateTime.MinValue
                || qe.DontSendBeforeDateUtc <= nowUtc);
            }

            query = query.Where(qe => qe.SentTries <= theQuery.MaxSendTries);
            query = theQuery.LoadNewest ?
                //load the newest records
                query.OrderByDescending(qe => qe.CreatedOnUtc) :
                //else, load by priority
                query.OrderByDescending(qe => qe.Priority)
                .ThenBy(qe => qe.CreatedOnUtc);

            var result = RetrieveUsingPaging(query, theQuery.PageIndex, theQuery.PageSize, true);
            return result;
        }
    }
}
