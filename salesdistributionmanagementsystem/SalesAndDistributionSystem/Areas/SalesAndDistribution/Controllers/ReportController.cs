using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using Oracle.ManagedDataAccess.Client;
using System.Linq;
using ServiceProvider = SalesAndDistributionSystem.Common.ServiceProvider;
using SalesAndDistributionSystem.Domain.Utility;
using System.IO;
using Microsoft.Extensions.Hosting;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using DinkToPdf;
using DinkToPdf.Contracts;
using SalesAndDistributionSystem.Domain.Common;
using System.Threading.Tasks;
using SalesAndDistributionSystem.Services.ReportSpecifier;
using ClosedXML.Excel;
using System.Data;
using SalesAndDistributionSystem.Services.Business.Security;
using SalesAndDistributionSystem.Domain.Models.ViewModels.Security;
using SalesAndDistributionSystem.Domain.Models.Entities.Security;
using SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using SalesAndDistributionSystem.Domain.Models.ReportModels.Common.ProductReports;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;

namespace SalesAndDistributionSystem.Areas.SalesAndDistribution.Controllers
{
    [Area("SalesAndDistribution")]
    [Authorize]
    public class ReportController : Controller
    {
        private readonly ILogger<ReportController> _logger;
        private readonly IReportManager _reportService;
        private readonly IConverter _converter;
        private readonly IReportSpecifier _reportSpecifier;
        private readonly ICommonServices _commonService;
        private readonly IReportConfigurationManager _reportManager;
        private readonly IHostEnvironment _hostManager;


        private readonly ServiceProvider Provider = new ServiceProvider();

        public ReportController(ILogger<ReportController> logger, IReportManager reportService, IConverter converter, IReportSpecifier reportSpecifier, ICommonServices commonService, IReportConfigurationManager reportManager, IHostEnvironment hostManager)
        {
            _logger = logger;
            _reportService = reportService;
            _converter = converter;
            _reportSpecifier = reportSpecifier;
            _commonService = commonService;
            _reportManager = reportManager;
            _hostManager = hostManager;
        }
        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution", "Operation");
        private string GetDbConnectionStringSecurity() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "Security", "Operation");

        public string GetCompany() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString();
        public string GetUnitId() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value.ToString();

        public IActionResult Index()
        {

            return View();
        }
        public IActionResult GetReports()
        {
            List<ReportDataSpecify> reportData = _reportSpecifier.GetReportList();
            foreach (ReportDataSpecify reportDataSpecify in reportData)
            {
                reportDataSpecify.ReportIdEncrypt = _commonService.Encrypt(reportDataSpecify.ReportId.ToString());
            }
            return View(reportData);
        }


        public async Task<IActionResult> MasterReport()
        {
            int User_id = Convert.ToInt32(User.Claims.Where(x => x.Type == ClaimsType.UserId).FirstOrDefault().Value);

            int Comp_id = Convert.ToInt32(User.Claims.Where(x => x.Type == ClaimsType.CompanyId).FirstOrDefault().Value);
            List<ReportPermission> reportData = await _reportManager.LoadReportPermissionData(GetDbConnectionStringSecurity(), Comp_id, User_id);
            reportData = reportData.Where(x => x.MENU_NAME == "SalesAndDistributionReport" && x.MENU_ACTION == "MasterReport").OrderBy(x => x.ORDER_BY_SLNO).ToList();

            foreach (ReportPermission reportDataSpecify in reportData)
            {
                reportDataSpecify.ReportIdEncrypt = _commonService.Encrypt(reportDataSpecify.REPORT_ID.ToString());

            }
            return View(reportData);
        }


        public async Task<IActionResult> InvoiceReport()
        {
            int User_id = Convert.ToInt32(User.Claims.Where(x => x.Type == ClaimsType.UserId).FirstOrDefault().Value);

            int Comp_id = Convert.ToInt32(User.Claims.Where(x => x.Type == ClaimsType.CompanyId).FirstOrDefault().Value);
            List<ReportPermission> reportData = await _reportManager.LoadReportPermissionData(GetDbConnectionStringSecurity(), Comp_id, User_id);
            reportData = reportData.Where(x => x.MENU_ACTION == "InvoiceReport").OrderBy(x => x.ORDER_BY_SLNO).ToList();

            foreach (ReportPermission reportDataSpecify in reportData)
            {
                reportDataSpecify.ReportIdEncrypt = _commonService.Encrypt(reportDataSpecify.REPORT_ID.ToString());

            }
            return View(reportData);
        }

