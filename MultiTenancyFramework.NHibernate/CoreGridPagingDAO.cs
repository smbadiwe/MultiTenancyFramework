using MultiTenancyFramework.Data;
using MultiTenancyFramework.Entities;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace MultiTenancyFramework.NHibernate
{
    public abstract class CoreGridPagingDAO<T, idT> : CoreGeneralDAO, ICoreGridPagingDAO<T, idT> where T : class, IBaseEntity<idT> where idT : IEquatable<idT>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="startIndex"></param>
        /// <param name="maxRows">A value of Zero means we're not paging</param>
        /// <param name="hasOrderBy"></param>
        /// <returns></returns>
        public RetrievedData<T> RetrieveUsingPaging(IQueryOver<T, T> queryable, int startIndex, int maxRows, bool hasOrderBy = false)
        {
            return RetrieveUsingPaging<T>(queryable, startIndex, maxRows, hasOrderBy);
        }

        /// <summary>
        /// Use this API it you did an aliastobean transform to type TTransform
        /// </summary>
        /// <typeparam name="TTransform"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="startIndex"></param>
        /// <param name="maxRows">A value of Zero means we're not paging</param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public RetrievedData<TTransform> RetrieveUsingPaging<TTransform>(IQueryOver<T, T> queryable, int startIndex, int maxRows, bool hasOrderBy = false)
            where TTransform : class //, IEntity<idT>
        {
            IEnumerable<TTransform> result;
            int totalCount = 0;
            IFutureValue<int> futureCount = null;
            if (maxRows > 0)
            {
                futureCount = queryable.Clone().Select(Projections.RowCount()).FutureValue<int>();
                result = queryable.Skip(startIndex * maxRows).Take(maxRows).Future<TTransform>();
            }
            else //get all
            {
                result = queryable.Future<TTransform>();
            }
            var toReturn = result.ToList();
            if (futureCount != null)
            {
                totalCount = futureCount.Value;
            }
            else
            {
                totalCount = toReturn.Count;
            }
            result = null;
            futureCount = null;
            return new RetrievedData<TTransform>
            {
                DataBatch = toReturn,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="startIndex"></param>
        /// <param name="maxRows">A value of Zero means we're not paging</param>
        /// <param name="hasOrderBy"></param>
        /// <returns></returns>
        public async Task<RetrievedData<T>> RetrieveUsingPagingAsync(IQueryable<T> queryable, int startIndex, int maxRows, bool hasOrderBy = false, CancellationToken token = default(CancellationToken))
        {
            IFutureEnumerable<T> result;
            int totalCount = 0;
            IFutureValue<int> futureCount = null;
            if (maxRows > 0)
            {
                futureCount = queryable.ToFutureValue(f => f.Count());
                result = queryable.Skip(startIndex * maxRows).Take(maxRows).ToFuture<T>();
            }
            else //get all
            {
                result = queryable.ToFuture<T>();
            }
            var toReturn = (await result.GetEnumerableAsync()).ToList();
            if (futureCount != null)
            {
                totalCount = futureCount.Value;
            }
            else
            {
                totalCount = toReturn.Count;
            }
            result = null;
            futureCount = null;
            return new RetrievedData<T>
            {
                DataBatch = toReturn ?? new List<T>(0),
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="startIndex"></param>
        /// <param name="maxRows">A value of Zero means we're not paging</param>
        /// <param name="hasOrderBy"></param>
        /// <returns></returns>
        public RetrievedData<T> RetrieveUsingPaging(IQueryable<T> queryable, int startIndex, int maxRows, bool hasOrderBy = false)
        {
            IFutureEnumerable<T> result;
            int totalCount = 0;
            IFutureValue<int> futureCount = null;
            if (maxRows > 0)
            {
                futureCount = queryable.ToFutureValue(f => f.Count());
                result = queryable.Skip(startIndex * maxRows).Take(maxRows).ToFuture<T>();
            }
            else //get all
            {
                result = queryable.ToFuture<T>();
            }
            var toReturn = result.ToList();
            if (futureCount != null)
            {
                totalCount = futureCount.Value;
            }
            else
            {
                totalCount = toReturn.Count;
            }
            result = null;
            futureCount = null;
            return new RetrievedData<T>
            {
                DataBatch = toReturn ?? new List<T>(0),
                TotalCount = totalCount
            };
        }

        public RetrievedData<TTransform> RetrieveUsingPaging<TTransform>(IQueryable<T> queryable, int startIndex, int maxRows, bool hasOrderBy = false) where TTransform : class
        {
            return RetrieveUsingPaging<TTransform>(queryable, startIndex, maxRows, hasOrderBy);

        }

        public Task<RetrievedData<TTransform>> RetrieveUsingPagingAsync<TTransform>(IQueryable<T> queryable, int startIndex, int maxRows, bool hasOrderBy = false, CancellationToken token = default(CancellationToken)) where TTransform : class
        {
            return RetrieveUsingPagingAsync<TTransform>(queryable, startIndex, maxRows, hasOrderBy, token);
        }
    }
}
