using MultiTenancyFramework.Caching;
using MultiTenancyFramework.Commands;
using MultiTenancyFramework.Data;
using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.IoC;
using SimpleInjector;
using System;
using System.Linq;

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
            if (container == null)
            {
                container = new Container();
            }

            var assemblies = IoCUtility.GetAssembliesForRegistration("MultiTenancyFramework.SimpleInjector", assembliesToScan);

            container.Register(typeof(ICommandHandler<>), assemblies);
            container.Register(typeof(IDbQueryHandler<,>), assemblies);

            var exportedTypes = assemblies.SelectMany(x => x.ExportedTypes).Where(x => !x.IsAbstract);

            #region Known Cases. This part is made necessary due to a limitation of Simple Injector
            var types = exportedTypes.Where(x => x.IsGenericType && x.Name.StartsWith("CoreDAO")).ToArray();
            if (types.Any(x => x.Name.EndsWith("2")))
                container.Register(typeof(ICoreDAO<,>), types.First(x => x.Name.EndsWith("2")));
            if (types.Any(x => x.Name.EndsWith("1")))
                container.Register(typeof(ICoreDAO<>), types.First(x => x.Name.EndsWith("1")));

            types = exportedTypes.Where(x => x.IsGenericType && x.Name.StartsWith("PrivilegeDAO")).ToArray();
            if (types.Any(x => x.Name.EndsWith("1")))
                container.Register(typeof(IPrivilegeDAO<>), types.First(x => x.Name.EndsWith("1")));

            types = exportedTypes.Where(x => x.IsGenericType && x.Name.StartsWith("AppUserDAO")).ToArray();
            if (types.Any(x => x.Name.EndsWith("1")))
                container.Register(typeof(IAppUserDAO<>), types.First(x => x.Name.EndsWith("1")));

            types = exportedTypes.Where(x => x.IsGenericType && x.Name.StartsWith("InstitutionDAO")).ToArray();
            if (types.Any(x => x.Name.EndsWith("1")))
                container.Register(typeof(IInstitutionDAO<>), types.First(x => x.Name.EndsWith("1")));
            #endregion

            // This is for convention-based registrations. Convention is IService/Service
            var registrations =
                from type in exportedTypes
                //where !type.IsAbstract
                where !type.IsGenericType
                where type.GetInterfaces().Any(x => x.Name.EndsWith(type.Name))
                select new { Service = type.GetInterfaces().Single(x => x.Name.EndsWith(type.Name)), Implementation = type }
                ;

            registrations = registrations.ToArray();
            foreach (var reg in registrations)
            {
                container.Register(reg.Service, reg.Implementation);
            }

            // Cache Manager
            container.Register<ICacheManager, MemoryCacheManager>(Lifestyle.Singleton);

            // Finally...
            container.Register(typeof(IServiceProvider), () => container);

            Container = container;
        }
    }
}
