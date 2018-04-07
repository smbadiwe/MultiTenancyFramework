using System;
using System.Linq;
using System.Threading.Tasks;
using MultiTenancyFramework.Tasks;

namespace MultiTenancyFramework.Core.TaskManager
{
    public class ScheduledTaskRunner
    {
        #region Fields

        private bool? _disabled;
        private ILogger logger;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor for ScheduledTaskRunner
        /// </summary>
        /// <param name="task">Task </param>
        public ScheduledTaskRunner(ScheduledTask task)
        {
            ScheduleTask = task;
            logger = Utilities.Logger;
            logger.SetNLogLogger("ScheduledTaskRunner");
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Initialize and execute task
        /// </summary>
        private async Task ExecuteTask()
        {
            if (IsDisabled)
                return;

            var type = Type.GetType(ScheduleTask.Type) ??
                //ensure that it works fine when only the type name is specified (do not require fully qualified names)
                AppDomain.CurrentDomain.GetAssemblies()
                .Select(a => a.GetType(ScheduleTask.Type))
                .FirstOrDefault(t => t != null);
            if (type == null)
                throw new Exception($"Schedule task ({ScheduleTask.Type}) cannot by instantiated");

            object instance = Activator.CreateInstance(type);

            var task = instance as IRunnableTask;
            if (task == null)
                return;

            var scheduleTaskService = new ScheduledTaskEngine(ScheduleTask.InstitutionCode);

            ScheduleTask.LastStartUtc = DateTime.UtcNow;
            //update appropriate datetime properties
            await scheduleTaskService.UpdateAsync(ScheduleTask);

            logger.Trace($"ScheduledTaskRunner: Now about to execute [{ScheduleTask.Name}] task");

            await task.Execute(ScheduleTask.InstitutionCode);

            logger.Trace($"ScheduledTaskRunner: Done executing [{ScheduleTask.Name}] task");

            ScheduleTask.LastEndUtc = ScheduleTask.LastSuccessUtc = DateTime.UtcNow;
            //update appropriate datetime properties
            await scheduleTaskService.UpdateAsync(ScheduleTask);
        }

        /// <summary>
        /// Is task already running?
        /// </summary>
        /// <param name="scheduleTask">Schedule task</param>
        /// <returns>Result</returns>
        protected virtual bool IsTaskAlreadyRunning(ScheduledTask scheduleTask)
        {
            //task run for the first time
            if (!scheduleTask.LastStartUtc.HasValue && !scheduleTask.LastEndUtc.HasValue)
                return false;

            var lastStartUtc = scheduleTask.LastStartUtc ?? DateTime.UtcNow;

            //task already finished
            if (scheduleTask.LastEndUtc.HasValue && lastStartUtc < scheduleTask.LastEndUtc)
                return false;

            //task wasn't finished last time
            if (lastStartUtc.AddSeconds(scheduleTask.Seconds) <= DateTime.UtcNow)
                return false;

            return true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes the task
        /// </summary>
        /// <param name="throwException">A value indicating whether exception should be thrown if some error happens</param>
        /// <param name="ensureRunOncePerPeriod">A value indicating whether we should ensure this task is run once per run period</param>
        public async Task Execute(bool throwException = false, bool ensureRunOncePerPeriod = true)
        {
            if (ScheduleTask == null || IsDisabled)
                return;

            if (ensureRunOncePerPeriod)
            {
                //task already running
                if (IsTaskAlreadyRunning(ScheduleTask))
                    return;

                //validation (so nobody else can invoke this method when he wants)
                if (ScheduleTask.LastEndUtc.HasValue && (DateTime.UtcNow - ScheduleTask.LastEndUtc).Value.TotalSeconds <
                    ScheduleTask.Seconds)
                    //too early
                    return;
            }

            try
            {
                await ExecuteTask();
            }
            catch (Exception exc)
            {
                logger.Error($"ScheduledTaskRunner: Error while running the '{ScheduleTask.Name}' schedule task:\n{exc.GetFullExceptionMessage()}.\nUpdating schedule task record now.");
                logger.Log(exc);

                var scheduleTaskService = new ScheduledTaskEngine(ScheduleTask.InstitutionCode);

                ScheduleTask.IsDisabled = ScheduleTask.StopOnError;
                ScheduleTask.LastEndUtc = DateTime.UtcNow;
                await scheduleTaskService.UpdateAsync(ScheduleTask);

                //log error
                logger.Trace($"ScheduledTaskRunner: Schedule task record [{ScheduleTask.Name}] updated. Aborting...");
                if (throwException)
                    throw;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Schedule task
        /// </summary>
        public ScheduledTask ScheduleTask { get; }

        /// <summary>
        /// A value indicating whether the task is enabled
        /// </summary>
        public bool IsDisabled
        {
            get
            {
                if (!_disabled.HasValue)
                    _disabled = ScheduleTask?.IsDisabled;

                return _disabled.HasValue && _disabled.Value;
            }
            set
            {
                _disabled = value;
            }

            #endregion

        }
    }
}
