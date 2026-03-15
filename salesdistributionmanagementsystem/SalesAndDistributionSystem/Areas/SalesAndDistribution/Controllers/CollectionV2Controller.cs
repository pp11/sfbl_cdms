using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Common;
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
    [Authorize]
    public class CollectionV2Controller : Controller
    {
        private readonly ICollectionManager _service;

        private readonly ICommonServices _comservice;

        private readonly ILogger<CollectionController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();

        public CollectionV2Controller(ICommonServices comservice, ICollectionManager service, ILogger<CollectionController> logger, IConfiguration configuration)
        {
            _comservice = comservice;
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }
        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");

        public string GetCompany() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString();
        public string GetCompanyName() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString();

        public string GetUnit() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value.ToString();


        [AuthorizeCheck]
        public IActionResult List()
        {
            _logger.LogInformation("Collection List(Collection/List) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View();
        }

        [AuthorizeCheck]
        public IActionResult InsertOrEdit(string Id = "0")
        {

            Collection_Mst _Mst = new Collection_Mst();

            if (Id != "0" || Id != "")
            {
                _Mst.COLLECTION_MST_ID_ENCRYPTED = Id;
            }
            _logger.LogInformation("Collection  InsertOrEdit (Collection/InsertOrEdit) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");
            return View(_Mst);
        }




        [HttpPost]
        public async Task<string> LoadData()
        {

            string comp_id = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value;

            return await _service.LoadData(GetDbConnectionString(), comp_id, GetUnit());

        }

        [HttpPost]
        public async Task<string> LoadBranchData() => await _service.LoadBranch(GetDbConnectionString());
        [HttpPost]
        public async Task<string> LoadCollectionMode() => await _service.LoadCollectionMode(GetDbConnectionString());
        [HttpPost]
        public async Task<string> LoadCustomerDaywiseBalance([FromBody] Collection_Dtl collection_Dtl) => await _service.LoadCustomerDaywiseBalanceV2(GetDbConnectionString(), collection_Dtl.CUSTOMER_ID.ToString(),GetUnit());


        [HttpPost]
        public async Task<string> GetEditDataById([FromBody] Collection_Mst mst)
        {
            var unit_id = mst.UNIT_ID == 0 ? User.GetUnitId() : mst.UNIT_ID;
            if (mst.q != null && mst.q != "0" && mst.q != "")
            {
                int _Id = Convert.ToInt32(_comservice.Decrypt(mst.q));
                string data = await _service.GetEditDataById(GetDbConnectionString(), _Id, unit_id);
                return data;
            }

            return null;
        }


        [HttpPost]
        public async Task<string> AddOrUpdate([FromBody] Collection_Mst model)
        {
            Result _result = new Result();

            if (model == null)
            {
                _result.Status = "No Data Found! Contact with software team";
            }
            else
            {
                try
                {
                    if (model.COLLECTION_MST_ID == 0)
                    {
                        model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        model.COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
                        //double check
                        model.COMPANY_ID = model.COMPANY_ID == 0 ? 1 : model.COMPANY_ID;
                        model.UNIT_ID = model.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : model.UNIT_ID;

                    }
                    else
                    {
                        model.UPDATED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                    }
                    return await _service.AddOrUpdate(GetDbConnectionString(), model);
                }
                catch (Exception ex)
                {
                    _result.Status = ex.Message;
                }

            }


            return JsonSerializer.Serialize(_result);


        }



        [HttpPost]
        public async Task<string> Update_Approval([FromBody] Collection_Mst model)
        {
            Result _result = new Result();

            if (model == null)
            {
                _result.Status = "No Changes Found!";
            }
            else
            {
                try
                {
                    model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                    model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                    model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                    model.COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
                    return await _service.Update_Approval(GetDbConnectionString(), model);
                }
                catch (Exception ex)
                {
                    _result.Status = ex.Message;
                }

            }


            return JsonSerializer.Serialize(_result);


        }
    }
}
