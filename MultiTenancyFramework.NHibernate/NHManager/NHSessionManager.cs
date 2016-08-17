using FluentNHibernate;
using FluentNHibernate.Automapping;
using MultiTenancyFramework.Data;
using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using MultiTenancyFramework.NHibernate.NHManager.Conventions;
using MultiTenancyFramework.NHibernate.NHManager.Listeners;
using MultiTenancyFramework.NHibernate.Queries;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.ConfigurationSchema;
using NHibernate.Event;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using Environment = NHibernate.Cfg.Environment;

namespace MultiTenancyFramework.NHibernate.NHManager
{
    public class NHSessionManager
    {
        private static readonly ConcurrentDictionary<string, Type[]> AssemblyClasses = new ConcurrentDictionary<string, Type[]>();

        /// <summary>
        /// Key is institution code
        /// </summary>
        public static readonly ConcurrentDictionary<string, ISessionFactory> SessionFactories = new ConcurrentDictionary<string, ISessionFactory>();

        /// <summary>
        /// Key is built from GetSessionKey method
        /// </summary>
        public static readonly ConcurrentDictionary<string, ISessionStorage> SessionStorages = new ConcurrentDictionary<string, ISessionStorage>();

        /// <summary>
        /// Key is built from GetSessionKey method
        /// </summary>
        public static readonly ConcurrentDictionary<string, IStatelessSession> StatelessSessionStorages = new ConcurrentDictionary<string, IStatelessSession>();

        public static Action<ISessionFactory> SessionFactoryCreated;

        /// <summary>
        /// At some point, we'll scan for the assemblies where entities are defined. So, supply these assemblies.
        /// (Just the assembly names).
        /// <para>Alternatively, you can provide these assembly names in config file as a comma-separated list:
        /// AppSettings key should be "EntityAssemblies"
        /// </para> 
        /// </summary>
        public static Func<string[]> AddEntityAssemblies;

        /// <summary>
        /// Use thi method to set DAO.EntityName when it's possible there is a subtype of the baseType 
        /// in the other entity assemblies
        /// </summary>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static string GetEntityNameToUseInNHSession(Type baseType)
        {
            foreach (var assemblyClassSet in AssemblyClasses)
            {
                foreach (var theType in assemblyClassSet.Value)
                {
                    if (baseType.IsAssignableFrom(theType) && !theType.Equals(baseType))
                    {
                        return theType.FullName;
                    }
                }
            }
            var entityAssemblies = AddEntityAssemblies?.Invoke() ?? new string[0];
            var allEntityAssemblies = new HashSet<string>(Utilities.EntityAssemblies.Union(entityAssemblies));
            allEntityAssemblies.Add("MultiTenancyFramework.Core"); //The only ony we're sure of.
            foreach (var assemblyName in allEntityAssemblies.Where(x => x.IsNotContainedIn(AssemblyClasses.Keys)))
            {
                try
                {
                    var assembly = Assembly.Load(assemblyName);
                    if (assembly != null)
                    {
                        var types = assembly.GetTypes().Where(x => x.IsClass).ToArray();
                        AssemblyClasses.TryAdd(assemblyName, types);
                        foreach (var theType in types)
                        {
                            if (baseType.IsAssignableFrom(theType) && !theType.Equals(baseType))
                            {
                                return theType.FullName;
                            }
                        }
                    }
                }
                catch
                { //Do Nothing
                }
            }
            return baseType.FullName;
        }

        internal static void CloseStorage(string institutionCode = null, bool isWebSession = true)
        {
            CloseStorage(GetSessionKey(institutionCode, isWebSession));
        }

