using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.Security;
using SalesAndDistributionSystem.Domain.Models.TableModels.Company;
using SalesAndDistributionSystem.Domain.Models.TableModels.Security;
using SalesAndDistributionSystem.Domain.Models.TableModels.User;
using SalesAndDistributionSystem.Domain.Models.ViewModels.Security;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Areas.Security.Controllers
{
    [Area("Security")]

    [Authorize]
    public class ReportConfigurationController : Controller
    {
        private readonly IReportConfigurationManager _service;
        private readonly ILogger<ReportConfigurationController> _logger;
        private readonly IConfiguration _configuration;
        public ReportConfigurationController(IReportConfigurationManager service, ILogger<ReportConfigurationController> logger, IConfiguration configuration)
        {
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }
        private string GetDbConnectionString() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.DbSpecifier).Value.ToString();
        [AuthorizeCheck]
        public IActionResult Index()
        {
            _logger.LogInformation("Report Module (ReportConfiguration/Index) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString());

            return View();
        }
        [AuthorizeCheck]
        public IActionResult RoleReportConfig()
        {
            _logger.LogInformation("Report Module (ReportConfiguration/RoleReportConfig) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString());

            return View();
        }
        public IActionResult UserReportConfig()
        {
            _logger.LogInformation("Report Module (ReportConfiguration/RoleReportConfig) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString());

            return View();
        }
        [HttpPost]
        public async Task<string> LoadData([FromBody] Company_Info company_Info)
        {
            int comp_id = company_Info == null || company_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : company_Info.COMPANY_ID;
            return await _service.LoadData(GetDbConnectionString(), comp_id);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> AddOrUpdate([FromBody] Report_Configuration model)
        {
            string result = "";

            if (model == null)
            {
                result = "No Changes Found!";
            }
            else
            {
                try
                {
                    if (model.REPORT_ID == 0)
                    {
                        model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        model.COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
                    }
                    else
                    {
                        model.UPDATED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                    }
                    result = await _service.AddOrUpdate(GetDbConnectionString(), model);
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }
            return Json(result);
        }
        [HttpPost]
        public async Task<IActionResult> ActivateReport([FromBody] Report_Configuration reportConfiguration)
        {
            string result = await _service.ActivateReport(GetDbConnectionString(), reportConfiguration.REPORT_ID);

            return Json(result);
        }
        [HttpPost]
        public async Task<IActionResult> DeactivateReport([FromBody] Report_Configuration reportConfiguration)
        {
            string result = await _service.DeactivateReport(GetDbConnectionString(), reportConfiguration.REPORT_ID);

            return Json(result);
        }
        //Role Report Configuration -------------------------------------------------------------------
        public async Task<string> RoleReportConfigSelectionList([FromBody] RoleReportConfigView roleMenuConfigView)
        {
            int comp_id = roleMenuConfigView.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : roleMenuConfigView.COMPANY_ID;
            string result = await _service.RoleReportConfigSelectionList(GetDbConnectionString(), comp_id, roleMenuConfigView.ROLE_ID);
            return result;
        }
        public async Task<string> GetSearchableRoles([FromBody] Role_Info model)
        {
            int comp_id = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
            return await _service.GetSearchableRoles(GetDbConnectionString(), comp_id, model.ROLE_NAME);

        }
        [HttpPost]
        public async Task<IActionResult> SaveRoleReportConfiguration([FromBody] List<Role_Report_Configuration> model)
        {
            string result = "";
            if (model == null)
            {
                result = "No data provided to insert!!!!";
            }
            else
            {
                int comp_id = model[0].COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model[0].COMPANY_ID;
                foreach (var item in model)
                {
                    if (item.ID == 0)
                    {
                        item.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        item.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        item.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        item.COMPANY_ID = item.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : item.COMPANY_ID;

                    }
                    else
                    {
                        item.UPDATED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        item.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        item.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                    }
                }
                result = await _service.AddRoleReportConfiguration(GetDbConnectionString(), model);

            }
            return Json(result);
        }
        //Report User Configuration----------------------------------------------------
        public async Task<string> UserReportConfigSelectionList([FromBody] UserReportConfigView UserMenuConfigView)
        {
            int comp_id = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value);
            string result = await _service.UserReportConfigSelectionList(GetDbConnectionString(), comp_id, UserMenuConfigView.USER_ID);
            return result;
        }
        public async Task<string> GetSearchableUsers([FromBody] User_Info model)
        {
            int comp_id = model == null || model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
            return await _service.GetSearchableUsers(GetDbConnectionString(), comp_id, model.USER_NAME);

        }
        public async Task<string> GetSearchableCentralUsers([FromBody] User_Info model)
        {
            return await _service.GetSearchableCentralUsers(GetDbConnectionString(), model.USER_NAME);

        }
        [HttpPost]
        public async Task<IActionResult> SaveUserReportConfiguration([FromBody] List<Report_User_Configuration> model)
        {
            string result = "";
            if (model == null)
            {
                result = "No data provided to insert!!!!";
            }
            else
            {
                int comp_id = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value);
                foreach (var item in model)
                {
                    if (item.ID == 0)
                    {
                        item.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        item.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        item.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        item.COMPANY_ID = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value);
                    }
                    else
                    {
                        item.UPDATED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        item.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        item.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                    }
                }
                result = await _service.AddUserReportConfiguration(GetDbConnectionString(), model);
            }
            return Json(result);
        }
    }
}
