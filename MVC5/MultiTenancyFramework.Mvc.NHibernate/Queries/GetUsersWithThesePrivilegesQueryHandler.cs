using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using MultiTenancyFramework.Mvc.Data.Queries;
using MultiTenancyFramework.NHibernate;
using NHibernate.Criterion;
using System.Collections.Generic;

namespace MultiTenancyFramework.Mvc.NHibernate.Queries
{
    public class GetUsersWithThesePrivilegesQueryHandler : CoreGeneralDAO, IDbQueryHandler<GetUsersWithThesePrivilegesQuery, IList<AppUser>>
    {
        public IList<AppUser> Handle(GetUsersWithThesePrivilegesQuery theQuery)
        {
            if (theQuery.Privileges == null || theQuery.Privileges.Length == 0) return null;

            var session = BuildSession();
            // Get the corresp. ActionAccessPrivilege instances
            var privilegeInstances = session.QueryOver<ActionAccessPrivilege>()
                .Where(x => x.Name.IsIn(theQuery.Privileges))
                .AndNot(x => x.IsDisabled).AndNot(x => x.IsDeleted)
                .List();
            if (privilegeInstances != null && privilegeInstances.Count > 0)
            {
                // Get the User-roles that have these privileges
                var query = string.Format(@"SELECT * FROM UserRoles WHERE IsDeleted <> 1 AND IsDisabled <> 1 AND InstitutionCode = '{1}'
                                            AND ((Privileges LIKE '%,{0},%' OR Privileges LIKE '{0},%')",
                                            privilegeInstances[0].Id, InstitutionCode);
                for (int i = 1; i < privilegeInstances.Count; i++)
                {
                    query += string.Format(" OR (Privileges LIKE '%,{0},%' OR Privileges LIKE '{0},%')", privilegeInstances[i].Id);
                }
                query += ");";
                var userRoles = RetrieveUsingDirectQuery<UserRole>(query);
                if (userRoles != null && userRoles.Count > 0)
                {
                    // Get the users that have these user-roles
                    query = string.Format(@"SELECT * FROM Users WHERE IsDeleted <> 1 AND IsDisabled <> 1 AND InstitutionCode = '{1}' 
                                            AND ((UserRoles LIKE '%,{0},%' OR UserRoles LIKE '{0},%')",
                                            userRoles[0].Id, InstitutionCode);
                    for (int i = 1; i < userRoles.Count; i++)
                    {
                        query += string.Format(" OR (UserRoles LIKE '%,{0},%' OR UserRoles LIKE '{0},%')", userRoles[i].Id);
                    }
                    query += ");";
                    var users = RetrieveUsingDirectQuery<AppUser>(query);
                    return users;
                }
            }
            return null;
        }

    }
}
