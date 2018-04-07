using System;
using System.Web.Mvc;

namespace MultiTenancyFramework.Mvc
{
    public class KeepAliveFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.RawUrl.Contains("keepalive/index"))
            {
                filterContext.Result = new EmptyResult();
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

    }
}
