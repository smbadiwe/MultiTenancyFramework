using MultiTenancyFramework.Data;
using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiTenancyFramework.Logic
{

    /// <summary>
    /// This aggregates all relevant interfaces and is fine for most cases. However, if you think it's too bloated for your entity,
    /// just implement the interfaces that suits yoou best.
    /// Hint: You might want to use this as a private member in your method so as to aid reuse.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CoreLogic<T> : CoreLogic<T, long> where T : class, IBaseEntity<long>
    {
        /// <summary>
        /// Use this to point to entities that are hosted centrally
        /// </summary>
        /// <param name="dao">The data access object. Set by calling <code>MyServiceLocator.GetInstance<ICoreDAO<T>>();</code></param>
        protected CoreLogic(ICoreDAO<T> dao) : base(dao)
        {

        }

        /// <summary>
        /// Use this to point to entities that might be institution-specific.
        /// </summary>
        /// <param name="dao">The data access object. Set by calling <code>MyServiceLocator.GetInstance<ICoreDAO<T>>();</code></param>
        /// <param name="institutionCode"></param>
        protected CoreLogic(ICoreDAO<T> dao, string institutionCode) : base(dao, institutionCode)
        {

        }

    }

    /// <summary>
    /// This aggregates all relevant interfaces and is fine for most cases. However, if you think it's too bloated for your entity,
    /// just implement the interfaces that suits yoou best.
    /// Hint: You might want to use this as a private member in your method so as to aid reuse.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="idT"></typeparam>
    public abstract class CoreLogic<T, idT> : CoreBaseLogic<T, idT> where T : class, IBaseEntity<idT> where idT : IEquatable<idT>
    {
        /// <summary>
        /// Use this to point to entities that are hosted centrally
        /// </summary>
        /// <param name="dao">The data access object. Set by calling <code>MyServiceLocator.GetInstance<ICoreDAO<T, idT>>();</code></param>
        protected CoreLogic(ICoreDAO<T, idT> dao) : base(dao)
        {

        }

        /// <summary>
        /// Use this to point to entities that might be institution-specific.
        /// </summary>
        /// <param name="dao">The data access object. Set by calling <code>MyServiceLocator.GetInstance<ICoreDAO<T, idT>>();</code></param>
        /// <param name="institutionCode"></param>
        protected CoreLogic(ICoreDAO<T, idT> dao, string institutionCode) : base(dao, institutionCode)
        {

        }

        private IDbQueryProcessor _queryProcessor;

        public IDbQueryProcessor QueryProcessor
        {
            get
            {
                if (_queryProcessor == null) _queryProcessor = Utilities.QueryProcessor;

                _queryProcessor.InstitutionCode = InstitutionCode;
                return _queryProcessor;
            }
        }

        public DateTime Now()
        {
            return DateTime.Now.GetLocalTime();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="fields">fields you're interested in getting their field values. If null, all fields are selected</param>
        /// <returns></returns>
        public virtual Dictionary<idT, T> RetrieveByIDsAsDictionary(idT[] IDs, params string[] fields)
        {
            return RetrieveByIDs(IDs, fields).ToDictionary(x => x.Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="fields">fields you're interested in getting their field values. If null, all fields are selected</param>
        /// <returns></returns>
        public virtual IList<T> RetrieveByIDs(idT[] IDs, params string[] fields)
        {
            _dao.InstitutionCode = InstitutionCode;
            _dao.EntityName = EntityName;
            return _dao.RetrieveByIDs(IDs, fields);
        }

        public virtual IList<idT> RetrieveIDs()
        {
            _dao.InstitutionCode = InstitutionCode;
            _dao.EntityName = EntityName;
            return _dao.RetrieveIDs();
        }

        /// <summary>
        /// Retrieves all.
        /// </summary>
        /// <param name="fields">fields you're interested in getting their field values. If null, all fields are selected</param>
        /// <returns></returns>
        public virtual IList<T> RetrieveAll(params string[] fields)
        {
            _dao.InstitutionCode = InstitutionCode;
            _dao.EntityName = EntityName;
            return _dao.RetrieveAll(fields);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields">fields you're interested in getting their field values. If null, all fields are selected</param>
        /// <returns></returns>
        public virtual IList<T> RetrieveAllActive(params string[] fields)
        {
            _dao.InstitutionCode = InstitutionCode;
            _dao.EntityName = EntityName;
            return _dao.RetrieveAllActive(fields);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields">fields you're interested in getting their field values. If null, all fields are selected</param>
        /// <returns></returns>
        public virtual IList<T> RetrieveAllInactive(params string[] fields)
        {
            _dao.InstitutionCode = InstitutionCode;
            _dao.EntityName = EntityName;
            return _dao.RetrieveAllInactive(fields);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields">fields you're interested in getting their field values. If null, all fields are selected</param>
        /// <returns></returns>
        public virtual IList<T> RetrieveAllDeleted(params string[] fields)
        {
            _dao.InstitutionCode = InstitutionCode;
            _dao.EntityName = EntityName;
            return _dao.RetrieveAllDeleted(fields);
        }

        /// <summary>
        /// Inserts list to database. It does nothing if <paramref name="entities"/> is null or empty
        /// </summary>
        /// <param name="entities"></param>
        public void Insert(IList<T> entities)
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
                    _dao.CommitChanges();
                }
                catch (Exception)
                {
                    _dao.RollbackChanges();
                    throw;
                }
                OnAfterCommittingListChanges(entities); 
            }
        }

        /// <summary>
        /// Updates records in database. It does nothing if <paramref name="entities"/> is null or empty
        /// </summary>
        /// <param name="entities"></param>
        public void Update(IList<T> entities)
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
                    _dao.CommitChanges();
                }
                catch (Exception)
                {
                    _dao.RollbackChanges();
                    throw;
                }
                OnAfterCommittingListChanges(entities); 
            }
        }

        /// <summary>
        /// Updates records in database. It does nothing if <paramref name="entities"/> is null or empty
        /// </summary>
        /// <param name="entities"></param>
        public void Merge(IList<T> entities)
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
                    _dao.CommitChanges();
                }
                catch (Exception)
                {
                    _dao.RollbackChanges();
                    throw;
                }
                OnAfterCommittingListChanges(entities); 
            }
        }

        /// <summary>
        /// Use this to insert bulk data into the db. A typical case is when you're uploading thousands of data.
        /// NB: This does nothing if <paramref name="entities"/> is null or empty.
        /// </summary>
        /// <param name="entities"></param>
        public void SqlBulkInsert(IList<T> entities)
        {
            if (entities != null && entities.Count > 0)
            {
                _dao.InstitutionCode = InstitutionCode;
                _dao.EntityName = EntityName;
                OnBeforeSavingList(entities);
                OnBeforeCommittingListChanges(entities);
                _dao.SqlBulkInsert(entities);
                OnAfterCommittingListChanges(entities); 
            }
        }
    }
}
