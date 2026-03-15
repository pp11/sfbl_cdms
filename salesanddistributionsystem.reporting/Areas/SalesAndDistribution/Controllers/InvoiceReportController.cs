using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using SalesAndDistributionSystem.Domain.Models.ReportModels.Common.ProductReports;
using SalesAndDistributionSystem.Reporting.Areas.Inventory.Data;
using SalesAndDistributionSystem.Reporting.Areas.SalesAndDistribution.Data;
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
using System.Web.UI.WebControls;

namespace SalesAndDistributionSystem.Reporting.Areas.SalesAndDistribution.Controllers
{
    public class InvoiceReportController : Controller
    {
        private readonly InvoiceReport service = new InvoiceReport();
        private readonly CommonServices commonservice = new CommonServices();
        private string dbName = string.Empty;

        //Sub Report For Invoice (Unit TP wise) : Invoice Report(../SalesAndDistribution/Report/InvoiceReport)
        void LocalReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            // get INVOICE_NO,SKU_ID,BONUS_STATUS from the parameters
            string INVOICE_NO = e.Parameters[0].Values[0];
            string SKU_ID = e.Parameters[1].Values[0];
            string BONUS_STATUS = e.Parameters[2].Values[0];
            // remove all previously attached Datasources, since we want to attach a
            // new one
            e.DataSources.Clear();

            // add retrieved dataset or you can call it list to data source
            e.DataSources.Add(new ReportDataSource()
            {
                Name = "InvoiceSubRptDataSet",
                Value = service.GetTpWiseInvoiceSubReport(new ReportParams() { DB = dbName, INVOICE_NO = INVOICE_NO, SKU_ID = SKU_ID, BONUS_STATUS = BONUS_STATUS })
            });


        }

