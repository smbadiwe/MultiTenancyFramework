using MultiTenancyFramework.Data;
using MultiTenancyFramework.Mvc.Identity;
using MultiTenancyFramework.Mvc.Logic;
using Owin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MultiTenancyFramework
{
    public static class MvcExtensions
    {
        /// <summary>
        /// Export data table as file. Only CSV for now
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="exportType"></param>
        /// <param name="entityName"></param>
        /// <param name="headerItems"></param>
        /// <returns></returns>
        public static async Task<FileContentResult> ExportFile(this MyDataTable dataTable, string exportType, string entityName, IDictionary<string, string> headerItems = null)
        {
            byte[] theBytes = null;
            string fileName = DateTime.Now.GetLocalTime().ToString("yyyy-MM-dd hh:mm:ss") + "_" + entityName;
            string mimeType = "";
            switch (exportType)
            {
                case "xls":
                case "xlsx":
                    theBytes = await dataTable.ToExcel(headerItems);
                    mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; // "application/ms-excel"; //
                    fileName += ".xlsx";
                    break;
                //case "pdf":
                //    mimeType = "application/pdf";
                //    fileName += ".pdf";
                //    theBytes = new byte[0];
                //    break;
                case "csv":
                default:
                    theBytes = await dataTable.ToCSV(headerItems);
                    mimeType = "text/csv";
                    fileName += ".csv";
                    break;
            }
            return new FileContentResult(theBytes, mimeType) { FileDownloadName = fileName };
        }
        
        /// <summary>
        /// Tells the system to use the MultiTenancy framework.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="settings">Every setting has a defaultt value, so you can safely use a new unstance</param>
        /// <returns></returns>
        public static IAppBuilder UseMultiTenancyFramework(this IAppBuilder app, MultiTenancyFrameworkSettings settings)
        {
            if (settings == null)
            {
                settings = new MultiTenancyFrameworkSettings();
            }
            DataCacheMVC.MultiTenancyFrameworkSettings = settings;
            return app;
        }

    }
}
