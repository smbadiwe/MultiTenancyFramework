namespace MultiTenancyFramework.Commands
{
    public interface ICommandProcessor
    {
        void Process(ICommand command);
    }
}
