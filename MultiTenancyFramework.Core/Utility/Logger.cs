using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using NLog;

namespace MultiTenancyFramework
{
    public class Logger : ILogger
    {
        private static NLog.Logger _logger = LogManager.GetLogger(ConfigurationHelper.AppSettingsItem<string>("AppName") ?? "MultiTenancyFramework");

        public void SetLogger(object logger)
        {
            var loggr = _logger as NLog.Logger;
            if (loggr != null)
                _logger = loggr;
        }

        public virtual void Log(string format, params object[] args)
        {
            if (!string.IsNullOrWhiteSpace(format))
                _logger.Info(format, args);
        }

        public virtual void Log(string info)
        {
            Log(info, null);
        }

        public virtual void Log(Exception ex)
        {
            try
            {
                LogToFile(ex, null);
                //HttpContext context = HttpContext.Current;
                //Task.Factory.StartNew(() => LogToFile(ex, context));
            }
            catch { }
        }
        
        private void LogToFile(Exception ex, HttpContext context) //, out bool isDone
        {
            if (ex == null) return;
            if (ex is System.Threading.ThreadAbortException) return;
            if (context == null) context = HttpContext.Current;

            LoggerConfigurationManager.LoadConfigFileAndSetLoggerConfigProp(context?.Server);

            //isDone = false;
            bool isInfo = string.IsNullOrWhiteSpace(ex.StackTrace);
            if (isInfo)
            {
                if (LoggerConfigurationManager.Enabled == false)
                {
                    //isDone = true;
                    return;
                }
            }
            var exMsg = BuildErrorMsg(ex, context, isInfo);

            if (!string.IsNullOrWhiteSpace(exMsg))
            {
                _logger.Log(isInfo ? LogLevel.Info : LogLevel.Error, exMsg);
                Emailer.EmailLogMessage(exMsg, isInfo);

                //WriteToFile(exMsg, context, false, isInfo);
            }
        }

        private string BuildErrorMsg(Exception ex, HttpContext context, bool isInfo)
        {
            if (ex != null)
            {
                if (context == null) context = HttpContext.Current;
                if (!isInfo) ex = ex.GetBaseException();

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
                return string.Format("{0}{1}", msg, string.IsNullOrWhiteSpace(page) ? "" : ("\r\n\t Method Called: \t" + page));
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
