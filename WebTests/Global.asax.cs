using MultiTenancyFramework;
using MultiTenancyFramework.Mvc;
using MultiTenancyFramework.Mvc.MvcUtils;
using MultiTenancyFramework.NHibernate.NHManager;
using MultiTenancyFramework.SimpleInjector;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using System.Reflection;
using System.Web.Mvc;

namespace WebTests
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var container = new Container();

            // Simple Injector for MVC
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();
            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            container.RegisterMvcIntegratedFilterProvider();
            
            // Simple Injector - other services
            var baseContainer = new BaseContainer(container: container);

            var resolver = new SimpleInjectorDependencyResolver(baseContainer.Container);
            DependencyResolver.SetResolver(resolver);

            // Very Important
            MyServiceLocator.SetIoCContainer(baseContainer.Container);
            MyMvcServiceLocator.SetIoCContainer(resolver);

            // Initialize MVC settings
            AppStartInitializer.Initialize();
            
            NHSessionManager.AddEntityAssemblies(new[] { "MultiTenancyFramework.Mvc" });
            
        }
    }
}
