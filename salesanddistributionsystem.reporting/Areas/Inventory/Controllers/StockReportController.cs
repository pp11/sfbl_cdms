using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using SalesAndDistributionSystem.Domain.Models.ReportModels.Common.ProductReports;
using SalesAndDistributionSystem.Reporting.Areas.Inventory.Data;
using SalesAndDistributionSystem.Reporting.ReportConst;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SalesAndDistributionSystem.Reporting.Areas.Inventory.Controllers
{
    public class StockReportController : Controller
    {
        private readonly StockReport service = new StockReport();
        private readonly CommonServices commonservice = new CommonServices();

        // Current Stock Report : Stock Report(../Inventory/Report/InventoryReport)
        public ActionResult Daily(string q)
        {
            try
            {
                if (q != null)
                {
                    q = commonservice.Decrypt(q);
                    ReportParams reportParameters = JsonConvert.DeserializeObject<ReportParams>(q);

                    if (reportParameters.SECRET_KEY == ProjectConstants.Secret_Key)
                    {

                        LocalReport localReport = new LocalReport();
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/StockReport/DailyStockReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DailyStockDataSet";
                        //reportDataSource.Value = service.GetDailyStockReport(reportParameters);
                        var dt = service.GetDailyStockReport(reportParameters);
                        if (reportParameters.QUERY == "HASVALUES")
                        {
                            Func<DataRow, bool> condition = row => Convert.ToInt32(row["STOCK_QTY"]) > 0;
                            DataRow[] filteredRows = dt.Select().Where(row => condition(row)).ToArray();
                            DataTable newTable = dt.Clone();
                            foreach (DataRow row in filteredRows)
                            {
                                newTable.ImportRow(row);
                            }
                            reportDataSource.Value = newTable;

                        }
                        else if (reportParameters.QUERY == "HASNOVALUES")
                        {
                            Func<DataRow, bool> condition = row => Convert.ToInt32(row["STOCK_QTY"]) <= 0;
                            DataRow[] filteredRows = dt.Select().Where(row => condition(row)).ToArray();
                            DataTable newTable = dt.Clone();
                            foreach (DataRow row in filteredRows)
                            {
                                newTable.ImportRow(row);
                            }
                            reportDataSource.Value = newTable;
                        }
                        else
                        {
                            reportDataSource.Value = dt;

                        }
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "DataSet00002";
                        reportDataSource1.Value = service.GetPageHeaderReport(reportParameters);

                        string reportType = reportParameters.REPORT_EXTENSION;
                        Warning[] warnings;
                        string mimeType;
                        string[] streamids;
                        string encoding;
                        string fileNameExtension = reportType == "Excel" ? "xlsx" : "pdf";
                        byte[] renderedBytes;
                        localReport.DataSources.Add(reportDataSource);
                        localReport.DataSources.Add(reportDataSource1);

                        renderedBytes = localReport.Render(reportType, "", out mimeType, out encoding, out fileNameExtension, out streamids, out warnings);

                        return new FileContentResult(renderedBytes, mimeType);

                    }
                    else
                    {
                        return Redirect("/Home/Index");
                    }
                }
                return Redirect("/Home/Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        // Stock Report (Date Wise) : Stock Report(../Inventory/Report/InventoryReport)
        public ActionResult DateWiseStock(string q)
        {
            try
            {
                if (q != null)
                {
                    q = commonservice.Decrypt(q);
                    ReportParams reportParameters = JsonConvert.DeserializeObject<ReportParams>(q);

                    if (reportParameters.SECRET_KEY == ProjectConstants.Secret_Key)
                    {

                        LocalReport localReport = new LocalReport();
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/StockReport/DateWiseStockReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DailyStockDataSet";
                        var dataTable = service.GetDateWiseStockReport(reportParameters);
                        if (reportParameters.QUERY == "HASVALUES")
                        {
                            Func<DataRow, bool> condition = row => Convert.ToInt32(row["STOCK_QTY"]) > 0;
                            DataRow[] filteredRows = dataTable.Select().Where(row => condition(row)).ToArray();
                            DataTable newTable = dataTable.Clone();
                            foreach (DataRow row in filteredRows)
                            {
                                newTable.ImportRow(row);
                            }
                            reportDataSource.Value = newTable;

                        }
                        else if (reportParameters.QUERY == "HASNOVALUES")
                        {
                            Func<DataRow, bool> condition = row => Convert.ToInt32(row["STOCK_QTY"]) <= 0;
                            DataRow[] filteredRows = dataTable.Select().Where(row => condition(row)).ToArray();
                            DataTable newTable = dataTable.Clone();
                            foreach (DataRow row in filteredRows)
                            {
                                newTable.ImportRow(row);
                            }
                            reportDataSource.Value = newTable;
                        }
                        else
                        {
                            reportDataSource.Value = dataTable;

                        }
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "DataSet00002";
                        reportDataSource1.Value = service.GetPageHeaderReport(reportParameters);
                        DataTable dt = new DataTable();
                        dt.Clear();
                        dt.Columns.Add("DATE_FROM");
                        dt.Columns.Add("DATE_TO");
                        dt.Columns.Add("UNIT_NAME");
                        DataRow _ravi = dt.NewRow();
                        _ravi["DATE_TO"] = reportParameters.DATE_TO;
                        dt.Rows.Add(_ravi);
                        ReportDataSource reportDataSource2 = new ReportDataSource();
                        reportDataSource2.Name = "DateRangeDataSet";
                        reportDataSource2.Value = dt;

                        string reportType = reportParameters.REPORT_EXTENSION;
                        Warning[] warnings;
                        string mimeType;
                        string[] streamids;
                        string encoding;
                        string fileNameExtension = reportType == "Excel" ? "xlsx" : "pdf";
                        byte[] renderedBytes;
                        localReport.DataSources.Add(reportDataSource);
                        localReport.DataSources.Add(reportDataSource1);
                        localReport.DataSources.Add(reportDataSource2);

                        renderedBytes = localReport.Render(reportType, "", out mimeType, out encoding, out fileNameExtension, out streamids, out warnings);

                        return new FileContentResult(renderedBytes, mimeType);

                    }
                    else
                    {
                        return Redirect("/Home/Index");
                    }
                }
                return Redirect("/Home/Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public ActionResult SKULedger(string q)
        {
            try
            {
                if (q != null)
                {
                    q = commonservice.Decrypt(q);
                    ReportParams reportParameters = JsonConvert.DeserializeObject<ReportParams>(q);

                    if (reportParameters.SECRET_KEY == ProjectConstants.Secret_Key)
                    {

                        LocalReport localReport = new LocalReport();
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/StockReport/SKULedgerReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DailyStockDataSet";
                        reportDataSource.Value = service.GetDateWiseStockReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "DataSet00002";
                        reportDataSource1.Value = service.GetPageHeaderReport(reportParameters);

                        string reportType = reportParameters.REPORT_EXTENSION;
                        Warning[] warnings;
                        string mimeType;
                        string[] streamids;
                        string encoding;
                        string fileNameExtension = reportType == "Excel" ? "xlsx" : "pdf";
                        byte[] renderedBytes;
                        localReport.DataSources.Add(reportDataSource);
                        localReport.DataSources.Add(reportDataSource1);

                        renderedBytes = localReport.Render(reportType, "", out mimeType, out encoding, out fileNameExtension, out streamids, out warnings);

                        return new FileContentResult(renderedBytes, mimeType);

                    }
                    else
                    {
                        return Redirect("/Home/Index");
                    }
                }
                return Redirect("/Home/Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public ActionResult RequisitionRaise(string q)
        {
            try
            {

                if (q != null)
                {
                    q = commonservice.Decrypt(q);
                    ReportParams reportParameters = JsonConvert.DeserializeObject<ReportParams>(q);

                    if (reportParameters.SECRET_KEY == ProjectConstants.Secret_Key)
                    {

                        LocalReport localReport = new LocalReport();
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/RequisitionReport/RequisitionRaiseReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "ReqRaiseDataSet";
                        reportDataSource1.Value = service.GetRequsitionRaiseReport(reportParameters);

                        string reportType = reportParameters.REPORT_EXTENSION;
                        Warning[] warnings;
                        string mimeType;
                        string[] streamids;
                        string encoding;
                        string fileNameExtension = reportType == "Excel" ? "xlsx" : "pdf";
                        byte[] renderedBytes;
                        localReport.DataSources.Add(reportDataSource);
                        localReport.DataSources.Add(reportDataSource1);

                        renderedBytes = localReport.Render(reportType, "", out mimeType, out encoding, out fileNameExtension, out streamids, out warnings);

                        return new FileContentResult(renderedBytes, mimeType);

                    }
                    else
                    {
                        return Redirect("/Home/Index");
                    }
                }
                return Redirect("/Home/Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public ActionResult RequisitionIssue(string q)
        {
            try
            {

                if (q != null)
                {
                    q = commonservice.Decrypt(q);
                    ReportParams reportParameters = JsonConvert.DeserializeObject<ReportParams>(q);

                    if (reportParameters.SECRET_KEY == ProjectConstants.Secret_Key)
                    {

                        LocalReport localReport = new LocalReport();
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/RequisitionReport/RequisitionIssueReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "ReqIssueDataSet";
                        reportDataSource1.Value = service.GetRequsitionIssueReport(reportParameters);

                        string reportType = reportParameters.REPORT_EXTENSION;
                        Warning[] warnings;
                        string mimeType;
                        string[] streamids;
                        string encoding;
                        string fileNameExtension = reportType == "Excel" ? "xlsx" : "pdf";
                        byte[] renderedBytes;
                        localReport.DataSources.Add(reportDataSource);
                        localReport.DataSources.Add(reportDataSource1);

                        renderedBytes = localReport.Render(reportType, "", out mimeType, out encoding, out fileNameExtension, out streamids, out warnings);

                        return new FileContentResult(renderedBytes, mimeType);

                    }
                    else
                    {
                        return Redirect("/Home/Index");
                    }
                }
                return Redirect("/Home/Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult RequisitionReceived(string q)
        {
            try
            {

                if (q != null)
                {
                    q = commonservice.Decrypt(q);
                    ReportParams reportParameters = JsonConvert.DeserializeObject<ReportParams>(q);

                    if (reportParameters.SECRET_KEY == ProjectConstants.Secret_Key)
                    {

                        LocalReport localReport = new LocalReport();
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/RequisitionReport/RequisitionReceivedReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "ReqReceivedDataSet";
                        reportDataSource1.Value = service.GetRequsitionReceivedReport(reportParameters);
                        string reportType = reportParameters.REPORT_EXTENSION;
                        Warning[] warnings;
                        string mimeType;
                        string[] streamids;
                        string encoding;
                        string fileNameExtension = reportType == "Excel" ? "xlsx" : "pdf";
                        byte[] renderedBytes;
                        localReport.DataSources.Add(reportDataSource);
                        localReport.DataSources.Add(reportDataSource1);

                        renderedBytes = localReport.Render(reportType, "", out mimeType, out encoding, out fileNameExtension, out streamids, out warnings);

                        return new FileContentResult(renderedBytes, mimeType);

                    }
                    else
                    {
                        return Redirect("/Home/Index");
                    }
                }
                return Redirect("/Home/Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public ActionResult RequisitionReturn(string q)
        {
            try
            {

                if (q != null)
                {
                    q = commonservice.Decrypt(q);
                    ReportParams reportParameters = JsonConvert.DeserializeObject<ReportParams>(q);

                    if (reportParameters.SECRET_KEY == ProjectConstants.Secret_Key)
                    {

                        LocalReport localReport = new LocalReport();
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/RequisitionReport/RequisitionReturnReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "ReqReturnDataSet";
                        reportDataSource1.Value = service.GetRequsitionReturnReport(reportParameters);

                        string reportType = reportParameters.REPORT_EXTENSION;
                        Warning[] warnings;
                        string mimeType;
                        string[] streamids;
                        string encoding;
                        string fileNameExtension = reportType == "Excel" ? "xlsx" : "pdf";
                        byte[] renderedBytes;
                        localReport.DataSources.Add(reportDataSource);
                        localReport.DataSources.Add(reportDataSource1);

                        renderedBytes = localReport.Render(reportType, "", out mimeType, out encoding, out fileNameExtension, out streamids, out warnings);

                        return new FileContentResult(renderedBytes, mimeType);

                    }
                    else
                    {
                        return Redirect("/Home/Index");
                    }
                }
                return Redirect("/Home/Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public ActionResult RequisitionReturnReceived(string q)
        {
            try
            {

                if (q != null)
                {
                    q = commonservice.Decrypt(q);
                    ReportParams reportParameters = JsonConvert.DeserializeObject<ReportParams>(q);

                    if (reportParameters.SECRET_KEY == ProjectConstants.Secret_Key)
                    {

                        LocalReport localReport = new LocalReport();
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/RequisitionReport/RequisitionReturnReceivedReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "ReqReturnReceivedDataSet";
                        reportDataSource1.Value = service.GetRequsitionReturnReveivedReport(reportParameters);

                        string reportType = reportParameters.REPORT_EXTENSION;
                        Warning[] warnings;
                        string mimeType;
                        string[] streamids;
                        string encoding;
                        string fileNameExtension = reportType == "Excel" ? "xlsx" : "pdf";
                        byte[] renderedBytes;
                        localReport.DataSources.Add(reportDataSource);
                        localReport.DataSources.Add(reportDataSource1);

                        renderedBytes = localReport.Render(reportType, "", out mimeType, out encoding, out fileNameExtension, out streamids, out warnings);

                        return new FileContentResult(renderedBytes, mimeType);

                    }
                    else
                    {
                        return Redirect("/Home/Index");
                    }
                }
                return Redirect("/Home/Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //Stock Consumption Report(now DateWiseStockRegister) : Stock Report(../Inventory/Report/InventoryReport)--StockConsumption
        public ActionResult DateWiseStockRegister(string q)
        {
            try
            {

                if (q != null)
                {
                    q = commonservice.Decrypt(q);
                    ReportParams reportParameters = JsonConvert.DeserializeObject<ReportParams>(q);

                    if (reportParameters.SECRET_KEY == ProjectConstants.Secret_Key)
                    {

                        LocalReport localReport = new LocalReport();
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/StockReport/DateWiseStockRegister.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "StockConsuptionDataSet";
                        var dt = service.GetStockConsumptionReport(reportParameters);
                        if (reportParameters.QUERY == "HASVALUES" && reportParameters.PRODUCT_STATUS == "ACTIVE")
                        {
                            //Func<DataRow, bool> condition = row => Convert.ToInt32(row["OPENING_QTY"]) + Convert.ToInt32(row["CLOSING_QTY"]) > 0 && row["PRODUCT_STATUS"].ToString() == "Active";
                            //Func<DataRow, bool> condition = row => Convert.ToInt32(row["OPENING_QTY"]) + Convert.ToInt32(row["CLOSING_QTY"])+Convert.ToInt32(row["TOTAL_IN"]) + Convert.ToInt32(row["TOTAL_OUT"]) + Convert.ToInt32(row["HAS_VALUE_CHECK"]) > 0 && row["PRODUCT_STATUS"].ToString() == "Active";
                            Func<DataRow, bool> condition = row => Convert.ToInt32(row["HAS_VALUE_CHECK"]) > 0 && row["PRODUCT_STATUS"].ToString() == "Active";

                            DataRow[] filteredRows = dt.Select().Where(row => condition(row)).ToArray();
                            DataTable newTable = dt.Clone();
                            foreach (DataRow row in filteredRows)
                            {
                                newTable.ImportRow(row);
                            }
                            reportDataSource1.Value = newTable;

                        }
                        else if (reportParameters.QUERY == "HASVALUES" && reportParameters.PRODUCT_STATUS == "INACTIVE")
                        {
                            //Func<DataRow, bool> condition = row => Convert.ToInt32(row["OPENING_QTY"]) + Convert.ToInt32(row["CLOSING_QTY"]) > 0 && row["PRODUCT_STATUS"].ToString() == "InActive";
                            //Func<DataRow, bool> condition = row => Convert.ToInt32(row["OPENING_QTY"]) + Convert.ToInt32(row["CLOSING_QTY"])+Convert.ToInt32(row["TOTAL_IN"]) + Convert.ToInt32(row["TOTAL_OUT"]) + Convert.ToInt32(row["HAS_VALUE_CHECK"]) > 0 && row["PRODUCT_STATUS"].ToString() == "InActive";
                            Func<DataRow, bool> condition = row =>  Convert.ToInt32(row["HAS_VALUE_CHECK"]) > 0 && row["PRODUCT_STATUS"].ToString() == "InActive";

                            DataRow[] filteredRows = dt.Select().Where(row => condition(row)).ToArray();
                            DataTable newTable = dt.Clone();
                            foreach (DataRow row in filteredRows)
                            {
                                newTable.ImportRow(row);
                            }
                            reportDataSource1.Value = newTable;

                        }
                        else if (reportParameters.QUERY == "HASVALUES" && reportParameters.PRODUCT_STATUS == "ALL")
                        {
                            //Func<DataRow, bool> condition = row => Convert.ToInt32(row["OPENING_QTY"]) + Convert.ToInt32(row["CLOSING_QTY"]) > 0;
                            //Func<DataRow, bool> condition = row => Convert.ToInt32(row["OPENING_QTY"]) + Convert.ToInt32(row["CLOSING_QTY"])+Convert.ToInt32(row["TOTAL_IN"]) + Convert.ToInt32(row["TOTAL_OUT"]) + Convert.ToInt32(row["HAS_VALUE_CHECK"]) > 0;
                            Func<DataRow, bool> condition = row =>  Convert.ToInt32(row["HAS_VALUE_CHECK"]) > 0;

                            DataRow[] filteredRows = dt.Select().Where(row => condition(row)).ToArray();
                            DataTable newTable = dt.Clone();
                            foreach (DataRow row in filteredRows)
                            {
                                newTable.ImportRow(row);
                            }
                            reportDataSource1.Value = newTable;

                        }
                        else if (reportParameters.QUERY == "HASNOVALUES" && reportParameters.PRODUCT_STATUS == "ACTIVE")
                        {
                            //Func<DataRow, bool> condition = row => Convert.ToInt32(row["OPENING_QTY"]) + Convert.ToInt32(row["CLOSING_QTY"]) <= 0 && row["PRODUCT_STATUS"].ToString() == "Active";
                            //Func<DataRow, bool> condition = row => Convert.ToInt32(row["OPENING_QTY"]) + Convert.ToInt32(row["CLOSING_QTY"])+Convert.ToInt32(row["TOTAL_IN"]) + Convert.ToInt32(row["TOTAL_OUT"]) + Convert.ToInt32(row["HAS_VALUE_CHECK"]) <= 0 && row["PRODUCT_STATUS"].ToString() == "Active";
                            Func<DataRow, bool> condition = row =>  Convert.ToInt32(row["HAS_VALUE_CHECK"]) <= 0 && row["PRODUCT_STATUS"].ToString() == "Active";

                            DataRow[] filteredRows = dt.Select().Where(row => condition(row)).ToArray();
                            DataTable newTable = dt.Clone();
                            foreach (DataRow row in filteredRows)
                            {
                                newTable.ImportRow(row);
                            }
                            reportDataSource1.Value = newTable;
                        }
                        else if (reportParameters.QUERY == "HASNOVALUES" && reportParameters.PRODUCT_STATUS == "INACTIVE")
                        {
                            //Func<DataRow, bool> condition = row => Convert.ToInt32(row["OPENING_QTY"]) + Convert.ToInt32(row["CLOSING_QTY"]) <= 0 && row["PRODUCT_STATUS"].ToString() == "InActive";
                            //Func<DataRow, bool> condition = row => Convert.ToInt32(row["OPENING_QTY"]) + Convert.ToInt32(row["CLOSING_QTY"])+Convert.ToInt32(row["TOTAL_IN"]) + Convert.ToInt32(row["TOTAL_OUT"]) + Convert.ToInt32(row["HAS_VALUE_CHECK"]) <= 0 && row["PRODUCT_STATUS"].ToString() == "InActive";
                            Func<DataRow, bool> condition = row =>  Convert.ToInt32(row["HAS_VALUE_CHECK"]) <= 0 && row["PRODUCT_STATUS"].ToString() == "InActive";

                            DataRow[] filteredRows = dt.Select().Where(row => condition(row)).ToArray();
                            DataTable newTable = dt.Clone();
                            foreach (DataRow row in filteredRows)
                            {
                                newTable.ImportRow(row);
                            }
                            reportDataSource1.Value = newTable;
                        }
                        else if (reportParameters.QUERY == "HASNOVALUES" && reportParameters.PRODUCT_STATUS == "ALL")
                        {
                            //Func<DataRow, bool> condition = row => Convert.ToInt32(row["OPENING_QTY"]) + Convert.ToInt32(row["CLOSING_QTY"]) <= 0;
                            //Func<DataRow, bool> condition = row => Convert.ToInt32(row["OPENING_QTY"]) + Convert.ToInt32(row["CLOSING_QTY"])+Convert.ToInt32(row["TOTAL_IN"]) + Convert.ToInt32(row["TOTAL_OUT"]) + Convert.ToInt32(row["HAS_VALUE_CHECK"]) <= 0;
                            Func<DataRow, bool> condition = row =>  Convert.ToInt32(row["HAS_VALUE_CHECK"]) <= 0;

                            DataRow[] filteredRows = dt.Select().Where(row => condition(row)).ToArray();
                            DataTable newTable = dt.Clone();
                            foreach (DataRow row in filteredRows)
                            {
                                newTable.ImportRow(row);
                            }
                            reportDataSource1.Value = newTable;
                        }
                        else if (reportParameters.QUERY == "ALL" && reportParameters.PRODUCT_STATUS == "ACTIVE")
                        {
                            Func<DataRow, bool> condition = row => row["PRODUCT_STATUS"].ToString() == "Active";
                            DataRow[] filteredRows = dt.Select().Where(row => condition(row)).ToArray();
                            DataTable newTable = dt.Clone();
                            foreach (DataRow row in filteredRows)
                            {
                                newTable.ImportRow(row);
                            }
                            reportDataSource1.Value = newTable;
                        }
                        else if (reportParameters.QUERY == "ALL" && reportParameters.PRODUCT_STATUS == "INACTIVE")
                        {
                            Func<DataRow, bool> condition = row => row["PRODUCT_STATUS"].ToString() == "InActive";
                            DataRow[] filteredRows = dt.Select().Where(row => condition(row)).ToArray();
                            DataTable newTable = dt.Clone();
                            foreach (DataRow row in filteredRows)
                            {
                                newTable.ImportRow(row);
                            }
                            reportDataSource1.Value = newTable;
                        }
                        else
                        {
                            reportDataSource1.Value = dt;

                        }
                        DataTable dtDate = new DataTable();
                        dtDate.Clear();
                        dtDate.Columns.Add("DATE_FROM");
                        dtDate.Columns.Add("DATE_TO");
                        dtDate.Columns.Add("UNIT_NAME");
                        DataRow _ravi = dtDate.NewRow();
                        _ravi["DATE_FROM"] = reportParameters.DATE_FROM;
                        _ravi["DATE_TO"] = reportParameters.DATE_TO;
                        _ravi["UNIT_NAME"] = reportParameters.UNIT_NAME;
                        dtDate.Rows.Add(_ravi);
                        ReportDataSource reportDataSource2 = new ReportDataSource();
                        reportDataSource2.Name = "DateRangeDataSet";
                        reportDataSource2.Value = dtDate;
                        string reportType = reportParameters.REPORT_EXTENSION;
                        Warning[] warnings;
                        string mimeType;
                        string[] streamids;
                        string encoding;
                        string fileNameExtension = reportType == "Excel" ? "xlsx" : "pdf";
                        byte[] renderedBytes;
                        localReport.DataSources.Add(reportDataSource);
                        localReport.DataSources.Add(reportDataSource1);
                        localReport.DataSources.Add(reportDataSource2);


                        renderedBytes = localReport.Render(reportType, "", out mimeType, out encoding, out fileNameExtension, out streamids, out warnings);

                        return new FileContentResult(renderedBytes, mimeType);

                    }
                    else
                    {
                        return Redirect("/Home/Index");
                    }
                }
                return Redirect("/Home/Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        //Batch Wise Stock Report : Stock Report(../Inventory/Report/InventoryReport)
        public ActionResult BatchWiseStock(string q)
        {
            try
            {
                if (q != null)
                {
                    q = commonservice.Decrypt(q);
                    ReportParams reportParameters = JsonConvert.DeserializeObject<ReportParams>(q);

                    if (reportParameters.SECRET_KEY == ProjectConstants.Secret_Key)
                    {

                        LocalReport localReport = new LocalReport();
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/StockReport/BatchWiseStockReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "BatchWiseStockDataSet";
                        var dataTable = service.GetBatchWiseStockReport(reportParameters);
                        if (reportParameters.QUERY == "HASVALUES")
                        {
                            Func<DataRow, bool> condition = row => Convert.ToInt32(row["STOCK_QTY"]) > 0;
                            DataRow[] filteredRows = dataTable.Select().Where(row => condition(row)).ToArray();
                            DataTable newTable = dataTable.Clone();
                            foreach (DataRow row in filteredRows)
                            {
                                newTable.ImportRow(row);
                            }
                            reportDataSource1.Value = newTable;

                        }
                        else if (reportParameters.QUERY == "HASNOVALUES")
                        {
                            Func<DataRow, bool> condition = row => Convert.ToInt32(row["STOCK_QTY"]) <= 0;
                            DataRow[] filteredRows = dataTable.Select().Where(row => condition(row)).ToArray();
                            DataTable newTable = dataTable.Clone();
                            foreach (DataRow row in filteredRows)
                            {
                                newTable.ImportRow(row);
                            }
                            reportDataSource1.Value = newTable;
                        }
                        else
                        {
                            reportDataSource1.Value = dataTable;

                        }
                        DataTable dt = new DataTable();
                        dt.Clear();
                        dt.Columns.Add("DATE_FROM");
                        dt.Columns.Add("DATE_TO");
                        dt.Columns.Add("UNIT_NAME");
                        DataRow _ravi = dt.NewRow();
                        _ravi["DATE_TO"] = reportParameters.DATE_TO;
                        dt.Rows.Add(_ravi);
                        ReportDataSource reportDataSource2 = new ReportDataSource();
                        reportDataSource2.Name = "DateRangeDataSet";
                        reportDataSource2.Value = dt;
                        string reportType = reportParameters.REPORT_EXTENSION;
                        Warning[] warnings;
                        string mimeType;
                        string[] streamids;
                        string encoding;
                        string fileNameExtension = reportType == "Excel" ? "xlsx" : "pdf";
                        byte[] renderedBytes;
                        localReport.DataSources.Add(reportDataSource);
                        localReport.DataSources.Add(reportDataSource1);
                        localReport.DataSources.Add(reportDataSource2);

                        renderedBytes = localReport.Render(reportType, "", out mimeType, out encoding, out fileNameExtension, out streamids, out warnings);

                        return new FileContentResult(renderedBytes, mimeType);

                    }
                    else
                    {
                        return Redirect("/Home/Index");
                    }
                }
                return Redirect("/Home/Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        //Batch Wise Stock Report (New V2): Stock Report(../Inventory/Report/InventoryReport)
        public ActionResult BatchWiseStockV2(string q)
        {
            try
            {
                if (q != null)
                {
                    q = commonservice.Decrypt(q);
                    ReportParams reportParameters = JsonConvert.DeserializeObject<ReportParams>(q);

                    if (reportParameters.SECRET_KEY == ProjectConstants.Secret_Key)
                    {

                        LocalReport localReport = new LocalReport();
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/StockReport/BatchWiseStockV2Report.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "BatchWiseStockV2DataSet";
                        var dataTable = service.GetBatchWiseStockV2Report(reportParameters);
                        if (reportParameters.QUERY == "HASVALUES")
                        {
                            Func<DataRow, bool> condition = row => Convert.ToInt32(row["STOCK_QTY"]) > 0;
                            DataRow[] filteredRows = dataTable.Select().Where(row => condition(row)).ToArray();
                            DataTable newTable = dataTable.Clone();
                            foreach (DataRow row in filteredRows)
                            {
                                newTable.ImportRow(row);
                            }
                            reportDataSource1.Value = newTable;

                        }
                        else if (reportParameters.QUERY == "HASNOVALUES")
                        {
                            Func<DataRow, bool> condition = row => Convert.ToInt32(row["STOCK_QTY"]) <= 0;
                            DataRow[] filteredRows = dataTable.Select().Where(row => condition(row)).ToArray();
                            DataTable newTable = dataTable.Clone();
                            foreach (DataRow row in filteredRows)
                            {
                                newTable.ImportRow(row);
                            }
                            reportDataSource1.Value = newTable;
                        }
                        else
                        {
                            reportDataSource1.Value = dataTable;

                        }
                        DataTable dt = new DataTable();
                        dt.Clear();
                        dt.Columns.Add("DATE_FROM");
                        dt.Columns.Add("DATE_TO");
                        dt.Columns.Add("UNIT_NAME");
                        DataRow _ravi = dt.NewRow();
                        _ravi["DATE_TO"] = reportParameters.DATE_TO;
                        dt.Rows.Add(_ravi);
                        ReportDataSource reportDataSource2 = new ReportDataSource();
                        reportDataSource2.Name = "DateRangeDataSet";
                        reportDataSource2.Value = dt;
                        string reportType = reportParameters.REPORT_EXTENSION;
                        Warning[] warnings;
                        string mimeType;
                        string[] streamids;
                        string encoding;
                        string fileNameExtension = reportType == "Excel" ? "xlsx" : "pdf";
                        byte[] renderedBytes;
                        localReport.DataSources.Add(reportDataSource);
                        localReport.DataSources.Add(reportDataSource1);
                        localReport.DataSources.Add(reportDataSource2);

                        renderedBytes = localReport.Render(reportType, "", out mimeType, out encoding, out fileNameExtension, out streamids, out warnings);

                        return new FileContentResult(renderedBytes, mimeType);

                    }
                    else
                    {
                        return Redirect("/Home/Index");
                    }
                }
                return Redirect("/Home/Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        //Finished Goods Holding Time: (../Inventory/Report/InventoryReport)
        public ActionResult FinishedGoodsHoldingTime(string q)
        {
            try
            {
                if (q != null)
                {
                    q = commonservice.Decrypt(q);
                    ReportParams reportParameters = JsonConvert.DeserializeObject<ReportParams>(q);

                    if (reportParameters.SECRET_KEY == ProjectConstants.Secret_Key)
                    {

                        LocalReport localReport = new LocalReport();
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/StockReport/FinishedGoodsHoldingTimeReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "GoodsHoldingTimeDataSet";
                        var dataTable = service.GetFinishedGoodsHoldingTimeReport(reportParameters);
                        if (reportParameters.QUERY == "HASVALUES")
                        {
                            Func<DataRow, bool> condition = row => Convert.ToInt32(row["STOCK_QTY"]) > 0;
                            DataRow[] filteredRows = dataTable.Select().Where(row => condition(row)).ToArray();
                            DataTable newTable = dataTable.Clone();
                            foreach (DataRow row in filteredRows)
                            {
                                newTable.ImportRow(row);
                            }
                            reportDataSource1.Value = newTable;

                        }
                        else if (reportParameters.QUERY == "HASNOVALUES")
                        {
                            Func<DataRow, bool> condition = row => Convert.ToInt32(row["STOCK_QTY"]) <= 0;
                            DataRow[] filteredRows = dataTable.Select().Where(row => condition(row)).ToArray();
                            DataTable newTable = dataTable.Clone();
                            foreach (DataRow row in filteredRows)
                            {
                                newTable.ImportRow(row);
                            }
                            reportDataSource1.Value = newTable;
                        }
                        else
                        {
                            reportDataSource1.Value = dataTable;

                        }
                        DataTable dt = new DataTable();
                        dt.Clear();
                        dt.Columns.Add("DATE_FROM");
                        dt.Columns.Add("DATE_TO");
                        dt.Columns.Add("UNIT_NAME");
                        DataRow _ravi = dt.NewRow();
                        _ravi["DATE_TO"] = reportParameters.DATE_TO;
                        dt.Rows.Add(_ravi);
                        ReportDataSource reportDataSource2 = new ReportDataSource();
                        reportDataSource2.Name = "DateRangeDataSet";
                        reportDataSource2.Value = dt;
                        string reportType = reportParameters.REPORT_EXTENSION;
                        Warning[] warnings;
                        string mimeType;
                        string[] streamids;
                        string encoding;
                        string fileNameExtension = reportType == "Excel" ? "xlsx" : "pdf";
                        byte[] renderedBytes;
                        localReport.DataSources.Add(reportDataSource);
                        localReport.DataSources.Add(reportDataSource1);
                        localReport.DataSources.Add(reportDataSource2);

                        renderedBytes = localReport.Render(reportType, "", out mimeType, out encoding, out fileNameExtension, out streamids, out warnings);

                        return new FileContentResult(renderedBytes, mimeType);

                    }
                    else
                    {
                        return Redirect("/Home/Index");
                    }
                }
                return Redirect("/Home/Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        //Batch Freezing : Stock Report(../Inventory/Report/InventoryReport)
        //public ActionResult BatchFreezing(string q)
        //{
        //    try
        //    {
        //        if (q != null)
        //        {
        //            q = commonservice.Decrypt(q);
        //            ReportParams reportParameters = JsonConvert.DeserializeObject<ReportParams>(q);
        //            if (reportParameters.SECRET_KEY == ProjectConstants.Secret_Key)
        //            {
        //                LocalReport localReport = new LocalReport();
        //                localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/StockReport/BatchFreezingReport.rdlc");
        //                ReportDataSource reportDataSource = new ReportDataSource();
        //                reportDataSource.Name = "BatchFreezingDataSet";
        //                reportDataSource.Value = service.BatchFreezing(reportParameters);

        //                ReportDataSource reportDataSource1 = new ReportDataSource();
        //                reportDataSource1.Name = "DataSet00002";
        //                reportDataSource1.Value = service.GetPageHeaderReport(reportParameters);
        //                DataTable dt = new DataTable();
        //                dt.Clear();
        //                dt.Columns.Add("DATE_FROM");
        //                dt.Columns.Add("DATE_TO");
        //                dt.Columns.Add("UNIT_NAME");
        //                DataRow _ravi = dt.NewRow();
        //                _ravi["DATE_TO"] = reportParameters.DATE_TO;
        //                _ravi["DATE_FROM"] = reportParameters.DATE_FROM;
        //                dt.Rows.Add(_ravi);
        //                ReportDataSource reportDataSource2 = new ReportDataSource();
        //                reportDataSource2.Name = "DateRangeDataSet";
        //                reportDataSource2.Value = dt;
        //                string reportType = reportParameters.REPORT_EXTENSION;
        //                Warning[] warnings;
        //                string mimeType;
        //                string[] streamids;
        //                string encoding;
        //                string fileNameExtension = reportType == "Excel" ? "xlsx" : "pdf";
        //                byte[] renderedBytes;
        //                localReport.DataSources.Add(reportDataSource);
        //                localReport.DataSources.Add(reportDataSource1);
        //                localReport.DataSources.Add(reportDataSource2);
        //                renderedBytes = localReport.Render(reportType, "", out mimeType, out encoding, out fileNameExtension, out streamids, out warnings);
        //                return new FileContentResult(renderedBytes, mimeType);
        //            }
        //            else
        //            {
        //                return Redirect("/Home/Index");
        //            }
        //        }
        //        return Redirect("/Home/Index");
        //    }
        //    catch (Exception exp)
        //    {
        //        throw exp;
        //    }
        //}
        public ActionResult BatchFreezing(string q)
        {
            try
            {
                if (q != null)
                {
                    q = commonservice.Decrypt(q);
                    ReportParams reportParameters = JsonConvert.DeserializeObject<ReportParams>(q);

                    if (reportParameters.SECRET_KEY == ProjectConstants.Secret_Key)
                    {

                        LocalReport localReport = new LocalReport();
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/StockReport/CurrentBatchFreeezReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "BatchWiseStockDataSet";
                        reportDataSource1.Value = service.GetCurrentBatchFreeezReport(reportParameters);
                        DataTable dt = new DataTable();
                        dt.Clear();
                        dt.Columns.Add("DATE_FROM");
                        dt.Columns.Add("DATE_TO");
                        dt.Columns.Add("UNIT_NAME");
                        DataRow _ravi = dt.NewRow();
                        _ravi["DATE_TO"] = reportParameters.DATE_TO;
                        dt.Rows.Add(_ravi);
                        ReportDataSource reportDataSource2 = new ReportDataSource();
                        reportDataSource2.Name = "DateRangeDataSet";
                        reportDataSource2.Value = dt;
                        string reportType = reportParameters.REPORT_EXTENSION;
                        Warning[] warnings;
                        string mimeType;
                        string[] streamids;
                        string encoding;
                        string fileNameExtension = reportType == "Excel" ? "xlsx" : "pdf";
                        byte[] renderedBytes;
                        localReport.DataSources.Add(reportDataSource);
                        localReport.DataSources.Add(reportDataSource1);
                        localReport.DataSources.Add(reportDataSource2);

                        renderedBytes = localReport.Render(reportType, "", out mimeType, out encoding, out fileNameExtension, out streamids, out warnings);

                        return new FileContentResult(renderedBytes, mimeType);

                    }
                    else
                    {
                        return Redirect("/Home/Index");
                    }
                }
                return Redirect("/Home/Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public ActionResult BatchFreezingTransection(string q)
        {
            try
            {
                if (q != null)
                {
                    q = commonservice.Decrypt(q);
                    ReportParams reportParameters = JsonConvert.DeserializeObject<ReportParams>(q);
                    if (reportParameters.SECRET_KEY == ProjectConstants.Secret_Key)
                    {
                        LocalReport localReport = new LocalReport();
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/StockReport/BatchFreezingTransectionReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "BatchFreezingDataSet";
                        reportDataSource.Value = service.BatchFreezingTansection(reportParameters);

                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "DataSet00002";
                        reportDataSource1.Value = service.GetPageHeaderReport(reportParameters);

                        DataTable dt = new DataTable();
                        dt.Clear();
                        dt.Columns.Add("DATE_FROM");
                        dt.Columns.Add("DATE_TO");
                        dt.Columns.Add("UNIT_NAME");
                        DataRow _ravi = dt.NewRow();
                        _ravi["DATE_TO"] = reportParameters.DATE_TO;
                        _ravi["DATE_FROM"] = reportParameters.DATE_FROM;
                        dt.Rows.Add(_ravi);
                        ReportDataSource reportDataSource2 = new ReportDataSource();
                        reportDataSource2.Name = "DateRangeDataSet";
                        reportDataSource2.Value = dt;
                        string reportType = reportParameters.REPORT_EXTENSION;
                        Warning[] warnings;
                        string mimeType;
                        string[] streamids;
                        string encoding;
                        string fileNameExtension = reportType == "Excel" ? "xlsx" : "pdf";
                        byte[] renderedBytes;
                        localReport.DataSources.Add(reportDataSource);
                        localReport.DataSources.Add(reportDataSource1);
                        localReport.DataSources.Add(reportDataSource2);
                        renderedBytes = localReport.Render(reportType, "", out mimeType, out encoding, out fileNameExtension, out streamids, out warnings);
                        return new FileContentResult(renderedBytes, mimeType);
                    }
                    else
                    {
                        return Redirect("/Home/Index");
                    }
                }
                return Redirect("/Home/Index");
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }
        //Stock Adjustment : Stock Report(../Inventory/Report/InventoryReport)
        public ActionResult StockAdjustment(string q)
        {
            try
            {
                if (q != null)
                {
                    q = commonservice.Decrypt(q);
                    ReportParams reportParameters = JsonConvert.DeserializeObject<ReportParams>(q);
                    if (reportParameters.SECRET_KEY == ProjectConstants.Secret_Key)
                    {
                        LocalReport localReport = new LocalReport();
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/StockReport/StockAdjustmentReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "StockAdjustmentDataSet";
                        reportDataSource.Value = service.StockAdjustment(reportParameters);

                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "DataSet00002";
                        reportDataSource1.Value = service.GetPageHeaderReport(reportParameters);

                        string reportType = reportParameters.REPORT_EXTENSION;
                        Warning[] warnings;
                        string mimeType;
                        string[] streamids;
                        string encoding;
                        string fileNameExtension = reportType == "Excel" ? "xlsx" : "pdf";
                        byte[] renderedBytes;
                        localReport.DataSources.Add(reportDataSource);
                        localReport.DataSources.Add(reportDataSource1);
                        renderedBytes = localReport.Render(reportType, "", out mimeType, out encoding, out fileNameExtension, out streamids, out warnings);
                        return new FileContentResult(renderedBytes, mimeType);
                    }
                    else
                    {
                        return Redirect("/Home/Index");
                    }
                }
                return Redirect("/Home/Index");
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }
    }



}
