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
    public class MarketController : Controller
    {
        private readonly IMarketManager _service;
        private readonly ILogger<MarketController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();

        public MarketController(IMarketManager service, ILogger<MarketController> logger, IConfiguration configuration)
        {
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }
        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");
       
        public string GetCompany() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString();

        [AuthorizeCheck]
        public IActionResult MarketInfo()
        {
            _logger.LogInformation("MarketInfo (Divison/MarketInfo) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View();
        }

    

        [HttpPost]
        public async Task<string> LoadData([FromBody] Market_Info market_Info)
        {

            int comp_id = market_Info == null || market_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : market_Info.COMPANY_ID;
            return await _service.LoadData(GetDbConnectionString(), comp_id);

        }
        [HttpPost]
        public async Task<string> MarketDropDownDataData([FromBody] Market_Info market_Info)
        {

            int comp_id = market_Info == null || market_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : market_Info.COMPANY_ID;
            return await _service.MarketDropDownDataData(GetDbConnectionString(), comp_id);

        }
        [HttpPost]
        public async Task<string> LoadActiveMarkets([FromBody] Market_Info market_Info)
        {

            int comp_id = market_Info == null || market_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : market_Info.COMPANY_ID;
            return await _service.LoadActiveMarkets(GetDbConnectionString(), comp_id);

        }
        

       [HttpPost]
        public async Task<string> GetSearchableMarket([FromBody] Market_Info market_Info)
        {

            int comp_id = market_Info == null || market_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : market_Info.COMPANY_ID;
            return await _service.GetSearchableMarket(GetDbConnectionString(), comp_id, market_Info.MARKET_NAME);

        }

        [HttpPost]
        public async Task<JsonResult> AddOrUpdate([FromBody] Market_Info model)
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
                    if (model.MARKET_ID == 0)
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

        [HttpPost]
        public async Task<string> GetDivitionToMarketRelation([FromBody] Market_Info market_Info)
        {
            //row_num : DIVISION_ID 
            int comp_id = market_Info == null || market_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : market_Info.COMPANY_ID;
            return await _service.GetDivitionToMarketRelation(GetDbConnectionString()/*, market_Info.ROW_NO*/);

        }

        //---------------Other Methods --------------------
        [HttpPost]
        public async Task<string> GenerateMarketCode([FromBody] Market_Info market_Info)
        {
            int comp_id = market_Info == null || market_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : market_Info.COMPANY_ID;
            return await _service.GenerateMarketCode(GetDbConnectionString(), comp_id.ToString());
          
        }

        
    }
}
