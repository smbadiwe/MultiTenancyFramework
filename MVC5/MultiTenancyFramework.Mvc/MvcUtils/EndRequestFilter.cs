using MultiTenancyFramework.Data;
using MultiTenancyFramework.Logic;
using System.Web.Mvc;

namespace MultiTenancyFramework.Mvc
{
    public class EndRequestFilter : IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            // Save log entries
            var logLogic = new LogLogic();
            logLogic.FlushRequestLogs();

            // Stop if static resource
            var webHelper = new WebHelper(filterContext.HttpContext);
            if (webHelper.IsStaticResource())
            {
                return;
            }

            // Close DB connections
            var mgr = MyServiceLocator.GetInstance<IDbSessionCleanup>();
            mgr.CloseDbConnections();
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
        }
    }
}
