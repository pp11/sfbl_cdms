using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Domain.Common;
using ServiceProvider = SalesAndDistributionSystem.Common.ServiceProvider;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.Inventory;
using SalesAndDistributionSystem.Services.Business.Security;
using SalesAndDistributionSystem.Services.Common;
using SalesAndDistributionSystem.Services.ReportSpecifier;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalesAndDistributionSystem.Domain.Models.ViewModels.Security;
using System;
using SalesAndDistributionSystem.Domain.Models.Entities.Security;
using SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem;
using DinkToPdf;
using System.Text.Json;
using System.IO;
using ClosedXML.Excel;
using System.Data;
using Microsoft.AspNetCore.Http;
using SalesAndDistributionSystem.Domain.Models.ReportModels.Common.ProductReports;
using SalesAndDistributionSystem.Common;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace SalesAndDistributionSystem.Areas.Inventory.Controllers
{
    [Area("Inventory")]
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


        public async Task<IActionResult> InventoryReport()
        {
            int User_id = Convert.ToInt32(User.Claims.Where(x => x.Type == ClaimsType.UserId).FirstOrDefault().Value);

            int Comp_id = Convert.ToInt32(User.Claims.Where(x => x.Type == ClaimsType.CompanyId).FirstOrDefault().Value);
            List<ReportPermission> reportData = await _reportManager.LoadReportPermissionData(GetDbConnectionStringSecurity(), Comp_id, User_id);
            reportData = reportData.Where(x => x.MENU_ACTION == "InventoryReport").OrderBy(x => x.ORDER_BY_SLNO).ToList();
            foreach (ReportPermission reportDataSpecify in reportData)
            {
                reportDataSpecify.ReportIdEncrypt = _commonService.Encrypt(reportDataSpecify.REPORT_ID.ToString());

            }
            return View(reportData);
        }
        public async Task<IActionResult> RequisitionReport()
        {
            int User_id = Convert.ToInt32(User.Claims.Where(x => x.Type == ClaimsType.UserId).FirstOrDefault().Value);
            int Comp_id = Convert.ToInt32(User.Claims.Where(x => x.Type == ClaimsType.CompanyId).FirstOrDefault().Value);
            List<ReportPermission> reportData = await _reportManager.LoadReportPermissionData(GetDbConnectionStringSecurity(), Comp_id, User_id);
            reportData = reportData.Where(x => x.MENU_ACTION == "RequisitionReport").OrderBy(x => x.ORDER_BY_SLNO).ToList();
            foreach (ReportPermission reportDataSpecify in reportData)
            {
                reportDataSpecify.ReportIdEncrypt = _commonService.Encrypt(reportDataSpecify.REPORT_ID.ToString());

            }
            return View(reportData);
        }

        public async Task<IActionResult> ReceiveReport()
        {
            int User_id = Convert.ToInt32(User.Claims.Where(x => x.Type == ClaimsType.UserId).FirstOrDefault().Value);

            int Comp_id = Convert.ToInt32(User.Claims.Where(x => x.Type == ClaimsType.CompanyId).FirstOrDefault().Value);
            List<ReportPermission> reportData = await _reportManager.LoadReportPermissionData(GetDbConnectionStringSecurity(), Comp_id, User_id);
            reportData = reportData.Where(x => x.MENU_ACTION == "ReceiveReport").OrderBy(x => x.ORDER_BY_SLNO).ToList();
            foreach (ReportPermission reportDataSpecify in reportData)
            {
                reportDataSpecify.ReportIdEncrypt = _commonService.Encrypt(reportDataSpecify.REPORT_ID.ToString());

            }
            return View(reportData);
        }

        public async Task<IActionResult> StockTransferReport()
        {
            int User_id = Convert.ToInt32(User.Claims.Where(x => x.Type == ClaimsType.UserId).FirstOrDefault().Value);

            int Comp_id = Convert.ToInt32(User.Claims.Where(x => x.Type == ClaimsType.CompanyId).FirstOrDefault().Value);
            List<ReportPermission> reportData = await _reportManager.LoadReportPermissionData(GetDbConnectionStringSecurity(), Comp_id, User_id);
            reportData = reportData.Where(x => x.MENU_ACTION == "StockTransferReport").OrderBy(x => x.ORDER_BY_SLNO).ToList();
            foreach (ReportPermission reportDataSpecify in reportData)
            {
                reportDataSpecify.ReportIdEncrypt = _commonService.Encrypt(reportDataSpecify.REPORT_ID.ToString());

            }
            return View(reportData);
        }

        public async Task<IActionResult> DistributionReport()
        {
            int User_id = Convert.ToInt32(User.Claims.Where(x => x.Type == ClaimsType.UserId).FirstOrDefault().Value);

            int Comp_id = Convert.ToInt32(User.Claims.Where(x => x.Type == ClaimsType.CompanyId).FirstOrDefault().Value);
            List<ReportPermission> reportData = await _reportManager.LoadReportPermissionData(GetDbConnectionStringSecurity(), Comp_id, User_id);
            reportData = reportData.Where(x => x.MENU_ACTION == "DistributionReport")
                .OrderBy(x => x.ORDER_BY_SLNO).ToList();
            foreach (ReportPermission reportDataSpecify in reportData)
            {
                reportDataSpecify.ReportIdEncrypt = _commonService.Encrypt(reportDataSpecify.REPORT_ID.ToString());

            }
            return View(reportData);
        }

        [HttpPost]
        public async Task<string> ReceiveBatches([FromBody] ReportParams reportParams)
        {
            if (!ModelState.IsValid)
            {
                var arr = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                return Newtonsoft.Json.JsonConvert.SerializeObject(arr);
            }
            return await _reportService.ReceiveBatches(GetDbConnectionString(), reportParams);
            //return new JsonResult(await _reportService.ReceiveBatches(GetDbConnectionString(), reportParams));
        }

        [HttpPost]
        public async Task<string> GetBatchesFromStock([FromBody] ReportParams reportParams)
        {
            if (!ModelState.IsValid)
            {
                var arr = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                return Newtonsoft.Json.JsonConvert.SerializeObject(arr);
            }
            return await _reportService.GetBatchesFromStock(GetDbConnectionString(), reportParams);
            //return new JsonResult(await _reportService.ReceiveBatches(GetDbConnectionString(), reportParams));
        }

        [HttpPost]
        public async Task<string> GetTransferNotes([FromBody] ReportParams reportParams)
        {
            if (!ModelState.IsValid)
            {
                var arr = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                return Newtonsoft.Json.JsonConvert.SerializeObject(arr);
            }
            return await _reportService.GetTransferNotes(GetDbConnectionString(), reportParams);
        }

        [HttpPost]
        public async Task<string> GetTransferNo([FromBody] ReportParams reportParams)
        {
            if (!ModelState.IsValid)
            {
                var arr = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                return Newtonsoft.Json.JsonConvert.SerializeObject(arr);
            }
            return await _reportService.GetTransferNo(GetDbConnectionString(), reportParams);
        }

        [HttpPost]
        public async Task<string> GetTransferRcvNo([FromBody] ReportParams reportParams)
        {
            if (!ModelState.IsValid)
            {
                var arr = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                return Newtonsoft.Json.JsonConvert.SerializeObject(arr);
            }
            return await _reportService.GetTransferRcvNo(GetDbConnectionString(), reportParams);
        }

        [HttpPost]
        public async Task<string> GetChallans([FromBody] ReportParams reportParams)
        {
            if (!ModelState.IsValid)
            {
                var arr = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                return Newtonsoft.Json.JsonConvert.SerializeObject(arr);
            }
            return await _reportService.GetChallans(GetDbConnectionString(), reportParams);
        }

        [HttpPost]
        public async Task<string> GetGiftChallans([FromBody] ReportParams reportParams)
        {
            if (!ModelState.IsValid)
            {
                var arr = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                return Newtonsoft.Json.JsonConvert.SerializeObject(arr);
            }
            return await _reportService.GetGiftChallans(GetDbConnectionString(), reportParams);
        }

        [HttpPost]
        public async Task<string> GetCustomers([FromBody] ReportParams reportParams)
        {
            if (!ModelState.IsValid)
            {
                var arr = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                return Newtonsoft.Json.JsonConvert.SerializeObject(arr);
            }
            return await _reportService.GetCustomers(GetDbConnectionString(), reportParams);
        }

        [HttpPost]
        public async Task<string> LoadData([FromBody] Report_Configuration report_Info)
        {

            int comp_id = report_Info == null || report_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : report_Info.COMPANY_ID;
            return await _reportManager.LoadData(GetDbConnectionStringSecurity(), comp_id);

        }

        #region Distribution Reports
        [HttpPost]
        public async Task<string> GetDistributionNumbers([FromBody] ReportParams reportParams)
        {
            if (!ModelState.IsValid)
            {
                var arr = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                return Newtonsoft.Json.JsonConvert.SerializeObject(arr);
            }
            return await _reportService.GetDistributionNumbers(GetDbConnectionString(), reportParams);
        }

        [HttpPost]
        public async Task<string> GetDistributionDeliveryNumbers([FromBody] ReportParams reportParams)
        {
            if (!ModelState.IsValid)
            {
                var arr = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                return Newtonsoft.Json.JsonConvert.SerializeObject(arr);
            }
            return await _reportService.GetDistributionDeliveryNumbers(GetDbConnectionString(), reportParams);
        }
        #endregion

        [HttpGet]
        public async Task<string> GetDivitionToMarketRelation()
        {
            return await _reportService.GetDivitionToMarketRelation(GetDbConnectionString());
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
            reportParameters.UNIT_ID = reportParameters.UNIT_ID; /*== null || reportParameters.UNIT_ID == "undefined" ? GetUnitId() : reportParameters.UNIT_ID;*/
            reportParameters.DB = GetDbConnectionString();
            reportParameters.DB_SECURITY = GetDbConnectionStringSecurity();
            if (!string.IsNullOrWhiteSpace(reportParameters.MST_ID))
            {
                if (reportParameters.MST_ID[0] == ',')
                {
                    reportParameters.MST_ID = "";
                }

            }
            string q = JsonSerializer.Serialize<ReportParams>(reportParameters);
            q = _commonService.Encrypt(q);
            string Url = "";
            /* START--------------------Stock Report(https://localhost:44305/Inventory/Report/InventoryReport)--------------------- */
            
            if (reportParameters.REPORT_ID == 3) //Current Stock Report
            {
                Url = ReportUrl + "Inventory/StockReport/Daily?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 5) //Stock Report (Date Wise) Report
            {   
                Url = ReportUrl + "Inventory/StockReport/DateWiseStock?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 14) //Stock Consumption Report
            {
                Url = ReportUrl + "Inventory/StockReport/DateWiseStockRegister?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 18) //Batch Wise Stock Report
            {
                Url = ReportUrl + "Inventory/StockReport/BatchWiseStock?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 83) //Batch Wise Stock Report (Version 2 | New Report)
            {
                Url = ReportUrl + "Inventory/StockReport/BatchWiseStockV2?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 84) //Finished Goods Holding Time
            {
                Url = ReportUrl + "Inventory/StockReport/FinishedGoodsHoldingTime?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 68) //SKU Ledger
            {
                Url = ReportUrl + "Inventory/StockReport/SKULedger?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 30) //Batch Freezing
            {
                Url = ReportUrl + "Inventory/StockReport/BatchFreezing?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 75) //Batch Freezing Transection
            {
                Url = ReportUrl + "Inventory/StockReport/BatchFreezingTransection?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 31) //Stock Adjustment
            {
                Url = ReportUrl + "Inventory/StockReport/StockAdjustment?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 27) //Date Wise Gift Stock Report
            {
                Url = ReportUrl + "Inventory/GiftItemReport/GiftStock?q=" + q;
            }
            /* -----------------------------------------------END-------------------------------------------------------- */
            else if (reportParameters.REPORT_ID == 6)
            {
                Url = ReportUrl + "Inventory/StockReport/RequisitionRaise?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 7)
            {
                Url = ReportUrl + "Inventory/StockReport/RequisitionIssue?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 8)
            {
                Url = ReportUrl + "Inventory/StockReport/RequisitionReceived?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 9)
            {
                Url = ReportUrl + "Inventory/StockReport/RequisitionReturn?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 10)
            {
                Url = ReportUrl + "Inventory/StockReport/RequisitionReturnReceived?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 15)
            {
                Url = ReportUrl + "Inventory/StockTransferReport/Transfer?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 16)
            {
                Url = ReportUrl + "Inventory/StockTransferReport/Receive?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 17)
            {
                Url = ReportUrl + "Inventory/FGReceiveReport/ReceiveRegister?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 19)
            {
                Url = ReportUrl + "Inventory/FGReceiveReport/ReceiveRegisterProductWise?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 20)
            {
                Url = ReportUrl + "Inventory/FGReceiveReport/FGReceiveFromProduction?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 22)
            {
                Url = ReportUrl + "Inventory/FGReceiveReport/FGReceiveFromOthers?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 23)
            {
                Url = ReportUrl + "Inventory/GiftItemReport/GiftReceive?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 24)
            {
                Url = ReportUrl + "Inventory/FGReceiveReport/RequisitionPendingForIssue?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 25)
            {
                Url = ReportUrl + "Inventory/GiftItemReport/GiftRegister?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 28)
            {
                Url = ReportUrl + "Inventory/FGReceiveReport/RequisitionPendingForDispatch?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 33)
            {
                Url = ReportUrl + "Inventory/FGReceiveReport/RequisitionPendingForReceive?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 37)
            {
                Url = ReportUrl + "Inventory/FGReceiveReport/OverallRequisitionStatus?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 38)
            {
                Url = ReportUrl + "Inventory/FGReceiveReport/OverallRequisitionStatusDetails?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 39)
            {
                Url = ReportUrl + "Inventory/DistributionReport/DispatchChallan?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 71)
            {
                Url = ReportUrl + "Inventory/DistributionReport/DispatchChallanV2?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 46)
            {
                Url = ReportUrl + "Inventory/DistributionReport/DistributionChallan?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 91)
            {
                Url = ReportUrl + "Inventory/DistributionReport/DeliveryChallan?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 90)
            {
                Url = ReportUrl + "Inventory/DistributionReport/HelperChallan?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 89)
            {
                Url = ReportUrl + "Inventory/DistributionReport/PickerChallan?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 50)
            {
                Url = ReportUrl + "Inventory/DistributionReport/PendingInvoices?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 50)
            {
                Url = ReportUrl + "Inventory/DistributionReport/PendingInvoices?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 51)
            {
                Url = ReportUrl + "Inventory/DistributionReport/SkuWisePendingDelivery?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 52)
            {
                Url = ReportUrl + "Inventory/DistributionReport/DateWisePendingDelivery?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 53)
            {
                Url = ReportUrl + "Inventory/DistributionReport/DateWiseDeliveryList?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 54)
            {
                Url = ReportUrl + "Inventory/DistributionReport/InvoiceWisePendingDelivery?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 74)
            {
                Url = ReportUrl + "Inventory/DistributionReport/InvoiceStatus?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 77)
            {
                Url = ReportUrl + "Inventory/DistributionReport/ReceivingForRefurbishment?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 88)
            {
                Url = ReportUrl + "Inventory/DistributionReport/FinalizingForRefurbishment?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 78)
            {
                Url = ReportUrl + "Inventory/FGReceiveReport/DailyBatchRcvRegister?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 79)
            {
                Url = ReportUrl + "Inventory/FGReceiveReport/DateWiseFgRcvFromOtherRegister?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 80)
            {
                Url = ReportUrl + "Inventory/FGReceiveReport/DateWiseMiscellaneousRegister?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 81)
            {
                Url = ReportUrl + "Inventory/FGReceiveReport/DateWiseReceivingRegister?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 82)
            {
                Url = ReportUrl + "Inventory/FGReceiveReport/DateWiseConsumptionRegi?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 87)
            {
                Url = ReportUrl + "Inventory/FGReceiveReport/BatchSizeVsReceivingQty?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 104)
            {
                Url = ReportUrl + "Inventory/FGReceiveReport/TransferVsReceive?q=" + q;
            }
            return Redirect(Url);

        }

        [HttpGet]
        public async Task<IActionResult> CreatePDF(string ReportId, string Color, ProductReportParameters reportParameters, string IsLogoApplicable = "Yes", string IsCompanyApplicable = "Yes", int companyId = 0)
        {
            int comp_id = companyId == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : companyId;
            string User_Name = User.Claims.Where(x => x.Type == ClaimsType.UserName).FirstOrDefault().Value;
            string User_Id = User.Claims.Where(x => x.Type == ClaimsType.UserId).FirstOrDefault().Value;
            ReportDataGenerator reportData = await _reportService.GeneratePDF(ReportId, GetDbConnectionString(), GetDbConnectionStringSecurity(), comp_id, IsLogoApplicable, IsCompanyApplicable, reportParameters, Convert.ToInt32(User_Id), GetDbConnectionStringSecurity());
            var globalSettings = new GlobalSettings
            {
                ColorMode = Color == "Color" ? DinkToPdf.ColorMode.Color : Color == "Grayscale" ? DinkToPdf.ColorMode.Grayscale : reportData.ColorMode == "Color" ? DinkToPdf.ColorMode.Color : DinkToPdf.ColorMode.Grayscale,
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

            ReportDataGenerator reportData = await _reportService.GeneratePDF(ReportId, GetDbConnectionString(), GetDbConnectionStringSecurity(), comp_id, IsLogoApplicable, IsCompanyApplicable, reportParameters, Convert.ToInt32(User_Id), GetDbConnectionStringSecurity());
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

                DataSet dt = await _reportService.GenerateExcel(ReportId, GetDbConnectionString(), GetDbConnectionStringSecurity(), comp_id, reportParameters);
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "_ExcelReport.xlsx");
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportXL(string ReportId, string UNIT_ID, string DATE_FROM, string DATE_TO, string DIVISITON_CODE, string REGION_CODE, string AREA_CODE, string TERRITORY_CODE, string CUSTOMER_ID, int COMPANY_ID)
        {
            ReportParams model = new ReportParams()
            {
                REPORT_ID = Convert.ToInt32(_commonService.Decrypt(ReportId)),
                UNIT_ID = UNIT_ID,
                DATE_FROM = DATE_FROM,
                DATE_TO = DATE_TO,
                DIVISION_CODE = DIVISITON_CODE,
                REGION_CODE = REGION_CODE,
                AREA_CODE = AREA_CODE,
                TERRITORY_CODE = TERRITORY_CODE,
                CUSTOMER_ID = CUSTOMER_ID,
                COMPANY_ID = COMPANY_ID

            };
            DataTable unitDt = new DataTable();
            var workbook = new XLWorkbook();
            string fileName = "";
            if (UNIT_ID != "ALL")
            {
                unitDt = await _reportService.GetUnitNameById(GetDbConnectionString(), UNIT_ID, User.GetComapanyId());

            }
            else
            {

                unitDt.Columns.Add("UNIT_NAME");
                unitDt.Columns.Add("UNIT_ADDRESS");
                DataRow _ravi = unitDt.NewRow();
                _ravi["UNIT_NAME"] = "All Deport";
                _ravi["UNIT_ADDRESS"] = "-";
                unitDt.Rows.Add(_ravi);

            }
            var unitName = unitDt.Rows[0]["UNIT_NAME"].ToString();
            var address = unitDt.Rows[0]["UNIT_ADDRESS"].ToString();
            var dt = new DataTable();
            if (model.REPORT_ID == 61)
            {
                dt = await _reportService.DistributionAndSkuWiseSalesReport(GetDbConnectionString(), model);
                workbook = dt.ToExcel("DIST_WISE&SKU_WISE_SALES_RPT", unitName, address, DATE_FROM, DATE_TO);
                fileName = "DIST_WISE&SKU_WISE_SALES_RPT.xlsx";

            }
            else if (model.REPORT_ID == 66)
            {
                dt = await _reportService.MonthlyPrimarySalesReport(GetDbConnectionString(), model);
                workbook = dt.ToExcel("Primary_Sales_Rpt_(Value)", unitName, address, DATE_FROM, DATE_TO);
                fileName = "Primary_Sales_Report_(Value).xlsx";


            }
            else if (model.REPORT_ID == 67)
            {
                dt = await _reportService.MonthlyPrimarySalesUnitReport(GetDbConnectionString(), model);
                workbook = dt.ToExcel("Primary_Sales_Rpt_(UNIT)", unitName, address, DATE_FROM, DATE_TO);
                fileName = "Primary_Sales_Report_(UNIT).xlsx";


            }
            using (MemoryStream stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
        //load report View permission

        [HttpPost]
        public string IsReportPermitted([FromBody] ReportPermission report)
        {
            List<ReportPermission> reportPermissions = JsonSerializer.Deserialize<List<ReportPermission>>(HttpContext.Session.GetString(ClaimsType.ReportPermission));
            return JsonSerializer.Serialize(_reportManager.IsReportPermitted(report.REPORT_ID, reportPermissions));

        }
    }
}

