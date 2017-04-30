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
            object valueToWrite = null;
            if (!Convert.IsDBNull(column.Value))
            {
                valueToWrite = column.Value;
                if (dataCol.DataType == typeof(DateTime))
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
                    else if (dataCol.DataType == typeof(bool))
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