        private static void CloseStorage(string dataSourceKey)
        {
            if (string.IsNullOrWhiteSpace(dataSourceKey)) return;

            ISessionStorage storage = null;

            if (SessionStorages.TryRemove(dataSourceKey, out storage))
            {
                if (storage != null) //
                {
                    //Get the current session from the storage object
                    ISession session = storage.Session;
                    if (session != null)
                    {
                        if (session.IsOpen && session.Transaction != null && session.IsConnected && session.Transaction.IsActive
                            && !session.Transaction.WasCommitted && !session.Transaction.WasRolledBack)
                        {
                            session.Transaction.Rollback();
                            session.Close();
                        }

                        session.Dispose();
                        storage = null;
                    }
                }
            }
        }

        public static bool IsWeb()
        {
            return HttpContext.Current != null;
        }

        public static string GetSessionKey(string institutionCode = null, bool isWebSession = true)
        {
            if (string.IsNullOrWhiteSpace(institutionCode)) institutionCode = Utilities.INST_DEFAULT_CODE;
            return institutionCode + "_" + (isWebSession ? "true" : "false");
        }

        /// <summary>
        /// NB: If your tables were created on the fly, then you'll have to pass in the entity name to get the right table.
        /// And, If you must pass in 'autoPersistenceModel', you should only pass in a dynamically created mapping file.
        /// </summary>
        /// <param name="institutionCode"></param>
        /// <param name="entityName"></param>
        /// <param name="dbConnection"></param>
        /// <param name="isWebSession"></param>
        /// <param name="doFreshSession"></param>
        /// <returns></returns>
        public static ISession GetSession(string institutionCode = "", IDbConnection dbConnection = null, bool doFreshSession = false)
        {
            bool isWebSession = IsWeb();
            string sessionKey = GetSessionKey(institutionCode, isWebSession);
            ISessionStorage storage = null;

            if (!doFreshSession)
            {
                SessionStorages.TryGetValue(sessionKey, out storage);
            }
            if (storage == null)
            {
                if (isWebSession)
                {
                    storage = new WebSessionStorage { InstitutionCode = institutionCode };
                }
                else
                {
                    storage = new NonWebSessionStorage();
                }
            }

            //Get the current session from the storage object
            ISession session = doFreshSession ? null : storage.Session;

            //Start a new session if no current session exists
            if (session == null || !session.IsOpen)
            {
                ISessionFactory fac;
                string instCode = sessionKey.Split('_')[0]; // will be 'Utilities.INST_DEFAULT_CODE' if 'institutionCode' is null or empty
                if (SessionFactories.TryGetValue(instCode, out fac) && fac != null)
                {
                    //Do what now? Nothing.
                }
                else
                {
                    fac = BuildFactory(instCode, sessionKey);
                }

                //Apply the interceptor if any was registered and open the session
                if (dbConnection == null)
                {
                    session = fac.OpenSession();
                }
                else
                {
                    session = fac.OpenSession(dbConnection);
                }
            }

            if (session != null)
            {
                session.EnableFilter(Utilities.InstitutionFilterName)
                    .SetParameter(Utilities.InstitutionCodeQueryParamName, institutionCode);

                //Begin a transaction
                if (!session.IsConnected || session.Transaction == null || !session.Transaction.IsActive || session.Transaction.WasCommitted || session.Transaction.WasRolledBack) //if (storage is WebSessionStorage)
                {
                    session.BeginTransaction();
                }
            }
            if (!doFreshSession)
            {
                //Update the storage with the new session
                storage.Session = session;
                SessionStorages[sessionKey] = storage;
            }
            if (isWebSession)
            {
                CurrentSessions[sessionKey] = (WebSessionStorage)storage;
            }

            return session;
        }

        /// <summary>
        /// Sometimes, a logged in tenant user moves between her space and the landlord's. So, this set will usually be not more than two entries
        /// </summary>
        internal static Dictionary<string, WebSessionStorage> CurrentSessions
        {
            get
            {
                if (!HttpContext.Current.Items.Contains(WebSessionStorage.CurrentSessionKey))
                {
                    HttpContext.Current.Items[WebSessionStorage.CurrentSessionKey] = new Dictionary<string, WebSessionStorage>();
                }
                return HttpContext.Current.Items[WebSessionStorage.CurrentSessionKey] as Dictionary<string, WebSessionStorage>;
            }
            set
            {
                HttpContext.Current.Items[WebSessionStorage.CurrentSessionKey] = value;
            }
        }

