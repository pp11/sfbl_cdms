using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.Entities.Target;
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
    [Authorize]
    public class ProductPriceController : Controller
    {
        private readonly IProductPriceManager _service;
        private readonly ILogger<ProductPriceController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();

        public ProductPriceController(IProductPriceManager service, ILogger<ProductPriceController> logger, IConfiguration configuration)
        {
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }
        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");

        public string GetCompany() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString();

        [AuthorizeCheck]
        public IActionResult ProductPriceInfo()
        {
            _logger.LogInformation("ProductPriceInfo (ProductPrice/ProductPriceInfo) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View();
        }
        [HttpPost]
        public async Task<string> LoadData([FromBody] Product_Price_Info product_Price_Info)
        {

            int comp_id = product_Price_Info == null || product_Price_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : product_Price_Info.COMPANY_ID;
            return await _service.LoadData(GetDbConnectionString(), comp_id);

        }

        [HttpPost]
        public async Task<string> GetSearchableProductPrice([FromBody] Product_Info product_Info)
        {

            int comp_id = product_Info == null || product_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : product_Info.COMPANY_ID;
            return await _service.GetSearchableProductPrice(GetDbConnectionString(), comp_id, product_Info.SKU_NAME);

        }

        [HttpPost]
        public async Task<decimal> UnitWiseSkuPrice([FromBody] Product_Info product_Info)
        {
            int comp_id = product_Info == null || product_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : product_Info.COMPANY_ID;
            return await _service.UnitWiseSkuPrice(GetDbConnectionString(), comp_id, User.GetUnitId(), product_Info.SKU_ID, product_Info.SKU_CODE);
        }

        [HttpPost]
        public async Task<decimal> SkuPrice([FromBody] Product_Info product_Info)
        {
            int comp_id = product_Info == null || product_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : product_Info.COMPANY_ID;
            return await _service.SkuPrice(GetDbConnectionString(), comp_id, User.GetUnitId(), product_Info.SKU_ID, product_Info.SKU_CODE);
        }

        [HttpPost]
        public async Task<JsonResult> AddOrUpdate([FromBody] Product_Price_Info model)
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
                    if (model.PRICE_ID == 0)
                    {
                        model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        model.PRICE_ENTRY_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;

                    }
                    else
                    {
                        model.UPDATED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        model.PRICE_ENTRY_DATE = DateTime.ParseExact(model.PRICE_ENTRY_DATE, "dd/MM/yyyy", null).ToString("dd/MM/yyyy hh:mm:ss");
                        model.PRICE_EFFECT_DATE = DateTime.ParseExact(model.PRICE_EFFECT_DATE, "dd/MM/yyyy", null).ToString("dd/MM/yyyy hh:mm:ss");

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


        [AuthorizeCheck]
        public IActionResult BatchPrice(string Id = "0")
        {
            BATCH_PRICE_MST _Mst = new BATCH_PRICE_MST();
            if (Id != "0" || Id != "")
            {
                _Mst.MST_ID_ENCRYPTED = Id;

            }
            _logger.LogInformation("In Market Sales  InsertOrEdit (Target/InMarketSales/index) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");
            return View(_Mst);
        }

        [AuthorizeCheck]
        public IActionResult TPUpdate(string Id = "0")
        {
            BATCH_PRICE_MST _Mst = new BATCH_PRICE_MST();
            if (Id != "0" || Id != "")
            {
                _Mst.MST_ID_ENCRYPTED = Id;

            }
            _logger.LogInformation("In Market Sales  InsertOrEdit (Target/InMarketSales/index) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");
            return View(_Mst);
        }


        [HttpPost]
        public async Task<string> loadBatchWiseStock([FromBody] BATCH_PRICE_MST model)
        {

            int comp_id = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
            return await _service.loadBatchWiseStock(GetDbConnectionString(), model);

        }

        [HttpPost]
        public async Task<string> loadBatchWiseStockTPUpdate([FromBody] BATCH_PRICE_MST model)
        {

            int comp_id = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
            return await _service.loadBatchWiseStockTPUpdate(GetDbConnectionString(), model);

        }
        [HttpPost]
        public async Task<JsonResult> AddOrUpdateBatchPrice([FromBody] BATCH_PRICE_MST model)
        {
            string result = "";

            if (model == null || model.BATCH_PRICE_DTL_LIST.Count <= 0)
            {
                result = "No Data Found!";
            }
            else
            {
                try
                {
                    if (model.MST_ID == 0)
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

                    result = await _service.AddOrUpdateBatchPrice(GetDbConnectionString(), model);

                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }

            }


            return Json(result);


        }


        [HttpPost]
        public async Task<JsonResult> AddOrUpdateTP([FromBody] BATCH_PRICE_MST model)
        {
            string result = "";

            if (model == null || model.BATCH_PRICE_DTL_LIST.Count <= 0)
            {
                result = "No Data Found!";
            }
            else
            {
                try
                {
                    if (model.MST_ID == 0)
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

                    result = await _service.AddOrUpdateTP(GetDbConnectionString(), model);

                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }

            }


            return Json(result);


        }

        [HttpPost]
        public async Task<string> BatchPriceLoadData([FromBody] BATCH_PRICE_MST model)
        {
            int comp_id = model == null || model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
            return await _service.BatchPriceLoadData(GetDbConnectionString(), comp_id);

        }
        [HttpPost]
        public async Task<string> GetEditDataById([FromBody] BATCH_PRICE_MST model)
        {
            BATCH_PRICE_MST batch_price_mst = new BATCH_PRICE_MST();
            //if (mst.q != null && mst.q != "0" && mst.q != "")
            //{
            //int _Id = Convert.ToInt32(_comservice.Decrypt(mst.q));
            batch_price_mst = await _service.GetEditDataById(GetDbConnectionString(), Convert.ToInt32(model.MST_ID_ENCRYPTED));
            //}
            return JsonSerializer.Serialize(batch_price_mst);
        }

    }
}
