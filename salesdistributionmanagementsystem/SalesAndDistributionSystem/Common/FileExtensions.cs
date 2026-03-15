using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Common
{
    public static class FileExtensions
    {
        public static async Task<DataSet> ToDataSet(this IFormFile file)
        {
            var ds = new DataSet();
            try
            {
                if (file != null && Path.GetExtension(file.FileName).ToLower() == ".xlsx")
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        XLWorkbook workbook = new XLWorkbook(memoryStream);
                        var wsList = workbook.Worksheets;
                        foreach (var worksheet in wsList)
                        {
                            ds.Tables.Add(SheetToTable(worksheet));
                        }
                    }
                    return ds;
                }
                else
                {
                    throw new Exception("Please select file with .xlsx extension!");
                }
            }
            catch(Exception exp)
            {
                throw exp;
            }
        }

        public static async Task<DataTable> ToDataTable(this IFormFile file, int sheetNumber=1)
        {
            try
            {
                if (file != null && Path.GetExtension(file.FileName).ToLower() == ".xlsx")
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        XLWorkbook workbook = new XLWorkbook(memoryStream);
                        return SheetToTable(workbook.Worksheet(sheetNumber));
                    }
                }
                else
                {
                    throw new Exception("Please select file with .xlsx extension!");
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        private static DataTable SheetToTable(IXLWorksheet worksheet)
        {
            DataTable dt = new DataTable();
            bool FirstRow = true;
            //Range for reading the cells based on the last cell used.  
            string readRange = "1:1";
            foreach (IXLRow row in worksheet.RowsUsed())
            {
                //If Reading the First Row (used) then add them as column name  
                if (FirstRow)
                {
                    //Checking the Last cellused for column generation in datatable  
                    readRange = string.Format("{0}:{1}", 1, row.LastCellUsed().Address.ColumnNumber);
                    foreach (IXLCell cell in row.Cells(readRange))
                    {
                        dt.Columns.Add(cell.Value.ToString());
                    }
                    FirstRow = false;
                }
                else
                {
                    //Adding a Row in datatable  
                    dt.Rows.Add();
                    int cellIndex = 0;
                    //Updating the values of datatable  
                    foreach (IXLCell cell in row.Cells(readRange))
                    {
                        dt.Rows[dt.Rows.Count - 1][cellIndex] = cell.Value.ToString();
                        cellIndex++;
                    }
                }
                //If no data in Excel file  
                if (FirstRow)
                {
                    throw new Exception("Empty Excel File!");
                }
            }

            return dt;
        }
    }
}
