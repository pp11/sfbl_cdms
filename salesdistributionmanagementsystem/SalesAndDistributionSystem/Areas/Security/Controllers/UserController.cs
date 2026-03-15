using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Models.TableModels.Security;
using SalesAndDistributionSystem.Domain.Models.TableModels.User;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.User;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SalesAndDistributionSystem.Areas.Security.User.Controllers
{
    [Area("Security")]
    public class UserController : Controller
    {
        private readonly IUserManager _service;
        private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ServiceProvider Provider = new ServiceProvider();

        public UserController(IUserManager service, ILogger<UserController> logger, IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _service = service;
            _logger = logger;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }


        private string GetDbConnectionString() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.DbSpecifier).Value.ToString();
        private string GetDbSDSConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");
        public string GetCompany() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString();
        public string GetUnit() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value.ToString();

        private string GetPermissionString() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.RolePermission).Value.ToString();

        private string GetUserTypeString() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString();
        public string GetDistributorCode() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.DistributorId).Value.ToString();

        public IActionResult Index()
        {
            _logger.LogInformation("User Config(User/Index)  Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");
            return View();
        }

        public IActionResult DefaultPage()
        {
            _logger.LogInformation("User DefaultPage(User/DefaultPage)  Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View();
        }

        public string LoadData()
        {
            int comp_id = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value);
            if (GetUserTypeString() == UserType.SuperAdmin)
            {
                return _service.GetUsers(GetDbConnectionString());
            }
            else
            {
                return _service.GetUsersByCompany(GetDbConnectionString(), comp_id);
            }
        }

        [HttpPost]
        public async Task<JsonResult> AddOrUpdate([FromBody] User_Info model)
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
                    if (model.USER_ID == 0)
                    {
                        model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        if (model.COMPANY_ID == 0)
                        {
                            model.COMPANY_ID = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value);
                        }
                    }
                    else
                    {
                        model.UPDATED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                    }
                    string Url = _hostingEnvironment.IsDevelopment() == true ? CodeConstants.Email_URL : CodeConstants.Email_URL_RELEASE;

                    result = await _service.AddOrUpdate(GetDbConnectionString(), model, _hostingEnvironment.WebRootPath + "/Templates/EmailTemplate/AccountVerification_EmailTemplate.cshtml", Url);
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }

            return Json(result);
        }

        [HttpPost]
        public string GetEmployeeWithoutAccount(User_Info model)
        {
            int comp_id = model == null || model.COMPANY_ID < 1 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
            return _service.GetEmployeesWithoutAccount(GetDbConnectionString(), comp_id);
        }

        //-------------------------Default Pages ------------------------------------------------------------------
        [HttpPost]
        public async Task<string> GetSearchableDefaultPages([FromBody] Default_Page model)
        {
            int comp_id = model == null || model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
            return await _service.LoadSearchableDefaultPages(GetDbConnectionString(), comp_id, model.MENU_NAME);
        }

        [HttpPost]
        public async Task<string> LoadDefaultPages([FromBody] Default_Page model)
        {
            int comp_id = model == null || model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
            return await _service.LoadDefaultPages(GetDbConnectionString(), comp_id);
        }

        [HttpPost]
        public async Task<JsonResult> AddOrUpdateDefaultPage([FromBody] Default_Page model)
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
                    if (model.ID == 0)
                    {
                        model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        if (model.COMPANY_ID == 0)
                        {
                            model.COMPANY_ID = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value);
                        }
                    }
                    else
                    {
                        model.UPDATED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                    }

                    result = await _service.AddOrUpdateDefaultPage(GetDbConnectionString(), model);
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }

            return Json(result);
        }

        //---------------User Verification--------------------------------------------------
        [HttpGet]
        public IActionResult AccountVerification(Auth auth)
        {
            return View(_service.IsVerified("myconn", auth.UniqueId));
        }

        [HttpGet]
        public IActionResult PagePermissionNotice(Auth auth)
        {
            return View();
        }

        [HttpPost]
        public string LoadUsersByCompanyId([FromBody] User_Info model)
        {
            int comp_id = model == null || model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
            return _service.GetUsersByCompany(GetDbConnectionString(), comp_id);
        }
        [HttpGet]
        [AuthorizeCheck]
        public IActionResult UserPasswords()
        {
            return View();
        }

        [HttpPost]
        public List<User_Info> GetPasswords(string password)
        {
            if (password == "admincanshowpassword")
            {
                return  _service.GetPasswords(GetDbConnectionString());
            }
            return new List<User_Info>();
        }

        //im//
        public async Task<string> GetAllCodeAndDropdownListData()
        {
            return await _service.GetAllCodeAndDropdownListData(GetDbSDSConnectionString(), GetCompany(), GetUnit());
        }

        [HttpPost]
        public async Task<IActionResult> ActivateUser([FromBody] User_Info user)
        {
            string result = await _service.ActivateUser(GetDbConnectionString(), user.USER_ID);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> DeactivateUser([FromBody] User_Info user)
        {
            string result = await _service.DeactivateUser(GetDbConnectionString(), user.USER_ID);
            return Json(result);
        }
    }
}