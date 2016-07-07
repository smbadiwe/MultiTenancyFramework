using System;

namespace MultiTenancyFramework
{
    public class Logger : ILogger
    {
        public virtual void Log(string info)
        {
            //throw new NotImplementedException();
        }

        public virtual void Log(Exception ex)
        {
            //throw new NotImplementedException();
        }

        public virtual void Log(string format, params object[] args)
        {
            //throw new NotImplementedException();
        }
    }
}
