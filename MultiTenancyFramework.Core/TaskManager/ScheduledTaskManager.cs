using MultiTenancyFramework.Data;
using MultiTenancyFramework.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MultiTenancyFramework.Core.TaskManager
{
    public class ScheduledTaskManager
    {

        #region Consts

        /// <summary>
        /// Schedule task path
        /// </summary>
        public const string ScheduleTaskPath = "scheduledtaskapi/runtask";
        private const int _notRunTasksInterval = 60 * 30; //30 minutes

        #endregion

        #region Fields

        private bool _isInitialized = false;
        private readonly ILogger _logger;
        private readonly List<TaskThread> _taskThreads = new List<TaskThread>();

        #endregion

        #region Ctor

        private ScheduledTaskManager()
        {
            _logger = Utilities.Logger;
            _logger.SetNLogLogger("ScheduledTaskManager");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the task manager
        /// </summary>
        public void Initialize()
        {
            _logger.Log(LoggingLevel.Trace, $"Initializing task manager. {_taskThreads.Count} task threads will be cleared and new ones built.");
            _taskThreads.Clear();

            var allInstitutions = MyServiceLocator.GetInstance<IInstitutionDAO>()
                .RetrieveAllActive();
            List<Institution> institutions;
            if (allInstitutions == null)
            {
                institutions = new List<Institution>(1) { new Institution { Code = Utilities.INST_DEFAULT_CODE } };
            }
            else
            {
                institutions = new List<Institution>(1 + allInstitutions.Count);
                institutions.Add(new Institution { Code = Utilities.INST_DEFAULT_CODE, Name = "Core" });
                institutions.AddRange(allInstitutions);
            }
            _logger.Log(LoggingLevel.Trace, $"Tasks will be queued for {institutions.Count} institutions");
            foreach (var institution in institutions)
            {
                try
                {
                    var taskService = new ScheduledTaskEngine(institution.Code);
                    var scheduleTasks = taskService.RetrieveAllActive();

                    _logger.Log(LoggingLevel.Trace, $"{scheduleTasks.Count} task(s) will be queued for '{institution.Name}'");
                    if (scheduleTasks.Count > 0)
                    {
                        //group by threads with the same seconds
                        foreach (var scheduleTaskGrouped in scheduleTasks.GroupBy(x => x.Seconds))
                        {
                            //create a thread
                            var taskThread = new TaskThread(institution.Code)
                            {
                                Seconds = scheduleTaskGrouped.Key,
                            };
                            foreach (var scheduleTask in scheduleTaskGrouped)
                            {
                                taskThread.AddTask(scheduleTask);
                            }
                            _taskThreads.Add(taskThread);
                        }

                        //sometimes a task period could be set to several hours (or even days).
                        //in this case a probability that it'll be run is quite small (an application could be restarted)
                        //we should manually run the tasks which weren't run for a long time
                        var notRunTasks = scheduleTasks
                            //find tasks with "run period" more than 30 minutes
                            .Where(x => x.Seconds >= _notRunTasksInterval)
                            .Where(x => !x.LastStartUtc.HasValue || x.LastStartUtc.Value.AddSeconds(x.Seconds) < DateTime.UtcNow)
                            ;
                        //create a thread for the tasks which weren't run for a long time
                        if (notRunTasks.Any())
                        {
                            var taskThread = new TaskThread(institution.Code)
                            {
                                RunOnlyOnce = true,
                                Seconds = 60 * 5 //let's run such tasks in 5 minutes after application start
                            };
                            foreach (var scheduleTask in notRunTasks)
                            {
                                taskThread.AddTask(scheduleTask);
                            }
                            _taskThreads.Add(taskThread);
                        }

                        _logger.Log(LoggingLevel.Trace, $"Done queueing the {scheduleTasks.Count} task(s) for '{institution.Name}'");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Log(LoggingLevel.Error, $"Error while queueing task(s) for '{institution.Name}': \n{ex.GetFullExceptionMessage()}\n");
                }
            }

            _isInitialized = true;
            _logger.Log(LoggingLevel.Trace, $"Done initializing.");

        }

        /// <summary>
        /// Starts the task manager
        /// </summary>
        public void Start()
        {
            if (!_isInitialized)
            {
                Initialize();
            }
            _logger.Log(LoggingLevel.Trace, $"Starting up the {_taskThreads.Count} task thread(s).");
            foreach (var taskThread in _taskThreads)
            {
                try
                {
                    taskThread.InitTimer();
                }
                catch (Exception ex)
                {
                    _logger.Log(LoggingLevel.Error, $"Error disposing thread: {taskThread} task thread(s).\n{ex.GetFullExceptionMessage()}");
                }
            }
            _logger.Log(LoggingLevel.Trace, $"Done starting up the {_taskThreads.Count} task thread(s).");
        }

        /// <summary>
        /// Stops the task manager
        /// </summary>
        public void Stop()
        {
            _logger.Log(LoggingLevel.Trace, $"Stopping the {_taskThreads.Count} task thread(s).");
            foreach (var taskThread in _taskThreads)
            {
                try
                {
                    taskThread.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.Log(LoggingLevel.Error, $"Error disposing thread: {taskThread} task thread(s).\n{ex.GetFullExceptionMessage()}");
                }
            }

            _isInitialized = false;
            _logger.Log(LoggingLevel.Trace, $"Done stopping the {_taskThreads.Count} task thread(s).");
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the task mamanger instance
        /// </summary>
        public static ScheduledTaskManager Instance { get; } = new ScheduledTaskManager();

        /// <summary>
        /// Gets a list of task threads of this task manager. This list is read-only
        /// </summary>
        public IList<TaskThread> TaskThreads => new ReadOnlyCollection<TaskThread>(_taskThreads);

        #endregion
    }
}
