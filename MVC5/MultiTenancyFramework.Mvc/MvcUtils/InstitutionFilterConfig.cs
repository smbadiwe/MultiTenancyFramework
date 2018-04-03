using System.Web.Mvc;

namespace MultiTenancyFramework.Mvc
{
    /// <summary>
    /// My custom Filter Config
    /// </summary>
    public class InstitutionFilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new KeepAliveFilter(), int.MinValue); // run first
            filters.Add(new GlobalAuthorizeAttribute());
            filters.Add(new GlobalExceptionFilterAttribute());
        }
    }
}
