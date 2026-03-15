using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.TableModels.Company;
using SalesAndDistributionSystem.Domain.Models.TableModels.Security;
using SalesAndDistributionSystem.Domain.Models.ViewModels.Security;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.Security;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Areas.Security.Menu.Controllers
{
    [Area("Security")]

    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeManager _service;
        private readonly ILogger<MenuMasterController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();

        public EmployeeController(IEmployeeManager service, ILogger<MenuMasterController> logger, IConfiguration configuration)
        {
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }
        private string GetDbConnectionString() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.DbSpecifier).Value.ToString();
        private string GetDbSDSConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");

        [AuthorizeCheck]
        public IActionResult Index()
        {
            _logger.LogInformation("Menu Module (MenuMaster/Index) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString());

            return View();
        }
        public string GetCompany() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString();
        public string GetUnit() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value.ToString();

        [HttpGet]
        public  string LoadData()
        {
            int comp_id = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value);
            return _service.LoadData(GetDbConnectionString(), comp_id);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> AddOrUpdate([FromBody] EMPLOYEE_INFO model)
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
                    //if (model.MENU_ID == 0)
                    //{
                    //    model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                    //    model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                    //    model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                    //    model.COMPANY_ID = model.COMPANY_ID ==0? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value): model.COMPANY_ID;

                    //}
                    //else
                    //{
                    //    model.UPDATED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                    //    model.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                    //    model.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                    //}

                    result = await _service.AddOrUpdate(GetDbConnectionString(),model);

                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }

            }


            return Json(result);


        }

        [HttpPost]
        public async Task<IActionResult> ActivateMenu([FromBody] Menu_Configuration menuCategory)
        {
            string result = await _service.ActivateMenu(GetDbConnectionString(), menuCategory.MENU_ID);

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> DeactivateMenu([FromBody] Menu_Configuration menuCategory)
        {
            string result = await _service.DeactivateMenu(GetDbConnectionString(), menuCategory.MENU_ID);

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] Menu_Configuration menuCategory)
        {
            string result = await _service.DeleteMenu(GetDbConnectionString(), menuCategory.MENU_ID);

            return Json(result);
        }


        public async Task<string> GetSearchableDistributor([FromBody] Customer_Info customer_Info)
        {

            int comp_id = customer_Info == null || customer_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : customer_Info.COMPANY_ID;
            return await _service.GetSearchableDistributor(GetDbSDSConnectionString(), comp_id, customer_Info.CUSTOMER_NAME);

        }


        //im//
        public async Task<string> GetAllCodeAndDropdownListData()
        {
            return await _service.GetAllCodeAndDropdownListData(GetDbSDSConnectionString(), GetCompany(), GetUnit());
        }
    }
}
