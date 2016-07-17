using MultiTenancyFramework.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MultiTenancyFramework
{
    public static class MvcExtensions
    {
        public static async Task<FileContentResult> ExportFile(this MyDataTable dataTable, string exportType, string entityName, IDictionary<string, object> headerItems = null)
        {
            byte[] theBytes = null;
            string fileName = DateTime.Now.GetLocalTime().ToString("yyyy-MM-dd hh:mm:ss") + "_" + entityName;
            string mimeType = "";
            switch (exportType)
            {
                //case "pdf":
                //    mimeType = "application/pdf";
                //    fileName += ".pdf";
                //    theBytes = new byte[0];
                //    break;
                case "csv":
                default:
                    mimeType = "text/csv";
                    fileName += ".csv";
                    theBytes = await dataTable.ToCSV(headerItems);
                    break;
                    //default:
                    //    mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; // "application/ms-excel"; //
                    //    fileName += ".xlsx";
                    //    theBytes = ToExcel(dataTable, headerItems);
                    //    break;
            }
            return new FileContentResult(theBytes, mimeType) { FileDownloadName = fileName };
        }
        
    }
}
