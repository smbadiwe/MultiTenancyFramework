using MultiTenancyFramework.Data;
using MultiTenancyFramework.Data.Queries;
using System;

namespace MultiTenancyFramework.Core.TaskManager.Tasks
{
    public class SearchQueuedEmailsQuery : DbPagingQuery, IDbQuery<RetrievedData<QueuedEmail>>
    {
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public DateTime? CreatedFromUtc { get; set; }
        public DateTime? CreatedToUtc { get; set; }
        public bool LoadNotSentItemsOnly { get; set; }
        public bool LoadOnlyItemsToBeSent { get; set; }
        public bool LoadNewest { get; set; }
        public int MaxSendTries { get; set; }

    }
}
