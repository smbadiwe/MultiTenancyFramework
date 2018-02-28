using System.Threading;
using System.Threading.Tasks;

namespace MultiTenancyFramework.Commands
{
    public interface ICommandProcessor
    {
        void Process(ICommand command);
        Task ProcessAsync(ICommandAsync command, CancellationToken token = default(CancellationToken));
    }
}
