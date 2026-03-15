using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using NUnit.Framework.Interfaces;
using SalesAndDistributionSystem.Domain.Models.ReportModels.Common.ProductReports;
using SalesAndDistributionSystem.Reporting.Areas.Inventory.Data;
using SalesAndDistributionSystem.Reporting.Models;
using SalesAndDistributionSystem.Reporting.ReportConst;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Xml.Linq;

namespace SalesAndDistributionSystem.Reporting.Areas.Inventory.Controllers
{
    public class DistributionReportController : Controller
    {
        private readonly DistributionReport _service = new DistributionReport();
        private readonly CommonServices _commonservice = new CommonServices();
        private string dbName = "";

        //Dispatch Challan (Depot) [deprecated]: Distribution Report(../Inventory/Report/DistributionReport)
        public ActionResult DispatchChallan(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/Distribution/DispatchChallan.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DispatchRequisitionDataSet";
                        reportDataSource.Value = _service.DispatchChallan(reportParameters);

                        ReportDataSource reportDataSource2 = new ReportDataSource();
                        reportDataSource2.Name = "DispatchProductSummeryDataSet";
                        reportDataSource2.Value = _service.DispatchProductSummery(reportParameters);

                        ReportDataSource reportDataSource3 = new ReportDataSource();
                        reportDataSource3.Name = "DataSet1";
                        reportDataSource3.Value = _service.DispatchMasterData(reportParameters);

                        ReportDataSource reportDataSource4 = new ReportDataSource();
                        reportDataSource4.Name = "DispatchReqProductDataSet";
                        reportDataSource4.Value = _service.DispatchReqProducts(reportParameters);

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
                        localReport.DataSources.Add(reportDataSource2);
                        localReport.DataSources.Add(reportDataSource3);
                        localReport.DataSources.Add(reportDataSource4);
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

        //Dispatch Challan (Depot) v.2: Distribution Report(../Inventory/Report/DistributionReport)
        public ActionResult DispatchChallanV2(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/Distribution/DispatchChallanV2.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DispatchChallanDataSet";
                        reportDataSource.Value = _service.DispatchChallanV2(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "DataSet00002";
                        reportDataSource1.Value = _service.GetPageHeaderReport(reportParameters);
                        localReport.SubreportProcessing += new SubreportProcessingEventHandler(LocalReport_DispatchSubreport);
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

        //Sub Report For Dispatch Challan v.2 : Distribution Report(../Inventory/Report/DistributionReport)
        void LocalReport_DispatchSubreport(object sender, SubreportProcessingEventArgs e)
        {
            // get MST_ID,DTL_ID from the parameters
            string MST_ID = e.Parameters[0].Values[0];
            string DISPATCH_PRODUCT_ID = e.Parameters[1].Values[0];
            // remove all previously attached Datasources, since we want to attach a
            // new one
            e.DataSources.Clear();

            // add retrieved dataset or you can call it list to data source
            e.DataSources.Add(new ReportDataSource()
            {
                Name = "StockTransferSubDataSet",
                Value = _service.DispatchChallanSubReportData(new ReportParams() { DB = dbName, MST_ID = MST_ID, DTL_ID = DISPATCH_PRODUCT_ID })
            });
        }

        //Delivery Challan(Customer): Distribution Report(../Inventory/Report/DistributionReport)
        public ActionResult DistributionChallan(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/Distribution/DeliveryChallan.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "CustomerInvoice";
                        reportDataSource.Value = _service.CustomerInvoiceByDistNo(reportParameters);

                        ReportDataSource reportDataSource2 = new ReportDataSource();
                        reportDataSource2.Name = "PickerProducts";
                        reportDataSource2.Value = _service.PickerProductsByDistNo(reportParameters);

                        ReportDataSource reportDataSource3 = new ReportDataSource();
                        reportDataSource3.Name = "HelperProducts";
                        reportDataSource3.Value = _service.HelperProductsByDistNo(reportParameters);

                        ReportDataSource reportDataSource4 = new ReportDataSource();
                        reportDataSource4.Name = "DeliveryChallan";
                        reportDataSource4.Value = _service.CustomerInvoice(reportParameters);


                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "DataSet00002";
                        reportDataSource1.Value = _service.GetPageHeaderReport(reportParameters);

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
                        localReport.DataSources.Add(reportDataSource3);
                        localReport.DataSources.Add(reportDataSource4);
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
        //Separate Delivery Challan (Customer) : Distribution Report(../Inventory/Report/DistributionReport)
        public ActionResult DeliveryChallan(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/Distribution/DeliveryChallanRptV2.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "CustomerInvoice";
                        reportDataSource.Value = _service.CustomerInvoiceByDistNo(reportParameters);

                        ReportDataSource reportDataSource4 = new ReportDataSource();
                        reportDataSource4.Name = "DeliveryChallan";
                        reportDataSource4.Value = _service.CustomerInvoice(reportParameters);

                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "DataSet00002";
                        reportDataSource1.Value = _service.GetPageHeaderReport(reportParameters);

                        localReport.SubreportProcessing += new SubreportProcessingEventHandler(LocalReport_SubreportProcessingV2);

                        string reportType = reportParameters.REPORT_EXTENSION;
                        Warning[] warnings;
                        string mimeType;
                        string[] streamids;
                        string encoding;
                        string fileNameExtension = reportType == "Excel" ? "xlsx" : "pdf";
                        byte[] renderedBytes;
                        localReport.DataSources.Add(reportDataSource);
                        localReport.DataSources.Add(reportDataSource1);
                        localReport.DataSources.Add(reportDataSource4);
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
        //Picker Challan(Customer): Distribution Report(../Inventory/Report/DistributionReport)
        public ActionResult PickerChallan(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/Distribution/PickerChallanRpt.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "CustomerInvoice";
                        reportDataSource.Value = _service.CustomerInvoiceByDistNo(reportParameters);

                        ReportDataSource reportDataSource2 = new ReportDataSource();
                        reportDataSource2.Name = "PickerProducts";
                        reportDataSource2.Value = _service.PickerProductsByDistNo(reportParameters);

                        ReportDataSource reportDataSource3 = new ReportDataSource();
                        reportDataSource3.Name = "HelperProducts";
                        reportDataSource3.Value = _service.HelperProductsByDistNo(reportParameters);

                        ReportDataSource reportDataSource4 = new ReportDataSource();
                        reportDataSource4.Name = "DeliveryChallan";
                        reportDataSource4.Value = _service.CustomerInvoice(reportParameters);


                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "DataSet00002";
                        reportDataSource1.Value = _service.GetPageHeaderReport(reportParameters);

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
                        localReport.DataSources.Add(reportDataSource3);
                        localReport.DataSources.Add(reportDataSource4);
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
        //Helper Challan(Customer): Distribution Report(../Inventory/Report/DistributionReport)
        public ActionResult HelperChallan(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/Distribution/HelperChallanRpt.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "CustomerInvoice";
                        reportDataSource.Value = _service.CustomerInvoiceByDistNo(reportParameters);

                        ReportDataSource reportDataSource2 = new ReportDataSource();
                        reportDataSource2.Name = "PickerProducts";
                        reportDataSource2.Value = _service.PickerProductsByDistNo(reportParameters);

                        ReportDataSource reportDataSource3 = new ReportDataSource();
                        reportDataSource3.Name = "HelperProducts";
                        reportDataSource3.Value = _service.HelperProductsByDistNo(reportParameters);

                        ReportDataSource reportDataSource4 = new ReportDataSource();
                        reportDataSource4.Name = "DeliveryChallan";
                        reportDataSource4.Value = _service.CustomerInvoice(reportParameters);


                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "DataSet00002";
                        reportDataSource1.Value = _service.GetPageHeaderReport(reportParameters);

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
                        localReport.DataSources.Add(reportDataSource3);
                        localReport.DataSources.Add(reportDataSource4);
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

        //Delivery Challan(Customer)- Sub Report
        void LocalReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            // get empID from the parameters
            string distributionNo = e.Parameters[0].Values[0];
            string db = e.Parameters[1].Values[0];
            string customerId = e.Parameters[2].Values[0];
            // remove all previously attached Datasources, since we want to attach a
            // new one
            e.DataSources.Clear();

            // add retrieved dataset or you can call it list to data source
            e.DataSources.Add(new ReportDataSource()
            {
                Name = "DeliveryProducts",
                Value = _service.DeliveryProductsByCustAndDist(new ReportParams() { DISPATCH_NO = distributionNo, DB = db, CUSTOMER_ID = customerId })
            });

            e.DataSources.Add(new ReportDataSource()
            {
                Name = "GiftItems",
                Value = _service.DeliveryGiftByCustAndDist(new ReportParams() { DISPATCH_NO = distributionNo, DB = db, CUSTOMER_ID = customerId })
            });
        }
        //Delivery Challan(Customer)- Sub Report
        void LocalReport_SubreportProcessingV2(object sender, SubreportProcessingEventArgs e)
        {
           
        }

        //Invoice Delivery Pending (Details): Distribution Report(../Inventory/Report/DistributionReport)
        public ActionResult PendingInvoices(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/Distribution/PendingInvoices.rdlc");
                        dbName = reportParameters.DB;

                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "DataSet00002";
                        reportDataSource1.Value = _service.GetPageHeaderReport(reportParameters);

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "PendingInvoices";
                        var data = _service.GetPendingInvoices(reportParameters).ToList<Invoice>();

                        if (!string.IsNullOrEmpty(reportParameters.CUSTOMER_ID) && reportParameters.CUSTOMER_ID != "undefined")
                        {
                            data = data.Where(e => e.CUSTOMER_ID == reportParameters.CUSTOMER_ID).ToList();
                        }

                        if (!string.IsNullOrEmpty(reportParameters.INVOICE_NO) && reportParameters.INVOICE_NO != "undefined")
                        {
                            data = data.Where(e => reportParameters.INVOICE_NO.Contains(e.INVOICE_NO)).ToList();
                        }

                        reportDataSource.Value = data.ToDataTable();

                        localReport.SubreportProcessing += new SubreportProcessingEventHandler(LocalReport_PendingGift);

                        string reportType = reportParameters.REPORT_EXTENSION;
                        Warning[] warnings;
                        string mimeType;
                        string[] streamids;
                        string encoding;
                        string fileNameExtension = reportType == "Excel" ? "xlsx" : "pdf";
                        byte[] renderedBytes;
                        localReport.DataSources.Add(reportDataSource);
                        localReport.DataSources.Add(reportDataSource1);
                        //localReport.DataSources.Add(reportDataSource2);
                        //localReport.DataSources.Add(reportDataSource3);
                        //localReport.DataSources.Add(reportDataSource4);
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

        //Invoice Delivery Pending(Details) Subreport
        void LocalReport_PendingGift(object sender, SubreportProcessingEventArgs e)
        {
            // get empID from the parameters
            string invoiceNo = e.Parameters[0].Values[0];
            string unitId = e.Parameters[1].Values[0];
            int companyId = Convert.ToInt32(e.Parameters[2].Values[0]);
            //int companyId = Convert.ToInt32(e.Parameters[3].Values[0]);
            // remove all previously attached Datasources, since we want to attach a
            // new one
            e.DataSources.Clear();

            //// add retrieved dataset or you can call it list to data source
            e.DataSources.Add(new ReportDataSource()
            {
                Name = "PendingGift",
                Value = _service.GetPendingGift(new ReportParams() { COMPANY_ID = companyId, DB = dbName, UNIT_ID = unitId, INVOICE_NO_FROM = invoiceNo })
            });

            //e.DataSources.Add(new ReportDataSource()
            //{
            //    Name = "GiftItems",
            //    Value = _service.DeliveryGiftByCustAndDist(new ReportParams() { DISPATCH_NO = distributionNo, DB = db, CUSTOMER_ID = customerId })
            //});
        }

        //SKU wise Delivery pending: Distribution Report(../Inventory/Report/DistributionReport)
        public ActionResult SkuWisePendingDelivery(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/Distribution/SkuWisePendingDelivery.rdlc");
                        dbName = reportParameters.DB;

                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "DataSet00002";
                        reportDataSource1.Value = _service.GetPageHeaderReport(reportParameters);

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "PendingInvoicesDataSet";
                        reportDataSource.Value = _service.GetSkuWisePending(reportParameters);/*.ToList<Invoice>();*/
                        //localReport.SubreportProcessing += new SubreportProcessingEventHandler(LocalReport_PendingGift);

                        string reportType = reportParameters.REPORT_EXTENSION;
                        Warning[] warnings;
                        string mimeType;
                        string[] streamids;
                        string encoding;
                        string fileNameExtension = reportType == "Excel" ? "xlsx" : "pdf";
                        byte[] renderedBytes;
                        localReport.DataSources.Add(reportDataSource);
                        localReport.DataSources.Add(reportDataSource1);
                        //localReport.DataSources.Add(reportDataSource2);
                        //localReport.DataSources.Add(reportDataSource3);
                        //localReport.DataSources.Add(reportDataSource4);
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

        //Date Wise Invoice Delivery Pending (List): Distribution Report(../Inventory/Report/DistributionReport)
        public ActionResult DateWisePendingDelivery(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/Distribution/DateWisePendingDelivery.rdlc");
                        dbName = reportParameters.DB;
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "DataSet00002";
                        reportDataSource1.Value = _service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "InvoiceWisePendingDataSet";
                        var data = _service.GetDateWisePendingInvoices(reportParameters).ToList<Invoice>();
                        if (!string.IsNullOrEmpty(reportParameters.CUSTOMER_ID) && reportParameters.CUSTOMER_ID != "undefined")
                        {
                            data = data.Where(e => e.CUSTOMER_ID == reportParameters.CUSTOMER_ID).ToList();
                        }
                        if (!string.IsNullOrEmpty(reportParameters.INVOICE_NO) && reportParameters.INVOICE_NO != "undefined")
                        {
                            data = data.Where(e => reportParameters.INVOICE_NO.Contains(e.INVOICE_NO)).ToList();
                        }
                        //data = data.Where(x => DateTime.Parse(x.INVOICE_DATE) >= DateTime.Parse(reportParameters.DATE_FROM) && DateTime.Parse(x.INVOICE_DATE) <= DateTime.Parse(reportParameters.DATE_TO)).ToList();
                        reportDataSource.Value = data.ToDataTable();
                        DataTable dt = new DataTable();
                        dt.Clear();
                        dt.Columns.Add("DATE_FROM");
                        dt.Columns.Add("DATE_TO");
                        dt.Columns.Add("UNIT_NAME");
                        DataRow _ravi = dt.NewRow();
                        _ravi["DATE_FROM"] = reportParameters.DATE_FROM;
                        _ravi["DATE_TO"] = reportParameters.DATE_TO;
                        dt.Rows.Add(_ravi);
                        ReportDataSource reportDataSource2 = new ReportDataSource();
                        reportDataSource2.Name = "DateRangeDataSet";
                        reportDataSource2.Value = dt;
                        //localReport.SubreportProcessing += new SubreportProcessingEventHandler(LocalReport_PendingGift);
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
                        //localReport.DataSources.Add(reportDataSource3);
                        //localReport.DataSources.Add(reportDataSource4);
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

        //Invoice Delivery Pending (List): Distribution Report(../Inventory/Report/DistributionReport)
        public ActionResult InvoiceWisePendingDelivery(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/Distribution/InvoiceWisePendingDelivery.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "InvoiceWisePendingDataSet";
                        var data = _service.GetAllPendingInvoices(reportParameters).ToList<Invoice>();
                        if (!string.IsNullOrEmpty(reportParameters.CUSTOMER_ID) && reportParameters.CUSTOMER_ID != "undefined")
                        {
                            data = data.Where(e => e.CUSTOMER_ID == reportParameters.CUSTOMER_ID).ToList();
                        }
                        if (!string.IsNullOrEmpty(reportParameters.INVOICE_NO) && reportParameters.INVOICE_NO != "undefined")
                        {
                            data = data.Where(e => reportParameters.INVOICE_NO.Contains(e.INVOICE_NO)).ToList();
                        }
                        reportDataSource.Value = data.ToDataTable();
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
            catch (Exception ex)
            {
                throw ex;
            }


        }

        //Delivery List : Distribution Report(../Inventory/Report/DistributionReport)
        public ActionResult DateWiseDeliveryList(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/Distribution/DateWiseDeliveryList.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DateWiseDeliveryListDataSet";
                        reportDataSource.Value = _service.GetDateWiseDeliveryList(reportParameters);
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Invoice Status : Distribution Report(../Inventory/Report/DistributionReport)
        public ActionResult InvoiceStatus(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/Distribution/InvoiceStatus.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "InvoiceStatusDataSet";
                        reportDataSource.Value = _service.InvoiceStatus(reportParameters);
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Invoice Status : Distribution Report(../Inventory/Report/DistributionReport)
        public ActionResult ReceivingForRefurbishment(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/Distribution/RefurbishmentReceivingRpt.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "RefurbishmentDataSet";
                        reportDataSource.Value = _service.RefurbishmentReceiveData(reportParameters);
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Invoice Status : Distribution Report(../Inventory/Report/DistributionReport)
        public ActionResult FinalizingForRefurbishment(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/Distribution/RefurbishmentFinalizingRpt.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "RefurbishmentDataSet";
                        reportDataSource.Value = _service.RefurbishmentReceiveFinalizeMstData(reportParameters);

                        ReportDataSource reportDataSource2 = new ReportDataSource();
                        reportDataSource2.Name = "RefurbishmentFinalizingDataSet";
                        reportDataSource2.Value = _service.RefurbishmentFinalizeData(reportParameters);

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