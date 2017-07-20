using System;
using System.Collections.Generic;

namespace MultiTenancyFramework.Entities
{
    public class UserRole : Entity
    {
        public virtual string Description { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this user-role is anticipated a-priori by the developer and thus configured out-of-the-box.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is anticipated a-priori; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsSystemProvided { get; set; }

        /// <summary>
        /// Comma-separated string of Privilege IDs
        /// </summary>
        public virtual string Privileges { get; set; } = string.Empty;

        public virtual HashSet<long> PrivilegeIDs
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Privileges)) return new HashSet<long>();

                var splitted = Privileges.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                HashSet<long> privileges = new HashSet<long>();
                foreach (var item in splitted)
                {
                    long id;
                    if (long.TryParse(item, out id))
                    {
                        privileges.Add(id);
                    }
                }
                return privileges;
            }
        }
    }
}
