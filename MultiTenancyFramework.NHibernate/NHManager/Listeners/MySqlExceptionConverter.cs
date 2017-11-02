//using MySql.Data.MySqlClient;
//using NHibernate;
using NHibernate.Exceptions;
using System;

namespace MultiTenancyFramework.NHibernate.NHManager.Listeners
{
    public class MySqlExceptionConverter : ISQLExceptionConverter
    {
        public Exception Convert(AdoExceptionContextInfo exInfo)
        {
            Utilities.Logger.Log("Inside our SqlExceptionConverter: EntityId: {0}. EntityName: {1}\nMessage: {2}.", exInfo.EntityId, exInfo.EntityName, exInfo.Message);
            //var sqle = ADOExceptionHelper.ExtractDbException(exInfo.SqlException) as MySqlException;
            //if (sqle != null)
            //{
            //    switch (sqle.Number)
            //    {
            //        case 547:
            //            return new ConstraintViolationException(exInfo.Message,
            //                sqle.InnerException, exInfo.Sql, null);
            //        case 208:
            //            return new SQLGrammarException(exInfo.Message,
            //                sqle.InnerException, exInfo.Sql);
            //        case 3960:
            //            return new StaleObjectStateException(exInfo.EntityName, exInfo.EntityId);
            //    }
            //}
            return SQLStateConverter.HandledNonSpecificException(exInfo.SqlException,
                exInfo.Message, exInfo.Sql);
        }
    }
}
