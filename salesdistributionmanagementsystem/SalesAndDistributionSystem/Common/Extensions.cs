
using ClosedXML.Excel;
using SalesAndDistributionSystem.Domain.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Common
{
    public static class Extensions
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? user.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "";
        }

        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? user.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User";
        }

        public static int GetComapanyId(this ClaimsPrincipal user)
        {
            return Convert.ToInt32(user.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value);
        }

        public static int GetUnitId(this ClaimsPrincipal user)
        {
            return Convert.ToInt32(user.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value.ToString());
        }

        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        public static XLWorkbook ToExcel(this DataTable dt, string fileName, string unitName, string unitAddress, string fromDate, string toDate)
        {
            XLWorkbook wb = new XLWorkbook();
            wb.Worksheets.Add(fileName);
            var ws = wb.Worksheets.First();
            ws.ShowGridLines = false;

            ws.Range(1, 1, 1, 10).Merge();
            ws.Cell(1, 1).Value = "Square Toiletriess Ltd.";
            ws.Cell(1, 1).Style.Font.FontSize = 16;
            ws.Cell(1, 1).Style.Font.Bold = true;

            ws.Range(2, 1, 2, 10).Merge();
            ws.Cell(2, 1).Value = "72 Mohakhali CA, Dhaka 1212";

            ws.Range(3, 1, 3, 10).Merge();
            ws.Cell(3, 1).Value = fileName; /*"Report: Distributor Wise And SKU Wise Sales Report";*/
            ws.Cell(3, 1).Style.Font.Bold = true;

            ws.Range(5, 1, 5, 10).Merge();
            ws.Cell(5, 1).Value = "Depot Name: " + unitName;
            ws.Cell(5, 1).Style.Font.Bold = true;

            ws.Range(6, 1, 6, 10).Merge();
            ws.Cell(6, 1).Value = "Depot Address: " + unitAddress;

            ws.Range(7, 1, 7, 10).Merge();
            ws.Cell(7, 1).Value = "From Date: " + fromDate + " To " + toDate;
            ws.Cell(7, 1).Style.Font.Bold = true;

            ws.Range(8, 1, 8, 10).Merge();
            ws.Cell(8, 1).Value = "Print Time: " + DateTime.Now.ToString("dd/MM/yy hh:mm:yy tt");

            int row = 9;
            int col = 1;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            foreach (DataColumn dc in dt.Columns)
            {
                ws.Cell(row, col).Value = textInfo.ToTitleCase(string.Join(" ", dc.ColumnName.Split('_')).ToLower());
                ws.Cell(row, col).Style.Font.Bold = true;
                ws.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell(row, col).Style.Alignment.WrapText = true;
                col++;
            }

            row++;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ws.Cell(row + i, j + 1).Value = dt.Rows[i][j];
                    ws.Cell(row + i, j + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }
            }

            return wb;
        }

        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();
            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }


    }
}
