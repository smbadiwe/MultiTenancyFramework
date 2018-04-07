using System;
using System.IO;
using System.Text;
using System.Web;
using NLog;
using Newtonsoft.Json;
using MultiTenancyFramework.Logic;

namespace MultiTenancyFramework
{
    public class Logger : ILogger
    {
        private NLog.Logger _logger = LogManager.GetLogger(ConfigurationHelper.AppSettingsItem<string>("AppName") ?? "MultiTenancyFramework");
        private LogLogic logLogic = new LogLogic();

        public void SetLogger(object logger)
        {
            if (logger != null)
            {
                var loggr = logger as NLog.Logger;
                if (loggr != null)
                    _logger = loggr;
            }
            else
            {
                _logger = LogManager.GetLogger(ConfigurationHelper.AppSettingsItem<string>("AppName") ?? "MultiTenancyFramework");
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
            var loglevel = GetLogLevel(level);
            if (!_logger.IsEnabled(loglevel)) return;

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

                _logger.Log(loglevel, format, args);

                logLogic.InsertLog(_logger.Name, HttpContext.Current, level, string.Format(format, args));
            }
        }

        public virtual void Log(Exception ex, bool isFatal = false)
        {
            if (ex == null) return;
            if (ex is System.Threading.ThreadAbortException) return;
            var context = HttpContext.Current;

            bool doNotSendEmail;
            var exMsg = BuildErrorMsg(ex, context, out doNotSendEmail);

            if (!string.IsNullOrWhiteSpace(exMsg))
            {
                LoggerConfigurationManager.LoadConfigFileAndSetLoggerConfigProp(context?.Server);
                var level = isFatal ? LoggingLevel.Fatal : LoggingLevel.Error;

                _logger.Log(GetLogLevel(level), exMsg);

                logLogic.InsertLog(_logger.Name, context, level, ex.Message, exMsg);

                if (false == doNotSendEmail)
                {
                    try
                    {
                        Emailer.EmailLogMessage(exMsg, false);
                    }
                    catch (Exception ex2)
                    {
                        Log(LoggingLevel.Warn, "Error sending log email: {0}", ex2.Message);
                    }
                }
            }
        }

        public void Log(string format, params object[] args)
        {
            Log(LoggingLevel.Debug, format, args);
        }
        
        public void Trace(string format, params object[] args)
        {
            Log(LoggingLevel.Trace, format, args);
        }

        public void Error(string format, params object[] args)
        {
            Log(LoggingLevel.Error, format, args);
        }

        public void Info(string format, params object[] args)
        {
            Log(LoggingLevel.Info, format, args);
        }

        private string BuildErrorMsg(Exception ex, HttpContext context, out bool doNotSendEmail)
        {
            doNotSendEmail = false;
            if (ex != null)
            {
                var genEx = ex as GeneralException;
                if (genEx != null)
                {
                    var excepts = new[]
                    {
                        ExceptionType.UnidentifiedInstitutionCode,
                        //ExceptionType.DoNothing,
                        ExceptionType.SetupFailure,
                        ExceptionType.SessionTimeOut,
                        ExceptionType.InvalidUserActionOrInput
                    };
                    doNotSendEmail = genEx.ExceptionType.IsContainedIn(excepts);
                    if (!doNotSendEmail)
                    {
                        genEx = genEx.InnerException as GeneralException;
                        doNotSendEmail = genEx != null && genEx.ExceptionType.IsContainedIn(excepts);
                    }
                }
                else
                {
                    ex = ex.GetBaseException();
                    if (ex == null) return string.Empty;
                }

                return ex.GetFullExceptionMessage(true, context);
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
                    System.Diagnostics.Trace.TraceInformation(exceptionMessage);
                }
                else
                {
                    System.Diagnostics.Trace.TraceError(exceptionMessage);
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
