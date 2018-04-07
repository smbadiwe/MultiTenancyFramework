using MultiTenancyFramework.Entities;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace MultiTenancyFramework.Data
{
    public partial interface ICoreReadsDAO<T, idT> : ICoreGeneralDAO where T : IBaseEntity<idT> where idT : IEquatable<idT>
    {
        IQueryable<T> Table { get; }

        Task<IList<idT>> RetrieveIDsAsync(CancellationToken token = default(CancellationToken));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="fields">fields you're interested in getting their field values. If null, all fields are selected</param>
        /// <returns></returns>
        Task<IList<T>> RetrieveByIDsAsync(idT[] IDs, string[] fields = null, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Retrieves all.
        /// </summary>
        /// <param name="fields">fields you're interested in getting their field values. If null, all fields are selected</param>
        /// <returns></returns>
        Task<IList<T>> RetrieveAllAsync(string[] fields = null, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields">fields you're interested in getting their field values. If null, all fields are selected</param>
        /// <returns></returns>
        Task<IList<T>> RetrieveAllActiveAsync(string[] fields = null, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields">fields you're interested in getting their field values. If null, all fields are selected</param>
        /// <returns></returns>
        Task<IList<T>> RetrieveAllInactiveAsync(string[] fields = null, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields">fields you're interested in getting their field values. If null, all fields are selected</param>
        /// <returns></returns>
        Task<IList<T>> RetrieveAllDeletedAsync(string[] fields = null, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Retrieve the first item found inthe db. This is useful for tables expected to have just one enty
        /// </summary>
        /// <returns></returns>
        Task<T> RetrieveOneAsync(CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Retrieves the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        Task<T> RetrieveAsync(idT id, CancellationToken token = default(CancellationToken));
        
        /// <summary>
        /// Retrieves the specified id. NB: Use this only when you're sure the 
        /// T with the id exists; otherwise, use .Retrieve
        /// Use this in cases when you only need the id of T to persist another entity that
        /// references T.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        Task<T> LoadAsync(idT id, CancellationToken token = default(CancellationToken));
    }

    public partial interface ICoreReadsDAO<T> : ICoreReadsDAO<T, long> where T : IEntity<long>
    {

    }
}
