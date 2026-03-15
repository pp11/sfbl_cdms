using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Common;
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

namespace SalesAndDistributionSystem.Areas.Security.Menu.Controllers
{
    [Area("Security")]
    [Authorize]
    public class MenuPermissionController : Controller
    {
        private readonly IMenuPermissionManager _service;
        private readonly IUserMenuConfigManager _UserMenuConfigservice;
        private readonly IHttpContextAccessor _Accessor;

        private readonly ILogger<MenuPermissionController> _logger;
        private readonly IConfiguration _configuration;

        public MenuPermissionController(IMenuPermissionManager service, ILogger<MenuPermissionController> logger, IConfiguration configuration, IUserMenuConfigManager UserMenuConfigservice, IHttpContextAccessor Accessor)
        {
            _service = service;
            _logger = logger;
            _configuration = configuration;
            _UserMenuConfigservice = UserMenuConfigservice;
            _Accessor = Accessor;
        }

        private string GetDbConnectionString() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.DbSpecifier).Value.ToString();

        private string GetPermissionString() => HttpContext.Session.GetString(ClaimsType.RolePermission) != null ? HttpContext.Session.GetString(ClaimsType.RolePermission).ToString() : null;

        [AuthorizeCheck]
        public IActionResult Index()
        {
            _logger.LogInformation("Menu Permission(MenuPermission / Index) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View();
        }

        [HttpPost]
        public IActionResult GetPermissions([FromBody] UserPermission model)
        {
            MenuDistribution distribution = JsonSerializer.Deserialize<MenuDistribution>(this.GetPermissionString());
            if (distribution != null)
            {
                PermittedMenu permittedMenu = distribution.PermittedMenus.Where(x => x.ACTION == model.Action_Name && x.CONTROLLER == model.Controller_Name).FirstOrDefault();
                permittedMenu.USER_TYPE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value;
                return Json(permittedMenu);
            }
            return null;
        }

        public async Task<string> UserMenuConfigSelectionList([FromBody] UserMenuConfigView UserMenuConfigView)
        {
            int comp_id = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value);
            string result = await _UserMenuConfigservice.UserMenuConfigSelectionList(GetDbConnectionString(), comp_id, UserMenuConfigView.USER_ID);
            return result;
        }

        public async Task<string> GetSearchableUsers([FromBody] User_Info model)
        {
            int comp_id = model == null || model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
            return await _UserMenuConfigservice.GetSearchableUsers(GetDbConnectionString(), comp_id, model.USER_NAME);
        }

        public async Task<string> GetSearchableCentralUsers([FromBody] User_Info model)
        {
            return await _UserMenuConfigservice.GetSearchableCentralUsers(GetDbConnectionString(), model.USER_NAME);
        }

        [HttpPost]
        public async Task<IActionResult> SaveRoleMenuConfiguration([FromBody] List<Menu_User_Configuration> model)
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
                    if (item.USER_CONFIG_ID == 0)
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
                result = await _UserMenuConfigservice.AddUserMenuConfiguration(GetDbConnectionString(), model);
            }

            return Json(result);
        }

        public IActionResult GetSidebarMenu()
        {
            var claims = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.RolePermission).Value;

            MenuDistribution menuDistribution = JsonSerializer.Deserialize<MenuDistribution>(claims);

            return PartialView("GetSidebarMenu", menuDistribution);
        }

        [HttpGet]
        public IActionResult GetMenuDistribution()
        {
            var claims = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.RolePermission).Value;

            MenuDistribution menuDistribution = JsonSerializer.Deserialize<MenuDistribution>(claims);
            return Json(menuDistribution.PermittedModules);
        }

        [HttpGet]
        public IActionResult GetUserInfo()
        {
            var claims = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value;

            return Json(claims);
        }

        [HttpPost]
        public string SearchableMenuLoad([FromBody] string MENU_NAME)
        {
            string user_id = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value;
            string comp_id = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value;
            return _service.SearchableMenuLoad(GetDbConnectionString(), comp_id, user_id, MENU_NAME);
        }

        //--------Menu And Nev Holder--------------------------        
        [HttpPost]
        public string MenuCookieHolerSet([FromBody] string Value)
        {
            if (Value != null)
            {
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddMinutes(720);
                _Accessor.HttpContext.Response.Cookies.Append("MenuHolder", Value, option);
                return "1";
            }
            return "0";
        }

        public string NavCookieHolderSet(string pageTop = "", string accordionSidebar = "")
        {
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddMinutes(720);
            _Accessor.HttpContext.Response.Cookies.Append("page-top", pageTop, option);
            _Accessor.HttpContext.Response.Cookies.Append("accordionSidebar", accordionSidebar, option);
            return "OK";
        }

        [HttpPost]
        public string MenuCookieHolerGet([FromBody] string Value)
        {
            string result = "";
            if (_Accessor.HttpContext.Request.Cookies["MenuHolder"] != null)
            {
                result = _Accessor.HttpContext.Request.Cookies["MenuHolder"];
            }
            return result;
        }

        [HttpPost]
        public string NavCookieHolderGet([FromBody] string Value)
        {
            string result = "";
            if (_Accessor.HttpContext.Request.Cookies["NavCookieHolder"] != null)
            {
                result = _Accessor.HttpContext.Request.Cookies["NavCookieHolder"];
            }
            return result;
        }
    }
}