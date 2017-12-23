using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using System.Collections.Generic;

namespace MultiTenancyFramework.Mvc.Data.Queries
{
    public class GetUsersWithThesePrivilegesQuery : IDbQuery<IList<AppUser>>
    {
        /// <summary>
        /// Gets or sets the name of the preferred entity. THis entity will typically be a child class of AppUser. Leave null if not available
        /// </summary>
        /// <value>
        /// The name of the preferred entity.
        /// </value>
        public string PreferredEntityName { get; set; }
        public string[] Privileges { get; set; }
    }
}
