using System.Collections.Generic;

namespace MultiTenancyFramework.Data
{
    public class RetrievedData<T>
    {
        /// <summary>
        /// The batch returned; usually not everything in the DB
        /// </summary>
        public IList<T> DataBatch { get; set; } = new List<T>();
        /// <summary>
        /// Total in the DB; not usually the same as DataBatch.Count
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets the Count of the DataBatch
        /// </summary>
        public int Count
        {
            get { return DataBatch == null ? 0 : DataBatch.Count; }
        }
    }
}
