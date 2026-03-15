using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ServiceProvider = SalesAndDistributionSystem.Common.ServiceProvider;

namespace SalesAndDistributionSystem.Areas.SalesAndDistribution.Controllers
{
    [Area("SalesAndDistribution")]

    public class AreaTerritoryRelationController : Controller
    {
        private readonly IAreaTerritoryRelationManager _service;
        private readonly ICommonServices _comservice;

        private readonly ILogger<AreaTerritoryRelationController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();

        public AreaTerritoryRelationController(ICommonServices comservice, IAreaTerritoryRelationManager service, ILogger<AreaTerritoryRelationController> logger, IConfiguration configuration)
        {
            _comservice = comservice;
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }
        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");

        public string GetCompany() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString();



        [AuthorizeCheck]
        public IActionResult InsertOrEdit(string Id = "0")
        {
            Area_Territory_Mst area_Territory_Mst = new Area_Territory_Mst();

            if (Id != "0" || Id != "")
            {
                area_Territory_Mst.AREA_TERRITORY_MST_ID_ENCRYPTED = Id;
            }
            _logger.LogInformation("AreaTerritoryRelation  InsertOrEdit(AreaTerritoryRelation/InsertOrEdit) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View(area_Territory_Mst);
        }
        [HttpPost]
        public async Task<string> GetEditDataById([FromBody] Area_Territory_Mst mst)
        {
            Area_Territory_Mst area_Territory_Mst = new Area_Territory_Mst();
            if (mst.q != null && mst.q != "0" && mst.q != "")
            {
                int _Id = Convert.ToInt32(_comservice.Decrypt(mst.q));
                area_Territory_Mst = await _service.LoadDetailData_ByMasterId_List(GetDbConnectionString(), _Id);

            }

            return JsonSerializer.Serialize(area_Territory_Mst);
        }



        [AuthorizeCheck]
        public IActionResult List()
        {
            _logger.LogInformation(" Area Territory Relation(AreaTerritoryRelation/List) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View();
        }


        [HttpPost]
        public async Task<string> LoadData_Master([FromBody] Area_Territory_Mst area_Info)
        {

            int comp_id = area_Info == null || area_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : area_Info.COMPANY_ID;
            return await _service.LoadData_Master(GetDbConnectionString(), comp_id);

        }
        [HttpPost]
        public async Task<string> Existing_Area_Load([FromBody] Area_Territory_Mst area_Info)
        {

            int comp_id = area_Info == null || area_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : area_Info.COMPANY_ID;
            return await _service.Existing_Area_Load(GetDbConnectionString(), comp_id);

        }
        [HttpPost]
        public async Task<string> Existing_Territory_Load([FromBody] Area_Territory_Mst area_Info)
        {

            int comp_id = area_Info == null || area_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : area_Info.COMPANY_ID;
            return await _service.Existing_Territory_Load(GetDbConnectionString(), comp_id);

        }


        [HttpPost]
        public async Task<string> LoadData_Detail([FromBody] Area_Territory_Dtl area_Info) => await _service.LoadData_DetailByMasterId(GetDbConnectionString(), area_Info.AREA_TERRITORY_MST_ID);


        [HttpPost]
        public async Task<JsonResult> AddOrUpdate([FromBody] Area_Territory_Mst model)
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
                    if (model.AREA_TERRITORY_MST_ID == 0)
                    {
                        model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.EFFECT_START_DATE = DateTime.ParseExact(model.EFFECT_START_DATE, "dd/MM/yyyy", null).ToString("dd/MM/yyyy hh:mm:ss");
                        model.EFFECT_END_DATE = DateTime.ParseExact(model.EFFECT_END_DATE, "dd/MM/yyyy", null).ToString("dd/MM/yyyy hh:mm:ss");
                        foreach (var item in model.area_Territory_Dtls)
                        {
                            item.EFFECT_START_DATE = DateTime.ParseExact(item.EFFECT_START_DATE, "dd/MM/yyyy", null).ToString("dd/MM/yyyy hh:mm:ss");
                            item.EFFECT_END_DATE = DateTime.ParseExact(item.EFFECT_END_DATE, "dd/MM/yyyy", null).ToString("dd/MM/yyyy hh:mm:ss");
                            item.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                            item.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                            item.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                            item.COMPANY_ID = item.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : item.COMPANY_ID;
                        }
                        model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();

                        model.COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;

                    }
                    else
                    {
                        model.UPDATED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        model.EFFECT_START_DATE = DateTime.ParseExact(model.EFFECT_START_DATE, "dd/MM/yyyy", null).ToString("dd/MM/yyyy hh:mm:ss");
                        model.EFFECT_END_DATE = DateTime.ParseExact(model.EFFECT_END_DATE, "dd/MM/yyyy", null).ToString("dd/MM/yyyy hh:mm:ss");

                        foreach (var item in model.area_Territory_Dtls)
                        {
                            item.EFFECT_START_DATE = DateTime.ParseExact(item.EFFECT_START_DATE, "dd/MM/yyyy", null).ToString("dd/MM/yyyy hh:mm:ss");
                            item.EFFECT_END_DATE = DateTime.ParseExact(item.EFFECT_END_DATE, "dd/MM/yyyy", null).ToString("dd/MM/yyyy hh:mm:ss");
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
    }
}
