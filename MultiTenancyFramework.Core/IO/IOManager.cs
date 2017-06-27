using MultiTenancyFramework.Data;
using System;
using System.Collections.Generic;

namespace MultiTenancyFramework.IO
{
    public class IOManager
    {
        public static string GetExportString(KeyValuePair<string, object> column, MyDataColumn dataCol, bool replaceNullsWithDefaultValue)
        {
            if (dataCol == null) throw new ArgumentNullException("dataCol", $"dataCol is null for column {column.Key} -> {column.Value}");

            object valueToWrite = column.Value;
            #region Write cell data
            if (valueToWrite != null && !Convert.IsDBNull(valueToWrite))
            {
                Type t = typeof(decimal), objType = valueToWrite.GetType();
                if (dataCol.DataType == t || objType == t)
                {
                    return Convert.ToDecimal(valueToWrite).ToMoney();
                }
                t = typeof(DateTime);
                if (dataCol.DataType == t || objType == t)
                {
                    return Convert.ToDateTime(valueToWrite).ToString("yyyy-MM-dd HH:mm:ss");
                }
                return valueToWrite.ToString();
            }
            else
            {
                if (dataCol.DataType == typeof(string))
                {
                    return "";
                }
                if (replaceNullsWithDefaultValue)
                {
                    if (dataCol.DataType == typeof(DateTime))
                    {
                        return new DateTime(1900, 1, 1).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    if (dataCol.DataType == typeof(decimal))
                    {
                        return "0.00";
                    }
                    if (dataCol.DataType == typeof(bool))
                    {
                        return "False";
                    }
                }
                else
                {
                    return "";
                }
            }
            return dataCol.DataType.IsValueType ? dataCol.DataType.GetDefaultValue().ToString() : "";
            #endregion
        }

    }
}
