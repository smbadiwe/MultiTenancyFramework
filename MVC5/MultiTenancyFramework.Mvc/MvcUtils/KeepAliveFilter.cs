using System.Web.Mvc;

namespace MultiTenancyFramework.Mvc
{
    public class KeepAliveFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.RawUrl.Contains("keepalive/index"))
            {
                filterContext.Result = new EmptyResult();
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
