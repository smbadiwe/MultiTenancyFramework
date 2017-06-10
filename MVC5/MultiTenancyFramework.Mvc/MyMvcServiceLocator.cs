using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MultiTenancyFramework.Mvc
{
    public static class MyMvcServiceLocator
    {
        public static IDependencyResolver ServiceProvider { get; set; }

        public static void SetIoCContainer(IDependencyResolver container)
        {
            ServiceProvider = container;
        }

        public static TDependency GetInstance<TDependency>()
        {
            try
            {
                return (TDependency)ServiceProvider.GetService(typeof(TDependency));
            }
            catch (Exception)
            {
                throw new ApplicationException("The needed dependency of type " + typeof(TDependency).Name +
                    " could not be located with the ServiceLocator. You'll need to register it.");
            }
        }

        public static object GetInstance(Type type)
        {
            try
            {
                return ServiceProvider.GetService(type);
            }
            catch (Exception)
            {
                throw new ApplicationException("The needed dependency of type " + type.Name +
                    " could not be located with the ServiceLocator. You'll need to register it.");
            }
        }

        public static IEnumerable<TDependency> GetInstances<TDependency>()
        {
            try
            {
                return ServiceProvider.GetServices(typeof(TDependency)).Cast<TDependency>();
            }
            catch (Exception)
            {
                throw new ApplicationException("The needed dependency of type " + typeof(TDependency).Name +
                    " could not be located with the ServiceLocator. You'll need to register it");
            }
        }

        public static IEnumerable<object> GetInstances(Type type)
        {
            try
            {
                return ServiceProvider.GetServices(type);
            }
            catch (Exception)
            {
                throw new ApplicationException("The needed dependency of type " + type.Name +
                    " could not be located with the ServiceLocator. You'll need to register it");
            }
        }
    }
}
