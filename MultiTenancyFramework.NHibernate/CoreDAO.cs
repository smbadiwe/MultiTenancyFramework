using MultiTenancyFramework.Data;
using MultiTenancyFramework.Entities;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace MultiTenancyFramework.NHibernate
{

    /// <summary>
    /// This holds the minimum data requirements for each entity. It does not include list retrievals and SQL bulk inserts
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CoreDAO<T> : CoreDAO<T, long>, ICoreDAO<T> where T : class, IEntity<long>
    {

    }

    /// <summary>
    /// This holds the minimum data requirements for each entity. It does not include list retrievals and SQL bulk inserts
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="idT"></typeparam>
    public class CoreDAO<T, idT> : CoreGridPagingDAO<T, idT>, ICoreDAO<T, idT> where T : class, IEntity<idT> where idT : IEquatable<idT>
    {
        public void CommitChanges()
        {
            var session = BuildSession();
            if (session.IsConnected && session.Transaction != null && session.Transaction.IsActive && !session.Transaction.WasCommitted)
            {
                session.Transaction.Commit();
            }
        }

        public void RollbackChanges()
        {
            var session = BuildSession();
            if (session.Transaction != null && session.IsConnected && session.Transaction.IsActive && !session.Transaction.WasCommitted && !session.Transaction.WasRolledBack)
            {
                session.Transaction.Rollback();
            }
        }

        public void Save(T obj)
        {
            var session = BuildSession();
            if (string.IsNullOrWhiteSpace(InstitutionCode)) obj.InstitutionCode = string.Empty;
            if (string.IsNullOrWhiteSpace(EntityName))
            {
                session.Save(obj);
            }
            else
            {
                session.Save(EntityName, obj);
            }
        }

        public void SaveOrUpdate(T obj)
        {
            //WARNING: session.SaveOrUpdate does not behave properly, and AuditLog listener has not been wired to it
            if (obj.Id == null || obj.Id.Equals(default(idT)))
            {
                Save(obj);
            }
            else
            {
                Update(obj);
            }
        }

        public void TakeOutPermanently(T obj)
        {
            Delete(obj);
        }

        public void Update(T obj)
        {
            // I use session.Merge here instead of session.Update for two reasons:
            // 1. It will throw error if for any reason you try to Update an item retrieved using a different session.
            // 2. It produces inconsistent auditlog.
            //
            // So, if you ever decide to bring back .Update, fix the above two issues first.
            //
            // - MultiTenancyFramework
            var session = BuildSession();
            if (string.IsNullOrWhiteSpace(InstitutionCode)) obj.InstitutionCode = string.Empty;
            obj.LastDateModified = DateTime.Now.GetLocalTime();
            if (string.IsNullOrWhiteSpace(EntityName))
            {
                session.Merge(obj);
            }
            else
            {
                session.Merge(EntityName, obj);
            }
        }

        public void Delete(T obj)
        {
            var session = BuildSession();
            if (string.IsNullOrWhiteSpace(InstitutionCode)) obj.InstitutionCode = string.Empty;
            obj.LastDateModified = DateTime.Now.GetLocalTime();
            if (string.IsNullOrWhiteSpace(EntityName))
            {
                session.Delete(obj);
            }
            else
            {
                session.Delete(EntityName, obj);
            }
        }

        public void Evict(T entity)
        {
            var session = BuildSession();
            if (string.IsNullOrWhiteSpace(InstitutionCode)) entity.InstitutionCode = string.Empty;
            session.Evict(entity);
        }

        public T Merge(T entity)
        {
            var session = BuildSession();
            if (string.IsNullOrWhiteSpace(InstitutionCode)) entity.InstitutionCode = string.Empty;
            entity.LastDateModified = DateTime.Now.GetLocalTime();
            if (string.IsNullOrWhiteSpace(EntityName))
            {
                session.Merge(entity);
            }
            else
            {
                session.Merge(EntityName, entity);
            }
            return entity;
        }

        public void Refresh(T obj)
        {
            var session = BuildSession();
            if (string.IsNullOrWhiteSpace(InstitutionCode)) obj.InstitutionCode = string.Empty;
            session.Refresh(obj);
        }

        public T Load(idT id)
        {
            var session = BuildSession();
            if (string.IsNullOrWhiteSpace(EntityName))
            {
                return session.Load<T>(id);
            }
            return session.Load(EntityName, id) as T;
        }

        public T Retrieve(idT id)
        {
            var session = BuildSession();
            if (string.IsNullOrWhiteSpace(EntityName))
            {
                return session.Get<T>(id);
            }
            return session.Get(EntityName, id) as T;
        }

        /// <summary>
        /// Retrieve the first item found inthe db. This is useful for tables expected to have just one enty
        /// </summary>
        /// <returns></returns>
        public T RetrieveOne()
        {
            var session = BuildSession();
            IQueryOver<T, T> query;
            if (string.IsNullOrWhiteSpace(EntityName))
            {
                query = session.QueryOver<T>();
            }
            else
            {
                query = session.QueryOver<T>(EntityName);
            }
            return query.Take(1).SingleOrDefault();
        }

        public IList<T> RetrieveAll()
        {
            var session = BuildSession();
            if (string.IsNullOrWhiteSpace(EntityName))
            {
                return session.QueryOver<T>().List();
            }
            return session.QueryOver<T>(EntityName).List();
        }

        public IList<T> RetrieveAllActive()
        {
            var session = BuildSession();
            if (string.IsNullOrWhiteSpace(EntityName))
            {
                return session.QueryOver<T>().Where(x => !x.IsDisabled).List();
            }
            return session.QueryOver<T>(EntityName).Where(x => !x.IsDisabled).List();
        }

        public IList<T> RetrieveAllDeleted()
        {
            var session = BuildSession();
            if (string.IsNullOrWhiteSpace(EntityName))
            {
                return session.QueryOver<T>().Where(x => !x.IsDeleted).List();
            }
            return session.QueryOver<T>(EntityName).Where(x => x.IsDeleted).List();
        }

        public IList<T> RetrieveAllInactive()
        {
            var session = BuildSession();
            if (string.IsNullOrWhiteSpace(EntityName))
            {
                return session.QueryOver<T>().Where(x => x.IsDisabled).List();
            }
            return session.QueryOver<T>(EntityName).Where(x => x.IsDisabled).List();
        }

        public IList<T> RetrieveByIDs(idT[] IDs)
        {
            var session = BuildSession();
            if (string.IsNullOrWhiteSpace(EntityName))
            {
                return session.QueryOver<T>().Where(x => x.Id.IsIn(IDs)).List();
            }
            return session.QueryOver<T>(EntityName).Where(x => x.Id.IsIn(IDs)).List();
        }

        public IList<idT> RetrieveIDs()
        {
            var session = BuildSession();
            if (string.IsNullOrWhiteSpace(EntityName))
            {
                return session.QueryOver<T>().Select(x => x.Id).List<idT>();
            }
            return session.QueryOver<T>(EntityName).Select(x => x.Id).List<idT>();
        }

        public IList<U> RetrieveUsingDirectQuery<U>(string query, bool clearSession = false) where U : class, IEntity<idT>
        {
            var session = BuildSession();
            return session.CreateSQLQuery(query).List<U>();
        }

        /// <summary>
        /// This one is only for when an IList will do.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IList RetrieveUsingDirectQuery2(string query, bool clearSession = false)
        {
            var session = BuildSession();
            return session.CreateSQLQuery(query).List();
        }

        public void SqlBulkInsert(IList<T> items, bool isDataMigration = false, string schema = "dbo")
        {
            SqlBulkInsert(items, string.Empty, isDataMigration, schema);
        }

        public void SqlBulkInsert(IList<T> items, string tableName, bool isDataMigration = false, string schema = "dbo")
        {
            var session = BuildSession();
            SqlBulkInsert(items, session.Connection, tableName, isDataMigration, schema);
        }

        public void SqlBulkInsert(IList<T> items, IDbConnection connection, string tableName, bool isDataMigration = false, string schema = "dbo")
        {
            if (string.IsNullOrWhiteSpace(tableName)) tableName = typeof(T).GetTableName();
            SqlManipulations.SqlBulkInsert<T, idT>(items, connection, tableName, EntityName, isDataMigration, schema);
        }

        public void SqlBulkInsert(Type TType, IList items, bool isDataMigration = false, string schema = "dbo")
        {
            SqlBulkInsert(TType, items, string.Empty, isDataMigration, schema);
        }

        public void SqlBulkInsert(Type TType, IList items, string tableName, bool isDataMigration = false, string schema = "dbo")
        {
            var session = BuildSession();
            SqlBulkInsert(TType, items, session.Connection, tableName, isDataMigration, schema);
        }

        public void SqlBulkInsert(Type TType, IList items, IDbConnection connection, string tableName, bool isDataMigration = false, string schema = "dbo")
        {
            if (string.IsNullOrWhiteSpace(tableName)) tableName = TType.GetTableName();
            SqlManipulations.SqlBulkInsert(TType, items, connection, tableName, EntityName, isDataMigration, schema);
        }

    }
}