        /// <summary>
        /// Whether or not the DB we're using is MySql
        /// </summary>
        /// <returns></returns>
        internal static bool IsMySqlDatabase()
        {
            HibernateConfiguration hc = System.Configuration.ConfigurationManager.GetSection(CfgXmlHelper.CfgSectionName) as HibernateConfiguration;
            if (hc != null && hc.SessionFactory != null)
            {
                var driver = hc.SessionFactory.Properties[Environment.ConnectionDriver];
                return (!string.IsNullOrWhiteSpace(driver) && driver.EndsWith("MySqlDataDriver"));
            }
            return false;
        }

        internal static string GetConnectionString()
        {
            HibernateConfiguration hc = System.Configuration.ConfigurationManager.GetSection(CfgXmlHelper.CfgSectionName) as HibernateConfiguration;
            if (hc != null && hc.SessionFactory != null)
            {
                return hc.SessionFactory.Properties[Environment.ConnectionString];
            }
            return null;
        }

        internal static ISessionFactory Init(string cfgFile, string sessionKey)
        {
            return Init(cfgFile, sessionKey, null);
        }

        private static ISessionFactory Init(string cfgFile, string sessionKey, IDictionary<string, string> cfgProperties)
        {
            Configuration cfg = ConfigureNHibernate(cfgFile, cfgProperties);

            var autoPersistenceModel = new AutoPersistenceModel();
            autoPersistenceModel.Conventions.Add<ClassMappingConvention>();
            autoPersistenceModel.Conventions.Add<ReferencesConvention>();
            autoPersistenceModel.Conventions.Add<EnumMappingConvention>();

            string instCode = sessionKey.Split('_')[0];
            AddMappingAssembliesTo(instCode, cfg, autoPersistenceModel);

            // Add the Listeners
            var auditLogEvent = new AuditLogEventListener();
            cfg.SetListener(ListenerType.Merge, auditLogEvent);
            cfg.SetListener(ListenerType.Flush, auditLogEvent);
            cfg.SetListener(ListenerType.PreDelete, auditLogEvent);

            try
            {
                var factory = cfg.BuildSessionFactory();

                try
                {
                    SessionFactoryCreated?.Invoke(factory);
                }
                catch { }

                SessionFactories[instCode] = factory;

                return factory;
            }
            catch (HibernateException ex)
            {
                throw new HibernateException("Cannot create session factory; " + ex.GetFullExceptionMessage());
            }
            finally //Trivial I guess, but GC may not happen as soon as I want it
            {
                cfg = null;
                autoPersistenceModel = null;
                auditLogEvent = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instCode">The modified Institution Code; will be 'Utilities.INST_DEFAULT_CODE' if not tenant</param>
        /// <param name="sessionKey"></param>
        private static ISessionFactory BuildFactory(string instCode, string sessionKey)
        {
            if (instCode != Utilities.INST_DEFAULT_CODE)
            {
                var institution = new GetInstitutionByCodeQueryHandler().Handle(new GetInstitutionByCodeQuery { Code = instCode });
                if (institution == null) throw new GeneralException($"No institution with Code - {instCode}", ExceptionType.UnidentifiedInstitutionCode);

                DatabaseConnection dbConn = null;
                if (institution.DatabaseConnectionId > 0)
                {
                    dbConn = new CoreDAO<DatabaseConnection>().Retrieve(institution.DatabaseConnectionId);
                }
                if (dbConn == null)
                {
                    throw new GeneralException($"Database has not been setup for Institution: '{institution.Name}'");
                }
                var cfgProps = new Dictionary<string, string>();
                //if (!IsMySqlDatabase())
                //{
                //    cfgProps.Add(Environment.DefaultSchema, institution.PreferredSchema ?? "dbo");
                //}
                cfgProps.Add(Environment.ConnectionString, dbConn.ConnectionString);

                return Init(null, sessionKey, cfgProps);
            }
            else
            {
                return Init(null, sessionKey);
            }
        }

        private static void AddMappingAssembliesTo(string instCode, Configuration cfg, AutoPersistenceModel autoPersistenceModel)
        {
            var hc = System.Configuration.ConfigurationManager.GetSection(CfgXmlHelper.CfgSectionName) as HibernateConfiguration;
            if (hc == null)
            {
                throw new HibernateConfigException("Cannot process NHibernate Section in config file.");
            }
            if (hc.SessionFactory != null)
            {
                var mappingFiles = new HashSet<string>(hc.SessionFactory.Mappings.Select(x => x.Assembly));

                //Doing this ensure framework libraries are captured without having duplicates
                var otherMappingFiles = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.GetName().Name.EndsWith(".NHibernate"));
                if (otherMappingFiles.Any())
                {
                    foreach (var file in otherMappingFiles)
                    {
                        mappingFiles.Add(file.GetName().Name);
                    }
                }
                otherMappingFiles = null;

                foreach (var mappingAssemblyCfg in mappingFiles)
                {
                    var assembly = Assembly.Load(mappingAssemblyCfg);

                    // Looks for any HBMs
                    cfg.AddAssembly(assembly);

                    // Looks for Fluents
                    if (instCode == Utilities.INST_DEFAULT_CODE)
                    {
                        autoPersistenceModel.AddMappingsFromAssembly(assembly);
                    }
                    else // => Tenant, so do not map those entities marked as Hosted Centrally
                    {
                        autoPersistenceModel = autoPersistenceModel.AddMappingsFromAssembly(assembly)
                            .Where(x => !typeof(IAmHostedCentrally).IsAssignableFrom(x));
                    }
                }

                cfg.BeforeBindMapping += (sender, args) => args.Mapping.autoimport = false;
            }

            cfg.AddAutoMappings(autoPersistenceModel);
            hc = null;
        }

        /// <summary>
        /// Gets the Nhibernate configuration from the file and applies the properties specified to it
        /// </summary>
        /// <param name="cfgFile">The Nhibernate Configuration filepath</param>
        /// <param name="cfgProperties">Configuration properties to apply</param>
        /// <returns>Nhibernate Configuration Object</returns>
        private static Configuration ConfigureNHibernate(string cfgFile, IDictionary<string, string> cfgProperties)
        {
            Configuration cfg;
            if (string.IsNullOrWhiteSpace(cfgFile))
            {
                cfg = new Configuration().Configure();
            }
            else
            {
                cfg = new Configuration().Configure(cfgFile);
            }

            bool canUseSchema = !IsMySqlDatabase();
            if (cfgProperties != null && cfgProperties.Count > 0)
            {
                if (!canUseSchema && cfgProperties.ContainsKey(Environment.DefaultSchema))
                {
                    cfgProperties.Remove(Environment.DefaultSchema);
                }

                foreach (var item in cfgProperties)
                {
                    cfg.Properties[item.Key] = item.Value;
                }
            }
            if (!canUseSchema && cfg.Properties.ContainsKey(Environment.DefaultSchema))
            {
                cfg.Properties.Remove(Environment.DefaultSchema);
            }
#if DEBUG
            cfg.Properties.Add(Environment.UseSqlComments, "true");
            cfg.Properties.Add(Environment.ShowSql, "true");
            cfg.Properties.Add(Environment.FormatSql, "true");
            cfg.Properties.Add(Environment.GenerateStatistics, "true");
#endif
            cfg.Properties.Add(Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider");
            cfg.Properties.Add(Environment.Isolation, "ReadCommitted");
            cfg.Properties.Add(Environment.ProxyFactoryFactoryClass, "NHibernate.Bytecode.DefaultProxyFactoryFactory, NHibernate");
            cfg.Properties.Add(Environment.CurrentSessionContextClass, "web");
            return cfg;
        }

    }
}
