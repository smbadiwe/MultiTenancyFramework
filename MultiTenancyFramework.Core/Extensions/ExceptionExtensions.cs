using System;
using System.Text;

namespace MultiTenancyFramework
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Gets the full exception message.
        /// </summary>
        /// <param name="ex">The exception thrown.</param>
        /// <param name="includeStackTrace"></param>
        /// <returns></returns>
        public static string GetFullExceptionMessage(this Exception ex, bool includeStackTrace = false)
        {
            StringBuilder sbMessages = new StringBuilder();
            StringBuilder sbStackTraces = new StringBuilder();
            if (includeStackTrace && !string.IsNullOrWhiteSpace(ex.StackTrace))
            {
                sbStackTraces.AppendFormat("\nEXCEPTION TYPE: {0}; STACK TRACE: {1}", ex.GetType().FullName, ex.StackTrace);
            }
            sbMessages.Append(ex.Message);
            var aggEx = ex as AggregateException;
            if (aggEx != null)
            {
                for (int i = 0; i < aggEx.InnerExceptions.Count; i++)
                {
                    sbMessages.AppendFormat(" and Aggregate Exception {0}: {1}\n", (i + 1), aggEx.InnerExceptions[i].Message);
                }
            }
            var sqlClientEx = ex as System.Data.Common.DbException;
            if (sqlClientEx != null)
            {
                if (sqlClientEx is System.Data.SqlClient.SqlException)
                {
                    var sqlClientExErrors = (sqlClientEx as System.Data.SqlClient.SqlException).Errors;
                    if (sqlClientExErrors != null && sqlClientExErrors.Count > 0)
                    {
                        sbMessages.AppendLine(" \nSQL CLIENT ERRORS: ");
                        for (int i = 0; i < sqlClientExErrors.Count; i++)
                        {
                            sbMessages.AppendFormat("\t{0}: Message - {1};\n", (i + 1), sqlClientExErrors[i]);
                        }
                    }
                }

                var sqlClientExData = sqlClientEx.Data;
                if (sqlClientExData != null && sqlClientExData.Count > 0)
                {
                    try
                    {
                        sbMessages.AppendFormat("\n SQL ERROR DATA DICT ITEM: \n\t = {0};\n", sqlClientExData["actual-sql-query"]);
                    }
                    catch { }
                }
            }
            var inner = ex.InnerException;
            if (inner != null)
            {
                var loaderEx = inner as System.Reflection.ReflectionTypeLoadException;
                if (loaderEx != null)
                {
                    var loaderInnerExes = loaderEx.LoaderExceptions;
                    for (int i = 0; i < loaderInnerExes.Length; i++)
                    {
                        sbMessages.AppendFormat(" and Loader Exception {0}: {1}\n", (i + 1), loaderInnerExes[i].Message);
                    }
                }
                while (inner != null)
                {
                    sbMessages.AppendFormat(" because {0} ", inner.GetFullExceptionMessage(includeStackTrace));
                    inner = inner.InnerException;
                }
            }
            return string.Format("{0}\n{1}", sbMessages, sbStackTraces);
        }
    }
}
