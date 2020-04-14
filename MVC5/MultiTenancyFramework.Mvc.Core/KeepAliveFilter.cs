using System;
using System.Web.Mvc;

namespace MultiTenancyFramework.Mvc
{
    public class KeepAliveFilter : IActionFilter
    {
        public string UrlPath { get; set; }
        public KeepAliveFilter(string urlPath = "keepalive/index")
        {
            UrlPath = urlPath;
        }
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.RawUrl.Contains(UrlPath))
            {
                filterContext.Result = new EmptyResult();
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

    }
}
