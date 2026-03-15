using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager;
using SalesAndDistributionSystem.Services.Business.Security;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Areas.SalesAndDistribution.Controllers
{
    [Area("SalesAndDistribution")]
    [Authorize]

    public class BonusController : Controller
    {
        private readonly IBonusManager _service;
        private readonly ILogger<AreaController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ICommonServices _common;

        private readonly ServiceProvider Provider = new ServiceProvider();

        public BonusController(IBonusManager service, ILogger<AreaController> logger, IConfiguration configuration, ICommonServices common)
        {
            _service = service;
            _logger = logger;
            _configuration = configuration;
            _common = common;
        }
        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");
       
        public string GetCompany() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString();
        public IActionResult List()
        {
            _logger.LogInformation("BonusConfig List(Bonus/List) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View();
        }
        public IActionResult BonusConfig(string Id = "0")
         {
            Bonus_Mst bonus_Mst = new Bonus_Mst();

            if (Id != "0" || Id != "")
            {
                bonus_Mst.BONUS_MST_ID_ENCRYPTED = Id;
            }
            _logger.LogInformation("BonusConfig (Bonus/BonusConfig) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View(bonus_Mst);
        }



        [HttpPost]
        public async Task<string> LoadData([FromBody] Bonus_Mst bonus_Mst)
        {

            int comp_id = bonus_Mst == null || bonus_Mst.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : bonus_Mst.COMPANY_ID;
            return await _service.LoadData(GetDbConnectionString(), comp_id);

        }

        [HttpPost]
        public async Task<string> LoadBaseProductData([FromBody] Bonus_Mst bonus_Mst)
        {

            int comp_id = bonus_Mst == null || bonus_Mst.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : bonus_Mst.COMPANY_ID;
            return await _service.LoadBaseProductData(GetDbConnectionString(), comp_id);

        }
        [HttpPost]
        public async Task<string> LoadProductData([FromBody] Bonus_Mst bonus_Mst)
        {

            int comp_id = bonus_Mst == null || bonus_Mst.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : bonus_Mst.COMPANY_ID;
            return await _service.LoadProductData(GetDbConnectionString(), comp_id);

        }
        [HttpPost]
        public async Task<string> LoadGroupData([FromBody] Bonus_Mst bonus_Mst)
        {

            int comp_id = bonus_Mst == null || bonus_Mst.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : bonus_Mst.COMPANY_ID;
            return await _service.LoadGroupData(GetDbConnectionString(), comp_id);

        }
        [HttpPost]
        public async Task<string> LoadBrandData([FromBody] Bonus_Mst bonus_Mst)
        {

            int comp_id = bonus_Mst == null || bonus_Mst.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : bonus_Mst.COMPANY_ID;
            return await _service.LoadBrandData(GetDbConnectionString(), comp_id);

        }

        [HttpPost]
        public async Task<string> LoadCategoryData([FromBody] Bonus_Mst bonus_Mst)
        {

            int comp_id = bonus_Mst == null || bonus_Mst.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : bonus_Mst.COMPANY_ID;
            return await _service.LoadCategoryData(GetDbConnectionString(), comp_id);

        }
        [HttpPost]
        public int LoadNewBonusNo([FromBody] Bonus_Mst bonus_Mst) => _service.LoadNewBonusNo(GetDbConnectionString());
       
        [HttpPost]
        public string LoadLocationTypes([FromBody] Bonus_Mst bonus_Mst)
        {

            int comp_id = bonus_Mst == null || bonus_Mst.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : bonus_Mst.COMPANY_ID;
            return  _service.LoadLocationTypes(GetDbConnectionString(), comp_id);

        }

        [HttpPost]
        public async Task<string> LoadLocation_ByLocationType([FromBody] Bonus_Mst bonus_Mst)
        {
            int comp_id = bonus_Mst == null || bonus_Mst.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : bonus_Mst.COMPANY_ID;
            return await _service.GetLocation_ByLocationType(GetDbConnectionString(), comp_id, bonus_Mst.LOCATION_TYPE );

        }
        [HttpPost]
        public async Task<string> GetProductDataFiltered([FromBody] ProductFilterParameters paramsList)
        {
            int comp_id = paramsList == null || paramsList.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : paramsList.COMPANY_ID;
            return await _service.GetProductDataFiltered(GetDbConnectionString(), comp_id, paramsList);

        }
        [HttpPost]
        public async Task<string> GetEditDataById([FromBody] Bonus_Mst mst)
        {
            Bonus_Mst bonus_Mst = new Bonus_Mst();
            if (mst.q != null && mst.q != "0" && mst.q != "")
            {
                int _Id = Convert.ToInt32(_common.Decrypt(mst.q));
                bonus_Mst = await _service.LoadEditDataByMasterId(GetDbConnectionString(), _Id);

            }

            return JsonSerializer.Serialize(bonus_Mst);
        }

        
        [HttpPost]
        public async Task<JsonResult> AddOrUpdate([FromBody] Bonus_Mst model)
        {
            int comp_id = model == null || model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;


            string result = "";

            if (model == null)
            {
                result = "No Changes Found!";
            }
            else
            {
                try
                {
                    if (model.BONUS_MST_ID == 0)
                    {

                        model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.ENTRY_DATE = model.ENTERED_DATE;
                        model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();

                        model.EFFECT_START_DATE = DateTime.ParseExact(model.EFFECT_START_DATE, "dd/MM/yyyy", null).ToString("dd/MM/yyyy hh:mm:ss");
                        model.EFFECT_END_DATE = DateTime.ParseExact(model.EFFECT_END_DATE, "dd/MM/yyyy", null).ToString("dd/MM/yyyy hh:mm:ss");
                       

                        model.COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
                        model.UNIT_ID =  Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value);

                    }
                    else
                    {
                        model.UPDATED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        model.EFFECT_START_DATE = DateTime.ParseExact(model.EFFECT_START_DATE, "dd/MM/yyyy", null).ToString("dd/MM/yyyy hh:mm:ss");
                        model.EFFECT_END_DATE = DateTime.ParseExact(model.EFFECT_END_DATE, "dd/MM/yyyy", null).ToString("dd/MM/yyyy hh:mm:ss");

                        model.UNIT_ID = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value);


                    }

                    result = await _service.AddOrUpdate(GetDbConnectionString(), comp_id, model);

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
