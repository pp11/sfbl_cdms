using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.Inventory.IManager;
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
    public class StockTransferRcvController : Controller
    {
        private readonly IStockTransferRcvManager _service;
        private readonly ILogger<StockTransferRcvController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();
        private readonly ICommonServices _comservice;
        public StockTransferRcvController(ICommonServices comservice, IStockTransferRcvManager service, ILogger<StockTransferRcvController> logger, IConfiguration configuration)
        {
            _comservice = comservice;
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }
        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");
        [AuthorizeCheck]
        public IActionResult StockTransferRcv(string Id = "0")
        {
            _logger.LogInformation("Stock Transfer Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ?
                User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");
            DEPOT_STOCK_TRANS_RCV_MST requisitionMst = new DEPOT_STOCK_TRANS_RCV_MST();
            requisitionMst.MST_ID_ENCRYPTED = Id;

            return View(requisitionMst);
        }

        [HttpPost]
        public async Task<string> AddOrUpdate([FromBody] DEPOT_STOCK_TRANS_RCV_MST model)
        {
            Result _result = new Result();
            if (model == null)
            {
                var arr = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
                _result.Status = "No Changes Found!";
            }
            else
            {
                try
                {
                    if (model.MST_ID == 0)
                    {

                        model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
  
                        model.TRANS_RCV_UNIT_ID = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value;

                        model.TRANS_RCV_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.TRANS_RCV_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        model.COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
                        if (model.stockTransferDtlRcvList != null && model.stockTransferDtlRcvList.Count > 0)
                        {
                            foreach (var item in model.stockTransferDtlRcvList)
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
                        model.TRANS_RCV_DATE = model.TRANS_RCV_DATE;
                        model.TRANS_RCV_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        foreach (var item in model.stockTransferDtlRcvList)
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

                    return  await _service.AddOrUpdate(GetDbConnectionString(), model);

                }
                catch (Exception ex)
                {
                    _result.Status = ex.Message;
                }

            }
            return JsonSerializer.Serialize(_result);
        }

        [HttpPost]
        public async Task<string> GetEditDataById([FromBody] DEPOT_STOCK_TRANS_RCV_MST mst)
        {
            DEPOT_STOCK_TRANS_RCV_MST stock_Mst = new DEPOT_STOCK_TRANS_RCV_MST();
            if (mst.q != null && mst.q != "0" && mst.q != "")
            {
                int _Id = Convert.ToInt32(_comservice.Decrypt(mst.q));
                stock_Mst = await _service.LoadDetailData_ByMasterId_List(GetDbConnectionString(), _Id);

            }
            stock_Mst.TRANSFER_UNIT_ID = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value;
            return JsonSerializer.Serialize(stock_Mst);
        }

        [HttpPost]
        public async Task<string> LoadData([FromBody] Domain.Models.ReportModels.Common.ProductReports.ReportParams model)
        {
            var unitId = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value;
            int comp_id = model.COMPANY_ID == null || model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
            return await _service.LoadData(GetDbConnectionString(), comp_id, unitId, model.DATE_FROM, model.DATE_TO);
        }
        [AuthorizeCheck]
        public IActionResult List()
        {
            _logger.LogInformation(" Area Territory Relation(AreaTerritoryRelation/List) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View();
        }
        [HttpPost]
        public async Task<string> LoadDispatchedTransferDtl([FromBody] DEPOT_STOCK_TRANSFER_MST mst)
        {
            return await _service.LoadDispatchedTransferDtl(GetDbConnectionString(), mst);

        }
        [HttpPost]
        public async Task<string> LoadDispatchedTransferData(int CompanyId)
        {
            var unitId = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value;
            int comp_id = CompanyId == 0 || CompanyId == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : CompanyId;
            return await _service.LoadDispatchedTransferData(GetDbConnectionString(), comp_id, unitId);
        }
    }
}
