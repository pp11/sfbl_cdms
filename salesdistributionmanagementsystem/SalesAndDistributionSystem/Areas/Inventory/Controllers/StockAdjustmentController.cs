using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.Inventory;
using SalesAndDistributionSystem.Services.Business.Inventory.IManager;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution;
using SalesAndDistributionSystem.Services.Business.Security;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    public class StockAdjustmentController : Controller
    {

        private readonly IStockAdjustmentManager _service;
        private readonly ILogger<StockAdjustmentController> _logger;

        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();

        public StockAdjustmentController(IStockAdjustmentManager service, ILogger<StockAdjustmentController> logger, IConfiguration configuration)
        {
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }

        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");

        public string GetCompanyID() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString();

        public string GetCompanyName() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyName).Value.ToString();


        public string GetUnitId() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value.ToString();

        public string GetUnitName() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitName).Value.ToString();


        public IActionResult StockAdjustment()
        {
            return View();
        }


        [HttpPost]
        public async Task<string> GetBatchList([FromBody] Stock_Adjustment stock_adjustment)
        {

            int company_id = stock_adjustment.COMPANY_ID;
            int unit_id = stock_adjustment.UNIT_ID;
            int sku_id = stock_adjustment.SKU_ID;
            return await _service.GetBatchList(GetDbConnectionString(), company_id, unit_id, sku_id);

        }

        [HttpPost]
        public async Task<string> GetSearchData([FromBody] Stock_Adjustment stock_adjustment)
        {

            int company_id = stock_adjustment.COMPANY_ID;
            int unit_id = stock_adjustment.UNIT_ID;
            return await _service.GetSearchData(GetDbConnectionString(), company_id, unit_id);

        }

        [HttpPost]
        public async Task<JsonResult> InsertOrUpdate([FromBody] Stock_Adjustment model)
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
                    if (model.ADJUSTMENT_ID == 0)
                    {
                        model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        //model.COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
                    }
                    else
                    {
                        model.UPDATED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                    }
                    result = await _service.InsertOrUpdate(GetDbConnectionString(), model);
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
