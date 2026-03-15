using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution;
using SalesAndDistributionSystem.Services.Business.Security;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Areas.SalesAndDistribution.Controllers
{
    [Area("SalesAndDistribution")]
    public class AdjustmentInfoController : Controller
    {

        private readonly IAdjustmentInfoManager _service;
        private readonly ILogger<AdjustmentInfoController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();

        public AdjustmentInfoController(IAdjustmentInfoManager service, ILogger<AdjustmentInfoController> logger, IConfiguration configuration)
        {
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }

        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");


        public string GetCompanyID() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString();

        public string GetCompanyName() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyName).Value.ToString();

        public IActionResult AdjustmentInfo()
        {
            return View();
        }



        [HttpPost]
        public async Task<string> InsertOrUpdate([FromBody] Adjustment_Info model)
        {
            Result result = new Result();
            if (model == null)
            {
                result.Status = "No Changes Found!";
            }
            else
            {
                try
                {
                    if (model.ADJUSTMENT_ID == 0)
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
                    return await _service.InsertOrUpdate(GetDbConnectionString(), model);
                }
                catch (Exception ex)
                {
                    result.Status = ex.Message;
                }
            }
            return JsonSerializer.Serialize(result);
        }



        [HttpPost]
        public async Task<string> SearchData([FromBody] Adjustment_Info model)
        {

            int companyId = model == null || model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
            return await _service.SearchData(GetDbConnectionString(), companyId);

        }

    }
}
