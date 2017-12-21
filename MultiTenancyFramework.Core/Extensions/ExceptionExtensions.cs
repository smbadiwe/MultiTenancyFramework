using System;
using System.Text;
using System.Web;

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
        public static string GetFullExceptionMessage(this Exception ex, bool includeStackTrace = false, HttpContext context = null)
        {
            if (ex == null) return string.Empty;

            StringBuilder sbMessages = new StringBuilder();
            StringBuilder sbStackTraces = new StringBuilder();
            if (includeStackTrace && !string.IsNullOrWhiteSpace(ex.StackTrace))
            {
                sbStackTraces.AppendFormat("\nEXCEPTION TYPE: {0}; STACK TRACE: {1}", ex.GetType().FullName, ex.StackTrace);
            }

            string source = ex.Source;
            if (string.IsNullOrWhiteSpace(source))
            {
                source = ex.TargetSite?.Name;
            }
            string page = string.Empty;
            if (context == null) context = HttpContext.Current;
            if (context != null)
            {
                try
                {
                    if (context.Request == null)
                    {
                        page += string.Format("[Source - {0}]", source);
                    }
                    else
                    {
                        page += string.Format("[Source - {0}] Request From: {1} | Url: {2} {3}", source, context.Request.UserHostAddress, context.Request.HttpMethod, context.Request.RawUrl);
                    }
                }
                catch
                {
                    page += string.Format("[Source - {0}]", source);
                }
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
                sbMessages.AppendLine(sqlClientEx.ToString());

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
            var loaderEx = ex as System.Reflection.ReflectionTypeLoadException;
            if (loaderEx != null)
            {
                var loaderInnerExes = loaderEx.LoaderExceptions;
                for (int i = 0; i < loaderInnerExes.Length; i++)
                {
                    sbMessages.AppendFormat(" and Loader Exception {0}: {1}\n", (i + 1), loaderInnerExes[i].Message);
                }
            }
            var inner = ex.InnerException;
            while (inner != null)
            {
                sbMessages.AppendFormat(" because {0} ", inner.GetFullExceptionMessage(includeStackTrace, context));
                inner = inner.InnerException;
            }
            return string.Format("{0}{1}\n{2}", sbMessages, page == string.Empty ? "" : ("\n" + page), sbStackTraces);
        }
    }
}
