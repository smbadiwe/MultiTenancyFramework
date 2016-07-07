using MultiTenancyFramework.Data;
using System.Linq;

namespace MultiTenancyFramework
{
    public static class EntityExtensions
    {
        public static RetrievedData<TSub> Cast<T, TSub>(this RetrievedData<T> list)
        {
            return new RetrievedData<TSub>
            {
                DataBatch = list.DataBatch.Cast<TSub>().ToList(),
                TotalCount = list.TotalCount
            };
        }

    }
}
