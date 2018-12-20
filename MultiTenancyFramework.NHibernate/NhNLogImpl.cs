using NHibernate;
using System;
using NLog;

namespace MultiTenancyFramework.NHibernate
{
    public class NLogFactory : INHibernateLoggerFactory
    {
        public INHibernateLogger LoggerFor(string keyName)
        {
            return new NLogLogger(LogManager.GetLogger(keyName));
        }

        public INHibernateLogger LoggerFor(Type type)
        {
            return new NLogLogger(LogManager.GetLogger(type.FullName));
        }
    }

    public class NLogLogger : INHibernateLogger
    {
        private readonly NLog.Logger logger;

        public NLogLogger(NLog.Logger logger)
        {
            this.logger = logger;
        }
        
        public bool IsEnabled(NHibernateLogLevel logLevel)
        {
            switch (logLevel)
            {
                case NHibernateLogLevel.Debug:
                    return logger.IsDebugEnabled;
                case NHibernateLogLevel.Error:
                    return logger.IsErrorEnabled;
                case NHibernateLogLevel.Info:
                    return logger.IsInfoEnabled;
                case NHibernateLogLevel.Fatal:
                    return logger.IsFatalEnabled;
                case NHibernateLogLevel.Trace:
                    return logger.IsTraceEnabled;
                case NHibernateLogLevel.Warn:
                    return logger.IsWarnEnabled;
                default:
                    return false;
            }
        }

        public void Log(NHibernateLogLevel logLevel, NHibernateLogValues state, Exception exception)
        {
            LogLevel level;
            switch (logLevel)
            {
                case NHibernateLogLevel.Debug:
                    level = LogLevel.Debug;
                    break;
                case NHibernateLogLevel.Error:
                    level = LogLevel.Error;
                    break;
                case NHibernateLogLevel.Info:
                    level = LogLevel.Info;
                    break;
                case NHibernateLogLevel.Fatal:
                    level = LogLevel.Fatal;
                    break;
                case NHibernateLogLevel.Trace:
                    level = LogLevel.Trace;
                    break;
                case NHibernateLogLevel.Warn:
                    level = LogLevel.Warn;
                    break;
                default:
                    level = LogLevel.Debug;
                    break;
            }

            if (exception is null)
            {
                logger.Log(level, state.Format, state.Args);
            }
            else
            {
                logger.Log(level, exception, state.Format, state.Args);
            }
        }
    }
}
