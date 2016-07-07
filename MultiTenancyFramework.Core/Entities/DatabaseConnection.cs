namespace MultiTenancyFramework.Entities
{
    public class DatabaseConnection : Entity, IAmHostedCentrally
    {
        public virtual string ConnectionString { get; set; }
        /// <summary>
        /// Comma-separated values of the InstitutionCode of the tenants whose data is on this DB
        /// </summary>
        public virtual string TenantsOnIt { get; set; }
        public virtual int MaximumNumberOfInstitutionsHosted { get; set; } = 1;
        public virtual int NumberOfInstitutionsCurrentlyHosted { get; set; } = 1;
    }
}
