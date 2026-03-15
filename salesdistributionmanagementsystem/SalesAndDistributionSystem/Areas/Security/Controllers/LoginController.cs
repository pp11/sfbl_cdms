using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.TableModels.Company;
using SalesAndDistributionSystem.Domain.Models.ViewModels.Security;
using SalesAndDistributionSystem.Domain.Models.ViewModels.User;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Models;
using SalesAndDistributionSystem.Services.Business.Company;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution;
using SalesAndDistributionSystem.Services.Business.Security;
using SalesAndDistributionSystem.Services.Business.User;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Areas.Security.User.Controllers
{
    [Area("Security")]
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IUserManager _accountService;
        private readonly IConfiguration _configuration;
        private readonly IMenuPermissionManager _menuService;
        private readonly ICompanyManager _companyService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _Accessor;
        private readonly IReportConfigurationManager _reportManager;
        private readonly ICommonServices _commonServices;


        public LoginController(ILogger<LoginController> logger, IUserManager accountService, IConfiguration configuration, IMenuPermissionManager menuPermission, ICompanyManager companyManager, IWebHostEnvironment hostingEnvironment, IHttpContextAccessor Accessor, IReportConfigurationManager reportManager, ICommonServices commonServices)
        {
            _logger = logger;
            this._accountService = accountService;
            _configuration = configuration;
            _menuService = menuPermission;
            _companyService = companyManager;
            _hostingEnvironment = hostingEnvironment;
            _Accessor = Accessor;
            _reportManager = reportManager;
            _commonServices = commonServices;
        }
        private string GetEmailOfCurrentUser() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.Email).Value.ToString();
        private string GetUserNameOfCurrentUser() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString();

        public IActionResult Index()
        {

            if (_Accessor.HttpContext.Request.Cookies["LoginMailHolder"] != null)
            {
                ViewBag.UserName = _Accessor.HttpContext.Request.Cookies["LoginMailHolder"];

            }
            if (_Accessor.HttpContext.Request.Cookies["LoginPassHolder"] != null)
            {
                ViewBag.UserPass = _Accessor.HttpContext.Request.Cookies["LoginPassHolder"];

            }
            if (_Accessor.HttpContext.Request.Cookies["LoginCompanyHolder"] != null)
            {
                ViewBag.UserCompany = _Accessor.HttpContext.Request.Cookies["LoginCompanyHolder"];

            }
            if (_Accessor.HttpContext.Request.Cookies["RoleNames"] != null)
            {
                ViewBag.UserCompany = _Accessor.HttpContext.Request.Cookies["RoleNames"];

            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Index([FromForm] Login model)
        {
            try
            {
                if (model.Email != null && model.Password != null && model.Email != "" && model.Password != "")
                {
                    Domain.Common.Auth _user = null;
                    _user = _accountService.GetUserByEmailAndCompany(GetDbConnectionString(model.CompanyId), model.Email, model.CompanyId);
                    if (_user != null)
                    {
                        model.CompanyId = _user.CompanyId;
                        if (_accountService.IsValidUser(GetDbConnectionString(model.CompanyId), model.Email, model.Password, _user.CompanyId, _user.Password))
                        {
                            MenuDistribution menuDistribution = await _menuService.LoadPermittedMenuByUserId(GetDbConnectionString(model.CompanyId), _user.UserId, _user.CompanyId);
                            string menuDis = JsonSerializer.Serialize(menuDistribution);
                            string defaultPage = _menuService.LoadUserDefaultPageById(GetDbConnectionString(model.CompanyId), _user.UserId);
                            defaultPage = defaultPage == null ? "Security/User/PagePermissionNotice" : defaultPage;
                            ServiceProvider provider = new ServiceProvider();
                            List<ReportPermission> reportPermissions = await _reportManager.LoadReportPermissionData(GetDbConnectionString(model.CompanyId), _user.CompanyId, _user.UserId);
                            string reportDis = JsonSerializer.Serialize(reportPermissions);

                            var claims = new List<Claim>()
                            {
                               new Claim(ClaimTypes.Name, _user.UserName),
                               new Claim(ClaimTypes.Role, "Administrator"),
                               new Claim(ClaimTypes.NameIdentifier, _user.UserId.ToString()),
                               new Claim(ClaimsType.UserName, _user.UserName),
                               new Claim(ClaimsType.UserId, _user.UserId.ToString()),
                               new Claim(ClaimsType.Email, _user.Email),
                               new Claim(ClaimsType.UserType, _user.UserType.ToString()),
                               new Claim(ClaimsType.CompanyId, _user.CompanyId.ToString()),
                               new Claim(ClaimsType.UnitId, _user.UnitId.ToString()),
                               new Claim(ClaimsType.UnitName, _user.UnitName.ToString()),

                               new Claim(ClaimsType.CompanyName, _user.CompanyName.ToString()),
                               new Claim(ClaimsType.DistributorId, _user.DistributorId.ToString()),
                               new Claim(ClaimsType.DefaultPage,defaultPage),
                               new Claim(ClaimsType.RoleNames,_user.RoleNames),
                               new Claim(ClaimsType.DbSpecifier, provider.GetConnectionString(_user.CompanyName))

                            };
                            HttpContext.Session.SetString(ClaimsType.RolePermission, menuDis);
                            HttpContext.Session.SetString(ClaimsType.ReportPermission, reportDis);

                            HttpContext.Session.SetString("DefaultPage", defaultPage);

                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                            var authProperties = new AuthenticationProperties
                            {
                                AllowRefresh = true,

                                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(1440),

                                IssuedUtc = DateTime.Now,

                            };

                            await HttpContext.SignInAsync(
                                CookieAuthenticationDefaults.AuthenticationScheme,
                                new ClaimsPrincipal(claimsIdentity),
                                authProperties);

                            _logger.LogInformation("User {Email} logged in at {Time}.", _user.Email, DateTime.UtcNow);
                            if (model.RememberMe == true)
                            {
                                CookieOptions option = new CookieOptions();
                                option.Expires = DateTime.Now.AddDays(29);
                                _Accessor.HttpContext.Response.Cookies.Append("LoginMailHolder", _user.Email, option);
                                _Accessor.HttpContext.Response.Cookies.Append("LoginPassHolder", _commonServices.Decrypt(_user.Password), option);
                                _Accessor.HttpContext.Response.Cookies.Append("LoginCompanyHolder", _user.CompanyId.ToString(), option);

                            }

                            string URL = "~/" + defaultPage;

                            return Redirect(URL);
                        }
                        else
                        {
                            ViewBag.errorMessage = "Error: Wrong Password Entered !";
                            ViewBag.UserName = model.Email;
                        }
                    }
                    else
                    {
                        ViewBag.errorMessage = "Error: Wrong Email Entered, No User Found.";
                        ViewBag.UserName = model.Email;
                    }

                }
                else
                {
                    ViewBag.errorMessage = "Please enter a valid email and password.";
                    ViewBag.UserName = model.Email;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

            }
            return View();
        }

        [AuthorizeCheck]
        public IActionResult ChangePassword()
        {

            _logger.LogInformation("Change Password(Login/ChangePassword) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString());
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(PasswordChangeModel changeModel)
        {
            if (changeModel != null)
            {
                changeModel.USER_ID = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString());
                changeModel.Email = GetEmailOfCurrentUser();
                changeModel.User_Name = GetUserNameOfCurrentUser();
                changeModel.Path = _hostingEnvironment.WebRootPath + "/Templates/EmailTemplate/AccountVerification_EmailTemplate.cshtml";
                changeModel.BaseUrl = _hostingEnvironment.IsDevelopment() == true ? CodeConstants.Email_URL : CodeConstants.Email_URL_RELEASE;
                ViewData["ErrMsg"] = await _accountService.UpdateUserPassword(GetDbConnectionString(), changeModel);
            }
            return View();
        }


        public async Task<IActionResult> ForgetPassword()
        {

            _logger.LogInformation("Forget Password(Login/ForgetPassword) Page Has been accessed");
            List<Company_Info> company_Infos = await _companyService.GetCompanyList(GetDbConnectionString());

            return View(company_Infos);
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(PasswordChangeModel changeModel)
        {
            if (changeModel != null && changeModel.Company_Id > 0)
            {
                Auth _auth = _accountService.GetUserByEmail(GetDbConnectionString(changeModel.Company_Id), changeModel.Email);
                if (_auth != null)
                {
                    changeModel.USER_ID = Convert.ToInt32(_auth.UserId);
                    changeModel.Email = _auth.Email;
                    changeModel.User_Name = _auth.UserName;
                    changeModel.Path = _hostingEnvironment.WebRootPath + "/Templates/EmailTemplate/AccountVerification_EmailTemplate.cshtml";
                    changeModel.BaseUrl = _hostingEnvironment.IsDevelopment() == true ? CodeConstants.Email_URL : CodeConstants.Email_URL_RELEASE;
                    ViewData["Notify"] = await _accountService.ForgetPasswordVerify(GetDbConnectionString(changeModel.Company_Id), changeModel);
                }
                else
                {
                    ViewData["Notify"] = "Please enter valid Email and Select Your Company";
                }
            }
            return View();
        }
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //_Accessor.HttpContext.Response.Cookies.Delete("MenuHolder");
            return LocalRedirect("/");
        }

        private string GetDbConnectionString(int Company_Id = 1)
        {
            ServiceProvider _serve = new ServiceProvider();
            string claim = _serve.GetConnectionString(Company_Id.ToString(), "Security");
            if (claim != null)
            {
                return claim;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
