using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Models.ViewModels.Security;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Common;
using System.Collections.Generic;
using System;
using System.Linq;
using SalesAndDistributionSystem.Services.Business.Security;
using SalesAndDistributionSystem.Services.Business.Target.IManager;
using System.Threading.Tasks;
using System.Text.Json;
using SalesAndDistributionSystem.Domain.Models.ReportModels.Common.ProductReports;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace SalesAndDistributionSystem.Areas.Target.Controllers
{
    [Area("Target")]
    [Authorize]
    public class ReportController : Controller
    {
        private readonly ILogger<ReportController> _logger;
        private readonly ServiceProvider Provider = new ServiceProvider();
        private readonly ITargetReportManager _reportService;
        private readonly IReportConfigurationManager _reportManager;
        private readonly ICommonServices _commonService;
        private readonly IHostEnvironment _hostManager;

        public ReportController(ILogger<ReportController> logger, ICommonServices commonService, ITargetReportManager reportService, IReportConfigurationManager reportManager, IHostEnvironment hostManager)
        {
            _logger = logger;
            _commonService = commonService;
            _reportService = reportService;
            _reportManager = reportManager;
            _hostManager = hostManager;
        }

        private string GetDbConnectionString() => Provider.GetConnectionString(User.GetComapanyId().ToString(), "SalesAndDistribution", "Operation");
        private string GetDbConnectionStringSecurity() => Provider.GetConnectionString(User.GetComapanyId().ToString(), "Security", "Operation");


        public async Task<IActionResult> TargetReport()
        {
            int User_id = Convert.ToInt32(User.GetUserId());

            int Comp_id = User.GetComapanyId();

            List<ReportPermission> reportData = await _reportManager.LoadReportPermissionData(GetDbConnectionStringSecurity(), Comp_id, User_id);
            reportData = reportData.Where(x => x.MENU_ACTION == "TargetReport").OrderBy(x => x.ORDER_BY_SLNO).ToList();
            foreach (ReportPermission reportDataSpecify in reportData)
            {
                reportDataSpecify.ReportIdEncrypt = _commonService.Encrypt(reportDataSpecify.REPORT_ID.ToString());

            }
            return View(reportData);
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
            reportParameters.DB = GetDbConnectionString();
            reportParameters.DB_SECURITY = GetDbConnectionStringSecurity();
            //reportParameters.REPORT_EXTENSION = "pdf";
            //reportParameters.REPORT_ID = ReportId;
            string q = JsonSerializer.Serialize<ReportParams>(reportParameters);
            q = _commonService.Encrypt(q);
            string Url = "";
            if (reportParameters.REPORT_ID == 47)
            {
                Url = ReportUrl + "Target/MarketTargetReport/Target?q=" + q;
            }
            else if(reportParameters.REPORT_ID == 49)
            {
                Url = ReportUrl + "Target/MarketTargetReport/InMarketSales?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 70)
            {
                Url = ReportUrl + "Target/MarketTargetReport/TargetSummary?q=" + q;
            }
            else if (reportParameters.REPORT_ID == 76)
            {
                Url = ReportUrl + "Target/MarketTargetReport/TargetDetail?q=" + q;
            }
            return Redirect(Url);
        }

    }
}
