using MultiTenancyFramework.Data;
using System;
using System.Collections.Generic;

namespace MultiTenancyFramework.IO
{
    public class IOManager
    {
        public static string GetExportString(KeyValuePair<string, object> column, MyDataColumn dataCol, bool replaceNullsWithDefaultValue)
        {
            #region Write cell data
            if (!Convert.IsDBNull(column.Value))
            {
                object valueToWrite = column.Value;
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
                    return "NULL";
                }
            }
            return dataCol.DataType.IsValueType ? dataCol.DataType.GetDefaultValue().ToString() : "NULL";
            #endregion
        }

    }
}
