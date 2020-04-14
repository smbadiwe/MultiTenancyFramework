using System;

namespace MultiTenancyFramework
{
    public class MyServiceLocator
    {
        public static IServiceProvider ServiceProvider { get; set; }

        public static void SetIoCContainer(IServiceProvider container)
        {
            ServiceProvider = container;
        }

        public static TDependency GetInstance<TDependency>()
        {
            TDependency service;
            try
            {
                service = (TDependency)ServiceProvider.GetService(typeof(TDependency));
            }
            catch (Exception)
            {
                throw new ApplicationException("The needed dependency of type " + typeof(TDependency).Name +
                    " could not be located with the ServiceLocator. You'll need to register it with " +
                    "the Common Service Locator (CSL) via your IoC's CSL adapter.");
            }
            return service;
        }

        public static object GetInstance(Type type)
        {
            object service;
            try
            {
                service = ServiceProvider.GetService(type);
            }
            catch (Exception)
            {
                throw new ApplicationException("The needed dependency of type " + type.Name +
                    " could not be located with the ServiceLocator. You'll need to register it with " +
                    "the Common Service Locator (CSL) via your IoC's CSL adapter.");
            }
            return service;
        }
    }

}