        public async Task<IActionResult> SalesAndCollectionReport()
        {
            int User_id = Convert.ToInt32(User.Claims.Where(x => x.Type == ClaimsType.UserId).FirstOrDefault().Value);

            int Comp_id = Convert.ToInt32(User.Claims.Where(x => x.Type == ClaimsType.CompanyId).FirstOrDefault().Value);
            List<ReportPermission> reportData = await _reportManager.LoadReportPermissionData(GetDbConnectionStringSecurity(), Comp_id, User_id);
            reportData = reportData.Where(x => x.MENU_ACTION == "SalesAndCollectionReport")
                .OrderBy(e => e.ORDER_BY_SLNO)
                .ToList();

            foreach (ReportPermission reportDataSpecify in reportData)
            {
                reportDataSpecify.ReportIdEncrypt = _commonService.Encrypt(reportDataSpecify.REPORT_ID.ToString());

            }
            return View(reportData);
        }

        [HttpPost]
        public async Task<string> LoadData([FromBody] Report_Configuration report_Info)
        {
            int comp_id = report_Info == null || report_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : report_Info.COMPANY_ID;
            return await _reportManager.LoadData(GetDbConnectionStringSecurity(), comp_id);
        }

        [HttpPost]
        public async Task<string> GetReturnInvoiceNumbers([FromBody] ReportParams report_Info)
        {
            report_Info.COMPANY_ID = report_Info == null || report_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : report_Info.COMPANY_ID;
            return await _reportManager.GetReturnInvoiceNumbers(GetDbConnectionString(), report_Info);
        }


