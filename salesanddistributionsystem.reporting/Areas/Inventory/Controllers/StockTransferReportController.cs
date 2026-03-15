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
using System.Web.Services.Description;
using System.Xml.Linq;

namespace SalesAndDistributionSystem.Reporting.Areas.Inventory.Controllers
{
    public class StockTransferReportController : Controller
    {
        private readonly StockTransferReport _service = new StockTransferReport();
        private readonly CommonServices _commonservice = new CommonServices();
        private string dbName = string.Empty;


        // GET: Inventory/StockTransfer
        public ActionResult Transfer(string q)
        {
            try
            {
                if (q != null)
                {
                    q = _commonservice.Decrypt(q);
                    ReportParams reportParameters = JsonConvert.DeserializeObject<ReportParams>(q);
                    if (reportParameters.SECRET_KEY == ProjectConstants.Secret_Key)
                    {
                        dbName = reportParameters.DB;
                        LocalReport localReport = new LocalReport();
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/StockTransferReport/StockTransferReportV3.rdlc");
                        
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = _service.GetPageHeaderReport(reportParameters);


                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "StockTransferDataSet";
                        reportDataSource1.Value = _service.StockTransferReportDataV2(reportParameters);

                     
                        localReport.SubreportProcessing += new SubreportProcessingEventHandler(LocalReport_SubreportProcessing);
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
        //Sub Report For Transfer : Stock Transfer Report(../Inventory/Report/StockTransferReport)
        void LocalReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            // get MST_ID,DTL_ID from the parameters
            string MST_ID = e.Parameters[0].Values[0];
            string DTL_ID = e.Parameters[1].Values[0];
            // remove all previously attached Datasources, since we want to attach a
            // new one
            e.DataSources.Clear();

            // add retrieved dataset or you can call it list to data source
            e.DataSources.Add(new ReportDataSource()
            {
                Name = "StockTransferSubDataSet",
                Value = _service.StockTransferSubReportData(new ReportParams() { DB = dbName, MST_ID = MST_ID, DTL_ID= DTL_ID })
            });


        }
        public ActionResult Receive(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/StockTransferReport/StockTransferReceiveReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "StockTransferDataSet";
                        reportDataSource.Value = _service.StockTransferReceiveReportData(reportParameters);

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