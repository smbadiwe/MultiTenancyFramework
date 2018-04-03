using MultiTenancyFramework.Entities;
using System;

namespace MultiTenancyFramework.Core.TaskManager
{
    /// <summary>
    /// This represents the task record as stored in DB
    /// </summary>
    /// <seealso cref="Entity" />
    public class ScheduledTask : Entity
    {
        /// <summary>
        /// Gets or sets the run period (in seconds)
        /// </summary>
        public virtual int Seconds { get; set; }

        /// <summary>
        /// Gets or sets the type of appropriate IScheduleTask class
        /// </summary>
        public virtual string Type { get; set; }
        
        /// <summary>
        /// Gets or sets the value indicating whether a task should be stopped on some error
        /// </summary>
        public virtual bool StopOnError { get; set; }

        /// <summary>
        /// Gets or sets the datetime when it was started last time
        /// </summary>
        public virtual DateTime? LastStartUtc { get; set; }
        /// <summary>
        /// Gets or sets the datetime when it was finished last time (no matter failed ir success)
        /// </summary>
        public virtual DateTime? LastEndUtc { get; set; }
        /// <summary>
        /// Gets or sets the datetime when it was sucessfully finished last time
        /// </summary>
        public virtual DateTime? LastSuccessUtc { get; set; }
    }
    
}
