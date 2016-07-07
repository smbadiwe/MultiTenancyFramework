using MultiTenancyFramework.Data;
using MultiTenancyFramework.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MultiTenancyFramework
{
    public static class OtherExtensions
    {
        public static async Task<byte[]> ToCSV(this MyDataTable dtable, IDictionary<string, object> headerItems = null, bool replaceNullsWithDefaultValue = false, string delimiter = ",")
        {
            return await CsvWriter.CreateCSVfile(dtable, headerItems, replaceNullsWithDefaultValue, delimiter);
        }

        /// <summary>
        /// Determines whether the specified item is in the given list (IEnumerable).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <param name="list">The list.</param>
        /// <returns>
        ///   <c>true</c> if the specified item is in the list (IEnumerable); otherwise, <c>false</c>.
        /// </returns>
        public static bool IsContainedIn<T>(this T item, IEnumerable<T> list)
        {
            return (list != null && list.Contains(item));
        }

        /// <summary>
        /// Determines whether the specified item [is not in] [the specified list].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <param name="list">The list.</param>
        /// <returns>
        ///   <c>true</c> if [the specified item] [is not in] the list; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNotContainedIn<T>(this T item, IEnumerable<T> list)
        {
            return !IsContainedIn(item, list);
        }

        /// <summary>
        /// Whether or not the given file is in use by another process
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool IsFileInUse(this FileInfo file)
        {
            try
            {
                //Don't change FileAccess to ReadWrite, 
                //because if a file is in readOnly, it fails. 
                using (var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    //file is not locked 
                    return false;
                }
            }
            catch //(IOException)
            {
                //the file is unavailable because it is: 
                //still being written to 
                //or being processed by another thread 
                //or does not exist (has already been processed) 
                return true;
            }
        }

        public static string ToReadableString(this Enum @enum)
        {
            FieldInfo fi = @enum.GetType().GetField(@enum.ToString());
            EnumDescriptionAttribute attribute = null;
            try
            {
                attribute = fi.GetCustomAttribute<EnumDescriptionAttribute>();
            }
            catch (ArgumentNullException)
            {
                return string.Empty;
            }
            if (attribute != null)
            {
                return attribute.Name;
            }
            else
            {
                return @enum.ToString().AsSplitPascalCasedString();
            }
        }

    }
}
