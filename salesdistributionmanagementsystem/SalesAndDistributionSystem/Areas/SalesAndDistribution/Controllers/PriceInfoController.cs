using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Areas.SalesAndDistribution.Controllers
{
    [Area("SalesAndDistribution")]
    public class PriceInfoController : Controller
    {
        private readonly ICustomerPriceInfoManager _service;
        private readonly ILogger<PriceInfoController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();
        private readonly ICommonServices _comservice;
        public PriceInfoController(ICommonServices comservice, ICustomerPriceInfoManager service, ILogger<PriceInfoController> logger, IConfiguration configuration)
        {
            _comservice = comservice;
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }
        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CustomerPriceInfo(string Id = "0",string mode = "update")
        {
            _logger.LogInformation("Customer Price Info  Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ?
                User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");
            Customer_SKU_Price_Mst priceMst = new Customer_SKU_Price_Mst();
            priceMst.CUSTOMER_PRICE_MSTID_ENCRYPTED = Id;
            priceMst.Mode = mode;
            return View(priceMst);
        }
        public string GetCompany() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString();

        [HttpPost]
        public async Task<string> GetEditDataById([FromBody] Customer_SKU_Price_Mst mst)
        {
            Customer_SKU_Price_Mst sku_Price_Mst = new Customer_SKU_Price_Mst();
            if (mst.q != null && mst.q != "0" && mst.q != "")
            {
                int _Id = Convert.ToInt32(_comservice.Decrypt(mst.q));
                sku_Price_Mst = await _service.LoadDetailData_ByMasterId_List(GetDbConnectionString(), _Id);
            }
            return JsonSerializer.Serialize(sku_Price_Mst);
        }
        public IActionResult List()
        {
            _logger.LogInformation("Region Area Relation List(RegionAreaRelation/List) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View();
        }

  
        public IActionResult InsertOrEdit(string Id = "0")
        {
            Region_Area_Mst region_Area_Mst = new Region_Area_Mst();

            if (Id != "0" || Id != "")
            {
                region_Area_Mst.REGION_AREA_MST_ID_ENCRYPTED = Id;
            }
            _logger.LogInformation("Region Area Relation  InsertOrEdit (RegionAreaRelation/InsertOrEdit) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View(region_Area_Mst);
        }

        [HttpPost]
        public async Task<JsonResult> AddOrUpdate([FromBody] Customer_SKU_Price_Mst model)
        {
            string result = "";
            if (!ModelState.IsValid)
            {
                var arr = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
            }
            if (model == null)
            {
                result = "No Changes Found!";
            }
            else
            {
                try
                {
                    if (model.CUSTOMER_PRICE_MSTID == 0)
                    {
                        model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.UNIT_ID = Convert.ToInt32( User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value);
                        model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        model.COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
                        if(model.customerSkuPriceList != null && model.customerSkuPriceList.Count > 0)
                        {
                            foreach(var item in model.customerSkuPriceList)
                            {
                                item.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                                item.UNIT_ID = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value);
                                item.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy");
                                item.STATUS = "Active";
                            }
                        }
                    }
                    else
                    {
                        model.UPDATED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.UNIT_ID = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value);
                        model.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        model.EFFECT_START_DATE = DateTime.ParseExact(model.EFFECT_START_DATE, "dd/MM/yyyy", null).ToString("dd/MM/yyyy hh:mm:ss");
                        model.EFFECT_END_DATE = DateTime.ParseExact(model.EFFECT_END_DATE, "dd/MM/yyyy", null).ToString("dd/MM/yyyy hh:mm:ss");

                        foreach (var item in model.customerSkuPriceList)
                        {

                            item.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                            item.UPDATED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                            item.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                            item.COMPANY_ID = item.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : item.COMPANY_ID;
                            item.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                            item.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                            item.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();

                        }
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

        [HttpGet]
        public async Task<string> GetCustomerExistingSKUData(int CompanyId,int CustomerId)
        {
            return await _service.GetCustomerExistingSKUData(GetDbConnectionString(), CompanyId, CustomerId);
        }

        [HttpPost]
        public async Task<string> LoadSKUPriceDtlDataRestrict([FromBody]  ExistingSKURestrictParams model)
        {
            string[] st = model.sku_id.Split(",");
            List<int> sku_id_ = new List<int>();
            foreach(var item in st)
            {
                sku_id_.Add(Convert.ToInt32(item));
            }

            return await _service.LoadSKUPriceDtlDataRestrict(GetDbConnectionString(), GetCompany(), model.cust_id, sku_id_, model.start_date,model.cust_ids);

        }

        [HttpPost]
        public async Task<string> LoadData_Master(int CompanyId)
        {
            int comp_id = CompanyId == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : CompanyId;
            return await _service.LoadData_Master(GetDbConnectionString(), comp_id);
        }

    }
}
