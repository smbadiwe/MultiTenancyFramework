using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MultiTenancyFramework.Unity
{
    public class BaseContainer
    {
        public UnityContainer Container { get; private set; }

        /// <summary>
        /// When in doubt, just use as it is, without setting anything yourself
        /// </summary>
        /// <param name="assembliesToScan">The assemblies containing Interfaces/Implementations</param>
        /// <param name="container">The Unity container</param>
        public BaseContainer(string[] assembliesToScan = null, UnityContainer container = null)
        {
            HashSet<Assembly> assemblies;
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            if (assembliesToScan == null || assembliesToScan.Length == 0)
            {
                assemblies = new HashSet<Assembly>(loadedAssemblies);
            }
            else
            {
                HashSet<string> assemblyNamesSet = new HashSet<string>(assembliesToScan);
                assemblies = new HashSet<Assembly>();
                foreach (var assemblyName in assemblyNamesSet)
                {
                    var assembly = loadedAssemblies.FirstOrDefault(x => x.GetName().Name == assemblyName);
                    try
                    {
                        if (assembly == null) assembly = Assembly.Load(assemblyName);
                        assemblies.Add(assembly);
                    }
                    catch { }
                }

                // Try to include all the MultiTenancyFramework.* assemblies
                foreach (var assembly in loadedAssemblies.Where(x => x.GetName().Name.StartsWith("MultiTenancyFramework")))
                {
                    assemblies.Add(assembly);
                }
            }

            Container = container ?? new UnityContainer();

            // Convention-based mapping: IService/Service; and Open generics too
            Container.RegisterTypes(
                AllClasses.FromAssemblies(assemblies),
                WithMappings.FromMatchingInterface,
                WithName.Default);
            
            // This won't work because Unity does not support IServiceProvider
            //Container.RegisterInstance(typeof(IServiceProvider), Container);
        }
    }
}
