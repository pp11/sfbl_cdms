using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.Inventory.Manager.IManager;
using SalesAndDistributionSystem.Services.Common;

namespace SalesAndDistributionSystem.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    public class RequisitionReturnReceiveController : Controller
    {
        private readonly IRequisitionReturnReceiveManager _service;
        private readonly ILogger<RequisitionReturnReceiveController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();
        private readonly ICommonServices _comservice;
        public RequisitionReturnReceiveController(ICommonServices comservice, IRequisitionReturnReceiveManager service, ILogger<RequisitionReturnReceiveController> logger, IConfiguration configuration)
        {
            _comservice = comservice;
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }
        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");
        public IActionResult RequisitionRetRcv(string Id = "0")
        {
            _logger.LogInformation("Requisition Return Receive  Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ?
                User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");
            DEPOT_REQUISITION_RET_RCV_MST requisitionMst = new DEPOT_REQUISITION_RET_RCV_MST();
            requisitionMst.MST_ID_ENCRYPTED = Id;
            return View(requisitionMst);
        }

        [HttpPost]
        public async Task<JsonResult> AddOrUpdate([FromBody] DEPOT_REQUISITION_RET_RCV_MST model)
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
                        //model.REQUISITION_UNIT_ID = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value);

                        model.RET_RCV_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        //model.RETURN_RCV_UNIT_ID = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value;
                        model.RET_RCV_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.RET_RCV_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        model.COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
                        if (model != null && model.requisitionRetRcvDtlList.Count > 0)
                        {
                            foreach (var item in model.requisitionRetRcvDtlList)
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

                        model.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();


                        foreach (var item in model.requisitionRetRcvDtlList)
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
        public async Task<string> GetEditDataById([FromBody] DEPOT_REQUISITION_RET_RCV_MST mst)
        {
            DEPOT_REQUISITION_RET_RCV_MST requisition_Mst = new DEPOT_REQUISITION_RET_RCV_MST();
            if (mst.q != null && mst.q != "0" && mst.q != "")
            {
                int _Id = Convert.ToInt32(_comservice.Decrypt(mst.q));
                requisition_Mst = await _service.LoadDetailData_ByMasterId_List(GetDbConnectionString(), _Id);

            }
            return JsonSerializer.Serialize(requisition_Mst);
        }

        [HttpPost]
        public async Task<string> LoadData(int CompanyId)
        {
            var unitId = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value;
            int comp_id = CompanyId == null || CompanyId == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : CompanyId;
            return await _service.LoadData(GetDbConnectionString(), comp_id, unitId);
        }
        public IActionResult List()
        {
            _logger.LogInformation(" Area Territory Relation(AreaTerritoryRelation/List) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View();
        }


    }
}
