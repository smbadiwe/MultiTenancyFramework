using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MultiTenancyFramework.Commands
{
    public sealed class CommandProcessor : ICommandProcessor
    {
        private IServiceProvider _serviceProvider;
        public CommandProcessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        [DebuggerStepThrough]
        public void Process(ICommand command)
        {
            if (command == null) throw new ArgumentNullException("command");

            var handlerType =
                typeof(ICommandHandler<>).MakeGenericType(command.GetType());

            dynamic handler = _serviceProvider.GetService(handlerType);
            handler.Handle((dynamic)command);
        }

        [DebuggerStepThrough]
        public Task ProcessAsync(ICommandAsync command, CancellationToken token = default(CancellationToken))
        {
            if (command == null) throw new ArgumentNullException("command");

            var handlerType =
                typeof(ICommandHandlerAsync<>).MakeGenericType(command.GetType());

            dynamic handler = _serviceProvider.GetService(handlerType);
            return handler.Handle((dynamic)command, (dynamic)token);
        }
    }
}
