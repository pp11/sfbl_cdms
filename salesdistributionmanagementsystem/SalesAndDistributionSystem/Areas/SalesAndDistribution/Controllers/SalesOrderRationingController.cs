using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ServiceProvider = SalesAndDistributionSystem.Common.ServiceProvider;

namespace SalesAndDistributionSystem.Areas.SalesAndDistribution.Controllers
{
    [Area("SalesAndDistribution")]
    public class SalesOrderRationingController : Controller
    {
        private readonly ISalesOrderRationingManager _service;
        private readonly ICommonServices _comservice;

        private readonly ILogger<SalesOrderRationingController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();

        public SalesOrderRationingController(ICommonServices comservice, ISalesOrderRationingManager service, ILogger<SalesOrderRationingController> logger, IConfiguration configuration)
        {
            _comservice = comservice;
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }
        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");
        private string GetDbSecurityConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "Security");

        public string GetCompany() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString();

        public string GetUnit() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value.ToString();


        public IActionResult InsertOrEdit()
        {
            _logger.LogInformation("Sales Order Reationing (SalesAndDistribution/SalesOrderRationing/InsertOrEdit) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");
            return View();
        }

       
      
        [HttpPost]
        public async Task<string> LoadRationingData([FromBody] OrderSKUFilterParameters division_Info)
        {
            division_Info.COMPANY_ID = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value);
            division_Info.UNIT_ID = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) ;
            division_Info.db_security = GetDbSecurityConnectionString();
            division_Info.db_sales = GetDbConnectionString();
            division_Info.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
            division_Info.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
            division_Info.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
            return await _service.LoadRationingData(division_Info);
        }
       
      

        [HttpPost]
        public async Task<JsonResult> AddOrUpdate([FromBody] List<SalesOrderRationing> model)
        {
            Result _result = new Result();

            if (model.Count ==0)
            {
                _result.Status = "No Data Found!";
            }
            else
            {
                try
                {
                   foreach(var item in model)
                    {
                        item.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        item.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        item.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        item.COMPANY_ID = item.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : item.COMPANY_ID;
                        item.UPDATED_BY = item.ENTERED_BY;
                        item.UPDATED_DATE = item.ENTERED_DATE;
                        item.UPDATED_TERMINAL = item.ENTERED_TERMINAL;
                        item.UNIT_ID = item.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : item.UNIT_ID;
                        item.db_security = GetDbSecurityConnectionString();
                        item.db_sales = GetDbConnectionString();
                        item.ORDER_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                    }
                   _result.Key  = await _service.AddOrUpdate(GetDbConnectionString(), model);
                   _result.Status = "1";
                }
                catch (Exception ex)
                {
                    _result.Status = ex.Message;
                }

            }


            return Json(_result);


        }
    }
}