        [HttpGet]
        public IActionResult GenerateReport(string ReportId, string Color, ReportParams reportParameters, string IsLogoApplicable = "Yes", string IsCompanyApplicable = "Yes", int companyId = 0)
        {

            string ReportUrl = _hostManager.IsDevelopment() == true ? CodeConstants.Report_URL : CodeConstants.Report_URL_RELEASE;
            int comp_id = companyId == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : companyId;
            string User_Name = User.Claims.Where(x => x.Type == ClaimsType.UserName).FirstOrDefault().Value;
            string User_Id = User.Claims.Where(x => x.Type == ClaimsType.UserId).FirstOrDefault().Value;
            reportParameters.USER_NAME = User_Name;
            reportParameters.SECRET_KEY = CodeConstants.Report_Secret_Key;
            reportParameters.COMPANY_ID = comp_id;
            reportParameters.UNIT_ID = reportParameters.UNIT_ID == null || reportParameters.UNIT_ID == "undefined" ? GetUnitId() : reportParameters.UNIT_ID;
            reportParameters.DB = GetDbConnectionString();
            reportParameters.DB_SECURITY = GetDbConnectionStringSecurity();
            string q = JsonSerializer.Serialize<ReportParams>(reportParameters);
            q = _commonService.Encrypt(q);
            string Url = "";
            /* START-------------------------Invoice Report(https://localhost:44305/SalesAndDistribution/Report/InvoiceReport)------------------------- */
            if (reportParameters.REPORT_ID == 11) //Invoice Report
            {
                Url = ReportUrl + "SalesAndDistribution/InvoiceReport/Invoice?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 57) //Invoice (Unit TP wise)
            {
                Url = ReportUrl + "SalesAndDistribution/InvoiceReport/TpWiseInvoice?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 55) //Order Report
            {
                Url = ReportUrl + "SalesAndDistribution/InvoiceReport/Order?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 13) //Customer Ledger (Central)
            {
                Url = ReportUrl + "SalesAndDistribution/InvoiceReport/DistributorLadger?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 95) //Customer Ledger V2(Central)
            {
                Url = ReportUrl + "SalesAndDistribution/InvoiceReport/DistributorLadgerV2?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 56) //Customer Ledger (Unit Wise)
            {
                Url = ReportUrl + "SalesAndDistribution/InvoiceReport/UnitWiseDistributorLadger?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 12) //Delivery Slip
            {
                Url = ReportUrl + "SalesAndDistribution/InvoiceReport/DeliverySlip?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 48) //Invoice wise Delivery Slip Report
            {
                Url = ReportUrl + "SalesAndDistribution/InvoiceReport/InvoiceWiseDeliverySlip?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 58) //Date Wise Sales Register
            {
                Url = ReportUrl + "SalesAndDistribution/InvoiceReport/SalesRegister?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 44) //Customer Outstanding Report
            {
                Url = ReportUrl + "SalesAndDistribution/InvoiceReport/CustomerOutstanding?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 92) //Customer Outstanding V2 (Central)
            {
                Url = ReportUrl + "SalesAndDistribution/InvoiceReport/CustomerOutstandingV2?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 45) //Customer Outstanding with invoice Report
            {
                Url = ReportUrl + "SalesAndDistribution/InvoiceReport/CustomerOutstandingWithInvoice?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 93) //Customer Outstanding with invoice Report V2
            {
                Url = ReportUrl + "SalesAndDistribution/InvoiceReport/CustomerOutstandingWithInvoiceV2?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 94) //Customer Outstanding with invoice Report V2
            {
                Url = ReportUrl + "SalesAndDistribution/InvoiceReport/CustOutstandingDtlV2?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 96) //Customer Outstanding with invoice Report V2
            {
                Url = ReportUrl + "SalesAndDistribution/InvoiceReport/CustOutstandingDtlV2Ck?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 60) //Customer Wise Outstanding (End Date Selection)
            {
                Url = ReportUrl + "SalesAndDistribution/InvoiceReport/CustOutstandingDtl?q=" + q;
            }
            /* ---------------------------------------END-------------------------------------------------------- */
            /* START------------Master Report(https://localhost:44305/SalesAndDistribution/Report/MasterReport)-------------- */
            else if (reportParameters.REPORT_ID == 2) //Product Price Information Report
            {
                Url = ReportUrl + "SalesAndDistribution/MasterReport/ProductPriceInformation?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 21) //Product Bonus Report
            {
                Url = ReportUrl + "SalesAndDistribution/MasterReport/ProductBonus?q=" + q;
            }
            else if(reportParameters.REPORT_ID == 29) //Customer Relation Report
            {
                Url = ReportUrl + "SalesAndDistribution/MasterReport/CustomerRelation?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 26) //Combo Bonus Report
            {
                Url = ReportUrl + "SalesAndDistribution/MasterReport/ComboBonus?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 32) //Location Report
            {
                Url = ReportUrl + "SalesAndDistribution/MasterReport/Location?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 34) //Customer info Report
            {
                Url = ReportUrl + "SalesAndDistribution/MasterReport/CustomerInfo?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 35) //Credit Policy Report
            {
                Url = ReportUrl + "SalesAndDistribution/MasterReport/CreditPolicy?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 36) //Customer Price Report
            {
                Url = ReportUrl + "SalesAndDistribution/MasterReport/CustomerPrice?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 69) //Product Bonus Line(Flat) Report
            {
                Url = ReportUrl + "SalesAndDistribution/MasterReport/ProductBonusLine?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 72) //Product Bonus (Unit Wise) Report
            {
                Url = ReportUrl + "SalesAndDistribution/MasterReport/ProductBonusLineWithUnit?q=" + q;
            }
            /* -------------------------------------------------------------------------END----------------------------------------------------------------------- */
            /* START-------------------Sales and Collection Report(https://localhost:44305/SalesAndDistribution/Report/SalesAndCollectionReport)------------------ */

            else if (reportParameters.REPORT_ID == 40) //Daily sales register Report
            {
                Url = ReportUrl + "SalesAndDistribution/SalesAndCollection/DailySalesRegister?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 41) //Daily collecion register Report
            {
                Url = ReportUrl + "SalesAndDistribution/SalesAndCollection/DailyCollectionRegister?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 42) //Invoice wise return Report
            {
                Url = ReportUrl + "SalesAndDistribution/SalesAndCollection/InvoiceWiseReturn?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 43) //Date wise return Report
            {
                Url = ReportUrl + "SalesAndDistribution/SalesAndCollection/DateWiseReturn?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 59) //Date Wise Sales Register
            {
                Url = ReportUrl + "SalesAndDistribution/SalesAndCollection/CustWiseSalesCollRetStmtRpt?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 62) //Month Wise Sales-Collection-Return Summary
            {
                Url = ReportUrl + "SalesAndDistribution/SalesAndCollection/MonthWiseSalesCollRtnSmry?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 63) //Sales Return Report (Value)
            {
                Url = ReportUrl + "SalesAndDistribution/SalesAndCollection/SalesReturnReport?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 64) //Sales Return Report (SKU)
            {
                Url = ReportUrl + "SalesAndDistribution/SalesAndCollection/SkuWiseSalesReturn?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 65) //Customer Collection Report
            {
                Url = ReportUrl + "SalesAndDistribution/SalesAndCollection/CustomerCollectionReport?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 85) //Accounts Datewise Sales Report
            {
                Url = ReportUrl + "SalesAndDistribution/SalesAndCollection/AccountsDatewiseSalesReport?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 86) //Sales Trend Analysis Report
            {
                Url = ReportUrl + "SalesAndDistribution/SalesAndCollection/SalesTrendAnalysisReport?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 102) 
            {
                Url = ReportUrl + "SalesAndDistribution/SalesAndCollection/CustWiseRemainingBnsRpt?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 103)
            {
                Url = ReportUrl + "SalesAndDistribution/SalesAndCollection/CustWiseRemBnsSummaryRpt?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 105)
            {
                Url = ReportUrl + "SalesAndDistribution/InvoiceReport/OrderVsInvoice?q=" + q;
            }
            /* -------------------------------------------------------------------------END----------------------------------------------------------------------- */
            else
            {
                Url = ReportUrl + "SalesAndDistribution/InvoiceReport/Order?q=" + q;
            }
            return Redirect(Url);
        }



        [HttpGet]
        public async Task<IActionResult> CreatePDF(string ReportId, string Color, ProductReportParameters reportParameters, string IsLogoApplicable = "Yes", string IsCompanyApplicable = "Yes", int companyId = 0)
        {
            int comp_id = companyId == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : companyId;
            string User_Name = User.Claims.Where(x => x.Type == ClaimsType.UserName).FirstOrDefault().Value;
            string User_Id = User.Claims.Where(x => x.Type == ClaimsType.UserId).FirstOrDefault().Value;
            ReportDataGenerator reportData = await _reportService.GeneratePDF(ReportId, GetDbConnectionString(), comp_id, IsLogoApplicable, IsCompanyApplicable, reportParameters, Convert.ToInt32(User_Id), GetDbConnectionStringSecurity());
            var globalSettings = new GlobalSettings
            {
                ColorMode = Color == "Color" ? ColorMode.Color : Color == "Grayscale" ? ColorMode.Grayscale : reportData.ColorMode == "Color" ? ColorMode.Color : ColorMode.Grayscale,
                Orientation = reportData.PageOrientation == "Landscape" ? Orientation.Landscape : Orientation.Portrait,
                PaperSize = reportData.PaperKind == "Letter" ? PaperKind.Letter : reportData.PaperKind == "A4" ? PaperKind.A4 : reportData.PaperKind == "Legal" ? PaperKind.Legal : PaperKind.LegalExtra,
                Margins = new MarginSettings { Top = reportData.MarginTop },
                DocumentTitle = reportData.DocumentTitle,
            };
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = reportData.HtmlContentData,
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "css", "ReportStyle.css") },
                FooterSettings = { FontName = "Arial", FontSize = 9, Center = "Powered by: Square Informatix Ltd - 2022", Right = "Page [page] of [toPage]", Left = "Printed By: " + User_Name, Line = true, Spacing = 1.8 },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Line = true, Spacing = 1.8 }
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            var file = _converter.Convert(pdf);
            return File(file, "application/pdf");
        }


        [HttpGet]
        public async Task<IActionResult> ReportPreview(string ReportId, ProductReportParameters reportParameters, string IsLogoApplicable = "Yes", string IsCompanyApplicable = "Yes", int companyId = 0)
        {
            int comp_id = companyId == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : companyId;
            string User_Name = User.Claims.Where(x => x.Type == ClaimsType.UserName).FirstOrDefault().Value;
            string User_Id = User.Claims.Where(x => x.Type == ClaimsType.UserId).FirstOrDefault().Value;

            ReportDataGenerator reportData = await _reportService.GeneratePDF(ReportId, GetDbConnectionString(), comp_id, IsLogoApplicable, IsCompanyApplicable, reportParameters, Convert.ToInt32(User_Id), GetDbConnectionStringSecurity());
            ViewBag.ReportParameters = reportParameters;
            ViewBag.ReportId = ReportId;
            ViewBag.IsLogoApplicable = IsLogoApplicable;

            ViewBag.IsCompanyApplicable = IsCompanyApplicable;
            ViewBag.companyId = companyId;

            return View(reportData);
        }

        [HttpGet]
        public async Task<IActionResult> ReportExcel(string ReportId, ProductReportParameters reportParameters, int companyId = 0)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                int comp_id = companyId == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : companyId;

                DataSet dt = await _reportService.GenerateExcel(ReportId, GetDbConnectionString(), comp_id, reportParameters);
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "_ExcelReport.xlsx");
                }
            }




        }

        //load report View permission
        [HttpPost]
        public string IsReportPermitted([FromBody] ReportPermission report)
        {
            List<ReportPermission> reportPermissions = JsonSerializer.Deserialize<List<ReportPermission>>(HttpContext.Session.GetString(ClaimsType.ReportPermission));
            return JsonSerializer.Serialize(_reportManager.IsReportPermitted(report.REPORT_ID, reportPermissions));

        }
        [HttpGet]
        public string GetDistributorId()
        {
            return JsonSerializer.Serialize(new { DistributorId = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.DistributorId).Value });

        }
    }
}
