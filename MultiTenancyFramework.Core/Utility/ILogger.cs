using System;

namespace MultiTenancyFramework
{
    public interface ILogger
    {
        void SetLogger(object logger);

        void Log(Exception ex);

        void Log(string info);

        void Log(string format, params object[] args);

    }
}
