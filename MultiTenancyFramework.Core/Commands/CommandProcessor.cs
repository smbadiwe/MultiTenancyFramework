using System;
using System.Diagnostics;

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
            var handlerType =
                typeof(ICommandHandler<>).MakeGenericType(command.GetType());

            dynamic handler = _serviceProvider.GetService(handlerType);
            handler.Handle((dynamic)command);
        }
    }
}
