using MultiTenancyFramework.Entities;
using MultiTenancyFramework.NHibernate.Maps;

namespace MultiTenancyFramework.Mvc.NHibernate.Maps
{
    public class ActionAccessPrivilegeMap : PrivilegeMap<ActionAccessPrivilege>
    {
        public ActionAccessPrivilegeMap()
        {
            Map(x => x.Action);
            Map(x => x.Controller);
            Map(x => x.Area);
        }
    }
}
