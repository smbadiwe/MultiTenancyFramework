using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;

namespace MultiTenancyFramework.WebAPI
{
    public class GlobalExceptionFilterAttribute : ExceptionFilterAttribute
    {
        //Exception Filters are used whenever a controller action throws an unhandled
        // exception that is not an HttpResponseException.
        public override void OnException(HttpActionExecutedContext context)
        {
            //TODO: GlobalExceptionFilterAttribute
            base.OnException(context);
        }

        public override Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            //TODO: GlobalExceptionFilterAttribute
            return base.OnExceptionAsync(actionExecutedContext, cancellationToken);
        }
    }
}
