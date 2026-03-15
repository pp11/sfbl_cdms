using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Areas.Security.Menu.Controllers;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Models.TableModels.Company;
using SalesAndDistributionSystem.Domain.Models.TableModels.Security;
using SalesAndDistributionSystem.Domain.Models.ViewModels.Security;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.Security;
using SalesAndDistributionSystem.Services.Business.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Areas.Security.Role.Controllers
{
    [Area("Security")]
    public class RoleController : Controller
    {
        private readonly IRoleManager _service;
        private readonly ILogger<RoleController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserManager _serviceUser;
        public RoleController(IRoleManager service, ILogger<RoleController> logger, IConfiguration configuration, IUserManager serviceUser)
        {
            _service = service;
            _logger = logger;
            _configuration = configuration;
            _serviceUser = serviceUser;
        }

        private string GetDbConnectionString() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.DbSpecifier).Value.ToString();

        [AuthorizeCheck]
        public IActionResult Index()
        {
      
            _logger.LogInformation("Role Config(Role/Index)  Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");


            return View();
        }

        [AuthorizeCheck]

        public IActionResult RoleMenuConfig()
        {
            _logger.LogInformation("Role Menu Config(Role/RoleMenuConfig) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View();
        }
        [AuthorizeCheck]

        public IActionResult RoleUserConfig()
        {
            _logger.LogInformation("Role User Config(Role/RoleUserConfig)  Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View();
        }
        [AuthorizeCheck]

        public IActionResult CentralRoleUserConfig()
        {
            _logger.LogInformation("CentralRole User Config(Role/CentralRoleUserConfig) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");
            return View();
        }
        [HttpPost]
        public string LoadData([FromBody] Company_Info company_Info)
        {
            int comp_id = company_Info==null || company_Info.COMPANY_ID==0? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value): company_Info.COMPANY_ID;
            return _service.LoadData(GetDbConnectionString(), comp_id);

        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> AddOrUpdate([FromBody] Role_Info model)
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
                    if (model.ROLE_ID == 0)
                    {
                        model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        model.COMPANY_ID = model.COMPANY_ID == 0? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value): model.COMPANY_ID;

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
        public async Task<IActionResult> ActivateRole([FromBody] Role_Info role_Info)
        {
            string result = await _service.ActivateRole(GetDbConnectionString(), role_Info.ROLE_ID);

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> DeactivateRole([FromBody] Role_Info role_Info)
        {
            string result = await _service.DeactivateRole(GetDbConnectionString(), role_Info.ROLE_ID);

            return Json(result);
        }

        //-------------------------------------Role Menu Confiq -------------------------------------------------------------------------------
        public async Task<string> RoleMenuConfigSelectionList([FromBody] RoleMenuConfigView roleMenuConfigView)
        {
            int comp_id = roleMenuConfigView.COMPANY_ID==0? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) :roleMenuConfigView.COMPANY_ID;
            string result = await _service.RoleMenuConfigSelectionList(GetDbConnectionString(), comp_id, roleMenuConfigView.ROLE_ID);
            return result;

        }


        public async Task<string> GetSearchableRoles([FromBody] Role_Info model)
        {
            int comp_id =model.COMPANY_ID==0? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value): model.COMPANY_ID;
            return await _service.GetSearchableRoles(GetDbConnectionString(), comp_id, model.ROLE_NAME);

        }

        [HttpPost]
        public async Task<IActionResult> SaveRoleMenuConfiguration([FromBody] List<Role_Menu_Configuration> model)
        {
            string result ="";
            if(model == null)
            {
                result  = "No data provided to insert!!!!";
            }
            else
            {
                int comp_id =model[0].COMPANY_ID == 0? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model[0].COMPANY_ID;
               foreach (var item in model)
                {
                    if (item.ROLE_CONFIG_ID == 0)
                    {
                        item.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        item.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        item.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        item.COMPANY_ID = item.COMPANY_ID==0? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value): item.COMPANY_ID;

                    }
                    else
                    {
                        item.UPDATED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        item.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        item.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                    }
                }
                result = await _service.AddRoleMenuConfiguration(GetDbConnectionString(), model);

            }


            return Json(result);
        }

        //------------------------------Role User Config ----------------------------------------------------------------------
        public async Task<string> RoleUserConfigSelectionList([FromBody] Role_User_Configuration roleUserConfigView)
        {
            int comp_id = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value);
            string result = await _service.RoleUserConfigSelectionList(GetDbConnectionString(), comp_id, roleUserConfigView.USER_ID);
            return result;

        }


        [HttpPost]
        public async Task<IActionResult> SaveRoleUserConfiguration([FromBody] List<Role_User_Configuration> model)
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
                   
                        item.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        item.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        item.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        item.COMPANY_ID = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value);
                        item.PERMITTED_BY = item.ENTERED_BY;
                        item.PERMITE_DATE = item.ENTERED_DATE;
                    
                    
                }
                result = await _service.AddRoleUserConfiguration(GetDbConnectionString(), model);

            }


            return Json(result);
        }


        //-----------------------Central Roll User Config -------------------------------------

        public async Task<string> RoleCentralUserConfigSelectionList([FromBody] Role_User_Configuration roleUserConfigView)
        {
            
            string result = await _service.RoleCentralUserConfigSelectionList(GetDbConnectionString(), roleUserConfigView.USER_ID);
            return result;

        }

        [HttpPost]
        public async Task<IActionResult> SaveCentralRoleUserConfiguration([FromBody] List<Role_User_Configuration> model)
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

                    item.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                    item.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                    item.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                    item.COMPANY_ID = _serviceUser.GetCompanyIdByUserId(GetDbConnectionString(), item.USER_ID);
                    item.PERMITTED_BY = item.ENTERED_BY;
                    item.PERMITE_DATE = item.ENTERED_DATE;


                }
                result = await _service.AddRoleUserConfiguration(GetDbConnectionString(), model);

            }


            return Json(result);
        }
    }
}
