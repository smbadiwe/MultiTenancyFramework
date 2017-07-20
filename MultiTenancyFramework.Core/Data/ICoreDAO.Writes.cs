using MultiTenancyFramework.Entities;
using System;

namespace MultiTenancyFramework.Data
{
    public interface ICoreWritesDAO<T, idT> : ICoreGeneralDAO, ICoreUnitOfWorkDAO where T : IBaseEntity<idT> where idT : IEquatable<idT>
    {
        void Refresh(T obj);

        /// <summary>
        /// Updates the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        void Update(T obj);

        /// <summary>
        /// Saves the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        void Save(T obj);

        void SaveOrUpdate(T obj);

        /// <summary>
        /// Deletes the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        void Delete(T obj);

        /// <summary>
        /// Actually deletes the specified obj from the db.
        /// </summary>
        /// <param name="obj">The obj.</param>
        void TakeOutPermanently(T obj);

        T Merge(T entity);

        void Evict(T entity);

    }

    public interface ICoreWritesDAO<T> : ICoreWritesDAO<T, long> where T : IEntity<long>
    {

    }
}
