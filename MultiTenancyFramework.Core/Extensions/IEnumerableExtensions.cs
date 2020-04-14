using Microsoft.CSharp.RuntimeBinder;
using MultiTenancyFramework.Data;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MultiTenancyFramework
{
    /// <summary>
    /// More in 'IEnumerableExtensions' in MultiTenancyFramework.Utils
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// This is currently designed to only work for primitive types as I am only using it for
        /// generic export. It can be made better.
        /// </summary>
        /// <typeparam name="T">This may be a type generated at runtime</typeparam>
        /// <param name="items"></param>
        /// <param name="validColumns">NB: No need to put S/No column. It's added automatically</param>
        /// <param name="generatedType">If null, it defaults to typeof(T). If <paramref name="T"/> is a generated type, supply the type here to aid in the resolution, because it will read as Object.</param>
        /// <returns></returns>
        public static MyDataTable ToDataTable<T>(this IList<T> items, IList<MyDataColumn> validColumns, Type generatedType = null)
        {
            if (generatedType == null) generatedType = typeof(T);
            var dt = new MyDataTable(generatedType.Name);
            dt.Columns.Add("No", new MyDataColumn("No", typeof(int)));
            foreach (var item in validColumns)
            {
                dt.Columns.Add(item.ColumnName, item);
            }
            string propName;
            for (int i = 0; i < items.Count; i++)
            {
                var row = dt.NewRow();
                row["No"] = i + 1;
                foreach (var col in dt.Columns)
                {
                    if (col.Key == "No") continue;

                    propName = col.Value.PropertyName;
                    object theVal = null;
                    try
                    {
                        theVal = generatedType.GetProperty(propName).GetValue(items[i], null);
                    }
                    catch (TargetException)
                    {
                        theVal = null;
                    }
                    catch (RuntimeBinderException)
                    {
                        theVal = null;
                    }
                    row[col.Key] = theVal;
                }
                dt.Rows.Add(row);
            }
            return dt;
        }

        /// <summary>
        /// This is currently designed to only work for primitive types as I am only using it for
        /// generic export. It can be made better.
        /// </summary>
        /// <typeparam name="T">This may be a type generated at runtime</typeparam>
        /// <param name="items"></param>
        /// <param name="validColumns">NB: No need to put S/No column. It's added automatically</param>
        /// <returns></returns>
        public static MyDataTable ToDataTable<T>(this IList<T> items, IList<MyDataColumn<T>> validColumns)
        {
            var generatedType = typeof(T);
            var dt = new MyDataTable(generatedType.Name);
            dt.Columns.Add("No", new MyDataColumn("No", typeof(int)));
            foreach (var item in validColumns)
            {
                dt.Columns.Add(item.ColumnName, item);
            }

            string propName;
            for (int i = 0; i < items.Count; i++)
            {
                var row = dt.NewRow();
                row["No"] = i + 1;
                foreach (var col in dt.Columns)
                {
                    if (col.Key == "No") continue;

                    propName = col.Value.PropertyName;
                    object theVal = null;
                    try
                    {
                        theVal = generatedType.GetProperty(propName).GetValue(items[i], null);
                    }
                    catch (TargetException)
                    {
                        theVal = null;
                    }
                    catch (RuntimeBinderException)
                    {
                        theVal = null;
                    }
                    row[col.Key] = theVal;
                }
                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}
