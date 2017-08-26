using System;

namespace MultiTenancyFramework
{
    public interface ILogger
    {
        void SetLogger(object logger);

        void Log(Exception ex, bool isFatal = false);

        void Log(string info);

        void Log(string format, params object[] args);

        void Log(LoggingLevel level, string info);

        void Log(LoggingLevel level, string format, params object[] args);

    }
}
