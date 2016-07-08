﻿using MultiTenancyFramework.Commands;
using MultiTenancyFramework.Data;
using MultiTenancyFramework.Data.Queries;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MultiTenancyFramework.SimpleInjector
{
    public class BaseContainer
    {
        public Container Container { get; private set; }

        /// <summary>
        /// When in doubt, just use as it is, without setting anything yourself
        /// </summary>
        /// <param name="assembliesToScan">The assemblies containing Interfaces/Implementations</param>
        /// <param name="container">The Simple Injector container</param>
        public BaseContainer(string[] assembliesToScan = null, Container container = null)
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
            Container = container ?? new Container();
            Container.Register(typeof(ICommandHandler<>), assemblies);
            Container.Register(typeof(IDbQueryHandler<,>), assemblies);

            var exportedTypes = assemblies.SelectMany(x => x.ExportedTypes);

            #region Known Cases. This part is made necessary due to a limitation of Simple Injector
            var types = exportedTypes.Where(x => x.IsGenericType && x.Name.StartsWith("CoreDAO")).ToArray();
            Container.Register(typeof(ICoreDAO<,>), types.First(x => x.Name.EndsWith("2")));
            Container.Register(typeof(ICoreDAO<>), types.First(x => x.Name.EndsWith("1")));
            
            types = exportedTypes.Where(x => x.IsGenericType && x.Name.StartsWith("PrivilegeDAO")).ToArray();
            Container.Register(typeof(IPrivilegeDAO<>), types.First(x => x.Name.EndsWith("1"))); 

            types = exportedTypes.Where(x => x.IsGenericType && x.Name.StartsWith("AppUserDAO")).ToArray();
            Container.Register(typeof(IAppUserDAO<>), types.First(x => x.Name.EndsWith("1"))); 

            types = exportedTypes.Where(x => x.IsGenericType && x.Name.StartsWith("InstitutionDAO")).ToArray();
            Container.Register(typeof(IInstitutionDAO<>), types.First(x => x.Name.EndsWith("1"))); 
            #endregion

            // This is for convention-based registrations. Convention is Service/IService
            var registrations =
                from type in exportedTypes
                where !type.IsAbstract
                where !type.IsGenericType
                where type.GetInterfaces().Any(x => x.Name.EndsWith(type.Name))
                select new { Service = type.GetInterfaces().Single(x => x.Name.EndsWith(type.Name)), Implementation = type }
                ;

            registrations = registrations.ToArray();
            foreach (var reg in registrations)
            {
                Container.Register(reg.Service, reg.Implementation);
            }

            // Finally...
            Container.Register(typeof(IServiceProvider), () => Container);
        }
    }
}