using MultiTenancyFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiTenancyFramework.NHibernate.Maps
{
    public class PrivilegeMap : PrivilegeMap<Privilege>
    {
    }

    public class PrivilegeMap<T> : EntityMap<T> where T : Privilege
    {
        public PrivilegeMap()
        {
            Map(x => x.Name);
            Map(x => x.Description);
            Map(x => x.Scope);
            Map(x => x.DisplayName);
            Map(x => x.IsDefault);
        }
    }
}
