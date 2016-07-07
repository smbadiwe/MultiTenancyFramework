using System;
using System.Collections.Generic;

namespace MultiTenancyFramework.Entities
{
    public class AppUser : Person
    {
        /// <summary>
        /// NOT MAPPED: 
        /// </summary>
        public virtual string InstitutionShortName { get; set; }
        public virtual string UserName { get; set; }
        /// <summary>
        /// Not set by default. To set, call the .SetRoleNames() method
        /// </summary>
        public virtual string RoleNames { get; set; }

        #region ASP.NET IdentityUser stuffs
        /// <summary>
        ///     True if the email is confirmed, default is false
        /// </summary>
        public virtual bool EmailConfirmed { get; set; }

        /// <summary>
        ///     The salted/hashed form of the user password. Same as Password property above
        /// </summary>
        public virtual string PasswordHash { get; set; }

        /// <summary>
        ///     A random value that should change whenever a users credentials have changed (password changed, login removed)
        /// </summary>
        public virtual string SecurityStamp { get; set; }

        /// <summary>
        ///     True if the phone number is confirmed, default is false
        /// </summary>
        public virtual bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        ///     Is two factor enabled for the user
        /// </summary>
        public virtual bool TwoFactorEnabled { get; set; }

        /// <summary>
        ///     DateTime in UTC when lockout ends, any time in the past is considered not locked out.
        /// </summary>
        public virtual DateTime? LockoutEndDateUtc { get; set; }

        /// <summary>
        ///     Is lockout enabled for this user
        /// </summary>
        public virtual bool LockoutEnabled { get; set; }

        /// <summary>
        ///     Used to record failures for the purposes of lockout
        /// </summary>
        public virtual int AccessFailedCount { get; set; }

        #endregion

        public virtual bool ForceChangeOfPassword { get; set; }

        /// <summary>
        /// A comma-separated string of userrole ids
        /// </summary>
        public virtual string UserRoles { get; set; }

        public virtual HashSet<long> UserRoleIDs
        {
            get
            {
                if (string.IsNullOrWhiteSpace(UserRoles)) return new HashSet<long>();

                var splitted = UserRoles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
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

    }
}
