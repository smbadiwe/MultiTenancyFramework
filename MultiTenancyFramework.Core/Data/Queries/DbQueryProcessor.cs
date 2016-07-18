using System;

namespace MultiTenancyFramework.Data.Queries
{
    public sealed class DbQueryProcessor : IDbQueryProcessor
    {
        private IServiceProvider _serviceProvider;
        public DbQueryProcessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public string InstitutionCode { get; set; }

        //[DebuggerStepThrough]
        public TResult Process<TResult>(IDbQuery<TResult> query)
        {
            var handlerType =
                typeof(IDbQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

            dynamic handler = _serviceProvider.GetService(handlerType);
            handler.InstitutionCode = InstitutionCode;

            return handler.Handle((dynamic)query);
        }
    }
}
