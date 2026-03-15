using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
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
    public class DistributionController : Controller
    {

        private readonly IDistributionManager _service;
        private readonly ILogger<DistributionController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();
        private readonly ICommonServices _comservice;
        public DistributionController(ICommonServices comservice, IDistributionManager service, ILogger<DistributionController> logger, IConfiguration configuration)
        {
            _comservice = comservice;
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }
        private string GetDbSecurityConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "Security");

        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");
        [AuthorizeCheck]
        public IActionResult Distribution(string Id = "0")
        {
            _logger.LogInformation("Customer Price Info  Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ?
                User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");
            DEPOT_REQ_DISTRIBUTION_MST distributionnMst = new DEPOT_REQ_DISTRIBUTION_MST();
            distributionnMst.MST_ID_ENCRYPTED = Id;

            return View(distributionnMst);
        }


        [HttpPost]
        public async Task<string> GetEditDataById([FromBody] DEPOT_DISPATCH_MST mst)
        {
            DEPOT_DISPATCH_MST requisition_Mst = new DEPOT_DISPATCH_MST();
            if (mst.q != null && mst.q != "0" && mst.q != "")
            {
                int _Id = Convert.ToInt32(_comservice.Decrypt(mst.q));
                requisition_Mst = await _service.LoadDistributionDetailData_ByMasterId(GetDbConnectionString(), _Id);

            }
            return JsonSerializer.Serialize(requisition_Mst);
        }

        [HttpPost]
        public async Task<JsonResult> AddOrUpdate([FromBody] DEPOT_DISPATCH_MST model)
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
                    model.UNIT_ID = model.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : model.UNIT_ID;
                    model.db_security = GetDbSecurityConnectionString();
                    model.db_sales = GetDbConnectionString();
                    model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                    model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                    model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                    model.COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;

                    if (model.MST_ID == 0)
                    {

                        //model.REQUISITION_UNIT_ID = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value);

                        model.DISPATCH_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");

                        model.DISPATCH_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.DISPATCH_UNIT_ID = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value);

                        if (model.requisitionIssueDtlList != null && model.requisitionIssueDtlList.Count > 0)
                        {
                            foreach (var item in model.requisitionIssueDtlList)
                            {
                                item.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                                item.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                                item.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                                item.DISPATCH_UNIT_ID = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value);

                                if (item.requisitionProductDtlList != null && item.requisitionProductDtlList.Count > 0)
                                {
                                    foreach (var product in item.requisitionProductDtlList)
                                    {
                                        if (product.PENDING_DISPATCH_QTY != 0)
                                        {
                                            product.DISPATCH_QTY = product.PENDING_DISPATCH_QTY;
                                        }

                                        product.DISPATCH_UNIT_ID = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value);
                                        product.ENTERED_BY = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value);
                                        product.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy");
                                        product.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();

                                    }
                                }
                            }

                        }


                    }
                    else
                    {
                        model.UPDATED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");

                        model.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        model.DISPATCH_DATE = model.DISPATCH_DATE;
                        model.DISPATCH_UNIT_ID = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value);
                        foreach (var item in model.requisitionIssueDtlList)
                        {


                            item.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                            item.UPDATED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                            item.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                            item.COMPANY_ID = item.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : item.COMPANY_ID;
                            item.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                            item.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                            item.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                            item.DISPATCH_UNIT_ID = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value);
                            if (item.requisitionProductDtlList != null && item.requisitionProductDtlList.Count > 0)
                            {
                                foreach (var product in item.requisitionProductDtlList)
                                {

                                    if (product.PENDING_DISPATCH_QTY != 0)
                                    {
                                        product.DISPATCH_QTY = product.PENDING_DISPATCH_QTY;
                                    }

                                    product.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                                    product.UPDATED_BY = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value);
                                    product.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                                    product.COMPANY_ID = item.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : item.COMPANY_ID;
                                    product.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                                    product.ENTERED_BY = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value);
                                    product.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                                    product.DISPATCH_UNIT_ID = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value);
                                }

                            }


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
        public async Task<string> LoadData(int CompanyId)
        {
            var unitId = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value;
            int comp_id = CompanyId == null || CompanyId == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : CompanyId;
            return await _service.LoadData(GetDbConnectionString(), comp_id);
        }
        [HttpPost]
        public async Task<string> LoadDistributionReqData(int CompanyId)
        {
            var unitId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value);
            int comp_id = CompanyId == null || CompanyId == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : CompanyId;
            return await _service.LoadDistributionReqData(GetDbConnectionString(), comp_id, unitId);
        }

        [HttpPost]
        public async Task<string> LoadDistributionProductDataByReqId(int CompanyId, int ReqId)
        {
            var unitId = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value;
            int comp_id = CompanyId == null || CompanyId == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : CompanyId;
            return await _service.LoadDistributionProductDataByReqId(GetDbConnectionString(), comp_id, ReqId);
        }
        [HttpGet]
        public async Task<string> LoadShipperDtlData(int CompanyId, int MstId)
        {
            var unitId = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value;
            int comp_id = CompanyId == null || CompanyId == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : CompanyId;
            return await _service.LoadShipperDtlData(GetDbConnectionString(), comp_id, MstId);
        }
        [HttpGet]
        public async Task<string> LoadDispatchBatchData(int CompanyId, int MstId)
        {
            var unitId = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value;
            int comp_id = CompanyId == null || CompanyId == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : CompanyId;
            return await _service.LoadDispatchBatchData(GetDbConnectionString(), comp_id, MstId);
        }
        [HttpGet]
        public async Task<string> GetPendingRequisition()
        {
            return await _service.GetPendingRequisition(GetDbConnectionString(), User.GetComapanyId(), User.GetUnitId());
        }
        [HttpGet]
        public async Task<string> GetPendingStock()
        {
            return await _service.GetPendingStock(GetDbConnectionString(), User.GetComapanyId(), User.GetUnitId());
        }
        [HttpGet]
        public async Task<string> GetProductsByRequisition(string id)
        {
            return await _service.GetProductsByRequisition(GetDbConnectionString(), User.GetComapanyId(), User.GetUnitId(), id);
        }

        [HttpGet]
        public async Task<string> GetProductsByStock(string id)
        {
            return await _service.GetProductsByStock(GetDbConnectionString(), User.GetComapanyId(), User.GetUnitId(), id);
        }

        [HttpGet]
        public async Task<string> GetTransferedProduct(string id, int unitid)
        {
            return await _service.GetProductsByStock(GetDbConnectionString(), User.GetComapanyId(), unitid, id);
        }
        [AuthorizeCheck]
        public IActionResult PendingList()
        {
            _logger.LogInformation(" Area Territory Relation(AreaTerritoryRelation/List) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View();
        }
        [AuthorizeCheck]
        public IActionResult List()
        {
            _logger.LogInformation(" Distribution (AreaTerritoryRelation/List) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View();
        }

    }
}
