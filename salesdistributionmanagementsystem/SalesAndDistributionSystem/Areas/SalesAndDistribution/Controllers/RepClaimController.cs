using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem;
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
    public class RepClaimController : Controller
    {
        private readonly ISalesOrderManager _service;
        private readonly ICommonServices _comservice;

        private readonly ILogger<SalesOrderController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();

        public RepClaimController(ICommonServices comservice, ISalesOrderManager service, ILogger<SalesOrderController> logger, IConfiguration configuration)
        {
            _comservice = comservice;
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }
        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");
        private string GetDbSecurityConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "Security");

        public string GetCompany() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString();

        public string GetUnit() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value.ToString();


        [AuthorizeCheck]
        public IActionResult InsertOrEdit(string Id = "0")
        {

            Order_Mst _Mst = new Order_Mst();

            if (Id != "0" || Id != "")
            {
                _Mst.ORDER_MST_ID_ENCRYPTED = Id;
            }
            _logger.LogInformation("Region Area Relation  InsertOrEdit (RegionAreaRelation/InsertOrEdit) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");
            return View(_Mst);
        }

        [AuthorizeCheck]
        public IActionResult OrderDetails(string Id = "0")
        {

            Order_Mst _Mst = new Order_Mst();

            if (Id != "0" || Id != "")
            {
                _Mst.ORDER_MST_ID_ENCRYPTED = Id;
            }
            _logger.LogInformation("Region Area Relation  InsertOrEdit (RegionAreaRelation/InsertOrEdit) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");
            return View(_Mst);
        }


        [AuthorizeCheck]
        public IActionResult List()
        {
            _logger.LogInformation("Region Area Relation List(RegionAreaRelation/List) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View();
        }
        [HttpPost]
        public async Task<string> LoadCustomerData([FromBody] Order_Mst region_Info)
        {

            int comp_id = region_Info == null || region_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : region_Info.COMPANY_ID;
            return await _service.LoadCustomer(GetDbConnectionString(), region_Info);

        }
        [HttpPost]
        public async Task<string> LoadProductsData([FromBody] Order_Mst region_Info)
        {

            int comp_id = region_Info == null || region_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : region_Info.COMPANY_ID;
            return await _service.LoadProducts(GetDbConnectionString(), comp_id.ToString(),GetUnit());

        }
       
        [HttpPost]
        public async Task<string> LoadData([FromBody] Order_Mst division_Info)
        {
            division_Info.COMPANY_ID = division_Info == null || division_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : division_Info.COMPANY_ID;
            division_Info.UNIT_ID = division_Info == null || division_Info.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : division_Info.UNIT_ID;
            division_Info.db_security = GetDbSecurityConnectionString();
            division_Info.db_sales = GetDbConnectionString();
            division_Info.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
            division_Info.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
            division_Info.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
            return await _service.LoadData(division_Info);

        }


        [HttpPost]
        public async Task<string> LoadFilteredData([FromBody] OrderSKUFilterParameters model)
        {
            model.COMPANY_ID = model == null || model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
            model.UNIT_ID = model == null || model.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : model.UNIT_ID;
            model.db_security = GetDbSecurityConnectionString();
            model.db_sales = GetDbConnectionString();
            model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
            model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
            model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
            return await _service.LoadFilteredData(model);

        }



        

        [HttpPost]
        public async Task<decimal> LoadProductPrice([FromBody] Order_Mst region_Info)
        {

            int comp_id = region_Info == null || region_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : region_Info.COMPANY_ID;
            return await _service.LoadProductPrice(GetDbConnectionString(), comp_id.ToString(), region_Info.SKU_ID, region_Info.ORDER_TYPE, region_Info.CUSTOMER_ID );

        }
        [HttpPost]
        public async Task<string> Get_Customer_Balance([FromBody] Order_Mst region_Info) => await _service.Get_Customer_Balance(GetDbConnectionString(), GetCompany(),GetUnit(), region_Info.CUSTOMER_ID.ToString());
        
        [HttpPost]
        public async Task<string> GetEditDataById([FromBody] Order_Mst mst)
        {
            Order_Mst order_Mst = new Order_Mst();
            if (mst.q != null && mst.q != "0" && mst.q != "")
            {
                int _Id = Convert.ToInt32(_comservice.Decrypt(mst.q));
                return await _service.GetEditDatabyId(GetDbConnectionString(), _Id,"");

            }

            return null;
        }


        [HttpPost]
        public async Task<JsonResult> AddOrUpdate([FromBody] Order_Mst model)
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
                    model.UPDATED_BY = model.ENTERED_BY;
                    model.UPDATED_DATE = model.ENTERED_DATE;
                    model.UPDATED_TERMINAL = model.ENTERED_TERMINAL;
                    model.UNIT_ID = model.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : model.UNIT_ID;
                    model.db_security = GetDbSecurityConnectionString();
                    model.db_sales = GetDbConnectionString();
                    model.ORDER_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                    model.ORDER_UNIT_ID = Convert.ToInt32(GetUnit());

                   _result.Key  = await _service.AddOrUpdate(GetDbConnectionString(), model);
                   _result.Status = "1";
                }
                catch (Exception ex)
                {
                    _result.Status = ex.Message;
                }

            }


            return Json(_result);


        }
    }
}
