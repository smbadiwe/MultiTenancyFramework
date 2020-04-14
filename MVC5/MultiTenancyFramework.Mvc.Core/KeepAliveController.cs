using System.Web.Mvc;

namespace MultiTenancyFramework.Mvc
{
    /// <summary>
    /// A scheduled task calls this keepalive/index to, you know, keep the app alive.
    /// </summary>
    /// <seealso cref="Controller" />
    [AllowAnonymous]
    public class KeepAliveController : Controller
    {
        public virtual ActionResult Index()
        {
            return new EmptyResult();
        }
    }
}
