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

    public class DistributorProductRelationController : Controller
    {
        private readonly IDistributorProductRelation _service;
        private readonly ICommonServices _comservice;

        private readonly ILogger<DistributorProductRelationController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();

        public DistributorProductRelationController(ICommonServices comservice, IDistributorProductRelation service, ILogger<DistributorProductRelationController> logger, IConfiguration configuration)
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
            Distributor_Product_Mst distributor_Product  = new Distributor_Product_Mst();
            if (Id != "0" || Id != "")
            {
                distributor_Product.MST_ID_ENCRYPTED = Id;
            }
            _logger.LogInformation("Distributor_Product InsertOrEdit(DivisionRegionRelation/InsertOrEdit) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View(distributor_Product);
        }
        [HttpPost]
        public async Task<string> GetEditDataById([FromBody] Distributor_Product_Mst mst)
        {
            Distributor_Product_Mst division_Region_Mst = new Distributor_Product_Mst();
            if (mst.q != null && mst.q != "0" && mst.q != "")
            {
                int _Id = Convert.ToInt32(_comservice.Decrypt(mst.q));
                division_Region_Mst = await _service.LoadDetailData_ByMasterId_List(GetDbConnectionString(), _Id);

            }

            return JsonSerializer.Serialize(division_Region_Mst);
        }
        [AuthorizeCheck]
        public IActionResult List()
        {
            _logger.LogInformation("DivisionInfo List(DivisionRegionRelation/List) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View();
        }
        [HttpPost]
        public async Task<string> LoadData_Master([FromBody] Distributor_Product_Mst division_Info)
        {

            int comp_id = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value);
            return await _service.LoadData_Master(GetDbConnectionString(), comp_id);

        }
        [HttpPost]
        public async Task<string> LoadData_Detail([FromBody] Distributor_Product_Mst division_Info) => await _service.LoadData_DetailByMasterId(GetDbConnectionString(), division_Info.MST_ID);

        [HttpPost]
        public async Task<JsonResult> AddOrUpdate([FromBody] Distributor_Product_Mst model)
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
