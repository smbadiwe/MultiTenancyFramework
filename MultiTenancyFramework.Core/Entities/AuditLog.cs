using System;
using System.Collections.Generic;

namespace MultiTenancyFramework.Entities
{
    public class AuditLog : BaseEntity, IDoNotNeedAudit
    {
        /// <summary>
        /// To set it, call the extension: SetTrailItems
        /// </summary>
        public virtual List<TrailItem> TrailItems { get; set; }

        /// <summary>
        /// NB: I set datatype to string because there's this IEntity<idT> that every entity implements.
        /// So, cast the Id accordingly. The defauldt id type is long.
        /// </summary>
        public virtual string EntityId { get; set; }

        public virtual EventType EventType { get; set; }
        public virtual string EventTypeStr { get { return EventType.ToReadableString(); } }

        public virtual DateTime EventDate { get; set; }
        public virtual string EventDateStr { get { return EventDate.ToString("dd-MMM-yyyy hh:mm:ss tt"); } }

        /// <summary>
        /// Serialized and compressed data. When deserialized, this will usually be a List<TrailItem>
        /// </summary>
        public virtual string AuditData { get; set; }

        /// <summary>
        /// The name of the entity whose record is tracked for audit
        /// </summary>
        public virtual string Entity { get; set; }

        public virtual string ApplicationName { get; set; }

        private string _userName;
        public virtual string UserName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_userName)) return "[[System]]";
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }

        public virtual long UserId { get; set; }

        public virtual string ClientIpAddress { get; set; }

        public virtual string ClientName { get; set; }

        public virtual string Remark { get; set; }
    }
}
