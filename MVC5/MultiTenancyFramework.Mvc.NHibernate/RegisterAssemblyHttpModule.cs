using MultiTenancyFramework.NHibernate.NHManager;
using System.Web;

namespace MultiTenancyFramework.Mvc.NHibernate
{
    /// <summary>
    /// This module simply adds 'MultiTenancyFramework.Mvc' to the list of entity assemblies
    /// and "MultiTenancyFramework.Mvc.NHibernate" to the list of mapping assemblies
    /// </summary>
    public class RegisterAssemblyHttpModule : IHttpModule
    {
        public void Dispose()
        {

        }

        public void Init(HttpApplication context)
        {
            NHSessionManager.AddEntityAssemblies(new[] { "MultiTenancyFramework.Mvc" });
            NHSessionManager.AddMappingAssemblies(new[] { "MultiTenancyFramework.Mvc.NHibernate" });
        }

    }
}
