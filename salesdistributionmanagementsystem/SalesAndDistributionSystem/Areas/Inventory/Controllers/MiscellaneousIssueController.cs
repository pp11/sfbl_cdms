using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    public class MiscellaneousIssueController : Controller
    {
        private readonly IMiscellaneousIssueManager _service;
        private readonly ILogger<MiscellaneousIssueController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();
        private readonly ICommonServices _comservice;
        public MiscellaneousIssueController(ICommonServices comservice, IMiscellaneousIssueManager service, ILogger<MiscellaneousIssueController> logger, IConfiguration configuration)
        {
            _comservice = comservice;
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }
        public IActionResult MiscellaneousIssue(string Id = "0")
        {
            string CurentDate = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.CurentDate = CurentDate;
            Miscellaneous_Issue_Mst model = new Miscellaneous_Issue_Mst();
            model.MST_ID_ENCRYPTED = Id;

            return View(model);
        }
        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");
        public string GetCompanyId() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString();
        public string GetCompanyName() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyName).Value.ToString();
        public string GetUnitId() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value.ToString();
        public string GetUnitName() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitName).Value.ToString();
        //public string GetUnitName() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitName).Value.ToString();
        [HttpPost]
        public async Task<string> LoadProductData([FromBody] Miscellaneous_Issue_Mst mst)
        {

            int comp_id = mst == null || mst.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : mst.COMPANY_ID;
            return await _service.LoadProductData(GetDbConnectionString(), comp_id,Convert.ToInt32(GetUnitId()));

        }
        //[HttpPost]
        //public async Task<string> LoadSKUBatchData([FromBody] Miscellaneous_Issue_Mst mst)
        //{
        //    if(mst == null)
        //    {
        //        return null;    
        //    }
        //    int comp_id = mst == null || mst.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : mst.COMPANY_ID;
        //    int unit_id = mst == null || mst.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : mst.UNIT_ID;
           
        //    return await _service.LoadSKUBatchData(GetDbConnectionString(), comp_id, unit_id, mst.SKU_ID);

        //}
        [HttpPost]
        public async Task<JsonResult> AddOrUpdate([FromBody] Miscellaneous_Issue_Mst model)
        {
            string result = "";

            if (!ModelState.IsValid)
            {
                var arr = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                return new JsonResult(arr);
            }
            else
            {
                try
                {
                    if (model.MST_ID == 0)
                    {

                        model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.UNIT_ID = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value);
                        Random rnd = new Random();
                        int num = rnd.Next();
                        model.ISSUE_NO = num.ToString();
                        model.ISSUE_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        //model.REQUISITION_RAISE_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        model.COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
                        if (model.MiscellaneousIssueDtlList != null && model.MiscellaneousIssueDtlList.Count > 0)
                        {
                            foreach (var item in model.MiscellaneousIssueDtlList)
                            {
                                item.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                                item.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy");
                                item.ISSUE_DATE= DateTime.Now.ToString("dd/MM/yyyy");
                                model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                                item.STATUS = "Active";
                                item.COMPANY_ID= model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
                                item.UNIT_ID= Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value);
                            }
                        }
                    }
                    else
                    {
                        model.UPDATED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        //model.REQUISITION_UNIT_ID = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value);
                        model.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        //model.REQUISITION_DATE = model.REQUISITION_DATE;

                        foreach (var item in model.MiscellaneousIssueDtlList)
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
        public async Task<string> GetEditDataById([FromBody] Miscellaneous_Issue_Mst mst)
        {
            Miscellaneous_Issue_Mst issue_Mst = new Miscellaneous_Issue_Mst();
            if (mst.q != null && mst.q != "0" && mst.q != "")
            {
                int _Id = Convert.ToInt32(_comservice.Decrypt(mst.q));
                issue_Mst = await _service.LoadDetailData_ByMasterId_List(GetDbConnectionString(), _Id);

            }
            return JsonSerializer.Serialize(issue_Mst);
        }
        public IActionResult MiscellaneousIssueList()
        {
            _logger.LogInformation(" Area Territory Relation(AreaTerritoryRelation/List) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View();
        }

        [HttpPost]
        public async Task<string> LoadData([FromBody] Domain.Models.ReportModels.Common.ProductReports.ReportParams model)
        {
            int comp_id = model.COMPANY_ID == null || model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
            return await _service.LoadData(GetDbConnectionString(), model.COMPANY_ID, model.DATE_FROM, model.DATE_TO);
        }
    }
}
