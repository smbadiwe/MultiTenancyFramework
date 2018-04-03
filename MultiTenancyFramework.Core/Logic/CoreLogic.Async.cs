using MultiTenancyFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MultiTenancyFramework.Logic
{
    /// <summary>
    /// This aggregates all relevant interfaces and is fine for most cases. However, if you think it's too bloated for your entity,
    /// just implement the interfaces that suits yoou best.
    /// Hint: You might want to use this as a private member in your method so as to aid reuse.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="idT"></typeparam>
    public abstract partial class CoreLogic<T, idT> : CoreBaseLogic<T, idT> where T : class, IBaseEntity<idT> where idT : IEquatable<idT>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="fields">fields you're interested in getting their field values. If null, all fields are selected</param>
        /// <returns></returns>
        public virtual async Task<Dictionary<idT, T>> RetrieveByIDsAsDictionaryAsync(idT[] IDs, string[] fields, CancellationToken token = default(CancellationToken))
        {
            var list = await RetrieveByIDsAsync(IDs, fields, token);
            return list?.ToDictionary(x => x.Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="fields">fields you're interested in getting their field values. If null, all fields are selected</param>
        /// <returns></returns>
        public virtual async Task<IList<T>> RetrieveByIDsAsync(idT[] IDs, string[] fields, CancellationToken token = default(CancellationToken))
        {
            _dao.InstitutionCode = InstitutionCode;
            _dao.EntityName = EntityName;
            return await _dao.RetrieveByIDsAsync(IDs, fields, token);
        }

        public virtual async Task<IList<idT>> RetrieveIDsAsync(CancellationToken token = default(CancellationToken))
        {
            _dao.InstitutionCode = InstitutionCode;
            _dao.EntityName = EntityName;
            return await _dao.RetrieveIDsAsync(token);
        }

        /// <summary>
        /// Retrieves all.
        /// </summary>
        /// <param name="fields">fields you're interested in getting their field values. If null, all fields are selected</param>
        /// <returns></returns>
        public virtual async Task<IList<T>> RetrieveAllAsync(string[] fields, CancellationToken token = default(CancellationToken))
        {
            _dao.InstitutionCode = InstitutionCode;
            _dao.EntityName = EntityName;
            return await _dao.RetrieveAllAsync(fields, token);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields">fields you're interested in getting their field values. If null, all fields are selected</param>
        /// <returns></returns>
        public virtual async Task<IList<T>> RetrieveAllActiveAsync(string[] fields, CancellationToken token = default(CancellationToken))
        {
            _dao.InstitutionCode = InstitutionCode;
            _dao.EntityName = EntityName;
            return await _dao.RetrieveAllActiveAsync(fields, token);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields">fields you're interested in getting their field values. If null, all fields are selected</param>
        /// <returns></returns>
        public virtual async Task<IList<T>> RetrieveAllInactiveAsync(string[] fields, CancellationToken token = default(CancellationToken))
        {
            _dao.InstitutionCode = InstitutionCode;
            _dao.EntityName = EntityName;
            return await _dao.RetrieveAllInactiveAsync(fields, token);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields">fields you're interested in getting their field values. If null, all fields are selected</param>
        /// <returns></returns>
        public virtual async Task<IList<T>> RetrieveAllDeletedAsync(string[] fields, CancellationToken token = default(CancellationToken))
        {
            _dao.InstitutionCode = InstitutionCode;
            _dao.EntityName = EntityName;
            return await _dao.RetrieveAllDeletedAsync(fields, token);
        }

        /// <summary>
        /// Inserts list to database. It does nothing if <paramref name="entities"/> is null or empty
        /// </summary>
        /// <param name="entities"></param>
        public async Task InsertAsync(IList<T> entities, CancellationToken token = default(CancellationToken))
        {
            if (entities != null && entities.Count > 0)
            {
                _dao.InstitutionCode = InstitutionCode;
                _dao.EntityName = EntityName;
                OnBeforeSavingList(entities);
                try
                {
                    foreach (var entity in entities)
                    {
                        _dao.Save(entity);
                    }
                    OnBeforeCommittingListChanges(entities);
                    await _dao.CommitChangesAsync(token);
                }
                catch (Exception)
                {
                    await _dao.RollbackChangesAsync(token);
                    throw;
                }
                OnAfterCommittingListChanges(entities);
            }
        }

        /// <summary>
        /// Updates records in database. It does nothing if <paramref name="entities"/> is null or empty
        /// </summary>
        /// <param name="entities"></param>
        public async Task UpdateAsync(IList<T> entities, CancellationToken token = default(CancellationToken))
        {
            if (entities != null && entities.Count > 0)
            {
                _dao.InstitutionCode = InstitutionCode;
                _dao.EntityName = EntityName;
                OnBeforeUpdatingList(entities);
                try
                {
                    foreach (var entity in entities)
                    {
                        _dao.Update(entity);
                    }
                    OnBeforeCommittingListChanges(entities);
                    await _dao.CommitChangesAsync(token);
                }
                catch (Exception)
                {
                    await _dao.RollbackChangesAsync(token);
                    throw;
                }
                OnAfterCommittingListChanges(entities);
            }
        }

        /// <summary>
        /// Updates records in database. It does nothing if <paramref name="entities"/> is null or empty
        /// </summary>
        /// <param name="entities"></param>
        public async Task MergeAsync(IList<T> entities, CancellationToken token = default(CancellationToken))
        {
            if (entities != null && entities.Count > 0)
            {
                _dao.InstitutionCode = InstitutionCode;
                _dao.EntityName = EntityName;
                OnBeforeUpdatingList(entities);
                try
                {
                    foreach (var entity in entities)
                    {
                        _dao.Merge(entity);
                    }
                    OnBeforeCommittingListChanges(entities);
                    await _dao.CommitChangesAsync(token);
                }
                catch (Exception)
                {
                    await _dao.RollbackChangesAsync(token);
                    throw;
                }
                OnAfterCommittingListChanges(entities);
            }
        }

        /// <summary>
        /// Saves the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public async Task<T> InsertAsync(T entity, CancellationToken token = default(CancellationToken))
        {
            _dao.InstitutionCode = InstitutionCode;
            _dao.EntityName = EntityName;
            OnBeforeSaving(entity);
            try
            {
                _dao.Save(entity);
                OnBeforeCommittingChanges(entity);
                await _dao.CommitChangesAsync(token);
            }
            catch (Exception)
            {
                await _dao.RollbackChangesAsync(token);
                throw;
            }
            OnAfterCommittingChanges(entity);
            return entity;
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public async Task<T> UpdateAsync(T entity, CancellationToken token = default(CancellationToken))
        {
            _dao.InstitutionCode = InstitutionCode;
            _dao.EntityName = EntityName;
            OnBeforeUpdating(entity);
            try
            {
                _dao.Update(entity);
                OnBeforeCommittingChanges(entity);
                await _dao.CommitChangesAsync(token);
            }
            catch (Exception)
            {
                await _dao.RollbackChangesAsync(token);
                throw;
            }
            OnAfterCommittingChanges(entity);
            return entity;
        }

        /// <summary>
        /// Merges the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public async Task<T> MergeAsync(T entity, CancellationToken token = default(CancellationToken))
        {
            _dao.InstitutionCode = InstitutionCode;
            _dao.EntityName = EntityName;
            OnBeforeUpdating(entity);
            try
            {
                _dao.Merge(entity);
                OnBeforeCommittingChanges(entity);
                await _dao.CommitChangesAsync(token);
            }
            catch (Exception)
            {
                await _dao.RollbackChangesAsync(token);
                throw;
            }
            OnAfterCommittingChanges(entity);
            return entity;
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public async Task DeleteAsync(T entity, CancellationToken token = default(CancellationToken))
        {
            _dao.InstitutionCode = InstitutionCode;
            _dao.EntityName = EntityName;
            OnBeforeDeleting(entity);
            try
            {
                _dao.Delete(entity);
                OnBeforeCommittingChanges(entity);
                await _dao.CommitChangesAsync(token);
            }
            catch (Exception)
            {
                await _dao.RollbackChangesAsync(token);
                throw;
            }
            OnAfterCommittingChanges(entity);
        }

    }
}
