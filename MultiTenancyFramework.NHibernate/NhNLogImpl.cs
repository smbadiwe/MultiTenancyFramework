using NHibernate;
using System;
using NLog;

namespace MultiTenancyFramework.NHibernate
{
    public class NLogFactory : ILoggerFactory
    {
        #region ILoggerFactory Members

        public IInternalLogger LoggerFor(Type type)
        {
            return new NLogLogger(LogManager.GetLogger(type.FullName));
        }

        public IInternalLogger LoggerFor(string keyName)
        {
            return new NLogLogger(LogManager.GetLogger(keyName));
        }

        #endregion
    }

    public class NLogLogger : IInternalLogger
    {
        private readonly NLog.Logger logger;

        public NLogLogger(NLog.Logger logger)
        {
            this.logger = logger;
        }

        #region Properties

        public bool IsDebugEnabled { get { return logger.IsDebugEnabled; } }

        public bool IsErrorEnabled { get { return logger.IsErrorEnabled; } }

        public bool IsFatalEnabled { get { return logger.IsFatalEnabled; } }

        public bool IsInfoEnabled { get { return logger.IsInfoEnabled; } }

        public bool IsWarnEnabled { get { return logger.IsWarnEnabled; } }

        #endregion

        #region IInternalLogger Methods

        public void Debug(object message, Exception exception)
        {
            if (message != null && exception != null)
                logger.Debug(exception, message.ToString());
        }

        public void Debug(object message)
        {
            if (message != null)
                logger.Debug(message.ToString());
        }

        public void DebugFormat(string format, params object[] args)
        {
            if (!string.IsNullOrWhiteSpace(format))
                logger.Debug(format, args);
        }

        public void Error(object message, Exception exception)
        {
            if (message != null && exception != null)
                logger.Error(exception, message.ToString());
        }

        public void Error(object message)
        {
            if (message != null)
                logger.Error(message.ToString());
        }

        public void ErrorFormat(string format, params object[] args)
        {
            if (!string.IsNullOrWhiteSpace(format))
                logger.Error(format, args);
        }

        public void Fatal(object message, Exception exception)
        {
            if (message != null && exception != null)
                logger.Fatal(exception, message.ToString());
        }

        public void Fatal(object message)
        {
            if (message != null)
                logger.Fatal(message.ToString());
        }

        public void Info(object message, Exception exception)
        {
            if (message != null && exception != null)
                logger.Info(exception, message.ToString());
        }

        public void Info(object message)
        {
            if (message != null)
                logger.Info(message.ToString());
        }

        public void InfoFormat(string format, params object[] args)
        {
            if (!string.IsNullOrWhiteSpace(format))
                logger.Info(format, args);
        }

        public void Warn(object message, Exception exception)
        {
            if (message != null && exception != null)
                logger.Warn(exception, message.ToString());
        }

        public void Warn(object message)
        {
            if (message != null)
                logger.Warn(message.ToString());
        }

        public void WarnFormat(string format, params object[] args)
        {
            if (!string.IsNullOrWhiteSpace(format))
                logger.Warn(format, args);
        }

        #endregion
    }
}
