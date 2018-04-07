using MultiTenancyFramework.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MultiTenancyFramework.Data
{
    public interface ICoreGridPagingDAO<T> : ICoreGridPagingDAO<T, long> where T : class, IBaseEntity<long>
    {
    }
    public interface ICoreGridPagingDAO<T, idT> where T : class, IBaseEntity<idT> where idT : IEquatable<idT>
    {
        RetrievedData<T> RetrieveUsingPaging(IQueryable<T> theQueryOver, int startIndex, int maxRows, bool hasOrderBy = false);
        Task<RetrievedData<T>> RetrieveUsingPagingAsync(IQueryable<T> theQueryOver, int startIndex, int maxRows, bool hasOrderBy = false, CancellationToken token = default(CancellationToken));
        RetrievedData<TTransform> RetrieveUsingPaging<TTransform>(IQueryable<T> theQueryOver, int startIndex, int maxRows, bool hasOrderBy = false)
            where TTransform : class;
        Task<RetrievedData<TTransform>> RetrieveUsingPagingAsync<TTransform>(IQueryable<T> theQueryOver, int startIndex, int maxRows, bool hasOrderBy = false, CancellationToken token = default(CancellationToken))
            where TTransform : class;
    }

}
