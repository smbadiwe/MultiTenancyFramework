using System;
using System.Linq.Expressions;

namespace MultiTenancyFramework.Data
{
    /// <summary>
    /// Represents a field (column) in the DB; or a field entity in data to be exported
    /// </summary>
    public class MyDataColumn<T> : MyDataColumn
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="func">Feature property access</param>
        public MyDataColumn(Expression<Func<T, object>> func)
            : this(func, null)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="func">Feature property access</param>
        /// <param name="columnName">Property name</param>
        public MyDataColumn(Expression<Func<T, object>> func, string columnName)
            : base(propertyName: func.GetName(), columnName: columnName, dataType: func.ReturnType)
        {
        }
    }

    /// <summary>
    /// Represents a field (column) in the DB; or a field entity in data to be exported
    /// </summary>
    public class MyDataColumn
    {
        /// <summary>
        /// Index of the column in the table
        /// </summary>
        public int ColumnIndex { get; set; }
        /// <summary>
        /// The name of the property in the entity represented by the <code>ColumnName</code>
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// The PropertyType of the data in the column in question; i.e., the PropertyType of the property whose name is <code>PropertyName</code>
        /// </summary>
        public Type DataType { get; set; }
        public string ColumnName { get; set; }
        
        /// <summary>
        /// Use this when coming from the UI; e.g. when you're trying to export data to Excel or CSV.
        /// <para>Thanks to C# 6, you can now use the nameof(...) to supply the <paramref name="propertyName"/>. If you 
        /// do not supply <paramref name="dataType"/>, then we assume string. 
        /// The generic version of this constructor will infer the type.</para>
        /// </summary>
        /// <param name="propertyName">The name of the property in the class. If using C# 6, use nameof to avoid magic strings</param>
        /// <param name="columnName">The name of the column being represented by this property. Defaults to <paramref name="propertyName"/>
        /// <param name="dataType">The data type of the <paramref name="propertyName"/> specified. Default is String</param>
        public MyDataColumn(string propertyName, string columnName = null, Type dataType = null)
        {
            if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException("propertyName");
            if (dataType == null) dataType = typeof(string);
            if (string.IsNullOrWhiteSpace(columnName)) columnName = propertyName.AsSplitPascalCasedString();

            PropertyName = propertyName;
            ColumnName = columnName;
            DataType = dataType;
        }

        /// <summary>
        /// Best to use this when data is coming directly from the DB. This then perfectly mimicks a DB field (column)
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="dataType"></param>
        public MyDataColumn(string columnName, Type dataType)
        {
            ColumnName = columnName;
            DataType = dataType;
        }

        public MyDataColumn(System.Reflection.PropertyInfo property)
        {
            PropertyName = property.Name;
            ColumnName = property.Name.AsSplitPascalCasedString();
            DataType = property.PropertyType;
        }

        public MyDataColumn() { }
    }
}
