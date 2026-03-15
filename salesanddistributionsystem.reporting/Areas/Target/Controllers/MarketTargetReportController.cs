using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using SalesAndDistributionSystem.Domain.Models.ReportModels.Common.ProductReports;
using SalesAndDistributionSystem.Reporting.Areas.Inventory.Data;
using SalesAndDistributionSystem.Reporting.Areas.Target.Data;
using SalesAndDistributionSystem.Reporting.ReportConst;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace SalesAndDistributionSystem.Reporting.Areas.Target.Controllers
{
    public class MarketTargetReportController : Controller
    {
        private readonly TargetReport _service = new TargetReport();
        private readonly CommonServices _commonservice = new CommonServices();
        // GET: Target/MarketTarget
        public ActionResult Target(string q)
        {
            //try
            {
                if (q != null)
                {
                    q = _commonservice.Decrypt(q);
                    ReportParams reportParameters = JsonConvert.DeserializeObject<ReportParams>(q);

                    if (reportParameters.SECRET_KEY == ProjectConstants.Secret_Key)
                    {
                        LocalReport localReport = new LocalReport();
                        localReport.ReportPath = Server.MapPath("~/Areas/Target/Report/CustomerTargetReport.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "TargetReportDataSet";
                        reportDataSource.Value = _service.TargetData(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "DataSet00002";
                        reportDataSource1.Value = _service.GetPageHeaderReport(reportParameters);

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
            //catch (Exception exp)
            //{
            //    throw exp;
            //}
        }
        public ActionResult InMarketSales(string q)
        {
            try
            {
                if (q != null)
                {
                    q = _commonservice.Decrypt(q);
                    ReportParams reportParameters = JsonConvert.DeserializeObject<ReportParams>(q);
                    if (reportParameters.SECRET_KEY == ProjectConstants.Secret_Key)
                    {
                        LocalReport localReport = new LocalReport();
                        localReport.ReportPath = Server.MapPath("~/Areas/Target/Report/InMarketSalesReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "InMarketSalesDataSet";
                        reportDataSource.Value = _service.GetInMarketSales(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "DataSet00002";
                        reportDataSource1.Value = _service.GetPageHeaderReport(reportParameters);

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
            catch (Exception exp) { 
                throw exp;
            }
        }
        // National/ Plant/ Division/ Region/ Distributor wise Ordering requirement report : Target Report(..Target/Report/TargetReport)
        public ActionResult TargetSummary(string q)
        {
            try
            {
                if (q != null)
                {
                    q = _commonservice.Decrypt(q);
                    ReportParams reportParameters = JsonConvert.DeserializeObject<ReportParams>(q);

                    if (reportParameters.SECRET_KEY == ProjectConstants.Secret_Key)
                    {
                        LocalReport localReport = new LocalReport();
                        localReport.ReportPath = Server.MapPath("~/Areas/Target/Report/TargetSummary.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = _service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "TargetSummaryDataSet";
                        reportDataSource1.Value = _service.TargetSummary(reportParameters);
                        //DateTime now = DateTime.Now;
                        //    //DateTime.ParseExact(reportParameters.INVOICE_DATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        //var startDate = new DateTime(now.Year, now.Month, 1);
                        DataTable dt = new DataTable();
                        dt.Clear();
                        dt.Columns.Add("DATE_FROM");
                        dt.Columns.Add("DATE_TO");
                        dt.Columns.Add("UNIT_NAME");
                        DataRow _ravi = dt.NewRow();
                        _ravi["DATE_FROM"] = reportParameters.YEAR;
                        _ravi["DATE_TO"] = reportParameters.MONTH_CODE;
                        _ravi["UNIT_NAME"] = reportParameters.UNIT_NAME;
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
        // National/ Plant/ Division/ Region/ Distributor wise Ordering requirement report : Target Report(..Target/Report/TargetReport)
        public ActionResult TargetDetail(string q)
        {
            try
            {
                if (q != null)
                {
                    q = _commonservice.Decrypt(q);
                    ReportParams reportParameters = JsonConvert.DeserializeObject<ReportParams>(q);

                    if (reportParameters.SECRET_KEY == ProjectConstants.Secret_Key)
                    {
                        LocalReport localReport = new LocalReport();
                        localReport.ReportPath = Server.MapPath("~/Areas/Target/Report/TargetDetail.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = _service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "TargetSummaryDataSet";
                        reportDataSource1.Value = _service.TargetDetail(reportParameters);
                        //DateTime now = DateTime.Now;
                        //    //DateTime.ParseExact(reportParameters.INVOICE_DATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        //var startDate = new DateTime(now.Year, now.Month, 1);
                        DataTable dt = new DataTable();
                        dt.Clear();
                        dt.Columns.Add("DATE_FROM");
                        dt.Columns.Add("DATE_TO");
                        dt.Columns.Add("UNIT_NAME");
                        DataRow _ravi = dt.NewRow();
                        _ravi["DATE_FROM"] = reportParameters.YEAR;
                        _ravi["DATE_TO"] = reportParameters.MONTH_CODE;
                        _ravi["UNIT_NAME"] = reportParameters.UNIT_NAME;
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
    }
}