using MultiTenancyFramework;
using MultiTenancyFramework.Mvc.MvcUtils;
using MultiTenancyFramework.NHibernate.NHManager;
using MultiTenancyFramework.SimpleInjector;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

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

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(baseContainer.Container));

            // Very Important
            MyServiceLocator.SetIoCContainer(baseContainer.Container);

            // Initialize MVC settings
            AppStartInitializer.Initialize();
            
            NHSessionManager.AddEntityAssemblies += () => new[] { "MultiTenancyFramework.Mvc", "Framework", "SchoolSoul.Core" };
            
        }
    }
}
