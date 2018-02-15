using System;
using System.Collections.Generic;

namespace MultiTenancyFramework.Entities
{
    public class DatabaseConnection : Entity, IAmHostedCentrally
    {
        public virtual string ConnectionString { get; set; }
        /// <summary>
        /// Comma-separated values of the InstitutionCode of the tenants whose data is on this DB
        /// </summary>
        public virtual string TenantsOnIt { get; set; }
        /// <summary>
        /// The InstitutionCode of the tenants whose data is on this DB
        /// </summary>
        public virtual HashSet<string> TenantCodes
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TenantsOnIt)) return new HashSet<string>();

                var splitted = TenantsOnIt.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                HashSet<string> userRoles = new HashSet<string>(splitted);
                return userRoles;
            }
        }
        public virtual int MaximumNumberOfInstitutionsHosted { get; set; } = 1;
        public virtual int NumberOfInstitutionsCurrentlyHosted { get; set; } = 1;
    }
}
