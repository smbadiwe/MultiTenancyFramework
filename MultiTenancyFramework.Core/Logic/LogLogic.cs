using System;
using System.Collections.Generic;
using System.Linq;
using MultiTenancyFramework.Entities;
using System.Threading.Tasks;
using MultiTenancyFramework.Data;
using System.Web;

namespace MultiTenancyFramework.Logic
{
    public class LogLogic : CoreLogic<Log>
    {
        public LogLogic() : base(MyServiceLocator.GetInstance<ICoreDAO<Log>>())
        {

        }
        
        /// <summary>
        /// Gets all log items
        /// </summary>
        /// <param name="fromUtc">Log item creation from; null to load all records</param>
        /// <param name="toUtc">Log item creation to; null to load all records</param>
        /// <param name="message">Message</param>
        /// <param name="logLevel">Log level; null to load all records</param>
        /// <param name="instCode">Institution code</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Log item items</returns>
        public async Task<RetrievedData<Log>> GetAllLogs(DateTime? fromUtc = null, DateTime? toUtc = null,
            string message = "", LoggingLevel? logLevel = null, string instCode = "",
            string ip = "", string sessionId = "", string username = "", string logger = "",
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            if (string.IsNullOrWhiteSpace(instCode))
                return new RetrievedData<Log>();

            var query = _dao.Table;
            if (fromUtc.HasValue && fromUtc.Value > DateTime.MinValue)
                query = query.Where(l => fromUtc.Value <= l.DateCreated);
            if (toUtc.HasValue && toUtc.Value > DateTime.MinValue)
                query = query.Where(l => toUtc.Value >= l.DateCreated);
            if (logLevel.HasValue)
                query = query.Where(l => logLevel == l.LoggingLevel);

            if (!string.IsNullOrWhiteSpace(instCode))
                query = query.Where(l => instCode == l.InstitutionId);
            if (!string.IsNullOrWhiteSpace(username))
                query = query.Where(l => username.StartsWith(l.UserId));
            if (!string.IsNullOrWhiteSpace(logger))
                query = query.Where(l => logger == l.Logger);
            if (!string.IsNullOrWhiteSpace(sessionId))
                query = query.Where(l => sessionId == l.SessionId);
            if (!string.IsNullOrWhiteSpace(ip))
                query = query.Where(l => ip == l.IpAddress);

            if (!string.IsNullOrWhiteSpace(message))
                query = query.Where(l => l.ShortMessage.Contains(message) || l.FullMessage.Contains(message));

            query = query.OrderByDescending(l => l.DateCreated);

            var log = await _dao.RetrieveUsingPagingAsync(query, pageIndex, pageSize);
            return log;
        }
        
        public IList<string> GetLoggers()
        {
            var query = _dao.Table;
            return query.Select(x => x.Logger).Distinct().ToList();
        }

        /// <summary>
        /// Adds the log to per-request cache. They will be flushed to DB at once, at the end of request.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AddRequestLog(Log log)
        {
            RequestLogs.Add(log);
        }

        /// <summary>
        /// Clears a log
        /// </summary>
        public async Task ClearLog()
        {
            await _dao.RunDirectQueryAsync($"TRUNCATE TABLE {typeof(Log).GetTableName()}");
        }

        /// <summary>
        /// Inserts a log item
        /// </summary>
        /// <param name="context">HTTP Context</param>
        /// <param name="logLevel">Log level</param>
        /// <param name="shortMessage">The short message</param>
        /// <param name="fullMessage">The full message</param>
        /// <param name="customer">The customer to associate log record with</param>
        /// <returns>A log item</returns>
        public void InsertLog(string loggerName, HttpContext context, LoggingLevel logLevel, string shortMessage, string fullMessage = null) //, Customer customer = null)
        {
            if (string.IsNullOrWhiteSpace(shortMessage)) return;
            ////check ignore word/phrase list?
            //if (IgnoreLog(shortMessage) || IgnoreLog(fullMessage))
            //    return null;

            var log = new Log
            {
                LoggingLevel = logLevel,
                ShortMessage = shortMessage,
                FullMessage = fullMessage,
                Logger = loggerName,
                DateCreated = Now()
            };
            
            if (context != null)
            {
                var webHelper = new SimpleWebHelper(context);

                log.IpAddress = webHelper.GetCurrentIpAddress();
                log.PageUrl = webHelper.GetThisPageUrl(true, true);
                log.ReferrerUrl = webHelper.GetUrlReferrer();
                try
                {
                    bool requestAvailable;
                    log.InstitutionId = webHelper.GetInstitutionCode(out requestAvailable) ?? Utilities.INST_DEFAULT_CODE;
                }
                catch (LogOutUserException)
                {
                    log.InstitutionId = Utilities.INST_DEFAULT_CODE;
                }
                catch (GeneralException ex)
                {
                    log.ShortMessage = $"[{ex.Message}] " + log.ShortMessage;
                    log.InstitutionId = Utilities.INST_DEFAULT_CODE;
                }
                log.UserId = context.User?.Identity?.Name;
                log.SessionId = webHelper.GetSessionId();

                AddRequestLog(log);
            }
            else
            {
                // Save it straight from here
                Insert(log);
            }
        }

        public void FlushRequestLogs()
        {
            Insert(RequestLogs);
        }

        public override void OnBeforeUpdatingList(IList<Log> e)
        {
            foreach (var elm in e)
                elm.SkipAudit = true;

            base.OnBeforeUpdatingList(e);
        }

        public override void OnBeforeUpdating(Log e)
        {
            e.SkipAudit = true;
            base.OnBeforeUpdating(e);
        }

        private List<Log> RequestLogs
        {
            get
            {
                if (HttpContext.Current == null || HttpContext.Current.Items == null)
                    throw new InvalidOperationException("This needs HTTP Context available");
                List<Log> list;
                if (HttpContext.Current.Items.Contains("RequestLogs"))
                {
                    list = HttpContext.Current.Items["RequestLogs"] as List<Log>;
                    if (list == null)
                    {
                        list = new List<Log>();
                        HttpContext.Current.Items["RequestLogs"] = list;
                    }
                    return list;
                }

                HttpContext.Current.Items["RequestLogs"] = list = new List<Log>();
                return list;
            }
        }
    }
}
