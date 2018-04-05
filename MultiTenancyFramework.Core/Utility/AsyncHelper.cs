using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiTenancyFramework
{
    public class AsyncHelper
    {
        private static readonly TaskFactory _myTaskFactory = new
      TaskFactory(CancellationToken.None,
                  TaskCreationOptions.None,
                  TaskContinuationOptions.None,
                  TaskScheduler.Default);

        public static TResult RunSync<TResult>(Task<TResult> task)
        {
            return task
              .GetAwaiter()
              .GetResult();
        }

        public static void RunSync(Task task)
        {
            task
              .GetAwaiter()
              .GetResult();
        }

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return _myTaskFactory
              .StartNew(func)
              .Unwrap()
              .GetAwaiter()
              .GetResult();
        }

        public static void RunSync(Func<Task> func)
        {
            _myTaskFactory
              .StartNew(func)
              .Unwrap()
              .GetAwaiter()
              .GetResult();
        }
    }
}
