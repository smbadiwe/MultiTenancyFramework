using MultiTenancyFramework.Data;
using MultiTenancyFramework.Entities;
using MultiTenancyFramework.NHibernate.NHManager;
using NHibernate;
using NHibernate.Transform;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace MultiTenancyFramework.NHibernate
{
    public abstract class CoreGeneralDAO : ICoreGeneralDAO
    {
        private string _institutionCode;
        /// <summary>
        /// To support multi-tenancy. 
        /// </summary>
        public string InstitutionCode
        {
            get
            {
                return _institutionCode;
            }
            set
            {
                if (value == null || value.Equals(Utilities.INST_DEFAULT_CODE, System.StringComparison.OrdinalIgnoreCase)) value = string.Empty;
                _institutionCode = value;
            }
        }

        public string EntityName { get; set; }

        public CoreGeneralDAO()
        {
            _institutionCode = string.Empty;
            EntityName = string.Empty;
        }

        /// <summary>
        /// Just call 'BuildSession' directly or cast this to 'ISession'.
        /// </summary>
        /// <returns></returns>
        public object CreateDatabaseSession()
        {
            return BuildSession();
        }

        public void ClearCurrentSession()
        {
            var session = BuildSession();
            session.Clear();
        }

        /// <summary>
        /// WARNING:
        /// <para>DO NOT use this session directly for anything other than retrieves (like constructing QueryOver or CreateCriteria). 
        /// For every other case, use what is provided in the base class</para>
        /// <para>NB: The returned session only Flushes when you commit. You can always change to .FlushMode to your taste.</para>
        /// </summary>
        /// <returns></returns>
        public virtual ISession BuildSession()
        {
            return NHSessionManager.GetSession(_institutionCode);
        }

        public void CloseSession()
        {
            NHSessionManager.CloseStorage(_institutionCode);
        }

        public IList<U> RetrieveUsingDirectQuery<U>(string query, bool clearSession = false, string entityName = null) where U : class //, IBaseEntity<long>
        {
            var session = BuildSession();

            if (string.IsNullOrWhiteSpace(entityName))
            {
                var typeofU = typeof(U);
                if (typeof(IBaseEntity<long>).IsAssignableFrom(typeofU))
                    return session.CreateSQLQuery(query).AddEntity(typeofU).List<U>();

                return session.CreateSQLQuery(query).SetResultTransformer(Transformers.AliasToBean(typeofU)).List<U>();
            }
            return session.CreateSQLQuery(query).AddEntity(entityName).List<U>();
        }

        /// <summary>
        /// This one is only for when an IList will do.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IList RetrieveUsingDirectQuery2(string query, bool clearSession = false)
        {
            var session = BuildSession();
            return session.CreateSQLQuery(query).List();
        }

        public void RunDirectQuery(string query, bool clearSession = false)
        {
            var session = BuildSession();
            session.CreateSQLQuery(query).ExecuteUpdate();
        }

        /// <summary>
        /// Run a query using ADO.NET. Default implementation supports SQL Server and MySql.
        /// This uses command.ExecuteNonQuery();, so is fit for non-select queries
        /// </summary>
        /// <param name="query">The query to run.</param>
        public virtual void RunDirectQueryADODotNET(string query, bool closeConnection = false)
        {
            var session = BuildSession();
            var connection = session.Connection;

            if (connection.State == ConnectionState.Open)
            {
                // For some reason I can't tell, if you use the already open conection, the query does not run.
                // So I close it and re-open it.
                connection.Close();
            }
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandTimeout = 120;
                command.CommandText = query;
                int rows = command.ExecuteNonQuery();
                Utilities.Logger.Log($"Query Ran:\n{query}\nRows Affected: {rows}. Database: {connection.Database}");
            }
            if (closeConnection)
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public IDbConnection GetConnection()
        {
            var session = BuildSession();
            return session.Connection;
        }

        public string GetConnectionString()
        {
            return NHSessionManager.GetConnectionString();
        }

        /// <summary>
        /// Whether or not the DB we're using is MySql
        /// </summary>
        /// <returns></returns>
        public bool IsMySql()
        {
            return NHSessionManager.IsMySqlDatabase();
        }

        /// <summary>
        /// When more than one entity can be mapped to a table, this will scan and select the correct one, based mostly on the inheritance structure.
        /// </summary>
        public void SetEntityName<T>()
        {
            EntityName = NHSessionManager.GetEntityNameToUseInNHSession(typeof(T));
        }
    }
}
