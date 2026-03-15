using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using SalesAndDistributionSystem.Domain.Models.ReportModels.Common.ProductReports;
using SalesAndDistributionSystem.Reporting.Areas.Inventory.Data;
using SalesAndDistributionSystem.Reporting.ReportConst;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SalesAndDistributionSystem.Reporting.Areas.Inventory.Controllers
{
    public class GiftItemReportController : Controller
    {
        private readonly GiftItemReport _service = new GiftItemReport();
        private readonly CommonServices _commonservice = new CommonServices();

        public ActionResult GiftReceive(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/GiftItemReport/GiftItemReceive.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "FGReceiveDataSet";
                        reportDataSource.Value = _service.GiftReceive(reportParameters);

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
            catch (Exception exp)
            {
                throw exp;
            }
        }

        public ActionResult GiftRegister(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/GiftItemReport/GiftReceiveRegister.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "ReceiveRegister";
                        reportDataSource.Value = _service.GiftReceiveRegister(reportParameters);

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
            catch (Exception exp)
            {
                throw exp;
            }
        }
        public ActionResult GiftStock(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/GiftItemReport/GiftStockReport.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "GiftStock";
                        reportDataSource.Value = _service.GiftStockRegister(reportParameters);

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
            catch (Exception exp)
            {
                throw exp;
            }
        }
    }
}