using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using System.Xml.Linq;

namespace MultiTenancyFramework
{
    public enum LoggingLevel
    {
        Trace,
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }

    public class LoggerConfigurationManager
    {

        public static void ConfigureNLogWithDefaults()
        {
            ConfigureNLog(true, null);
        }

        /// <summary>
        /// Configures the n log.
        /// </summary>
        /// <param name="logToFile">if set to <c>true</c> [log to file].</param>
        /// <param name="ruleNameAndFileSet">The rule name and &lt; file name, Logging Level &gt;. Key is rule name; value is file name (not full path).</param>
        public static void ConfigureNLog(bool logToFile, Dictionary<string, Tuple<string, LoggingLevel>> ruleNameAndFileSet)
        {
            // Step 1. Create configuration object 
            var config = new LoggingConfiguration();

            // Step 2. Create targets and add them to the configuration 
            string format = @"${time}|${pad:padding=5:inner=${level:uppercase=true}}| ${message}";

            // Step 3. Set target properties 
            if (logToFile)
            {
                string format2 = @"${time}|${pad:padding=5:inner=${level:uppercase=true}}|${logger}| ${message}";
                string fileName = "${shortdate}.log", logDir = LogDirectory;
                if (ruleNameAndFileSet != null && ruleNameAndFileSet.Count > 0)
                {
                    var nullTarget = new NullTarget("BlackHole");
                    config.AddTarget("BlackHole", nullTarget);
                    int i = 0;
                    foreach (var item in ruleNameAndFileSet)
                    {
                        if (string.IsNullOrWhiteSpace(item.Key) || item.Key == "*") continue;

                        if (!string.IsNullOrWhiteSpace(item.Value.Item1) && item.Value.Item1 != fileName)
                        {
                            var name = "file_" + (++i);
                            var target = new FileTarget
                            {
                                EnableFileDelete = true,
                                CreateDirs = true,
                                Layout = format2,
                                FileName = Path.Combine(logDir, item.Value.Item1),
                                Name = name,
                            };
                            config.AddTarget(name, target);
                            var level = item.Value.Item2;
                            config.LoggingRules.Add(new LoggingRule(item.Key, LogLevel.FromString(level.ToString()), target) { Final = true });
                            if (level != LoggingLevel.Trace)
                            {
                                config.LoggingRules.Add(new LoggingRule(item.Key, LogLevel.Trace, nullTarget) { Final = true });
                            }
                        }
                    }
                }

                var fileTarget = new FileTarget
                {
                    EnableFileDelete = true,
                    CreateDirs = true,
                    Layout = format,
                    FileName = Path.Combine(logDir, fileName),
                    Name = "file",
                };
                config.AddTarget("file", fileTarget);
                config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, fileTarget));
            }

#if DEBUG
            var consoleTarget = new ColoredConsoleTarget
            {
                Name = "console",
                Layout = format,
            };
            config.AddTarget("console", consoleTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, consoleTarget));
            var debugTarget = new DebuggerTarget
            {
                Name = "debug",
                Layout = format,
            };
            config.AddTarget("debug", debugTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, debugTarget));
#endif

            // Step 5. Activate the configuration
            LogManager.Configuration = config;
        }

        private static HttpServerUtility _server;
        public static void LoadConfigFileAndSetLoggerConfigProp(HttpServerUtility server)
        {
            if (LoggerConfig != null) return;

            _server = server;
            string xmlFilePath = null;
            if (server == null)
            {
                xmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LoggingConfig.xml");
            }
            else
            {
                xmlFilePath = server.MapPath("~/LoggingConfig.xml");
            }

            XDocument doc;
            try
            {
                if (!File.Exists(xmlFilePath))
                {
                    // Paranoia
                    xmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LoggingConfig.xml");

                    if (!File.Exists(xmlFilePath))
                    {
                        doc = XDocument.Parse(@"<LoggingConfig>
                                                  <!--Where the log files are -->
                                                  <LogDirectory>Logs</LogDirectory>
                                                  <!--When false, only actual errors are logged-->
                                                  <Enabled>true</Enabled>
                                                  <!--in MB. The system must have at least this amount of space-->
                                                  <SizeLimit>100</SizeLimit>
                                                </LoggingConfig>");
                        doc.Save(xmlFilePath);
                    }
                    else
                    {
                        doc = XDocument.Load(xmlFilePath);
                    }
                }
                else
                {
                    doc = XDocument.Load(xmlFilePath);
                }
            }
            catch
            {
                doc = XDocument.Parse(@"<LoggingConfig>
                                            <!--Where the log files are -->
                                            <LogDirectory>Logs</LogDirectory>
                                            <!--When false, only actual errors are logged-->
                                            <Enabled>true</Enabled>
                                            <!--in MB. The system must have at least this amount of space-->
                                            <SizeLimit>100</SizeLimit>
                                        </LoggingConfig>");
                try
                {
                    doc.Save(xmlFilePath);
                }
                catch { }
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
                string dirRead = LoggerConfig == null ? "Logs" : LoggerConfig["LogDirectory"];
                if (string.IsNullOrWhiteSpace(dirRead)) dirRead = "Logs";

                dirRead = dirRead.Replace("~/", "");
                if (_server == null)
                {
                    return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dirRead);
                }
                else
                {
                    return _server == null ? "Logs" : _server.MapPath("~/" + dirRead);
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
