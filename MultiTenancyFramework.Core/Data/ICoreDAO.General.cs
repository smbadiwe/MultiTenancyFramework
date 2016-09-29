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
        /// Run a query using ADO.NET
        /// </summary>
        /// <param name="query">The query to run.</param>
        void RunDirectQueryADODotNET(string query, bool closeConnection = false);

        /// <summary>
        /// When more than one entity can be mapped to a table, this will scan and select the correct one, based mostly on the inheritance structure.
        /// </summary>
        void SetEntityName<T>();
    }
    
}
