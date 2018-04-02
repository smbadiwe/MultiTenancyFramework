using MultiTenancyFramework.Data;
using MultiTenancyFramework.Entities;
using MultiTenancyFramework.NHibernate.NHManager;
using NHibernate;
using NHibernate.Transform;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

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

        protected virtual void CommitChanges(ISession session)
        {
            if (session.IsConnected && session.Transaction != null && session.Transaction.IsActive && !session.Transaction.WasCommitted)
            {
                session.Transaction.Commit();
            }
        }

        public virtual void CommitChanges()
        {
            CommitChanges(BuildSession());
        }

        public virtual void RollbackChanges()
        {
            RollbackChanges(BuildSession());
        }

        protected virtual void RollbackChanges(ISession session)
        {
            if (session.Transaction != null && session.IsConnected && session.Transaction.IsActive && !session.Transaction.WasCommitted && !session.Transaction.WasRolledBack)
            {
                session.Transaction.Rollback();
            }
        }

        public virtual async Task CommitChangesAsync(CancellationToken token = default(CancellationToken))
        {
            await CommitChangesAsync(BuildSession(), token);
        }

        public virtual async Task RollbackChangesAsync(CancellationToken token = default(CancellationToken))
        {
            await RollbackChangesAsync(BuildSession(), token);
        }

        protected virtual async Task CommitChangesAsync(ISession session, CancellationToken token = default(CancellationToken))
        {
            if (session.IsConnected && session.Transaction != null && session.Transaction.IsActive && !session.Transaction.WasCommitted)
            {
                await session.Transaction.CommitAsync(token);
            }
        }

        protected virtual async Task RollbackChangesAsync(ISession session, CancellationToken token = default(CancellationToken))
        {
            if (session.Transaction != null && session.IsConnected && session.Transaction.IsActive && !session.Transaction.WasCommitted && !session.Transaction.WasRolledBack)
            {
                await session.Transaction.RollbackAsync();
            }
        }

        public void CloseSession()
        {
            NHSessionManager.CloseStorage(_institutionCode);
        }

        private IQuery GetSqlQueryObject<U>(string query, bool clearSession = false, string entityName = null) where U : class //, IBaseEntity<long>
        {
            var session = BuildSession();

            var sqlQuery = session.CreateSQLQuery(query);
            if (string.IsNullOrWhiteSpace(entityName))
            {
                var typeofU = typeof(U);
                if (typeof(IBaseEntity<long>).IsAssignableFrom(typeofU))
                {
                    sqlQuery.AddEntity(typeofU);
                }
                else
                {
                    sqlQuery.SetResultTransformer(Transformers.AliasToBean(typeofU));
                }
            }
            else
            {
                sqlQuery.AddEntity(entityName);
            }

            return sqlQuery;
        }
        public IList<U> RetrieveUsingDirectQuery<U>(string query, bool clearSession = false, string entityName = null) where U : class //, IBaseEntity<long>
        {
            try
            {
                var sqlQuery = GetSqlQueryObject<U>(query, clearSession, entityName);
                return sqlQuery.List<U>();
            }
            catch (System.Exception ex)
            {
                Utilities.Logger.Log(ex);
                throw;
            }
        }

        public async Task<IList<U>> RetrieveUsingDirectQueryAsync<U>(string query, bool clearSession = false, string entityName = null, CancellationToken token = default(CancellationToken)) where U : class //, IBaseEntity<long>
        {
            try
            {
                var sqlQuery = GetSqlQueryObject<U>(query, clearSession, entityName);
                return await sqlQuery.ListAsync<U>(token);
            }
            catch (System.Exception ex)
            {
                Utilities.Logger.Log(ex);
                throw;
            }
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
        /// This one is only for when an IList will do.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IList> RetrieveUsingDirectQuery2Async(string query, bool clearSession = false, CancellationToken token = default(CancellationToken))
        {
            var session = BuildSession();
            return await session.CreateSQLQuery(query).ListAsync(token);
        }

        public async Task RunDirectQueryAsync(string query, bool clearSession = false, CancellationToken token = default(CancellationToken))
        {
            var session = BuildSession();
            await session.CreateSQLQuery(query).ExecuteUpdateAsync(token);
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

        /// <summary>
        /// Run a query using ADO.NET. Default implementation supports SQL Server and MySql.
        /// This uses command.ExecuteNonQuery();, so is fit for non-select queries
        /// </summary>
        /// <param name="query">The query to run.</param>
        public virtual async Task RunDirectQueryADODotNETAsync(string query, bool closeConnection = false, CancellationToken token = default(CancellationToken))
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
                int rows = await command.ExecuteNonQueryAsync(token);
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
