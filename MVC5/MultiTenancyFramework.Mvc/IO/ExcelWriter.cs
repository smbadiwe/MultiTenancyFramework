using MultiTenancyFramework.Data;
using MultiTenancyFramework.Mvc.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiTenancyFramework.IO
{
    public class ExcelWriter
    {
        public ExcelWriter(Action<ExcelStyle> setCaptionStyle = null)
        {
            if (setCaptionStyle == null) setCaptionStyle = DefaultCaptionStyle;

            SetCaptionStyle = setCaptionStyle;
        }

        public Action<ExcelStyle> SetCaptionStyle { get; set; }
        public bool UseDropdownlistsForAssociatedEntities { get; set; }
        private void DefaultCaptionStyle(ExcelStyle style)
        {
            style.Fill.PatternType = ExcelFillStyle.Solid;
            style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
            style.Font.Bold = true;
        }

        /// <summary>
        /// Export objects to XLSX (MS Excel)
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="properties">Class access to the object through its properties</param>
        /// <param name="itemsToExport">The objects to export</param>
        /// <returns></returns>
        public virtual async Task<byte[]> ExportToXlsx<T>(PropertyByName<T>[] properties, IEnumerable<T> itemsToExport)
        {
            return await Task.Run(() =>
            {
                using (var stream = new MemoryStream())
                {
                    // ok, we can run the real code of the sample now
                    using (var xlPackage = new ExcelPackage(stream))
                    {
                        // uncomment this line if you want the XML written out to the outputDir
                        //xlPackage.DebugMode = true; 

                        // get handles to the worksheets
                        var worksheet = xlPackage.Workbook.Worksheets.Add(typeof(T).Name);
                        var fWorksheet = xlPackage.Workbook.Worksheets.Add("DataForFilters");
                        fWorksheet.Hidden = eWorkSheetHidden.VeryHidden;

                        //create Headers and format them 
                        var manager = new PropertyManager<T>(properties.Where(p => !p.Ignore));
                        manager.WriteCaption(worksheet, SetCaptionStyle);

                        var row = 2;
                        foreach (var items in itemsToExport)
                        {
                            manager.CurrentObject = items;
                            manager.WriteToXlsx(worksheet, row++, UseDropdownlistsForAssociatedEntities, fWorksheet: fWorksheet);
                        }

                        xlPackage.Save();
                    }
                    return stream.ToArray();
                }
            });
        }

        public virtual async Task<byte[]> ExportToXlsx(MyDataTable dataTable, IDictionary<string, string> headerItems = null,
            bool replaceNullsWithDefaultValue = false)
        {
            return await Task.Run(() =>
            {
                using (var stream = new MemoryStream())
                {
                    // ok, we can run the real code of the sample now
                    using (var xlPackage = new ExcelPackage(stream))
                    {
                        // uncomment this line if you want the XML written out to the outputDir
                        //xlPackage.DebugMode = true; 

                        // get handles to the worksheets
                        var worksheet = xlPackage.Workbook.Worksheets.Add("Exported data");
                        var row = 1;
                        // General Titles
                        if (headerItems != null && headerItems.Count > 0)
                        {
                            foreach (var item in headerItems)
                            {
                                worksheet.Cells[row, 1].Value = item.Key;
                                worksheet.Cells[row, 2].Value = item.Value;
                                row++;
                            }
                            row++;
                        }

                        var poz = 1;
                        _properties = dataTable.Columns;
                        foreach (var propertyByName in _properties)
                        {
                            propertyByName.Value.ColumnIndex = poz;
                            poz++;
                        }

                        //create Headers and format them 
                        WriteCaption(worksheet, SetCaptionStyle, row);
                        row++;

                        foreach (var item in dataTable.Rows)
                        {
                            WriteToXlsx(worksheet, item, row++, UseDropdownlistsForAssociatedEntities);
                        }

                        var file = new FileInfo(@"C:\SOMA\Deeds\OpenSourceProjs\MultiTenancyFramework\MyTestExcel.xlsx");
                        
                        xlPackage.SaveAs(file); 
                        //xlPackage..Save();
                    }
                    return new byte[0]; // stream.ToArray();
                }
            });
        }


        /// <summary>
        /// All properties
        /// </summary>
        private Dictionary<string, MyDataColumn> _properties;

        /// <summary>
        /// Curent row to acsess
        /// </summary>
        public MyDataRow CurrentRow { get; set; }

        /// <summary>
        /// Write object data to XLSX worksheet
        /// </summary>
        /// <param name="worksheet">Data worksheet</param>
        /// <param name="row">Row</param>
        /// <param name="rowIndex">Row index</param>
        /// <param name="exportImportUseDropdownlistsForAssociatedEntities">Indicating whether need create dropdown list for export</param>
        /// <param name="cellOffset">Cell offset</param>
        /// <param name="fWorksheet">Filters worksheet</param>
        private void WriteToXlsx(ExcelWorksheet worksheet, MyDataRow row, int rowIndex, bool exportImportUseDropdownlistsForAssociatedEntities, int cellOffset = 0, ExcelWorksheet fWorksheet = null)
        {
            if (row == null) return;

            foreach (var tblCell in row)
            {
                var prop = _properties[tblCell.Key];
                var cell = worksheet.Cells[rowIndex, prop.ColumnIndex + cellOffset];
                cell.Value = IOManager.GetExportString(tblCell, prop, true);
            }
        }

        /// <summary>
        /// Write caption (first row) to XLSX worksheet
        /// </summary>
        /// <param name="worksheet">worksheet</param>
        /// <param name="setStyle">Detection of cell style</param>
        /// <param name="row">Row num</param>
        /// <param name="cellOffset">Cell offset</param>
        private void WriteCaption(ExcelWorksheet worksheet, Action<ExcelStyle> setStyle, int row = 1, int cellOffset = 0)
        {
            foreach (var caption in _properties.Values)
            {
                var cell = worksheet.Cells[row, caption.ColumnIndex + cellOffset];
                cell.Value = caption.ColumnName;
                setStyle(cell.Style);
            }
        }

    }
}
