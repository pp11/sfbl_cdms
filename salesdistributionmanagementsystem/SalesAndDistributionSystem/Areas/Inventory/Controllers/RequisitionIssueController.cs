using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;


namespace SalesAndDistributionSystem.Areas.SalesAndDistribution.Controllers
{
    [Area("Inventory")]
    public class RequisitionIssueController : Controller
    {
        private readonly IRequisitionIssueManager _service;
        private readonly ILogger<RequisitionIssueController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();
        private readonly ICommonServices _comservice;
        public RequisitionIssueController(ICommonServices comservice, IRequisitionIssueManager service, ILogger<RequisitionIssueController> logger, IConfiguration configuration)
        {
            _comservice = comservice;
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }
        private string GetDbSecurityConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "Security");

        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");
        public IActionResult RequisitionIssue(string Id = "0")
        {
            _logger.LogInformation("Customer Price Info  Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ?
                User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");
            DEPOT_REQUISITION_ISSUE_MST requisitionMst = new DEPOT_REQUISITION_ISSUE_MST();
            requisitionMst.MST_ID_ENCRYPTED = Id;
            return View(requisitionMst);
        }

        [HttpPost]
        public async Task<JsonResult> AddOrUpdate([FromBody] DEPOT_REQUISITION_ISSUE_MST model)
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

                    Random rnd = new Random();
                    int num = rnd.Next();
                    model.COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;

                    if (model.MST_ID == 0)
                    {

                        model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        //model.REQUISITION_UNIT_ID = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value);
                    
                        model.REQUISITION_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.ISSUE_UNIT_ID = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value;
                        model.ISSUE_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.ISSUE_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        if (model.requisitionIssueDtlList != null && model.requisitionIssueDtlList.Count > 0)
                        {
                            foreach (var item in model.requisitionIssueDtlList)
                            {
                                item.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                                item.COMPANY_ID = Convert.ToInt32( User.Claims.FirstOrDefault(c => c.Type == ClaimsType.CompanyId)?.Value);
                              item.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
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
                        model.REQUISITION_DATE = model.REQUISITION_DATE;

                        foreach (var item in model.requisitionIssueDtlList)
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
        public async Task<string> LoadProductData(int CompanyId,int UnitId)
        {

            int comp_id = CompanyId == null || CompanyId == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : CompanyId;
            int ISSUE_UNIT_ID = UnitId == null || UnitId == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value):UnitId;
            return await _service.LoadProductData(GetDbConnectionString(), comp_id, ISSUE_UNIT_ID);

        }
        [HttpPost]
        public async Task<string> GetEditDataById([FromBody] DEPOT_REQUISITION_ISSUE_MST mst)
        {
            DEPOT_REQUISITION_ISSUE_MST requisition_Mst = new DEPOT_REQUISITION_ISSUE_MST();
            if (mst.q != null && mst.q != "0" && mst.q != "")
            {
                int _Id = Convert.ToInt32(_comservice.Decrypt(mst.q));
                requisition_Mst = await _service.LoadDetailData_ByMasterId_List(GetDbConnectionString(), _Id);

            }
            return JsonSerializer.Serialize(requisition_Mst);
        }

        [HttpPost]
        public async Task<string> GetIssueBatchDetailDataById([FromBody] DEPOT_REQUISITION_ISSUE_MST mst)
        {
            List<DEPOT_REQUISITION_ISSUE_BATCH> issue_batch_list = new List<DEPOT_REQUISITION_ISSUE_BATCH>();
            if (mst.q != null && mst.q != "0" && mst.q != "")
            {
                int _Id = Convert.ToInt32(_comservice.Decrypt(mst.q));
                issue_batch_list = await _service.LoadBatchDetailData_ByMasterId_List(GetDbConnectionString(), _Id);

            }
            return JsonSerializer.Serialize(issue_batch_list);
        }
        public IActionResult List()
        {
            _logger.LogInformation(" Area Territory Relation(AreaTerritoryRelation/List) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View();
        }

        public IActionResult PendingList()
        {
            _logger.LogInformation(" Area Territory Relation(AreaTerritoryRelation/List) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View();
        }
        [HttpPost]
        public async Task<string> LoadData(int CompanyId)
        {
             
            int comp_id = CompanyId == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : CompanyId;

            return await _service.LoadData(GetDbConnectionString(), comp_id, User.GetUnitId());

        }
    }
}
