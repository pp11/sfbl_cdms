using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.ViewModels.Inventory;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SalesAndDistributionSystem.Areas.SalesAndDistribution.Controllers
{
    [Area("SalesAndDistribution")]
    public class SkuCommissionController : Controller
    {
        private readonly ISkuCommissionManager _service;
        private readonly ILogger<CustomerCommissionController> _logger;
        private readonly ServiceProvider Provider = new ServiceProvider();
        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");

        public SkuCommissionController(ISkuCommissionManager service, ILogger<CustomerCommissionController> logger)
        {
            _service = service;
            _logger = logger;
        }

        public IActionResult SkuCommission()
        {
            return View();
        }

        [HttpGet]
        public async Task<string> LoadData()
        {
            return await _service.LoadData(GetDbConnectionString(), User.GetComapanyId());
        }
       
        [HttpGet]
        public async Task<string> GetDetails(int id)
        {
            return await _service.GetDetails(GetDbConnectionString(), id);
        }
        [HttpPost]
        public async Task<string> GetMarketWiseCustomers([FromBody] CommissionDoneParams model)
        {
            return await _service.GetMarketWiseCustomers(GetDbConnectionString(), model.Market_Code);
        }
        [HttpGet]
        public async Task<string> GetComissionDoneCustomers(string Sku_Code)
        {
            return await _service.GetComissionDoneCustomers(GetDbConnectionString(), Sku_Code);
        }
        [HttpPost]
        public async Task<string> GetCustomer([FromBody] CUSTOMER_WISE_SKU_COMM_ADD_MST model)
        {
            return await _service.GetCustomer(GetDbConnectionString(), User.GetComapanyId(), model.UNIT_ID, model.SKU_ID);
        }
        [HttpPost]
        public async Task<string> GetCustomerMarketwise([FromBody] CommissionDoneParams model)
        {
            return await _service.GetCustomerMarketwise(GetDbConnectionString(), User.GetComapanyId(), model.SKU_ID,model.Market_Code, model.CUSTOMER_TYPE_ID==null || model.CUSTOMER_TYPE_ID ==""? "C.CUSTOMER_TYPE_ID": model.CUSTOMER_TYPE_ID, model.CUSTOMER_STATUS == null || model.CUSTOMER_STATUS == "" ? "C.CUSTOMER_STATUS" : @"'"+model.CUSTOMER_STATUS+"'");
        }

        [HttpPost]
        public async Task<string> Add([FromBody]  CUSTOMER_WISE_SKU_COMM_ADD_MST model)
        {
            try
            {
                model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                model.ENTERED_BY = User.GetUserId();
                model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();

              return  await _service.Add(GetDbConnectionString(), model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<JsonResult> Update([FromBody]  CUSTOMER_WISE_SKU_COMM_ADD_DTL model)
        {
            if (!ModelState.IsValid)
            {
                var arr = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                return new JsonResult(arr);
            }
            try
            {
                model.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                model.UPDATED_BY = User.GetUserId();
                model.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();

                await _service.Update(GetDbConnectionString(), model);
                return Json("1");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public async Task<string> DeleteMst(int id)
        {
            try
            {
                await _service.DeleteMst(GetDbConnectionString(), id);
                return "1";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public async Task<string> DeleteDtl(int id)
        {
            try
            {
                await _service.DeleteDtl(GetDbConnectionString(), id);
                return "1";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public async Task<string> Process(int id)
        {
            try
            {
                await _service.Process(GetDbConnectionString(), id);
                return "1";
            }
            catch(Exception exp)
            {
                throw exp;
            }
        }
    }
}
