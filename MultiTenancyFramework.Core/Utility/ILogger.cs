using System;

namespace MultiTenancyFramework
{
    public interface ILogger
    {
        void Log(Exception ex);

        void Log(string info);

        void Log(string format, params object[] args);

    }
}
