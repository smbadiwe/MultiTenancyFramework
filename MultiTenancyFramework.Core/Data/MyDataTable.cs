using System.Collections.Generic;

namespace MultiTenancyFramework.Data
{
    /// <summary>
    /// A very minimal version of the usual DataTable. This is all I need, not that over-bloated one from System.Data
    /// </summary>
    public class MyDataTable
    {
        public MyDataTable(string tableName)
        {
            TableName = tableName;
            Rows = new List<MyDataRow>();
            Columns = new Dictionary<string, MyDataColumn>();
        }

        public MyDataRow NewRow()
        {
            return new MyDataRow(this);
        }

        public string TableName { get; set; }
        public List<MyDataRow> Rows { get; private set; }
        public Dictionary<string, MyDataColumn> Columns { get; private set; }
    }
}
