using System.Collections.Generic;

namespace MultiTenancyFramework.Data
{
    /// <summary>
    /// A very minimal version of the usual DataTable. This is all I need, not that over-bloated one from System.Data
    /// </summary>
    public class MyDataTable
    {
        /// <summary>
        /// A very minimal version of the usual DataTable. This is all I need, not that over-bloated one from System.Data
        /// </summary>
        public MyDataTable(string tableName)
        {
            TableName = tableName;
            Rows = new List<MyDataRow>();
            Columns = new Dictionary<string, MyDataColumn>();
        }

        /// <summary>
        /// Return a new row
        /// </summary>
        /// <returns></returns>
        public MyDataRow NewRow()
        {
            return new MyDataRow(this);
        }

        /// <summary>
        /// Get or set the table name
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Get the rows of the data table
        /// </summary>
        public List<MyDataRow> Rows { get; private set; }

        /// <summary>
        /// Get the columns of the data table. NB: Key is <see cref="MyDataColumn.ColumnName"/>.
        /// </summary>
        public Dictionary<string, MyDataColumn> Columns { get; private set; }
    }
}
