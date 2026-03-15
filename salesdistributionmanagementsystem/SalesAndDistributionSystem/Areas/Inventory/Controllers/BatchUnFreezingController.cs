using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.Inventory.IManager;
using SalesAndDistributionSystem.Services.Business.Inventory.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SalesAndDistributionSystem.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    [Authorize]
    public class BatchUnFreezingController : Controller
    {
        [AuthorizeCheck]
        public IActionResult Index()
        {
            return View();
        }

        private readonly IBatchUnFreezingManager _service;
        private readonly ILogger<BatchUnFreezingController> _logger;

        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();

        public BatchUnFreezingController(IBatchUnFreezingManager service, ILogger<BatchUnFreezingController> logger, IConfiguration configuration)
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


        [HttpPost]
        public async Task<string> GetBatchList([FromBody] BatchFreezingMst batchFreezingMst)
        {

            int company_id = batchFreezingMst.COMPANY_ID;
            int unit_id = batchFreezingMst.UNIT_ID;
            int sku_id = batchFreezingMst.SKU_ID;
            return await _service.GetBatchList(GetDbConnectionString(), company_id, unit_id, sku_id);

        }


        [HttpPost]
        public async Task<string> GetSkuList([FromBody] BatchUnFreezingMst batchFreezingMst)
        {
            int company_id = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimsType.CompanyId)?.Value);
            int unit_id = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value);
            return await _service.GetSkuList(GetDbConnectionString(), company_id, unit_id);

        }

        [HttpPost]
        public async Task<JsonResult> InsertOrUpdate([FromBody] BatchUnFreezingMst model)
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
                    if (model.MST_ID == 0)
                    {
                        model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                    }
                    else
                    {
                        model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        model.UPDATED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                    }

                    result = await _service.InsertOrUpdate(GetDbConnectionString(), model);

                }
                catch (Exception ex)
                {

                    if (ex.Message.Contains("ORA-00001"))
                    {
                        result = "unique constraint violated! This Item already exists in database";
                    }

                    result = ex.Message;

                }

            }


            return Json(result);


        }



        [HttpPost]
        public async Task<string> GetDtlData([FromBody] BatchUnFreezingMst model)
        {
            int mst_id = model.MST_ID;
            return await _service.GetDtlData(GetDbConnectionString(), mst_id);
        }


        [HttpPost]
        public async Task<string> GetMstData([FromBody] BatchUnFreezingMst model)
        {
            int company_id = model.COMPANY_ID;
            int unit_id = model.UNIT_ID;

            return await _service.GetMstData(GetDbConnectionString(), company_id, unit_id);
        }
        [AuthorizeCheck]
        public IActionResult BatchUnFreezing()
        {
            return View();
        }
    }
}
