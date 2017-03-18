using Microsoft.CSharp.RuntimeBinder;
using MultiTenancyFramework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MultiTenancyFramework
{
    public static class IEnumerableExtensions
    {
        public static string ToCommaSeparatedList<T>(this IEnumerable<T> list, string separator = ",", bool trimEnd = false)
        {
            if (!list.Any()) return string.Empty;
            var sb = new StringBuilder();
            foreach (var item in list)
            {
                sb.AppendFormat("{0}{1}", item, separator);
            }
            if (trimEnd)
            {
                var str = sb.ToString();
                return str.Substring(0, str.Length - 1);
            }
            return sb.ToString();
        }

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
                dt.Columns.Add(item.ColumnName, new MyDataColumn(item.ColumnName, item.DataType));
            }
            var validColumnsDict = validColumns.ToDictionary(x => x.ColumnName);
            string propName;
            for (int i = 0; i < items.Count; i++)
            {
                var row = dt.NewRow();
                row["No"] = i + 1;
                foreach (var col in dt.Columns)
                {
                    if (col.Key == "No") continue;

                    propName = validColumnsDict[col.Key].PropertyName;
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
        /// divides a given 'enumerable' (eg List) into 'chunkSize' slammer lists
        /// <para>USAGE</para>
        /// <para>var src = new[] { 1, 2, 3, 4, 5, 6 };</para>
        /// <para>var c3 = src.Chunks(3);      // {{1, 2, 3}, {4, 5, 6}}; </para>
        /// <para>var c4 = src.Chunks(4);      // {{1, 2, 3, 4}, {5, 6}}; </para>
        /// <para>var sum = c3.Select(c => c.Sum());    // {6, 15}</para>
        /// <para>var count = c3.Count();                 // 2</para>
        /// <para>var take2 = c3.Select(c => c.Take(2));  // {{1, 2}, {4, 5}}</para>
        /// <para>var batch1 = c3.Select(c => c.ToList()).First(); // list:{1, 2, 3}</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Chunks<T>(this IEnumerable<T> enumerable,
                                                    int chunkSize)
        {
            if (chunkSize < 1) throw new ArgumentException("chunkSize must be positive");

            using (var e = enumerable.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    var remaining = chunkSize;    // elements remaining in the current chunk
                    var innerMoveNext = new Func<bool>(() => --remaining > 0 && e.MoveNext());

                    yield return e.GetChunk(innerMoveNext);
                    while (innerMoveNext()) {/* discard elements skipped by inner iterator */}
                }
            }
        }


        /// <summary>
        /// NB: This throws an exception if the key does not exist in the DejaVuObject. To be safe, call TryGet method.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Get(this IDictionary<string, object> dictionary, string key)
        {
            return dictionary[key];
        }

        public static void Set(this IDictionary<string, object> dictionary, string key, object value)
        {
            dictionary[key] = value;
        }

        public static bool TryGet(this IDictionary<string, object> dictionary, string key, out object value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets all the items in <paramref name="source"/> that are duplicated
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> GetDuplicates<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IEqualityComparer<TKey> comparer)
        {
            var hash = new HashSet<TKey>(comparer);
            return source.Where(item => !hash.Add(selector(item))); //=> if you can't add it, it means it's been added before, hence a duplicate.
        }

        /// <summary>
        /// Gets all the items in the collection that are duplicated
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> GetDuplicates<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            return source.GetDuplicates(x => x, comparer);
        }

        /// <summary>
        /// Gets all the items in the collection that are duplicated
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> GetDuplicates<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.GetDuplicates(selector, null);
        }

        /// <summary>
        /// Removes all duplicates from <paramref name="source"/> and returns the distinct values
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> GetDistinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IEqualityComparer<TKey> comparer)
        {
            var hash = new HashSet<TKey>(comparer);
            return source.Where(item => hash.Add(selector(item))); //=> if you can add it, it means it has not been added before, hence a distinct.
        }

        public static IEnumerable<TSource> GetDistinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.GetDistinct(selector, null);
        }

        private static IEnumerable<T> GetChunk<T>(this IEnumerator<T> e,
                                                  Func<bool> innerMoveNext)
        {
            do yield return e.Current;
            while (innerMoveNext());
        }
    }
}
