using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace MultiTenancyFramework
{
    public class Logger : ILogger
    {
        public virtual void Log(string info)
        {
            Log(new Exception(info));
        }

        public virtual void Log(Exception ex)
        {
            try
            {
                HttpContext context = HttpContext.Current;
                Task.Factory.StartNew(() => LogToFile(ex, context));
            }
            catch { }
        }

        public virtual void Log(string format, params object[] args)
        {
            Log(new Exception(string.Format(format, args)));
        }

        private void LogToFile(Exception ex, HttpContext context) //, out bool isDone
        {
            if (ex is System.Threading.ThreadAbortException) return;
            if (context == null) context = HttpContext.Current;

            ConfigurationManager.LoadConfigFileAndSetLoggerConfigProp(context);

            //isDone = false;
            bool isInfo = string.IsNullOrWhiteSpace(ex.StackTrace);
            if (isInfo)
            {
                if (ConfigurationManager.Enabled == false)
                {
                    //isDone = true;
                    return;
                }
            }
            var exMsg = this.BuildErrorMsg(ex, context, isInfo);

            Emailer.EmailLogMessage(exMsg, isInfo);

            WriteToFile(exMsg, context, false, isInfo);
        }

        private string BuildErrorMsg(Exception ex, HttpContext context, bool isInfo)
        {
            if (ex != null)
            {
                if (context == null) context = HttpContext.Current;
                if (!isInfo) ex = ex.GetBaseException();

                if (ex == null) return string.Empty;

                string page = string.Empty;
                string date = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss.ffffffK tt");
                string msg = ex.GetFullExceptionMessage(true);
                if (ex.TargetSite != null)
                {
                    page = ex.TargetSite.Name;
                }
                else if (context != null)
                {
                    try
                    {
                        page += string.Format(". [Source - {2}] Request From: {0} | Url: {1}", context.Request.UserHostAddress, context.Request.RawUrl, ex.Source);
                    }
                    catch
                    {
                        page += string.Format(" [Source - {0}]", ex.Source);
                    }
                }
                //string trace = ex.StackTrace;
                return string.Format("\r\n[{0}]\r\n Message: \t{1}{2}\r\n", date, msg, string.IsNullOrWhiteSpace(page) ? "" : ("\r\n Method Called: \t" + page));
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
                string logDirectory = ConfigurationManager.LogDirectory;
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

    public class ConfigurationManager
    {
        private static HttpContext _context;
        public static void LoadConfigFileAndSetLoggerConfigProp(HttpContext context)
        {
            _context = context;
            if (LoggerConfig != null) return;

            string xmlFilePath = null;
            if (context == null)
            {
                xmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LoggingConfig.xml");
            }
            else
            {
                xmlFilePath = context.Server.MapPath("~/LoggingConfig.xml");
            }
            if (string.IsNullOrWhiteSpace(xmlFilePath)) throw new ApplicationException("Could not get any LoggingConfig file.");
            if (!xmlFilePath.EndsWith("LoggingConfig.xml")) throw new ApplicationException("No LoggingConfig file supplied");

            XDocument doc;
            try
            {
                doc = XDocument.Load(xmlFilePath);
            }
            catch
            {
                xmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LoggingConfig.xml"); //HttpContext.Current != null ? HttpContext.Current.Server.MapPath("bin/LoggingConfig.xml") : ;

                try
                {
                    doc = XDocument.Load(xmlFilePath);
                }
                catch
                {
                    doc = XDocument.Parse(string.Format(@"
                                            <LoggingConfig>
                                              <!--Where the log files are -->
                                              <LogDirectory>Logs</LogDirectory>
                                              <!--When false, only actual errors are logged-->
                                              <Enabled>true</Enabled>
                                              <!--in MB. The system must have at least this amount of space-->
                                              <SizeLimit>100</SizeLimit>
                                            </LoggingConfig>", AppDomain.CurrentDomain.FriendlyName));
                    try
                    {
                        doc.Save(xmlFilePath);
                    }
                    catch { }
                }
            }
            XElement rootNode = doc.Root;
            if (rootNode.Name.LocalName != "LoggingConfig") throw new ApplicationException("Invalid root node. The root node should be named 'LoggingConfig'.");
            if (!rootNode.HasElements) throw new ApplicationException("Root nod contains no elements.");
            var config = new NameValueCollection();
            foreach (var element in rootNode.Elements())
            {
                if (element.Name == "MailSetup")
                {
                    if (element.HasElements)
                    {
                        foreach (var elementj in element.Elements())
                        {
                            config.Add(elementj.Name.LocalName, elementj.Value);
                        }
                    }
                }
                else
                {
                    config.Add(element.Name.LocalName, element.Value);
                }
            }
            LoggerConfig = config; // (System.Configuration.ConfigurationManager.GetSection("SOMA.Framework.Logging") as NameValueCollection);
        }

        public static void UnloadConfigFileAndResetLoggerConfigProp()
        {
            LoggerConfig = null;
        }

        public static string LogDirectory
        {
            get
            {
                var dirRead = LoggerConfig["LogDirectory"];
                if (string.IsNullOrWhiteSpace(dirRead)) dirRead = "Logs";

                dirRead = dirRead.Replace("~/", "");
                if (_context == null)
                {
                    return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dirRead);
                }
                else
                {
                    return _context.Server == null ? "Logs" : _context.Server.MapPath("~/" + dirRead);
                }
            }
        }

        public static decimal SizeLimit
        {
            get
            {
                try
                {
                    return Convert.ToDecimal(LoggerConfig["SizeLimit"]);
                }
                catch
                {
                    return 100;
                }
            }
        }

        public static bool Enabled
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(LoggerConfig["Enabled"]);
                }
                catch
                {
                    return false;
                }
            }
        }

        private static NameValueCollection LoggerConfig { get; set; }
        
    }

}
