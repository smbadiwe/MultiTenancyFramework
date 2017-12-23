using System;
using System.Threading.Tasks;

namespace MultiTenancyFramework
{
    /// <summary>
    /// Runner for background tasks
    /// </summary>
    public class BackgroundTaskRunner
    {
        /// <summary>
        /// Runs the specified action in a fire-and-forget fashion
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="errorHandler">The error handler.</param>
        /// <exception cref="System.ArgumentNullException">task</exception>
        public static void Run(Action action, Action<Exception> errorHandler = null)
        {
            if (action == null)
                throw new ArgumentNullException("task");

            var task = Task.Run(action);
            if (errorHandler == null)
            {
                task.ContinueWith(DefaultErrorContinuation,
                    TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted);
            }
            else
            {
                task.ContinueWith(t => errorHandler(t.Exception?.GetBaseException()),
                    TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted);

            }
        }

        private static void DefaultErrorContinuation(Task t)
        {
            try
            {
                if (t.Exception != null)
                    Utilities.Logger.Log(t.Exception.GetBaseException());
                t.Wait();
            }
            catch { }
        }
    }
}