        // Invoice : Invoice Report(../SalesAndDistribution/Report/InvoiceReport)
        public ActionResult Invoice(string q)
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
                        if(reportParameters.DOT_PRINTER=="YES")
                        {
                            localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/InvoiceReport/InvoiceReportLetter.rdlc");

                        }
                        else
                        {
                            localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/InvoiceReport/InvoiceReport.rdlc");

                        }
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);


                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "InvoiceDataSet";
                        reportDataSource1.Value = service.GetInvoiceReport(reportParameters);
                        DataTable dt = new DataTable();
                        dt.Clear();
                        dt.Columns.Add("DATE_FROM");
                        dt.Columns.Add("DATE_TO");
                        dt.Columns.Add("UNIT_NAME");
                        DataRow _ravi = dt.NewRow();
                        _ravi["DATE_FROM"] = reportParameters.DATE_FROM;
                        _ravi["DATE_TO"] = reportParameters.DATE_TO;
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
                throw ex.InnerException;
            }


        }

        // Invoice (Unit TP wise) : Invoice Report(../SalesAndDistribution/Report/InvoiceReport)
        public ActionResult TpWiseInvoice(string q)
        {
            try
            {

                if (q != null)
                {
                    q = commonservice.Decrypt(q);
                    ReportParams reportParameters = JsonConvert.DeserializeObject<ReportParams>(q);

                    if (reportParameters.SECRET_KEY == ProjectConstants.Secret_Key)
                    {
                        dbName = reportParameters.DB;

                        LocalReport localReport = new LocalReport();

                        if (reportParameters.DOT_PRINTER == "YES")
                        {
                            localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/InvoiceReport/TpWiseInvoiceLetterReport.rdlc");

                        }
                        else
                        {
                            localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/InvoiceReport/TpWiseInvoiceReport.rdlc");

                        }
                        //localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/InvoiceReport/TpWiseInvoiceReportCopy.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);


                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "InvoiceDataSet";
                        //reportDataSource1.Value = service.GetTpWiseInvoiceReport(reportParameters);
                        reportDataSource1.Value = service.GetInvoiceReport(reportParameters);
                        DataTable dt = new DataTable();
                        dt.Clear();
                        dt.Columns.Add("DATE_FROM");
                        dt.Columns.Add("DATE_TO");
                        dt.Columns.Add("UNIT_NAME");
                        DataRow _ravi = dt.NewRow();
                        _ravi["DATE_FROM"] = reportParameters.DATE_FROM;
                        _ravi["DATE_TO"] = reportParameters.DATE_TO;
                        _ravi["UNIT_NAME"] = reportParameters.UNIT_NAME;
                        dt.Rows.Add(_ravi);
                        ReportDataSource reportDataSource2 = new ReportDataSource();
                        reportDataSource2.Name = "DateRangeDataSet";
                        reportDataSource2.Value = dt;
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
                throw ex.InnerException;
            }
        }

        // Delivery Slip : Invoice Report(../SalesAndDistribution/Report/InvoiceReport)
        public ActionResult DeliverySlip(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/InvoiceReport/DeliverySlipReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "DeliverySlip2";
                        reportDataSource1.Value = service.GetDeliverySlipReport(reportParameters);
                        DataTable dt = new DataTable();
                        dt.Clear();
                        dt.Columns.Add("INVOICE_FROM");
                        dt.Columns.Add("INVOICE_TO");
                        dt.Columns.Add("UNIT_NAME");
                        DataRow _ravi = dt.NewRow();
                        _ravi["INVOICE_FROM"] = reportParameters.INVOICE_NO_FROM;
                        _ravi["INVOICE_TO"] = reportParameters.INVOICE_NO_TO;
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

        // Date Wise Sales Register : Invoice Report(../SalesAndDistribution/Report/InvoiceReport)
        public ActionResult SalesRegister(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/InvoiceReport/SalesRegisterReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "SalesRegisterDataSet";
                        reportDataSource1.Value = service.GetSalesRegisterReport(reportParameters);
                        DataTable dt = new DataTable();
                        dt.Clear();
                        dt.Columns.Add("DATE_FROM");
                        dt.Columns.Add("DATE_TO");
                        dt.Columns.Add("UNIT_NAME");
                        DataRow _ravi = dt.NewRow();
                        _ravi["DATE_FROM"] = reportParameters.DATE_FROM;
                        _ravi["DATE_TO"] = reportParameters.DATE_TO;
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

        // Invoice Wise Delivery Slip : Invoice Report(../SalesAndDistribution/Report/InvoiceReport)
        public ActionResult InvoiceWiseDeliverySlip(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/InvoiceReport/DeliverySlipInvoiceWiseReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "DeliverySlip2";
                        reportDataSource1.Value = service.InvoiceWiseDeliverySlip(reportParameters);
                        DataTable dt = new DataTable();
                        dt.Clear();
                        dt.Columns.Add("INVOICE_FROM");
                        dt.Columns.Add("INVOICE_TO");
                        dt.Columns.Add("UNIT_NAME");
                        dt.Columns.Add("DATE_FROM");
                        DataRow _ravi = dt.NewRow();
                        _ravi["INVOICE_FROM"] = reportParameters.INVOICE_NO_FROM;
                        _ravi["INVOICE_TO"] = reportParameters.INVOICE_NO_TO;
                        _ravi["UNIT_NAME"] = reportParameters.UNIT_NAME;
                        _ravi["DATE_FROM"] = reportParameters.INVOICE_DATE;
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

        //Customer Ledger (Central) : Invoice Report(../SalesAndDistribution/Report/InvoiceReport)
        public ActionResult DistributorLadger(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/InvoiceReport/DistributorLadgerReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "SalesRegisterDataSet";
                        //reportDataSource1.Value = service.GetSalesRegisterReport(reportParameters);
                        DataTable dt = new DataTable();
                        dt.Clear();
                        dt.Columns.Add("DATE_FROM");
                        dt.Columns.Add("DATE_TO");
                        dt.Columns.Add("UNIT_NAME");
                        DataRow _ravi = dt.NewRow();
                        _ravi["DATE_FROM"] = reportParameters.DATE_FROM;
                        _ravi["DATE_TO"] = reportParameters.DATE_TO;
                        _ravi["UNIT_NAME"] = reportParameters.UNIT_NAME;
                        dt.Rows.Add(_ravi);
                        ReportDataSource reportDataSource2 = new ReportDataSource();
                        reportDataSource2.Name = "DateRangeDataSet";
                        reportDataSource2.Value = dt;
                        ReportDataSource reportDataSource3 = new ReportDataSource();
                        reportDataSource3.Name = "DistributorLadgerDataSet";
                        reportDataSource3.Value = service.GetDistributorLadgerReport(reportParameters);

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
                        localReport.DataSources.Add(reportDataSource3);


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

        //Customer Ledger V2(Central) : Invoice Report(../SalesAndDistribution/Report/InvoiceReport)
        public ActionResult DistributorLadgerV2(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/InvoiceReport/DistributorLadgerReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "SalesRegisterDataSet";
                        //reportDataSource1.Value = service.GetSalesRegisterReport(reportParameters);
                        DataTable dt = new DataTable();
                        dt.Clear();
                        dt.Columns.Add("DATE_FROM");
                        dt.Columns.Add("DATE_TO");
                        dt.Columns.Add("UNIT_NAME");
                        DataRow _ravi = dt.NewRow();
                        _ravi["DATE_FROM"] = reportParameters.DATE_FROM;
                        _ravi["DATE_TO"] = reportParameters.DATE_TO;
                        _ravi["UNIT_NAME"] = reportParameters.UNIT_NAME;
                        dt.Rows.Add(_ravi);
                        ReportDataSource reportDataSource2 = new ReportDataSource();
                        reportDataSource2.Name = "DateRangeDataSet";
                        reportDataSource2.Value = dt;
                        ReportDataSource reportDataSource3 = new ReportDataSource();
                        reportDataSource3.Name = "DistributorLadgerDataSet";
                        reportDataSource3.Value = service.GetDistributorLadgerReportV2(reportParameters);

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
                        localReport.DataSources.Add(reportDataSource3);


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

        //Customer Ledger (Unit Wise) : Invoice Report(../SalesAndDistribution/Report/InvoiceReport)
        public ActionResult UnitWiseDistributorLadger(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/InvoiceReport/UnitWiseDistributorLadgerReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "DistributorLadgerDataSet";
                        reportDataSource1.Value = service.GetUnitWiseDistributorLadgerReport(reportParameters);

                        DataTable dt = new DataTable();
                        dt.Clear();
                        dt.Columns.Add("DATE_FROM");
                        dt.Columns.Add("DATE_TO");
                        dt.Columns.Add("UNIT_NAME");
                        DataRow _ravi = dt.NewRow();
                        _ravi["DATE_FROM"] = reportParameters.DATE_FROM;
                        _ravi["DATE_TO"] = reportParameters.DATE_TO;
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

        //Order : Invoice Report(../SalesAndDistribution/Report/InvoiceReport)
        public ActionResult Order(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/InvoiceReport/OrderReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);


                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "InvoiceDataSet";
                        reportDataSource1.Value = service.GetOrderReport(reportParameters);
                        DataTable dt = new DataTable();
                        dt.Clear();
                        dt.Columns.Add("DATE_FROM");
                        dt.Columns.Add("DATE_TO");
                        dt.Columns.Add("UNIT_NAME");
                        DataRow _ravi = dt.NewRow();
                        _ravi["DATE_FROM"] = reportParameters.DATE_FROM;
                        _ravi["DATE_TO"] = reportParameters.DATE_TO;
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
                throw ex.InnerException;
            }


        }

        //Customer Outstanding : Invoice Report(../SalesAndDistribution/Report/InvoiceReport)
        public ActionResult CustomerOutstanding(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/InvoiceReport/CustomerOutstandingReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "CustomerOutstandingDataSet";
                        reportDataSource1.Value = service.GetCustomerOutstandingReport(reportParameters);

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
        //Customer Outstanding V2 (Central) : Invoice Report(../SalesAndDistribution/Report/InvoiceReport)
        public ActionResult CustomerOutstandingV2(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/InvoiceReport/CustomerOutstandingReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "CustomerOutstandingDataSet";
                        reportDataSource1.Value = service.GetCustomerOutstandingReportV2(reportParameters);

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

        //Customer Outstanding With Invoice: Invoice Report(../SalesAndDistribution/Report/InvoiceReport) 
        public ActionResult CustomerOutstandingWithInvoice(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/InvoiceReport/InvoiceWiseOutstandingReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "CustomerOutstandingWithInvoiceDataSet";
                        reportDataSource1.Value = service.GetCustomerOutstandingWithInvoiceReport(reportParameters);
                        DateTime now = DateTime.ParseExact(reportParameters.INVOICE_DATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        var startDate = new DateTime(now.Year, now.Month, 1);

                        DataTable dt = new DataTable();
                        dt.Clear();
                        dt.Columns.Add("DATE_FROM");
                        dt.Columns.Add("DATE_TO");
                        dt.Columns.Add("UNIT_NAME");
                        DataRow _ravi = dt.NewRow();
                        _ravi["DATE_FROM"] = startDate.ToString("dd/MM/yyyy");
                        _ravi["DATE_TO"] = reportParameters.INVOICE_DATE;
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
        //Invoice Wise Outstanding V2(Central): Invoice Report(../SalesAndDistribution/Report/InvoiceReport) 
        public ActionResult CustomerOutstandingWithInvoiceV2(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/InvoiceReport/InvoiceWiseOutstandingReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "CustomerOutstandingWithInvoiceDataSet";
                        reportDataSource1.Value = service.GetCustomerOutstandingWithInvoiceReportV2(reportParameters);
                        DateTime now = DateTime.ParseExact(reportParameters.INVOICE_DATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        var startDate = new DateTime(now.Year, now.Month, 1);

                        DataTable dt = new DataTable();
                        dt.Clear();
                        dt.Columns.Add("DATE_FROM");
                        dt.Columns.Add("DATE_TO");
                        dt.Columns.Add("UNIT_NAME");
                        DataRow _ravi = dt.NewRow();
                        _ravi["DATE_FROM"] = startDate.ToString("dd/MM/yyyy");
                        _ravi["DATE_TO"] = reportParameters.INVOICE_DATE;
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

        //Customer Wise Outstanding (End Date Selection) : Invoice Report(../SalesAndDistribution/Report/InvoiceReport)
        public ActionResult CustOutstandingDtl(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/InvoiceReport/CustOutstandingTillDateReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "CustOutstandingDtlDataSet";
                        reportDataSource1.Value = service.GetCustOutstandingDtlReport(reportParameters);

                        DateTime now = DateTime.ParseExact(reportParameters.INVOICE_DATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        var startDate = new DateTime(now.Year, now.Month, 1);

                        DataTable dt = new DataTable();
                        dt.Clear();
                        dt.Columns.Add("DATE_FROM");
                        dt.Columns.Add("DATE_TO");
                        dt.Columns.Add("UNIT_NAME");
                        DataRow _ravi = dt.NewRow();
                        _ravi["DATE_FROM"] =  startDate.ToString("dd/MM/yyyy");
                        _ravi["DATE_TO"] = reportParameters.INVOICE_DATE;
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
        //Customer Wise Outstanding V2(Details) : Invoice Report(../SalesAndDistribution/Report/InvoiceReport)
        public ActionResult CustOutstandingDtlV2(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/InvoiceReport/CustOutstandingTillDateReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "CustOutstandingDtlDataSet";
                        reportDataSource1.Value = service.GetCustOutstandingDtlReportV2(reportParameters);

                        DateTime now = DateTime.ParseExact(reportParameters.INVOICE_DATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        var startDate = new DateTime(now.Year, now.Month, 1);

                        DataTable dt = new DataTable();
                        dt.Clear();
                        dt.Columns.Add("DATE_FROM");
                        dt.Columns.Add("DATE_TO");
                        dt.Columns.Add("UNIT_NAME");
                        DataRow _ravi = dt.NewRow();
                        _ravi["DATE_FROM"] = startDate.ToString("dd/MM/yyyy");
                        _ravi["DATE_TO"] = reportParameters.INVOICE_DATE;
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
        public ActionResult CustOutstandingDtlV2Ck(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/InvoiceReport/CustOutstandingTillDateReportCHK.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "CustOutstandingDtlDataSet";
                        reportDataSource1.Value = service.GetCustOutstandingDtlReportV2CK(reportParameters);

                        DateTime now = DateTime.ParseExact(reportParameters.INVOICE_DATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        var startDate = new DateTime(now.Year, now.Month, 1);

                        DataTable dt = new DataTable();
                        dt.Clear();
                        dt.Columns.Add("DATE_FROM");
                        dt.Columns.Add("DATE_TO");
                        dt.Columns.Add("UNIT_NAME");
                        DataRow _ravi = dt.NewRow();
                        _ravi["DATE_FROM"] = reportParameters.DATE_FROM;
                        _ravi["DATE_TO"] = reportParameters.DATE_TO;
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
        public ActionResult OrderVsInvoice(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/InvoiceReport/OrderVsInvoiceReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "OrderVsInvoiceDataSet";
                        reportDataSource1.Value = service.OrderVsInvoice(reportParameters);
                        DataTable dt = new DataTable();
                        dt.Clear();
                        dt.Columns.Add("DATE_FROM");
                        dt.Columns.Add("DATE_TO");
                        dt.Columns.Add("UNIT_NAME");
                        DataRow _ravi = dt.NewRow();
                        _ravi["DATE_FROM"] = reportParameters.DATE_FROM;
                        _ravi["DATE_TO"] = reportParameters.DATE_TO;
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