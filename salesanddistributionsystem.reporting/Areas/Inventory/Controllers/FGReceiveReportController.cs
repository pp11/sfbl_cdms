using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using SalesAndDistributionSystem.Domain.Models.ReportModels.Common.ProductReports;
using SalesAndDistributionSystem.Reporting.Areas.Inventory.Data;
using SalesAndDistributionSystem.Reporting.ReportConst;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SalesAndDistributionSystem.Reporting.Areas.Inventory.Controllers
{
    public class FGReceiveReportController : Controller
    {
        private readonly FGReceive _service = new FGReceive();
        private readonly CommonServices _commonservice = new CommonServices();

        // GET: Inventory/FGReceiveReport/ReceiveRegister
        public ActionResult ReceiveRegister(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/FGReceiveReport/ReceiveRegister.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "ReceiveRegister";
                        reportDataSource.Value = _service.ReceiveRegisterData(reportParameters);

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

        public ActionResult ReceiveRegisterProductWise(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/FGReceiveReport/ReceiveRegisterProductWise.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "ReceiveRegister";
                        reportDataSource.Value = _service.ReceiveRegisterProductWiseData(reportParameters);

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

        public ActionResult FGReceiveFromProduction(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/FGReceiveReport/FGReceiveFromProduction.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "FGReceiveDataSet";
                        reportDataSource.Value = _service.FGReceiveFromProduction(reportParameters);

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
        
        public ActionResult FGReceiveFromOthers(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/FGReceiveReport/FGReceiveFromOthers.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "FGReceiveDataSet";
                        reportDataSource.Value = _service.FGReceiveFromOthers(reportParameters);

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

        public ActionResult RequisitionPendingForIssue(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/FGReceiveReport/RequisitionPending.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "RequisitionPendingDataSet";
                        reportDataSource.Value = _service.RequisitionPending(reportParameters);

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

        public ActionResult RequisitionPendingForDispatch(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/FGReceiveReport/RequisitionPendingForDispatch.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "RequisitionPendingDataSet";
                        reportDataSource.Value = _service.RequisitionPendingForDispatch(reportParameters);

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

        public ActionResult RequisitionPendingForReceive(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/FGReceiveReport/RequisitionPendingForReceive.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "RequisitionPendingDataSet";
                        reportDataSource.Value = _service.RequisitionPendingForReceive(reportParameters);

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

        public ActionResult OverallRequisitionStatus(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/FGReceiveReport/OverallRequisitionStatusReport.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "OverallRequisitionStatus";
                        reportDataSource.Value = _service.OverallRequisitionStatus(reportParameters);

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

        public ActionResult OverallRequisitionStatusDetails(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/FGReceiveReport/OverallRequisitionStatusDetailsReport.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "OverallRequisitionStatus";
                        reportDataSource.Value = _service.OverallRequisitionStatusDetails(reportParameters);

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
        public ActionResult DailyBatchRcvRegister(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/FGReceiveReport/DailyBatchRcvRegisterReport.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DailyBatchRcvRegisterDataSet";
                        reportDataSource.Value = _service.DailyBatchRcvRegister(reportParameters);

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
        public ActionResult DateWiseFgRcvFromOtherRegister(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/FGReceiveReport/DateWiseFgRcvFromOtherRegisterReport.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DateWiseFgRcvFromOtherRegDataSet";
                        reportDataSource.Value = _service.DateWiseFgRcvFromOtherRegister(reportParameters);

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
        public ActionResult DateWiseMiscellaneousRegister(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/FGReceiveReport/DateWiseMiscellaneousRegisterReport.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DateWiseMiscellaneousRegDataSet";
                        reportDataSource.Value = _service.DateWiseMiscellaneousRegister(reportParameters);

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
        public ActionResult DateWiseReceivingRegister(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/FGReceiveReport/DateWiseReceivingRegisterReport.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DateWiseReceivingRegisterDataSet";
                        reportDataSource.Value = _service.DateWiseReceivingRegister(reportParameters);

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
        public ActionResult DateWiseConsumptionRegi(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/FGReceiveReport/DateWiseConsumptionRegiReport.rdlc");

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DateWiseConsumptionRegiDataSet";
                        reportDataSource.Value = _service.DateWiseConsumptionRegi(reportParameters);

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
        public ActionResult BatchSizeVsReceivingQty(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/FGReceiveReport/BatchSizeVsReceivingQtyReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "BatchSizeVsReceivingQtyDataSet";
                        reportDataSource.Value = _service.BatchSizeVsReceivingQty(reportParameters);
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
        public ActionResult TransferVsReceive(string q)
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
                        localReport.ReportPath = Server.MapPath("~/Areas/Inventory/Reports/FGReceiveReport/TransferVsReceiveReport.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "TransferVsRcv";
                        reportDataSource.Value = _service.TransferVsReceive(reportParameters);
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


    }
}