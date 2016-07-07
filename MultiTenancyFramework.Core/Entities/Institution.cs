namespace MultiTenancyFramework.Entities
{
    public class Institution : Entity, IAmHostedCentrally
    {
        /// <summary>
        /// True means the modification was done by the system (the landlord').
        /// False means the owning institution modified their data themselves.
        /// Null means it has not been modified
        /// </summary>
        public virtual bool? LastModificationDoneByUs { get; set; } = null;

        public virtual string Code { get; set; }
        
        public virtual string Email { get; set; }

        public virtual string Phone { get; set; }

        public virtual string ShortName { get; set; }
        
        public virtual long DatabaseConnectionId { get; set; }
        
        /// <summary>
        /// Username for initial login before DB is alloted to institution
        /// </summary>
        public virtual string TempUserName { get; set; }

        /// <summary>
        /// Username for initial login before DB is alloted to institution
        /// </summary>
        public virtual string TempPassword { get; set; }
        
        public override string ToString()
        {
            return $"Id: {Id}; Name: {Name}; Code: {Code}";
        }
    }
}
