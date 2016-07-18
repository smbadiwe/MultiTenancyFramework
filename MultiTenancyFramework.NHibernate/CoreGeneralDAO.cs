using MultiTenancyFramework.Data;
using MultiTenancyFramework.NHibernate.NHManager;
using MySql.Data.MySqlClient;
using NHibernate;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

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
            get { return _institutionCode; }
            set
            {
                if (value == Utilities.INST_DEFAULT_CODE) value = string.Empty;
                _institutionCode = value;
            }
        }

        public string EntityName { get; set; } = string.Empty;

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
        /// </summary>
        /// <returns></returns>
        public virtual ISession BuildSession()
        {
            return NHSessionManager.GetSession(InstitutionCode);
        }

        public void CloseSession()
        {
            NHSessionManager.CloseStorage(InstitutionCode);
        }

        public void RunDirectQuery(string query, bool clearSession = false)
        {
            var session = BuildSession();
            session.CreateSQLQuery(query);
        }

        public void RunDirectQueryADODotNET(string query, bool closeConnection = false)
        {
            var session = BuildSession();
            var connection = session.Connection;

            if (connection.State != ConnectionState.Open) connection.Open();
            DbCommand command;
            if (connection is MySqlConnection)
            {
                command = new MySqlCommand(query, connection as MySqlConnection);
            }
            else
            {
                command = new SqlCommand(query, connection as SqlConnection);
            }
            using (command)
            {
                command.ExecuteScalar();
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

    }
}
