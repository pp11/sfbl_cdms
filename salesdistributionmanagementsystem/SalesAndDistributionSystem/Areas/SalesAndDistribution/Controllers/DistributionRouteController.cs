using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Common;

namespace SalesAndDistributionSystem.Areas.SalesAndDistribution.Controllers
{
    [Area("SalesAndDistribution")]
    public class DistributionRouteController : Controller
    {
        private readonly IDistributionRouteManager _service;
        private readonly ILogger<AreaController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();
        public DistributionRouteController(IDistributionRouteManager service,
            ILogger<AreaController> logger,
            IConfiguration configuration)
        {
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }

        private string GetDbConnectionString() => Provider.GetConnectionString(User.GetUserId(), "SalesAndDistribution");


        [HttpPost]
        public async Task<string> LoadData([FromBody] Distribution_Route_Info model)
        {
            var companyId = model.COMPANY_ID != 0 ? model.COMPANY_ID : User.GetComapanyId();
            return await _service.LoadData(GetDbConnectionString(), companyId);
        }

        [HttpGet]
        public IActionResult Route()
        {
            _logger.LogInformation("Distribution Route Info (SalesAndDistribution/DistributionRoute/Route) Page Has been accessed By " + User.GetUserName() + " ( ID= " + User.GetUserId() + " )");
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> AddOrUpdate([FromBody] Distribution_Route_Info model)
        {
            var result = new Result();

            if (!ModelState.IsValid)
            {
                var arr = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                return new JsonResult(arr);
            }
            else
            {
                try
                {
                    if (model.DIST_ROUTE_ID == 0)
                    {
                        model.ENTERED_BY = User.GetUserId();
                        model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        model.COMPANY_ID = User.GetComapanyId();
                    }
                    else
                    {
                        model.UPDATED_BY = User.GetUserId();
                        model.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                    }

                    result.Key = await _service.AddOrUpdate(GetDbConnectionString(), model);
                    result.Status = "1";
                }
                catch (Exception ex)
                {
                    result.Status = ex.Message;
                }
            }

            return Json(result);
        }
    }
}
