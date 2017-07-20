using MultiTenancyFramework.Entities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MultiTenancyFramework.Data
{
    public interface ICoreReadsDAO<T, idT> : ICoreGeneralDAO where T : IBaseEntity<idT> where idT : IEquatable<idT>
    {
        IList<idT> RetrieveIDs();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="fields">fields you're interested in getting their field values. If null, all fields are selected</param>
        /// <returns></returns>
        IList<T> RetrieveByIDs(idT[] IDs, params string[] fields);

        /// <summary>
        /// This one is when you need it as List of T.
        /// <para>Be sure to send in parametized query, else you're on your own!</para>
        /// </summary>
        /// <param name="query">WARNING: Be sure to send in parametized query, else you're on your own!</param>
        /// <returns></returns>
        IList<U> RetrieveUsingDirectQuery<U>(string query, bool clearSession = false) where U : class, IBaseEntity<idT>;

        /// <summary>
        /// This one is only for when an IList will do.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IList RetrieveUsingDirectQuery2(string query, bool clearSession = false);

        /// <summary>
        /// Retrieves all.
        /// </summary>
        /// <param name="fields">fields you're interested in getting their field values. If null, all fields are selected</param>
        /// <returns></returns>
        IList<T> RetrieveAll(params string[] fields);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields">fields you're interested in getting their field values. If null, all fields are selected</param>
        /// <returns></returns>
        IList<T> RetrieveAllActive(params string[] fields);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields">fields you're interested in getting their field values. If null, all fields are selected</param>
        /// <returns></returns>
        IList<T> RetrieveAllInactive(params string[] fields);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields">fields you're interested in getting their field values. If null, all fields are selected</param>
        /// <returns></returns>
        IList<T> RetrieveAllDeleted(params string[] fields);

        /// <summary>
        /// Retrieve the first item found inthe db. This is useful for tables expected to have just one enty
        /// </summary>
        /// <returns></returns>
        T RetrieveOne();

        /// <summary>
        /// Retrieves the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        T Retrieve(idT id);

        /// <summary>
        /// Retrieves the specified id. NB: Use this only when you're sure the 
        /// T with the id exists; otherwise, use .Retrieve
        /// Use this in cases when you only need the id of T to persist another entity that
        /// references T.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        T Load(idT id);

    }

    public interface ICoreReadsDAO<T> : ICoreReadsDAO<T, long> where T : IEntity<long>
    {

    }
}
