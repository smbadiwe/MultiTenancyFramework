using Microsoft.CSharp.RuntimeBinder;
using MultiTenancyFramework.Data;
using MultiTenancyFramework.Entities;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace MultiTenancyFramework.NHibernate
{
    public class SqlManipulations
    {
        /// <summary>
        /// NB: This may not work well when you have some DB column names different from the property names
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="idT"></typeparam>
        /// <param name="items"></param>
        /// <param name="connection"></param>
        /// <param name="columnsInTheTable"></param>
        /// <param name="tableName"></param>
        /// <param name="entityName"></param>
        /// <param name="isDataMigration">For SQL Server: if true, then we need to Keep Identity</param>
        /// <param name="schema">For SQL Server: To be appended to the table name</param>
        [Obsolete("Use the async/await version instead.")]
        public static void SqlBulkInsert(Type TType, IList items, IDbConnection connection, string tableName, string entityName = null, bool isDataMigration = false, string schema = "dbo")
        {
            if (items == null) return;
            if (items.Count == 0)
            {
                //Logger.Log("SQL Bulk Insert - Nothing to insert for " + entityName);
                return;
            }
            string typeName = TType.Name;
            if (string.IsNullOrWhiteSpace(tableName)) tableName = TType.GetTableName();
            if (string.IsNullOrWhiteSpace(entityName)) entityName = "null";
            tableName = tableName.Replace("[", "").Replace("]", "");
            if (string.IsNullOrWhiteSpace(schema)) schema = "dbo";

            PropertyInfo[] props;
            MyDataTable dt = FillInDataColumns(TType, typeof(long), connection, tableName, schema, out props);

            //Fill in the rows
            var savedBy = getSaverID();
            foreach (var item in items)
            {
                BuildNewRow(dt, props, item, savedBy);
            }

            //Finally...
            DoBulkInsert(dt, connection, tableName, isDataMigration, schema);
        }

        /// <summary>
        /// NB: This may not work well when you have some DB column names different from the property names
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="idT"></typeparam>
        /// <param name="items"></param>
        /// <param name="connection"></param>
        /// <param name="columnsInTheTable"></param>
        /// <param name="tableName"></param>
        /// <param name="entityName"></param>
        /// <param name="isDataMigration">For SQL Server: if true, then we need to Keep Identity</param>
        /// <param name="schema">For SQL Server: To be appended to the table name</param>
        public static async Task SqlBulkInsert<T, idT>(IList<T> items, IDbConnection connection, string tableName, string entityName = null, bool isDataMigration = false, string schema = "dbo") where T : class, IBaseEntity<idT> where idT : IEquatable<idT>
        {
            if (items == null) return;
            if (items.Count == 0)
            {
                //Logger.Log("SQL Bulk Insert - Nothing to insert for " + entityName);
                return;
            }
            string typeName = typeof(T).Name;
            if (string.IsNullOrWhiteSpace(tableName)) tableName = typeof(T).GetTableName();
            if (string.IsNullOrWhiteSpace(entityName)) entityName = "null";
            tableName = tableName.Replace("[", "").Replace("]", "");
            if (string.IsNullOrWhiteSpace(schema)) schema = "dbo";

            PropertyInfo[] props;
            MyDataTable dt = FillInDataColumns(typeof(T), typeof(idT), connection, tableName, schema, out props);

            //Fill in the rows
            var saverID = getSaverID();
            foreach (var item in items)
            {
                BuildNewRow(dt, props, item, saverID);
            }

            //Finally...
            await DoBulkInsert(dt, connection, tableName, isDataMigration, schema);
        }

        private static async Task DoBulkInsert(MyDataTable dt, IDbConnection connection, string tableName, bool isDataMigration = false, string schema = "dbo")
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
            connection.Open();
            var con = connection as MySqlConnection;
            if (con != null)
            {
                string strFile = string.Format("{0}_{1}.csv", tableName, DateTime.UtcNow.ToString("yyyyMMMddhhmmss"));

                //Create directory if not exist... Make sure directory has required rights..
                var baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MySQLTemp");

                if (!Directory.Exists(baseDir)) Directory.CreateDirectory(baseDir);

                var theFile = Path.Combine(baseDir, strFile);
                //If file does not exist then create it and right data into it..
                if (!File.Exists(theFile))
                {
                    using (File.Create(theFile))
                    {
                    }
                }

                string fieldTerminator = "\t";
                string lineTerminator = "\r\n";
                //Generate csv file from where data read
                await IO.CsvWriter.CreateCSVfile(dt, theFile, true, fieldTerminator);
                try
                {
                    RunMySqlCommand(con, "SET FOREIGN_KEY_CHECKS=0");
                    MySqlBulkLoader bl = new MySqlBulkLoader(con)
                    {
                        TableName = tableName,
                        FieldTerminator = fieldTerminator,
                        LineTerminator = lineTerminator,
                        FileName = theFile,
                        NumberOfLinesToSkip = 0
                    };
                    bl.Columns.Clear();
                    foreach (var col in dt.Columns)
                    {
                        bl.Columns.Add(col.Key);
                    }
                    int count = bl.Load();
                    RunMySqlCommand(con, "SET FOREIGN_KEY_CHECKS=1");
                }
                finally
                {
                    try
                    {
                        File.Delete(theFile);
                    }
                    catch { }
                }
            }
            else // SQL Server
            {
                #region Deal with later - or never
                //dt.TableName = string.Format("[{0}].[{1}]", schema, tableName);
                //var sqlConn = connection as SqlConnection;
                //SqlBulkCopyOptions sqlBulkCopyOptions;
                //if (isDataMigration)
                //{
                //    //NB: The Bitwise OR (|) is NOT the same as OR. What this particular ORing does, in English, is actually AND; i.e., we want to 
                //    //Keep Identity and Keep nulls also.
                //    sqlBulkCopyOptions = SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.KeepNulls;
                //}
                //else
                //{
                //    sqlBulkCopyOptions = SqlBulkCopyOptions.KeepNulls;
                //}
                //using (var copy = new SqlBulkCopy(sqlConn, sqlBulkCopyOptions, null)
                //{
                //    BulkCopyTimeout = 10000,
                //    DestinationTableName = dt.TableName,
                //    NotifyAfter = 5000,
                //})
                //{
                //    foreach (DataColumn column in dt.Columns)
                //    {
                //        copy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                //    }
                //    copy.WriteToServer(dt);
                //} 
                #endregion
            }
        }

        private static long getSaverID()
        {
            long savedBy = 0;
            if (HttpContext.Current != null)
            {
                try
                {
                    var theUser = NHUtils.CurrentUser;
                    if (theUser != null)
                    {
                        savedBy = theUser.Id;
                    }
                }
                catch { }
            }
            return savedBy;
        }

        private static MyDataTable FillInDataColumns(Type TType, Type idType, IDbConnection connection, string tableName, string schema, out PropertyInfo[] props)
        {
            string prepend = "";
            if (connection is SqlConnection)
            {
                prepend = $"use {connection.Database}; ";
            }
            string sqlForColumnNames = string.Format("{0}select column_name from information_schema.columns where table_name = '{1}' and table_schema = '{2}'"
                , prepend, tableName, string.IsNullOrWhiteSpace(prepend) ? connection.Database : schema);
            var columnsInDestinationTable = ExecuteSelectQuery(sqlForColumnNames, connection);
            props = TType.GetProperties().Where(x => x.CanWrite && x.GetCustomAttribute<NotMappedAttribute>() == null).ToArray();

            MyDataTable dt = new MyDataTable(tableName);
            var propsToReturn = new List<PropertyInfo>();
            //Gather datatable columns
            var classes = props.Where(x => x.PropertyType.IsClass && x.PropertyType != typeof(string));
            var compositeClasses = classes.Where(x => x.GetCustomAttribute<CompositeMappingModifyFieldNamesAttribute>() != null);
            var nonCompositeClasses = classes.Where(x => x.GetCustomAttribute<CompositeMappingModifyFieldNamesAttribute>() == null);
            var propsDict = props.ToDictionary(x => x.GetPropertyName());
            foreach (var col in columnsInDestinationTable)
            {
                if (dt.Columns.ContainsKey(col)) continue;

                Type type = null;
                if (propsDict.ContainsKey(col))
                {
                    var prop = propsDict[col];
                    type = prop.IsTypeNullable() ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType;
                }
                else
                {
                    if (col.EndsWith("Id")) //=> we may be dealing with a reference mapping
                    {
                        #region we may be dealing with a reference mapping
                        foreach (var comp in nonCompositeClasses)
                        {
                            if (type != null) break;
                            var innerProps = comp.PropertyType.GetProperties().Where(x => x.CanWrite).ToArray();
                            if (innerProps.Any(x => x.Name == "Id")) // Handle properties mapped as Reference 
                            {
                                string columnName = string.Format("{0}Id", comp.Name.StartsWith("The") ? comp.Name.Remove(0, 3) : comp.Name);
                                if (col == columnName)
                                {
                                    type = idType;
                                    break;
                                }
                            }
                        }
                        if (type == null)
                        {
                            foreach (var comp in compositeClasses)
                            {
                                if (type != null) break;
                                var innerProps = comp.PropertyType.GetProperties().Where(x => x.CanWrite && x.GetCustomAttribute<NotMappedAttribute>() == null).ToArray();
                                var modifyFieldNameAttr = comp.GetCustomAttribute<CompositeMappingModifyFieldNamesAttribute>();

                                for (int j = 0; j < innerProps.Length; j++)
                                {
                                    var innerProp = innerProps[j];
                                    if (modifyFieldNameAttr.UseAllPropertiesWithTheirDefaultNames)
                                    {
                                        if (col != innerProp.GetPropertyName()) continue;
                                    }
                                    else
                                    {
                                        if (!modifyFieldNameAttr.FieldAndPropNames.ContainsKey(innerProp.GetPropertyName())) continue;

                                        if (col != modifyFieldNameAttr.FieldAndPropNames[innerProp.GetPropertyName()]) continue;
                                    }
                                    type = innerProp.IsTypeNullable() ? Nullable.GetUnderlyingType(innerProp.PropertyType) : innerProp.PropertyType;
                                    break;
                                }
                            }
                        }

                        if (type == null)
                        {
                            // let it be Int64.
                            type = typeof(long);
                        } 
                        #endregion
                    }
                    else //=> we may be dealing with a composite mapping
                    {
                        #region MyRegion
                        foreach (var comp in compositeClasses)
                        {
                            if (type != null) break;
                            var innerProps = comp.PropertyType.GetProperties().Where(x => x.CanWrite && x.GetCustomAttribute<NotMappedAttribute>() == null).ToArray();
                            var modifyFieldNameAttr = comp.GetCustomAttribute<CompositeMappingModifyFieldNamesAttribute>();

                            for (int j = 0; j < innerProps.Length; j++)
                            {
                                var innerProp = innerProps[j];
                                if (modifyFieldNameAttr.UseAllPropertiesWithTheirDefaultNames)
                                {
                                    if (col != innerProp.GetPropertyName()) continue;
                                }
                                else
                                {
                                    if (!modifyFieldNameAttr.FieldAndPropNames.ContainsKey(innerProp.GetPropertyName())) continue;

                                    if (col != modifyFieldNameAttr.FieldAndPropNames[innerProp.GetPropertyName()]) continue;
                                }
                                type = innerProp.IsTypeNullable() ? Nullable.GetUnderlyingType(innerProp.PropertyType) : innerProp.PropertyType;
                                break;
                            }
                        } 
                        #endregion
                    }
                }
                try
                {
                    if (type.IsEnum)
                    {
                        type = idType;
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Failed to determine type for " + col + " when trying to fill DataTable Column", ex);
                }

                dt.Columns.Add(col, new MyDataColumn(col, type));
            }

            return dt;
        }

        private static void RunMySqlCommand(MySqlConnection con, string query)
        {
            using (var command = con.CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
        }

        private static void BuildNewRow(MyDataTable dt, PropertyInfo[] props, object item, long savedBy)
        {
            var row = dt.NewRow();
            for (int j = 0; j < props.Length; j++)
            {
                var prop = props[j];
                if (!dt.Columns.ContainsKey(prop.GetPropertyName()))
                {
                    if (prop.PropertyType.IsClass && prop.PropertyType != typeof(string))
                    {
                        var innerProps = prop.PropertyType.GetProperties();
                        if (innerProps.Any(x => x.Name == "Id")) // Handle properties mapped as Reference 
                        {
                            string columnName = string.Format("{0}Id", prop.Name.StartsWith("The") ? prop.Name.Remove(0, 3) : prop.Name);
                            if (!dt.Columns.ContainsKey(columnName)) continue;

                            object theRefValue;
                            try
                            {
                                dynamic propValue = prop.GetValue(item, null);
                                theRefValue = propValue.Id;
                            }
                            catch (TargetException)
                            {
                                theRefValue = null;
                            }
                            catch (RuntimeBinderException)
                            {
                                theRefValue = null;
                            }
                            catch (Exception) { throw; }
                            if (theRefValue == null)
                            {
                                row[columnName] = System.DBNull.Value;
                                continue;
                            }
                            row[columnName] = theRefValue;
                        }
                        else //Handling Compositely mapped Class properties
                        {
                            var modifyFieldNameAttr = prop.GetCustomAttribute<CompositeMappingModifyFieldNamesAttribute>();

                            for (int k = 0; k < innerProps.Length; k++)
                            {
                                var innerProp = innerProps[k];
                                string innerPropName = null;
                                if (modifyFieldNameAttr.UseAllPropertiesWithTheirDefaultNames)
                                {
                                    if (!dt.Columns.ContainsKey(innerProp.GetPropertyName())) continue;

                                    innerPropName = innerProp.GetPropertyName();
                                }
                                else
                                {
                                    if (!modifyFieldNameAttr.FieldAndPropNames.ContainsKey(innerProp.GetPropertyName())) continue;

                                    innerPropName = modifyFieldNameAttr.FieldAndPropNames[innerProp.GetPropertyName()];
                                    if (!dt.Columns.ContainsKey(innerPropName)) continue;
                                }
                                object theInnerValue;
                                try
                                {
                                    theInnerValue = innerProp.GetValue(prop.GetValue(item, null), null);
                                    if (theInnerValue == null && innerProp.IsTypeNullable())
                                    {
                                        theInnerValue = Nullable.GetUnderlyingType(innerProp.PropertyType).GetDefaultValue(); //innerProp.PropertyType.GenericTypeArguments[0].GetDefaultValue();
                                    }
                                }
                                catch (TargetException)
                                {
                                    theInnerValue = null;
                                }
                                catch (Exception) { throw; }
                                if (string.IsNullOrWhiteSpace(innerPropName)) innerPropName = modifyFieldNameAttr.FieldAndPropNames[innerProp.GetPropertyName()];
                                if (theInnerValue == null)
                                {
                                    row[innerPropName] = DBNull.Value;
                                    continue;
                                }

                                if (innerProp.PropertyType == typeof(DateTime) && theInnerValue != null && Convert.ToDateTime(theInnerValue) < new DateTime(1900, 1, 1))
                                {
                                    theInnerValue = new DateTime(1900, 1, 1);
                                }

                                if (innerProp.PropertyType.IsEnum && theInnerValue != null)
                                {
                                    //DO NOT use Enum.Parse(innerProp.PropertyType, theInnerValue.ToString()); since we store enums as integers conventionally
                                    theInnerValue = (int)theInnerValue;
                                }
                                row[innerPropName] = theInnerValue;
                            }
                        }
                        continue;
                    }
                    continue;
                }

                object theValue;
                try
                {
                    if (prop.Name == "DateMigrated")
                    {
                        theValue = DateTime.Now.GetLocalTime();
                    }
                    else if (prop.Name == "SavedBy")
                    {
                        theValue = savedBy;
                    }
                    else
                    {
                        theValue = prop.GetValue(item, null);
                    }
                    if (theValue == null)
                    {
                        if (prop.Name == "DateCreated")
                        {
                            theValue = new DateTime(1900, 1, 1);
                        }
                        else if (prop.Name == "IsDisabled" || prop.Name == "IsDeleted")
                        {
                            theValue = false;
                        }
                        else if (prop.IsTypeNullable())
                        {
                            var mainType = Nullable.GetUnderlyingType(prop.PropertyType);
                            if (mainType == typeof(DateTime))
                                theValue = new DateTime(1900, 1, 1);
                            else
                                theValue = mainType.GetDefaultValue();
                        }
                    }
                }
                catch (TargetException)
                {
                    theValue = null;
                }
                catch (Exception) { throw; }
                if (theValue == null)
                {
                    row[prop.GetPropertyName()] = DBNull.Value;
                    continue;
                }
                if (prop.PropertyType == typeof(DateTime) && theValue != null && Convert.ToDateTime(theValue) < new DateTime(1900, 1, 1))
                {
                    theValue = new DateTime(1900, 1, 1);
                }
                if (prop.PropertyType.IsEnum && theValue != null)
                {
                    //DO NOT use Enum.Parse(prop.PropertyType, theValue.ToString()); since we store enums as integers conventionally
                    theValue = (int)theValue;
                }

                row[prop.GetPropertyName()] = theValue;
            }

            dt.Rows.Add(row);
        }

        private static HashSet<string> ExecuteSelectQuery(string query, IDbConnection connection, object[] Params = null)
        {
            var result = new HashSet<string>();
            try
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
                connection.Open();
                var sqlConn = connection as SqlConnection;
                if (sqlConn == null)
                {
                    using (MySqlCommand Scmd = new MySqlCommand(query))
                    {
                        Scmd.Connection = connection as MySqlConnection;
                        if (Params != null)
                        {
                            Scmd.Parameters.AddRange(Params);
                        }
                        using (var reader = Scmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    result.Add(reader.GetString(0));
                                }
                            }
                        }
                    }
                }
                else
                {
                    using (SqlCommand Scmd = new SqlCommand(query))
                    {
                        Scmd.Connection = sqlConn;
                        if (Params != null)
                        {
                            Scmd.Parameters.AddRange(Params);
                        }
                        using (var reader = Scmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    result.Add(reader.GetString(0));
                                }
                            }
                        }
                    }
                }
            }
            catch (DbException e)
            {
                //Logger.Log(new ApplicationException("DBException e.ErrorCode = " + e.ErrorCode, e));
                return null;
            }
            catch (Exception e)
            {
                //Logger.Log(e);
                return null;
            }
            return result;
        }

    }
}
