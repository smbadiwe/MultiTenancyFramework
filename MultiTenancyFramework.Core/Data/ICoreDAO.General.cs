using MultiTenancyFramework.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace MultiTenancyFramework.Data
{
    public interface ICoreGeneralDAO
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

        /// <summary>
        /// Run a query using ADO.NET.
        /// </summary>
        /// <param name="query">The query to run.</param>
        void RunDirectQueryADODotNET(string query, bool closeConnection = false);

        /// <summary>
        /// This one is when you need it as List of T.
        /// <para>Be sure to send in parametized query, else you're on your own!</para>
        /// </summary>
        /// <param name="query">WARNING: Be sure to send in parametized query, else you're on your own!</param>
        /// <returns></returns>
        IList<U> RetrieveUsingDirectQuery<U>(string query, bool clearSession = false) where U : class, IBaseEntity<long>;

        /// <summary>
        /// This one is only for when an IList will do.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IList RetrieveUsingDirectQuery2(string query, bool clearSession = false);

        /// <summary>
        /// When more than one entity can be mapped to a table, this will scan and select the correct one, based mostly on the inheritance structure.
        /// </summary>
        void SetEntityName<T>();
    }
    
}
