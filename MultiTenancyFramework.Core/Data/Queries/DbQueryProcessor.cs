using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

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
       
        [DebuggerStepThrough]
        public TResult Process<TResult>(IDbQuery<TResult> query)
        {
            if (query == null) throw new ArgumentNullException("query");

            var handlerType =
                typeof(IDbQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

            dynamic handler = _serviceProvider.GetService(handlerType);
            handler.InstitutionCode = InstitutionCode;

            return handler.Handle((dynamic)query);
        }

        [DebuggerStepThrough]
        public Task<TResult> ProcessAsync<TResult>(IDbQueryAsync<TResult> query, CancellationToken token = default(CancellationToken))
        {
            if (query == null) throw new ArgumentNullException("query");

            var handlerType =
                typeof(IDbQueryHandlerAsync<,>).MakeGenericType(query.GetType(), typeof(TResult));

            dynamic handler = _serviceProvider.GetService(handlerType);
            handler.InstitutionCode = InstitutionCode;

            return handler.Handle((dynamic)query, (dynamic)token);
        }
    }
}
