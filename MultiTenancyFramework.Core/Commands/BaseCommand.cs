namespace MultiTenancyFramework.Commands
{
    /// <summary>
    /// Inherit from this base class if your command execution is going to affect tenants (institutions)
    /// </summary>
    public abstract class BaseCommand : ICommand
    {
        public string InstitutionCode { get; set; }
    }
}
