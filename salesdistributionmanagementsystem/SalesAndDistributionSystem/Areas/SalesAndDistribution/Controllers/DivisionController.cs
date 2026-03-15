using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution;
using SalesAndDistributionSystem.Services.Business.Security;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Areas.SalesAndDistribution.Controllers
{
    [Area("SalesAndDistribution")]
    public class DivisionController : Controller
    {
        private readonly IDivisionManager _service;
        private readonly ILogger<DivisionController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();

        public DivisionController(IDivisionManager service, ILogger<DivisionController> logger, IConfiguration configuration)
        {
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }
        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");
        private string GetDbSecurityConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "Security");

        public string GetCompany() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString();

        [AuthorizeCheck]
        public IActionResult DivisionInfo()
        {
            _logger.LogInformation("DivisionInfo (Divison/DivisionInfo) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View();
        }

    

        [HttpPost]
        public async Task<string> LoadData([FromBody] Division_Info division_Info)
        {

            division_Info.COMPANY_ID  = division_Info == null || division_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : division_Info.COMPANY_ID;
            division_Info.UNIT_ID = division_Info.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : division_Info.UNIT_ID;
            division_Info.db_security = GetDbSecurityConnectionString();
            division_Info.db_sales = GetDbConnectionString();
            division_Info.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
            division_Info.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
            division_Info.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();

            return await _service.LoadData(division_Info);

        }
        [HttpPost]
        public async Task<string> LoadActiveDivisionData([FromBody] Division_Info division_Info)
        {

            division_Info.COMPANY_ID = division_Info == null || division_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : division_Info.COMPANY_ID;
            division_Info.db_security = GetDbSecurityConnectionString();
            division_Info.db_sales = GetDbConnectionString();

            return await _service.LoadActiveDivisionData(division_Info);

        }
        [HttpPost]
        public async Task<string> GetSearchableDivision([FromBody] Division_Info division_Info)
        {

            division_Info.COMPANY_ID = division_Info == null || division_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : division_Info.COMPANY_ID;
            return await _service.GetSearchableDivision(division_Info);

        }

        [HttpPost]
        public async Task<JsonResult> AddOrUpdate([FromBody] Division_Info model)
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
                    model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                    model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                    model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                    model.COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
                    model.UPDATED_BY = model.ENTERED_BY;
                    model.UPDATED_DATE = model.ENTERED_DATE;
                    model.UPDATED_TERMINAL = model.ENTERED_TERMINAL;
                    model.UNIT_ID = model.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : model.UNIT_ID;
                    model.db_security = GetDbSecurityConnectionString();
                    model.db_sales = GetDbConnectionString();
                    result = await _service.AddOrUpdate( model);

                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }

            }


            return Json(result);


        }

        //---------------Other Methods --------------------
        [HttpPost]
        public async Task<string> GenerateDivisionCode([FromBody] Division_Info division_Info)
        {
            division_Info.COMPANY_ID = division_Info == null || division_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : division_Info.COMPANY_ID;
            return await _service.GenerateDivisionCode(division_Info);
          
        }

        
    }
}
