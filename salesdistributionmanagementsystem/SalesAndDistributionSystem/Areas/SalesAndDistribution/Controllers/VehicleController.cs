using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Areas.SalesAndDistribution.Controllers
{
    [Area("SalesAndDistribution")]
    [Authorize]
    public class VehicleController : Controller
    {
        private readonly IVehicleManager _service;
        private readonly ILogger<VehicleController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();

        public VehicleController(IVehicleManager service, ILogger<VehicleController> logger, IConfiguration configuration)
        {
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }

        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");

        public string GetCompany() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString();

        [AuthorizeCheck]
        public IActionResult VehicleInfo()
        {
            _logger.LogInformation("VehicleInfo Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View();
        }

        [HttpPost]
        public async Task<string> LoadData([FromBody] Vehicle_Info vehicle_Info)
        {
            int comp_id = vehicle_Info == null || vehicle_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : vehicle_Info.COMPANY_ID;
            return await _service.LoadData(GetDbConnectionString(), comp_id);
        }

        [HttpPost]
        public async Task<string> GetVehicleJsonList(Vehicle_Info vehicle_Info)
        {
            int comp_id = vehicle_Info == null || vehicle_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : vehicle_Info.COMPANY_ID;
            return await _service.GetVehicleJsonList(GetDbConnectionString(), comp_id);
        }

        [HttpPost]
        public async Task<string> GetMeasuringUnit([FromBody] Vehicle_Info vehicle_Info)
        {
            int comp_id = vehicle_Info == null || vehicle_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : vehicle_Info.COMPANY_ID;
            return await _service.GetMeasuingUnit(GetDbConnectionString(), comp_id, vehicle_Info.Measuring_Unit_Type);
        }

        [HttpPost]
        public async Task<string> GetSearchableDriver([FromBody] Vehicle_Info vehicle_Info)
        {
            int comp_id = vehicle_Info == null || vehicle_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : vehicle_Info.COMPANY_ID;
            return await _service.GetSearchableVehicle(GetDbConnectionString(), comp_id, vehicle_Info.VEHICLE_NO);
        }

        [HttpPost]
        public async Task<JsonResult> AddOrUpdate([FromBody] Vehicle_Info model)
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
                    if (model.VEHICLE_ID == 0)
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
    }
}