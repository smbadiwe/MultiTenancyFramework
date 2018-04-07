using MultiTenancyFramework.Entities;
using System;

namespace MultiTenancyFramework.Data
{
    /// <summary>
    /// This holds the minimum data requirements for each entity. It does not include list retrievals and SQL bulk inserts
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="idT"></typeparam>
    public interface ICoreDAO<T, idT> : ICoreReadsDAO<T, idT>, ICoreGridPagingDAO<T, idT>, ICoreBulkInsertDAO<T, idT>, ICoreWritesDAO<T, idT> where T : class, IBaseEntity<idT> where idT : IEquatable<idT>
    {

    }

    /// <summary>
    /// This holds the minimum data requirements for each entity. It does not include list retrievals and SQL bulk inserts
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICoreDAO<T> : ICoreDAO<T, long> where T : class, IBaseEntity<long>
    {

    }
}
