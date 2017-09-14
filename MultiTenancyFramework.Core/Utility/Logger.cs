using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using NLog;
using Newtonsoft.Json;

namespace MultiTenancyFramework
{
    public class Logger : ILogger
    {
        private static NLog.Logger _logger = LogManager.GetLogger(ConfigurationHelper.AppSettingsItem<string>("AppName") ?? "MultiTenancyFramework");
        
        public void SetLogger(object logger)
        {
            if (logger != null)
            {
                var loggr = logger as NLog.Logger;
                if (loggr != null)
                    _logger = loggr;
            }
        }

        private LogLevel GetLogLevel(LoggingLevel level)
        {
            switch (level)
            {
                case LoggingLevel.Debug:
                    return LogLevel.Debug;
                case LoggingLevel.Error:
                    return LogLevel.Error;
                case LoggingLevel.Info:
                    return LogLevel.Info;
                case LoggingLevel.Warn:
                    return LogLevel.Warn;
                case LoggingLevel.Fatal:
                    return LogLevel.Fatal;
                case LoggingLevel.Trace:
                default:
                    return LogLevel.Trace;
            }
        }

        /// <summary>
        /// Logs the specified format.
        /// </summary>
        /// <param name="level">The logging level.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments. If an object instance is part of the list, it will be JSON-serialized</param>
        public virtual void Log(LoggingLevel level, string format, params object[] args)
        {
            if (!string.IsNullOrWhiteSpace(format))
            {
                if (args != null && args.Length > 0)
                {
                    for (int i = args.Length - 1; i >= 0; i--)
                    {
                        if (args[i] != null)
                        {
                            if (args[i].GetType().IsPrimitiveType()) continue;

                            try
                            {
                                args[i] = JsonConvert.SerializeObject(args[i]);
                            }
                            catch { }
                        }
                    }
                }

                _logger.Log(GetLogLevel(level), format, args);
            }
        }

        public void Log(string format, params object[] args)
        {
            Log(LoggingLevel.Debug, format, args);
        }

        public virtual void Log(string info)
        {
            Log(LoggingLevel.Debug, info, null);
        }

        public virtual void Log(LoggingLevel level, string info)
        {
            Log(level, info, null);
        }

        public virtual void Log(Exception ex, bool isFatal = false)
        {
            if (ex == null) return;
            if (ex is System.Threading.ThreadAbortException) return;
            var context = HttpContext.Current;

            LoggerConfigurationManager.LoadConfigFileAndSetLoggerConfigProp(context?.Server);
            
            var genEx = ex as GeneralException;
            bool doNotSendEmail = genEx != null && 
                (genEx.ExceptionType == ExceptionType.UnidentifiedInstitutionCode ||
                genEx.ExceptionType == ExceptionType.DoNothing ||
                genEx.ExceptionType == ExceptionType.SetupFailure);
            var exMsg = BuildErrorMsg(ex, context);

            if (!string.IsNullOrWhiteSpace(exMsg))
            {
                _logger.Log(isFatal ? LogLevel.Fatal : LogLevel.Error, exMsg);
                if (false == doNotSendEmail)
                    Emailer.EmailLogMessage(exMsg, false);
            }
        }

        private string BuildErrorMsg(Exception ex, HttpContext context)
        {
            if (ex != null)
            {
                if (context == null) context = HttpContext.Current;
                ex = ex.GetBaseException();

                if (ex == null) return string.Empty;

                string msg = ex.GetFullExceptionMessage(true);
                if (string.IsNullOrWhiteSpace(msg)) return string.Empty;

                string page = string.Empty;
                if (ex.TargetSite != null && !string.IsNullOrWhiteSpace(ex.TargetSite.Name))
                {
                    page = ex.TargetSite.Name;
                }
                else if (context != null)
                {
                    try
                    {
                        page += string.Format("[Source - {2}] Request From: {0} | Url: {1}", context.Request?.UserHostAddress, context.Request?.RawUrl, ex.Source);
                    }
                    catch
                    {
                        page += string.Format("[Source - {0}]", ex.Source);
                    }
                }
                //string trace = ex.StackTrace;
                return string.Format("{0}{1}", msg, string.IsNullOrWhiteSpace(page) ? "" : ("\n\t Method Called: \t" + page));
            }
            return string.Empty;
        }

        private void WriteToFile(string exceptionMessage, HttpContext context, bool isTest, bool isInfo)
        {
            try
            {
                Console.WriteLine(exceptionMessage);
                if (isInfo)
                {
                    Trace.TraceInformation(exceptionMessage);
                }
                else
                {
                    Trace.TraceError(exceptionMessage);
                }
                if (context == null) context = HttpContext.Current;
                string logDirectory = LoggerConfigurationManager.LogDirectory;
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }
                var filePath = Path.Combine(logDirectory, (DateTime.Now.ToString("dd-MMM-yyyy") + (isTest ? "_Test.txt" : ".txt")));

                try
                {
                    using (var sw = new StreamWriter(filePath, true, Encoding.UTF8, 4096))
                    {
                        sw.WriteAsync(exceptionMessage);
                    }
                }
                catch
                {
                    System.Threading.Thread.Sleep(2000); // to wait a little bit to make sure the file is released by any other process holding it before trying again
                    using (var sw = new StreamWriter(filePath, true, Encoding.UTF8, 4096))
                    {
                        sw.WriteAsync(exceptionMessage);
                    }
                }
            }
            catch { }
        }

    }

}
