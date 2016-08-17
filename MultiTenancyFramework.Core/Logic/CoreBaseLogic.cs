using MultiTenancyFramework.Data;
using MultiTenancyFramework.Entities;
using System;
using System.Collections.Generic;

namespace MultiTenancyFramework.Logic
{
    public abstract class CoreBaseLogic<T> : CoreBaseLogic<T, long> where T : class, IEntity<long>
    {
        /// <summary>
        /// Use this to point to entities that are hosted centrally
        /// </summary>
        /// <param name="dao">The data access object. Set by calling <code>MyServiceLocator.GetInstance<ICoreDAO<T>>();</code></param>
        protected CoreBaseLogic(ICoreDAO<T> dao) : base(dao)
        {

        }

        /// <summary>
        /// Use this to point to entities that might be institution-specific.
        /// </summary>
        /// <param name="dao">The data access object. Set by calling <code>MyServiceLocator.GetInstance<ICoreDAO<T>>();</code></param>
        /// <param name="institutionCode"></param>
        protected CoreBaseLogic(ICoreDAO<T> dao, string institutionCode) : base(dao, institutionCode)
        {

        }
    }

    public abstract class CoreBaseLogic<T, idT> where T : class, IEntity<idT> where idT : IEquatable<idT>
    {
        protected ICoreDAO<T, idT> _dao;

        /// <summary>
        /// Use this to point to entities that are hosted centrally
        /// </summary>
        /// <param name="dao">The data access object. Set by calling <code>MyServiceLocator.GetInstance<ICoreDAO<T, idT>>();</code></param>
        protected CoreBaseLogic(ICoreDAO<T, idT> dao) : this(dao, "")
        {

        }

        /// <summary>
        /// Use this to point to entities that might be institution-specific.
        /// </summary>
        /// <param name="dao">The data access object. Set by calling <code>MyServiceLocator.GetInstance<ICoreDAO<T, idT>>();</code></param>
        /// <param name="institutionCode"></param>
        protected CoreBaseLogic(ICoreDAO<T, idT> dao, string institutionCode)
        {
            _dao = dao;
            _dao.InstitutionCode = InstitutionCode = _institutionCode;

            //Because this may have been preset in the incoming 'dao' instance, we do...
            EntityName = _dao.EntityName;
        }

        #region Events
        public Action<T> BeforeDeleting;
        public virtual void OnBeforeDeleting(T e)
        {
            BeforeDeleting?.Invoke(e);
        }

        public Action<T> BeforeSaving;
        public virtual void OnBeforeSaving(T e)
        {
            BeforeSaving?.Invoke(e);
        }

        public Action<IList<T>> BeforeSavingList;
        public virtual void OnBeforeSavingList(IList<T> e)
        {
            BeforeSavingList?.Invoke(e);
        }

        public Action<T> BeforeUpdating;
        public virtual void OnBeforeUpdating(T e)
        {
            BeforeUpdating?.Invoke(e);
        }

        public Action<IList<T>> BeforeUpdatingList;
        public virtual void OnBeforeUpdatingList(IList<T> e)
        {
            BeforeUpdatingList?.Invoke(e);
        }

        public Action<T> BeforeCommittingChanges;
        public virtual void OnBeforeCommittingChanges(T e)
        {
            BeforeCommittingChanges?.Invoke(e);
        }

        public Action<IList<T>> BeforeCommittingListChanges;
        public virtual void OnBeforeCommittingListChanges(IList<T> e)
        {
            BeforeCommittingListChanges?.Invoke(e);
        }

        public Action<T> AfterCommittingChanges;
        public virtual void OnAfterCommittingChanges(T e)
        {
            AfterCommittingChanges?.Invoke(e);
        }

        public Action<IList<T>> AfterCommittingListChanges;
        public virtual void OnAfterCommittingListChanges(IList<T> e)
        {
            AfterCommittingListChanges?.Invoke(e);
        }
        #endregion

        private string _institutionCode;
        /// <summary>
        /// To support multi-tenancy. 
        /// </summary>
        public virtual string InstitutionCode
        {
            get
            {
                return _institutionCode;
            }
            set
            {
                if (value == null || value == Utilities.INST_DEFAULT_CODE) value = string.Empty;
                _institutionCode = value;
            }
        }

        /// <summary>
        /// Repreasents some variations within an entity that may need to be supported
        /// </summary>
        public virtual string EntityName { get; set; }

        /// <summary>
        /// Retrieves the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public virtual T Retrieve(idT id)
        {
            _dao.InstitutionCode = _institutionCode;
            _dao.EntityName = EntityName;
            return _dao.Retrieve(id);
        }

        /// <summary>
        /// Saves the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public T Insert(T entity)
        {
            _dao.InstitutionCode = _institutionCode;
            _dao.EntityName = EntityName;
            OnBeforeSaving(entity);
            try
            {
                _dao.Save(entity);
                OnBeforeCommittingChanges(entity);
                _dao.CommitChanges();
            }
            catch (Exception)
            {
                _dao.RollbackChanges();
                throw;
            }
            OnAfterCommittingChanges(entity);
            return entity;
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public T Update(T entity)
        {
            _dao.InstitutionCode = _institutionCode;
            _dao.EntityName = EntityName;
            OnBeforeUpdating(entity);
            try
            {
                _dao.Update(entity);
                OnBeforeCommittingChanges(entity);
                _dao.CommitChanges();
            }
            catch (Exception)
            {
                _dao.RollbackChanges();
                throw;
            }
            OnAfterCommittingChanges(entity);
            return entity;
        }

        /// <summary>
        /// Merges the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public T Merge(T entity)
        {
            _dao.InstitutionCode = _institutionCode;
            _dao.EntityName = EntityName;
            OnBeforeUpdating(entity);
            try
            {
                _dao.Merge(entity);
                OnBeforeCommittingChanges(entity);
                _dao.CommitChanges();
            }
            catch (Exception)
            {
                _dao.RollbackChanges();
                throw;
            }
            OnAfterCommittingChanges(entity);
            return entity;
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Delete(T entity)
        {
            _dao.InstitutionCode = _institutionCode;
            _dao.EntityName = EntityName;
            OnBeforeDeleting(entity);
            try
            {
                _dao.Delete(entity);
                OnBeforeCommittingChanges(entity);
                _dao.CommitChanges();
            }
            catch (Exception)
            {
                _dao.RollbackChanges();
                throw;
            }
            OnAfterCommittingChanges(entity);
        }

        /// <summary>
        /// Refreshes the specified entity. Usually used for approval purposes
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Refresh(T entity)
        {
            _dao.InstitutionCode = _institutionCode;
            _dao.EntityName = EntityName;
            _dao.Refresh(entity);
        }

    }
}
