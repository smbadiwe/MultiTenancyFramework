using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;

namespace MultiTenancyFramework.WebAPI
{
    public class GlobalExceptionHandler : ExceptionHandler
    {
        //Exception Handlers are called after Exception Filters and Exception Loggers, 
        // and only if the exception has not already been handled.
        public override void Handle(ExceptionHandlerContext context)
        {
            //TODO: Implement GlobalExceptionHandler
            base.Handle(context);
        }

        public override Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            //TODO: Implement GlobalExceptionHandler
            return base.HandleAsync(context, cancellationToken);
        }
    }
}
