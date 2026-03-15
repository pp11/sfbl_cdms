
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Areas.SalesAndDistribution.Controllers;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.Inventory.Manager.IManager;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    [Authorize]

    public class StockTransferController : Controller
    {
        private readonly IStockTransferManager _service;
        private readonly ILogger<StockTransferController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();
        private readonly ICommonServices _comservice;
        public StockTransferController(ICommonServices comservice, IStockTransferManager service, ILogger<StockTransferController> logger, IConfiguration configuration)
        {
            _comservice = comservice;
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }
        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");
        [AuthorizeCheck]
        public IActionResult StockTransfer(string Id = "0")
        {
            _logger.LogInformation("Stock Transfer Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ?
                User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");
            DEPOT_STOCK_TRANSFER_MST requisitionMst = new DEPOT_STOCK_TRANSFER_MST();
            requisitionMst.MST_ID_ENCRYPTED = Id;
          
            return View(requisitionMst);
        }
        [HttpPost]
        public async Task<string> GetEditDataById([FromBody] DEPOT_STOCK_TRANSFER_MST  mst)
          {
            DEPOT_STOCK_TRANSFER_MST stock_Mst = new DEPOT_STOCK_TRANSFER_MST();
            if (mst.q != null && mst.q !=     "0" && mst.q != "")
            {
                int _Id = Convert.ToInt32(_comservice.Decrypt(mst.q));
                stock_Mst = await _service.LoadDetailData_ByMasterId_List(GetDbConnectionString(), _Id);

            }
            stock_Mst.TRANSFER_UNIT_ID = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value;
            return JsonSerializer.Serialize(stock_Mst);
        }
       
        [HttpPost]
        public async Task<string> LoadProductData([FromBody] Bonus_Mst bonus_Mst)
        {

            int comp_id = bonus_Mst == null || bonus_Mst.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : bonus_Mst.COMPANY_ID;
            string unitId = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value;
            return await _service.LoadProductData(GetDbConnectionString(), comp_id, unitId);

        }
        [HttpPost]
        public async Task<JsonResult> AddOrUpdate([FromBody] DEPOT_STOCK_TRANSFER_MST model)
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
                    if (model.MST_ID == 0)
                    {

                        model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.TRANSFER_UNIT_ID = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value;
                        model.TRANSFER_TYPE = "P";
                        model.TRANSFER_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.TRANSFER_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        model.COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
                        if (model.stockTransferDtlList != null && model.stockTransferDtlList.Count > 0)
                        {
                            foreach (var item in model.stockTransferDtlList)
                            {
                                item.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                                item.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy");
                                model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                                item.STATUS = "Active";
                            }
                        }
                    }
                    else
                    {
                        model.UPDATED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.TRANSFER_UNIT_ID = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value;
                        model.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        model.TRANSFER_TYPE = "P";
                        foreach (var item in model.stockTransferDtlList)
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

        [HttpPost]
        public async Task<string> LoadReceivableTransferdData(int CompanyId)
        {
            var unitId = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value;
            int comp_id = CompanyId == null || CompanyId == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : CompanyId;
            return await _service.LoadReceivableTransfer(GetDbConnectionString(), comp_id, unitId);
        }

        public async Task<string> LoadRcvUnitStock(int rcvUnitId,int skuId)
        {
            return await _service.LoadRcvUnitStock(GetDbConnectionString(), rcvUnitId, skuId);
        }

        [HttpPost]
        public async Task<string> LoadData([FromBody] Domain.Models.ReportModels.Common.ProductReports.ReportParams model)
        {
            var unitId = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value;
            int comp_id = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
            return await _service.LoadData(GetDbConnectionString(), comp_id, unitId, model.DATE_FROM, model.DATE_TO);
        }
        [AuthorizeCheck]
        public IActionResult List()
        {
            _logger.LogInformation(" Area Territory Relation(AreaTerritoryRelation/List) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View();
        }
        

    }
}
