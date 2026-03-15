using DocumentFormat.OpenXml.EMMA;
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
    public class ProductController : Controller
    {
        private readonly IProductManager _service;
        private readonly ILogger<ProductController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();

        public ProductController(IProductManager service, ILogger<ProductController> logger, IConfiguration configuration)
        {
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }
        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");
       
        public string GetCompany() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString();

        [AuthorizeCheck]
        public IActionResult ProductInfo()
        {
            _logger.LogInformation("ProductInfo (Product/ProductInfo) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View();
        }

        [HttpGet]
        public async Task<string> LoadSkuCodeData()
        {

            int comp_id =  Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value);
            return await _service.LoadSkuCodeData(GetDbConnectionString(), comp_id);

        }  
        [HttpGet]
        public async Task<string> LoadPackSizeData()
        {

            int comp_id =  Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value);
            return await _service.LoadPackSizeData(GetDbConnectionString(), comp_id);

        }
        public IActionResult ProductPrimaryInfo()
        {
            _logger.LogInformation("ProductInfo (Product/ProductPrimaryInfo) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");
            return View();
        }
        [HttpPost]
        public async Task<string> LoadProductPrimaryData([FromBody] Product_Info product_Info)
        {

            int comp_id = product_Info == null || product_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : product_Info.COMPANY_ID;
            return await _service.LoadProductPrimaryData(GetDbConnectionString(), comp_id);

        }

        [HttpPost]
        public async Task<string> LoadData([FromBody] Product_Info product_Info)
        {

            int comp_id = product_Info == null || product_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : product_Info.COMPANY_ID;
            return await _service.LoadData(GetDbConnectionString(), comp_id);

        } 
        [HttpPost]
        public async Task<string> LoadProductdropdownData([FromBody] Product_Info product_Info)
        {

            int comp_id = product_Info == null || product_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : product_Info.COMPANY_ID;
            return await _service.LoadProductdropdownData(GetDbConnectionString(), comp_id);

        }

        [HttpPost]
        public async Task<string> LoadDataFromView([FromBody] Product_Info product_Info)
        {
            int comp_id = product_Info == null || product_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : product_Info.COMPANY_ID;
            return await _service.LoadDataFromView(GetDbConnectionString(), comp_id);
        }
        [HttpPost]
        public async Task<string> LoadDropDownDataFromView([FromBody] Product_Info product_Info)
        {

            int comp_id = product_Info == null || product_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : product_Info.COMPANY_ID;
            return await _service.LoadDropDownDataFromView(GetDbConnectionString(), comp_id);
        }

        [HttpPost]
        public async Task<string> LoadFilteredData([FromBody] Price_Dtl_Param Price_Dt_Info)
        {
            if (!ModelState.IsValid)
            {
                var arr = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
            }
            int comp_id = Price_Dt_Info == null || Price_Dt_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : Price_Dt_Info.COMPANY_ID;
            Price_Dt_Info.COMPANY_ID = comp_id;
            Price_Dt_Info.db = GetDbConnectionString();
            
            return await _service.LoadFilteredData(Price_Dt_Info);

        }
        

       [HttpPost]
        public async Task<string> GetSearchableProduct([FromBody] Product_Info product_Info)
        {

            int comp_id = product_Info == null || product_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : product_Info.COMPANY_ID;
            return await _service.GetSearchableProduct(GetDbConnectionString(), comp_id, product_Info.SKU_NAME);

        }
        [HttpGet]
        public async Task<string> LoadSKU_DEPOTData() => await _service.LoadSKU_DEPOTData(GetDbConnectionString(),1);

        [HttpGet]
        public async Task<JsonResult> DeleteSkuDepotRelation(int sku_depo_id)
        {
            string result = "";

            if (sku_depo_id >0)
            {
                result = await _service.DeleteSkuDepoRelation(GetDbConnectionString(), sku_depo_id.ToString());
            }
            else
            {
                result = "Please provide the right relation data";
            }


            return Json(result);


        }
        [HttpPost]
        public async Task<JsonResult> AddSkuDepotRelation([FromBody] SKU_DEPOT_RELATION model)
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
                    if (model.SKU_DEPO_ID == 0)
                    {
                        model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();

                        model.COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;

                    }
                    
                    result = await _service.AddOSkuDepoRelation(GetDbConnectionString(), model);

                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }

            }


            return Json(result);


        }
        [HttpPost]
        public async Task<JsonResult> AddOrUpdate([FromBody] Product_Info model)
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
                    if (model.SKU_ID == 0)
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

        //---------------Other Methods --------------------
        [HttpPost]
        public async Task<string> GenerateProductCode([FromBody] Product_Info product_Info)
        {
            int comp_id = product_Info == null || product_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : product_Info.COMPANY_ID;
            return await _service.GenerateProductCode(GetDbConnectionString(), comp_id.ToString());
          
        }

        [HttpPost]
        public async Task<string> LoadProductByProductCode([FromBody] Product_Info product_Info)
        {
            int comp_id = product_Info == null || product_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : product_Info.COMPANY_ID;
            return await _service.LoadProductByProductCode(GetDbConnectionString(), comp_id, product_Info.SKU_CODE);

        }
        [HttpGet]
        public async Task<string> LoadProductSegmentInfo()
        {
            return await _service.LoadProductSegmentInfo(GetDbConnectionString());
        }


    }
}
