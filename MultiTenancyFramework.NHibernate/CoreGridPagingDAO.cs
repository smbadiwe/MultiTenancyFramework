using MultiTenancyFramework.Data;
using MultiTenancyFramework.Entities;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiTenancyFramework.NHibernate
{
    public abstract class CoreGridPagingDAO<T, idT> : CoreGeneralDAO where T : class, IEntity<idT> where idT : IEquatable<idT>
    {
        public IEnumerable<T> RetrieveAllNoPaging<T>(IQueryOver<T, T> theQueryOver, bool hasOrderBy = false)
            where T : class, IEntity<idT>
        {
            IEnumerable<T> result;
            int totalCount = 0;
            IFutureValue<int> futureCount = null;
            if (hasOrderBy)
            {
                totalCount = theQueryOver.RowCount();
                result = theQueryOver.Future<T>();
            }
            else
            {
                futureCount = theQueryOver.Clone().Select(Projections.RowCount()).FutureValue<int>();
                result = theQueryOver.Future<T>();
            }
            var toReturn = result.ToList();
            if (futureCount != null) totalCount = futureCount.Value;
            return toReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="theQueryOver"></param>
        /// <param name="startIndex"></param>
        /// <param name="maxRows">A value of Zero means we're not paging</param>
        /// <param name="hasOrderBy"></param>
        /// <returns></returns>
        public RetrievedData<T> RetrieveUsingPaging<T>(IQueryOver<T, T> theQueryOver, int startIndex, int maxRows, bool hasOrderBy = false)
            where T : class, IEntity<idT>
        {
            IEnumerable<T> result;
            int totalCount = 0;
            IFutureValue<int> futureCount = null;
            if (maxRows > 0)
            {
                if (hasOrderBy)
                {
                    totalCount = theQueryOver.RowCount();
                    result = theQueryOver.Skip(startIndex * maxRows).Take(maxRows).Future<T>();
                }
                else
                {
                    futureCount = theQueryOver.Clone().Select(Projections.RowCount()).FutureValue<int>();
                    result = theQueryOver.Skip(startIndex * maxRows).Take(maxRows).Future<T>();
                }
            }
            else //get all
            {
                result = theQueryOver.Future<T>();
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
            return new RetrievedData<T>
            {
                DataBatch = toReturn,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// Use this API it you did an aliastobean transform to type TTransform
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TTransform"></typeparam>
        /// <param name="theQueryOver"></param>
        /// <param name="startIndex"></param>
        /// <param name="maxRows">A value of Zero means we're not paging</param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public RetrievedData<TTransform> RetrieveUsingPaging<T, TTransform>(IQueryOver<T, T> theQueryOver, int startIndex, int maxRows, bool hasOrderBy = false)
            where T : class, IEntity<idT> where TTransform : class, IEntity<idT>
        {
            IEnumerable<TTransform> result;
            int totalCount = 0;
            IFutureValue<int> futureCount = null;
            if (maxRows > 0)
            {
                if (hasOrderBy)
                {
                    totalCount = theQueryOver.RowCount();
                    result = theQueryOver.Skip(startIndex * maxRows).Take(maxRows).Future<TTransform>();
                }
                else
                {
                    futureCount = theQueryOver.Clone().Select(Projections.RowCount()).FutureValue<int>();
                    result = theQueryOver.Skip(startIndex * maxRows).Take(maxRows).Future<TTransform>();
                }
            }
            else //get all
            {
                result = theQueryOver.Future<TTransform>();
            }
            var toReturn = result.ToList();
            if (futureCount != null) totalCount = futureCount.Value;
            return new RetrievedData<TTransform>
            {
                DataBatch = toReturn,
                TotalCount = totalCount
            };
        }

    }
}
