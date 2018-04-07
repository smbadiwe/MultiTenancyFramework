using System;

namespace MultiTenancyFramework
{
    public interface ILogger
    {
        void SetLogger(object logger);

        void Log(Exception ex, bool isFatal = false);
        
        void Trace(string format, params object[] args);

        void Error(string format, params object[] args);

        void Info(string format, params object[] args);

        /// <summary>
        /// Logs the specified message using <see cref="LoggingLevel.Debug"/>
        /// </summary>
        /// <param name="format">The message format.</param>
        /// <param name="args">The arguments.</param>
        void Log(string format, params object[] args);

        void Log(LoggingLevel level, string format, params object[] args);

    }
}
