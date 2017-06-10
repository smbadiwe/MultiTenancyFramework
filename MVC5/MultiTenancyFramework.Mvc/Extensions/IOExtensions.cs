using MultiTenancyFramework.Data;
using MultiTenancyFramework.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiTenancyFramework
{
    public static class IOExtensions
    {
        public static async Task<byte[]> ToCSV(this MyDataTable dtable, IDictionary<string, string> headerItems = null, bool replaceNullsWithDefaultValue = false, string delimiter = ",")
        {
            return await CsvWriter.CreateCSVfile(dtable, headerItems, replaceNullsWithDefaultValue, delimiter);
        }

        public static async Task<byte[]> ToExcel(this MyDataTable dtable, IDictionary<string, string> headerItems = null, bool replaceNullsWithDefaultValue = false, string delimiter = ",")
        {
            var writer = new ExcelWriter();
            return await writer.ExportToXlsx(dtable, headerItems, replaceNullsWithDefaultValue);
        }
    }
}
