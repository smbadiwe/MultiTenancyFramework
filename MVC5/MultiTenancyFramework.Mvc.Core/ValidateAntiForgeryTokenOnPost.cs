using System;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;

namespace MultiTenancyFramework.Mvc
{
    public class ValidateAntiForgeryTokenOnPost : FilterAttribute, IAuthorizationFilter
    {
        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var ignoreToken = filterContext.ActionDescriptor.GetCustomAttributes(typeof(IgnoreAntiForgeryTokenAttribute), true)
                            .Cast<IgnoreAntiForgeryTokenAttribute>().FirstOrDefault();
                    if (ignoreToken == null)
                    {
                        AntiForgery.Validate();
                    }
                }
                catch (HttpAntiForgeryException)
                {
                    if (filterContext.RequestContext.HttpContext.Request.IsAuthenticated)
                    {
                        throw;
                    }
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class IgnoreAntiForgeryTokenAttribute : Attribute
    {

    }
}
