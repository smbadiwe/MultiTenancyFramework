using System;
using System.Collections.Generic;

namespace MultiTenancyFramework.Entities
{
    public class DatabaseConnection : Entity, IAmHostedCentrally
    {
        public virtual string ConnectionString { get; set; }
        /// <summary>
        /// Comma-separated values of the IDs of the tenants whose data is on this DB
        /// </summary>
        public virtual string TenantsOnIt { get; set; }
        /// <summary>
        /// The IDs of the tenants whose data is on this DB
        /// </summary>
        public virtual HashSet<long> TenantsIDs
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TenantsOnIt)) return new HashSet<long>();

                var splitted = TenantsOnIt.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                HashSet<long> userRoles = new HashSet<long>();
                foreach (var item in splitted)
                {
                    long id;
                    if (long.TryParse(item, out id))
                    {
                        userRoles.Add(id);
                    }
                }
                return userRoles;
            }
        }
        public virtual int MaximumNumberOfInstitutionsHosted { get; set; } = 1;
        public virtual int NumberOfInstitutionsCurrentlyHosted { get; set; } = 1;
    }
}
