using SOMA.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MultiTenancyFramework.Core
{
    public static class FileExportExtensions
    {
        public static async Task<byte[]> ToCSV(this MyDataTable dtable, IDictionary<string, object> headerItems = null, bool replaceNullsWithDefaultValue = false, string delimiter = ",")
        {
            return await IO.CsvEngine.CreateCSVfile(dtable, headerItems, replaceNullsWithDefaultValue, delimiter);
        }

        //public static byte[] ToExcel(this MyDataTable dataTable, IDictionary<string, object> headerItems = null)
        //{
        //    byte[] theByte = null;
        //    using (var memory = new MemoryStream())
        //    {
        //        using (ExcelPackage pck = new ExcelPackage())
        //        {
        //            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet 1");
        //            ws.Cells["A1"].LoadFromDataTable(dataTable, true);

        //            #region Adding relevant headers

        //            if (headerItems != null && headerItems.Count > 0)
        //            {
        //                int numOfColumnsToMerge = dataTable.Columns.Count;
        //                #region Headings
        //                string headerCell; // = Alphabets[0] + 1; //A1; inst. name
        //                                   //int numberOfAdditionalRowsAdded = headerItems.Count;
        //                                   //ws.InsertRow(3, numberOfAdditionalRowsAdded);
        //                int i = 0;
        //                for (int j = 0; j < headerItems.Count; j++)
        //                {
        //                    i = j + 1;
        //                    ws.InsertRow(i, 1);
        //                    headerCell = Utilities.Alphabets[0] + i;
        //                    if (headerItems.Keys.ElementAt(j) == "CompanyName")
        //                    {
        //                        ws.Cells[headerCell].Value = headerItems.Values.ElementAt(j);
        //                        ws.Cells[headerCell].Style.Font.Size = 15;
        //                    }
        //                    else
        //                    {
        //                        //if (_context.Session[GeneralWebUtility.USE_SEPARATE_CELL_FOR_VALUE] != null)
        //                        //{
        //                        //    if ((bool)_context.Session[GeneralWebUtility.USE_SEPARATE_CELL_FOR_VALUE])
        //                        //    {
        //                        //        ws.Cells[headerCell].Value = headerItems.Keys.ElementAt(j);
        //                        //        headerCell = Utilities.Alphabets[1] + (i + 3);
        //                        //        ws.Cells[headerCell].Value = headerItems.Values.ElementAt(j);
        //                        //    }
        //                        //    else
        //                        //    {
        //                        //        ws.Cells[headerCell].Value = headerItems.Values.ElementAt(j);
        //                        //    }
        //                        //}
        //                        //else
        //                        {
        //                            ws.Cells[headerCell].Value = headerItems.Values.ElementAt(j);
        //                        }
        //                    }
        //                    ws.Cells[headerCell].Style.Font.Bold = true;
        //                    ws.Cells[i, 1, i, numOfColumnsToMerge].Merge = true;
        //                    ws.Cells[headerCell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //                }
        //                ws.InsertRow(i + 1, 1);

        //                #endregion
        //            }

        //            //if (!string.IsNullOrWhiteSpace(imageUrl))
        //            //{
        //            //    ws.InsertRow(1, 1);
        //            //    ws.Cells["A1"].Style.Font.Size = 20;
        //            //}
        //            #endregion

        //            ws.HeaderFooter.OddFooter.LeftAlignedText =
        //               string.Format("Printed by {0} at {1}", WebUtilities.GetCurrentlyLoggedInUser(), DateTime.UtcNow);
        //            ws.HeaderFooter.OddFooter.RightAlignedText =
        //               string.Format("Page {0} of {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);

        //            theByte = pck.GetAsByteArray();

        //            //Save
        //            //pck.Save();
        //        }
        //        return theByte;
        //    }
        //}

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
                    theBytes = await ToCSV(dataTable, headerItems);
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
