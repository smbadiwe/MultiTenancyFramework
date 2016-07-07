using MultiTenancyFramework.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace MultiTenancyFramework.Data
{
    public interface ICoreBulkInsertDAO<T, idT> where T : IEntity<idT> where idT : IEquatable<idT>
    {
        /// <summary>
        /// Use this to insert bulk data into the db. A typical case is when you're uploading thousands of data
        /// </summary>
        /// <param name="items"></param>
        /// <param name="isDataMigration">For SQL Server Bulk Copy option; to determine whether or not to keep identity from source</param>
        /// <param name="schema">For SQL Server: To be appended to the table name</param>
        void SqlBulkInsert(IList<T> items, bool isDataMigration = false, string schema = "dbo");

        /// <summary>
        /// Use this to insert bulk data into the db. A typical case is when you're uploading thousands of data
        /// </summary>
        /// <param name="items"></param>
        /// <param name="isDataMigration">For SQL Server Bulk Copy option; to determine whether or not to keep identity from source</param>
        /// <param name="schema">For SQL Server: To be appended to the table name</param>
        void SqlBulkInsert(IList<T> items, string tableName, bool isDataMigration = false, string schema = "dbo");

        /// <summary>
        /// Use this to insert bulk data into the db. A typical case is when you're uploading thousands of data
        /// </summary>
        /// <param name="items"></param>
        /// <param name="isDataMigration">For SQL Server Bulk Copy option; to determine whether or not to keep identity from source</param>
        /// <param name="schema">For SQL Server: To be appended to the table name</param>
        void SqlBulkInsert(IList<T> items, IDbConnection connection, string tableName, bool isDataMigration = false, string schema = "dbo");

        /// <summary>
        /// Use this to insert bulk data into the db. A typical case is when you're uploading thousands of data
        /// </summary>
        /// <param name="TType">All item in <paramref name="items"/>  must be of this type</param>
        /// <param name="items"></param>
        /// <param name="isDataMigration">For SQL Server Bulk Copy option; to determine whether or not to keep identity from source</param>
        /// <param name="schema">For SQL Server: To be appended to the table name</param>
        void SqlBulkInsert(Type TType, IList items, bool isDataMigration = false, string schema = "dbo");

        /// <summary>
        /// Use this to insert bulk data into the db. A typical case is when you're uploading thousands of data
        /// </summary>
        /// <param name="TType">All item in <paramref name="items"/>  must be of this type</param>
        /// <param name="items"></param>
        /// <param name="isDataMigration">For SQL Server Bulk Copy option; to determine whether or not to keep identity from source</param>
        /// <param name="schema">For SQL Server: To be appended to the table name</param>
        void SqlBulkInsert(Type TType, IList items, string tableName, bool isDataMigration = false, string schema = "dbo");

        /// <summary>
        /// Use this to insert bulk data into the db. A typical case is when you're uploading thousands of data
        /// </summary>
        /// <param name="TType">All item in <paramref name="items"/>  must be of this type</param>
        /// <param name="items"></param>
        /// <param name="isDataMigration">For SQL Server Bulk Copy option; to determine whether or not to keep identity from source</param>
        /// <param name="schema">For SQL Server: To be appended to the table name</param>
        void SqlBulkInsert(Type TType, IList items, IDbConnection connection, string tableName, bool isDataMigration = false, string schema = "dbo");

    }

    public interface ICoreBulkInsertDAO<T> : ICoreBulkInsertDAO<T, long> where T : IEntity<long>
    {

    }
}
