using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using SalesAndDistributionSystem.Domain.Models.ReportModels.Common.ProductReports;
using SalesAndDistributionSystem.Reporting.Areas.SalesAndDistribution.Data;
using SalesAndDistributionSystem.Reporting.ReportConst;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SalesAndDistributionSystem.Reporting.Areas.SalesAndDistribution.Controllers
{
    public class MasterReportController : Controller
    {
        private readonly MasterReport service = new MasterReport();
        private readonly CommonServices commonservice = new CommonServices();

        // GET: SalesAndDistribution/ProductReport
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Products(string reportExtention = "pdf")
        {
            try
            {

                LocalReport localReport = new LocalReport();
                localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/ProductReport/ProductReport.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource();
                reportDataSource.Name = "DataSet00002";
                reportDataSource.Value = service.GetProductReport();
                ReportDataSource reportDataSource1 = new ReportDataSource();
                reportDataSource.Name = "ProductBonusDataSet";
                reportDataSource1.Value = service.GetProductReport();

                string reportType = reportExtention;
                Warning[] warnings;
                string mimeType;
                string[] streamids;
                string encoding;
                string fileNameExtension = reportType == "Excel" ? "xlsx" : "pdf";
                byte[] renderedBytes;
                localReport.DataSources.Add(reportDataSource);

                renderedBytes = localReport.Render(reportType, "", out mimeType, out encoding, out fileNameExtension, out streamids, out warnings);

                return new FileContentResult(renderedBytes, mimeType);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public ActionResult ProductBonus(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/MasterReport/ProductBonusReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "ProductBonusDataSet";
                        reportDataSource1.Value = service.GetProductBounusReport(reportParameters);

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
                throw ex.InnerException;
            }


        }
        public ActionResult ProductBonusLine(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/MasterReport/ProductBonusLineReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "ProductBonusDataSet";
                        reportDataSource1.Value = service.GetProductBounusLineReport(reportParameters);

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
        public ActionResult ProductBonusLineWithUnit(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/MasterReport/ProductBonusLineWhUnitReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "ProductBonusLineDataSet";
                        reportDataSource1.Value = service.GetProductBounusLineReport(reportParameters);

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

        public ActionResult ComboBonus(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/MasterReport/ComboBonusReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "ComboBonusDataSet";
                        reportDataSource1.Value = service.GetComboBounusReport(reportParameters);

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
                throw ex.InnerException;
            }


        }
        public ActionResult ProductPriceInformation(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/MasterReport/ProductPriceInformationReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "ProductPriceDataSet";
                        reportDataSource1.Value = service.GetProductPriceInformationReport(reportParameters);

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
                throw ex.InnerException;
            }


        } 
        public ActionResult CustomerRelation(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/MasterReport/CustomerRelationReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "CustomerRelationDataSet";
                        reportDataSource1.Value = service.GetCustomerRelationReport(reportParameters);

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
                throw ex.InnerException;
            }


        }
        public ActionResult Location(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/MasterReport/LocationReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "CustomerInfoDataSet";
                        reportDataSource1.Value = service.GetLocationReport(reportParameters);

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
                throw ex.InnerException;
            }


        }
        public ActionResult CustomerPrice(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/MasterReport/CustomerPriceReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "CustomerPriceDataSet";
                        reportDataSource1.Value = service.GetCustomerPriceReport(reportParameters);

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
                throw ex.InnerException;
            }


        }
        public ActionResult CustomerInfo(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/MasterReport/CustomerInfoReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "CustomerInfoOnlyDataSet";
                        reportDataSource1.Value = service.GetCustomerInfoReport(reportParameters);

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
                throw ex.InnerException;
            }


        }
        public ActionResult CreditPolicy(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/SalesAndDistribution/Reports/MasterReport/CreditPolicyReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet00002";
                        reportDataSource.Value = service.GetPageHeaderReport(reportParameters);
                        ReportDataSource reportDataSource1 = new ReportDataSource();
                        reportDataSource1.Name = "CreditPolicyDataSet";
                        reportDataSource1.Value = service.GetCreditPolicyReport(reportParameters);

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
                throw ex.InnerException;
            }


        }
    }
}