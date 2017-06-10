using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;

namespace MultiTenancyFramework.WebAPI
{
    public class GlobalExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            //TODO: Do whatever logging you need to do here.
            base.Log(context);
        }

        public override Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            //TODO: Do whatever logging you need to do here.
            return base.LogAsync(context, cancellationToken);
        }
    }
}
