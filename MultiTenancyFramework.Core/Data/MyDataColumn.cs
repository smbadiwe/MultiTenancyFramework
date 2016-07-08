﻿using System;

namespace MultiTenancyFramework.Data
{
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
        /// Use this when coming from the UI; e.g. when you're trying to export data to Excel of CSV
        /// </summary>
        /// <param name="propertyName">The name of the property in the class</param>
        /// <param name="columnName">The name of the column being represented by this property. Defaults to <paramref name="propertyName"/>
        /// <param name="dataType">The data type of the <paramref name="propertyName"/> specified. Default is String</param>
        public MyDataColumn(string propertyName, string columnName = null, Type dataType = null)
        {
            if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException("propertyName");
            if (dataType == null) dataType = typeof(string);
            if (string.IsNullOrWhiteSpace(columnName)) columnName = propertyName;

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

        public MyDataColumn() { }
    }
}