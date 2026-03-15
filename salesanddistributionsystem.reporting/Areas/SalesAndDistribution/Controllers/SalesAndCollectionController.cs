using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using SalesAndDistributionSystem.Domain.Models.ReportModels.Common.ProductReports;
using SalesAndDistributionSystem.Reporting.Areas.SalesAndDistribution.Data;
using SalesAndDistributionSystem.Reporting.ReportConst;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SalesAndDistributionSystem.Reporting.Areas.SalesAndDistribution.Controllers
{
    public class SalesAndCollectionController : Controller
    {

        private readonly SalesAndCollectionReport _service = new SalesAndCollectionReport();
        private readonly CommonServices _commonservice = new CommonServices();
        
        //Daily Sales Register : Sales and Collection Report(MainSite/SalesAndDistribution/Report/SalesAndCollectionReport)
        public ActionResult DailySalesRegister(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/SalesAndCollectionReport/DailySalesRegister.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DailySalesRegister";
                        reportDataSource.Value = _service.DailySalesRegister(reportParameters);

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

        //Sales Trend Analysis Report : Sales and Collection Report(MainSite/SalesAndDistribution/Report/SalesAndCollectionReport)
        public ActionResult SalesTrendAnalysisReport(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/SalesAndCollectionReport/SalesTrendAnalysisReportReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "SalesTrendAnalysisDataSet";
                        reportDataSource.Value = _service.GetSalesTrendAnalysisData(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "DataSet00002";
                        reportDataSource1.Value = _service.GetPageHeaderReport(reportParameters);
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
                        reportDataSource2.Name = "DataRangeDataSet";
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

        //Daily/Customer Wise Collection Register : Sales and Collection Report(MainSite/SalesAndDistribution/Report/SalesAndCollectionReport)
        public ActionResult DailyCollectionRegister(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/SalesAndCollectionReport/DailyCollectionRegister.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DailyCollectionRegister";
                        reportDataSource.Value = _service.CollectionRegister(reportParameters);

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
        public ActionResult InvoiceWiseReturn(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/SalesAndCollectionReport/InvoiceWiseReturn.rdlc");

                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "InvoiceWiseReturn";
                        reportDataSource1.Value = _service.InvoiceWiseReturn(reportParameters);

                        ReportDataSource reportDataSource2 = new ReportDataSource();
                        reportDataSource2.Name = "ProductBonusReturn";
                        reportDataSource2.Value = _service.ReturnProductBonus(reportParameters);

                        ReportDataSource reportDataSource3 = new ReportDataSource();
                        reportDataSource3.Name = "ReturnComboGiftBonus";
                        reportDataSource3.Value = _service.ReturnComboGiftBonus(reportParameters);

                        ReportDataSource reportDataSource5 = new ReportDataSource();
                        reportDataSource5.Name = "ReturnGiftBonus";
                        reportDataSource5.Value = _service.ReturnGiftBonus(reportParameters);

                        ReportDataSource reportDataSource4 = new ReportDataSource();
                        reportDataSource4.Name = "ReturnComboBonus";
                        reportDataSource4.Value = _service.ReturnComboBonus(reportParameters);



                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = _service.GetPageHeaderReport(reportParameters);

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
                        localReport.DataSources.Add(reportDataSource5);
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
        public ActionResult DateWiseReturn(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/SalesAndCollectionReport/DateWiseReturn.rdlc");

                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "InvoiceWiseReturn";
                        reportDataSource1.Value = _service.DateWiseReturn(reportParameters);

                        ReportDataSource reportDataSource2 = new ReportDataSource();
                        reportDataSource2.Name = "ProductBonusReturn";
                        reportDataSource2.Value = _service.DateWiseReturnProductBonus(reportParameters);

                        ReportDataSource reportDataSource3 = new ReportDataSource();
                        reportDataSource3.Name = "ReturnComboGiftBonus";
                        reportDataSource3.Value = _service.DateWiseReturnComboGiftBonus(reportParameters);

                        ReportDataSource reportDataSource5 = new ReportDataSource();
                        reportDataSource5.Name = "ReturnGiftBonus";
                        reportDataSource5.Value = _service.DateWiseReturnGiftBonus(reportParameters);

                        ReportDataSource reportDataSource4 = new ReportDataSource();
                        reportDataSource4.Name = "ReturnComboBonus";
                        reportDataSource4.Value = _service.DateWiseReturnComboBonus(reportParameters);


                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = _service.GetPageHeaderReport(reportParameters);

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
                        localReport.DataSources.Add(reportDataSource5);
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
        //Customer Wise Sales-Collection-Return Statement : Sales and Collection Report(MainSite/SalesAndDistribution/Report/SalesAndCollectionReport)
        public ActionResult CustWiseSalesCollRetStmtRpt(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/SalesAndCollectionReport/CustWiseSalesCollRetStmtReport.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "CusWiseSalesCollDataSet";
                        reportDataSource.Value = _service.GetCustWiseSalesCollRetStmtRpt(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "DataSet00002";
                        reportDataSource1.Value = _service.GetPageHeaderReport(reportParameters);
                       
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
                        reportDataSource2.Name = "DataRangeDataSet";
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
        //Sales Return Report (SKU) : Sales and Collection Report(MainSite/SalesAndDistribution/Report/SalesAndCollectionReport)
        public ActionResult SkuWiseSalesReturn(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/SalesAndCollectionReport/SkuWiseSalesReturnReport.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "SalesReturnDataSet";
                        reportDataSource.Value = _service.GetSkuWiseSalesReturnRpt(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "DataSet00002";
                        reportDataSource1.Value = _service.GetPageHeaderReport(reportParameters);

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
                        reportDataSource2.Name = "DataRangeDataSet";
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
        
        public ActionResult AccountsDatewiseSalesReport(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/SalesAndCollectionReport/AccountsDateWiseSalesReport.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "AccountsDateWiseSalesDataSet";
                        reportDataSource.Value = _service.GetAccountsSalesReportDatewise(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "DataSet00002";
                        reportDataSource1.Value = _service.GetPageHeaderReport(reportParameters);

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
                        reportDataSource2.Name = "DataRangeDataSet";
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
        public ActionResult CustomerCollectionReport(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/SalesAndCollectionReport/CustomerCollectionReport.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "CustomerCollectionDataSet";
                        reportDataSource.Value = _service.GetCustomerCollectionReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "DataSet00002";
                        reportDataSource1.Value = _service.GetPageHeaderReport(reportParameters);

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
                        reportDataSource2.Name = "DataRangeDataSet";
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
        //Sales Return Report (Value) : Sales and Collection Report(MainSite/SalesAndDistribution/Report/SalesAndCollectionReport)
        public ActionResult SalesReturnReport(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/SalesAndCollectionReport/SalesReturnReport.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "SalesReturnDataSet";
                        reportDataSource.Value = _service.GetSalesReturnReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "DataSet00002";
                        reportDataSource1.Value = _service.GetPageHeaderReport(reportParameters);

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
                        reportDataSource2.Name = "DataRangeDataSet";
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
        //Customer Wise Sales-Collection-Return Statement : Sales and Collection Report(MainSite/SalesAndDistribution/Report/SalesAndCollectionReport)
        public ActionResult MonthWiseSalesCollRtnSmry(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/SalesAndCollectionReport/MonthWiseSalesCollRtnSmryReport.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "MonthWiseSalesCollRetDataSet";
                        reportDataSource.Value = _service.GetMonthWiseSalesCollRtnSmry(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "DataSet00002";
                        reportDataSource1.Value = _service.GetPageHeaderReport(reportParameters);

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
                        reportDataSource2.Name = "DataRangeDataSet";
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

        //Daily/Customer Wise Collection Register : Sales and Collection Report(MainSite/SalesAndDistribution/Report/SalesAndCollectionReport)
        public ActionResult CustWiseRemainingBnsRpt(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/SalesAndCollectionReport/CustWiseRemainingBnsRpt.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "CustWiseRemBonusDataSet";
                        reportDataSource.Value = _service.GetCustWiseRemainingBnsRpt(reportParameters);

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
        public ActionResult CustWiseRemBnsSummaryRpt(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/SalesAndCollectionReport/CustWiseRemSummaryBnsRpt.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "CustWiseRemBonusDataSet";
                        reportDataSource.Value = _service.GetCustWiseRemainingBnsSummaryRpt(reportParameters);

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