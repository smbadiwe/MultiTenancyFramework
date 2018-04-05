namespace MultiTenancyFramework.Commands
{
    /// <summary>
    /// Inherit from this base class if your command execution is going to affect tenants (institutions)
    /// </summary>
    public abstract class BaseCommand : ICommand
    {
        public string InstitutionCode { get; set; }
        private ILogger _logger;
        /// <summary>
        /// Logger to log errors and/or messages
        /// </summary>
        public virtual ILogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = Utilities.Logger;
                    _logger.SetNLogLogger(GetType().FullName);
                }
                return _logger;
            }
        }

    }

    public abstract class BaseCommandAsync : ICommandAsync
    {
        public string InstitutionCode { get; set; }
        private ILogger _logger;
        /// <summary>
        /// Logger to log errors and/or messages
        /// </summary>
        public virtual ILogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = Utilities.Logger;
                    _logger.SetNLogLogger(GetType().FullName);
                }
                return _logger;
            }
        }

    }
}
