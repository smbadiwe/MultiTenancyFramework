using MultiTenancyFramework.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace MultiTenancyFramework.Data
{
    public interface ICoreGeneralDAO : ICoreUnitOfWorkDAO
    {
        IDbConnection GetConnection();

        /// <summary>
        /// Whether or not the DB we're using is MySql
        /// </summary>
        /// <returns></returns>
        bool IsMySql();
        string GetConnectionString();

        /// <summary>
        /// To support multi-tenancy. 
        /// </summary>
        string InstitutionCode { get; set; }

        /// <summary>
        /// Repreasents some variations within an entity that may need to be supported
        /// </summary>
        string EntityName { get; set; }

        void CloseSession();
        
        /// <summary>
        /// Clears the current session.
        /// </summary>
        void ClearCurrentSession();
        
        object CreateDatabaseSession();
        
        void RunDirectQuery(string query, bool clearSession = false);

        Task RunDirectQueryAsync(string query, bool clearSession = false, CancellationToken token = default(CancellationToken));
        
        /// <summary>
        /// Run a query using ADO.NET.
        /// </summary>
        /// <param name="query">The query to run.</param>
        void RunDirectQueryADODotNET(string query, bool closeConnection = false);

        /// <summary>
        /// Run a query using ADO.NET.
        /// </summary>
        /// <param name="query">The query to run.</param>
        /// <param name="token">Cancellation token</param>
        Task RunDirectQueryADODotNETAsync(string query, bool closeConnection = false, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// This one is when you need it as List of U.
        /// <para>Be sure to send in parametized query, else you're on your own!</para>
        /// </summary>
        /// <param name="query">WARNING: Be sure to send in parametized query, else you're on your own!</param>
        /// <param name="clearSession"></param>
        /// <param name="entityName">If null, we use typeof(U). Set this if U has a child class you prefer to get results with, but is not available in this assembly</param>
        /// <returns></returns>
        IList<U> RetrieveUsingDirectQuery<U>(string query, bool clearSession = false, string entityName = null) where U : class; //, IBaseEntity<long>;

        /// <summary>
        /// This one is when you need it as List of U.
        /// <para>Be sure to send in parametized query, else you're on your own!</para>
        /// </summary>
        /// <param name="query">WARNING: Be sure to send in parametized query, else you're on your own!</param>
        /// <param name="clearSession"></param>
        /// <param name="entityName">If null, we use typeof(U). Set this if U has a child class you prefer to get results with, but is not available in this assembly</param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        Task<IList<U>> RetrieveUsingDirectQueryAsync<U>(string query, bool clearSession = false, string entityName = null, CancellationToken token = default(CancellationToken)) where U : class;
        
        /// <summary>
        /// This one is only for when an IList will do.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IList RetrieveUsingDirectQuery2(string query, bool clearSession = false);

        /// <summary>
        /// This one is only for when an IList will do.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        Task<IList> RetrieveUsingDirectQuery2Async(string query, bool clearSession = false, CancellationToken token = default(CancellationToken));
        
        /// <summary>
        /// When more than one entity can be mapped to a table, this will scan and select the correct one, based mostly on the inheritance structure.
        /// </summary>
        void SetEntityName<T>();
    }
    
}
