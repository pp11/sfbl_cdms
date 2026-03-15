using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Authorization;
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
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using ServiceProvider = SalesAndDistributionSystem.Common.ServiceProvider;

namespace SalesAndDistributionSystem.Areas.SalesAndDistribution.Controllers
{
    [Area("SalesAndDistribution")]
    [Authorize]
    public class SalesOrderController : Controller
    {
        private readonly ISalesOrderManager _service;
        private readonly ICommonServices _comservice;

        private readonly ILogger<SalesOrderController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();

        public SalesOrderController(ICommonServices comservice, ISalesOrderManager service, ILogger<SalesOrderController> logger, IConfiguration configuration)
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
            ViewBag.UserType = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString();
            ViewBag.UserId= User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString();
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
            if (User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString() == "Distributor")
            {
                region_Info.IsDistributor = "True";
                region_Info.IsOSM = "False";

                region_Info.CUSTOMER_CODE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.DistributorId).Value.ToString();
            }
            else if (User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString() == "OSM")
            {
                region_Info.IsDistributor = "False";
                region_Info.IsOSM = "True";
                region_Info.CUSTOMER_CODE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.DistributorId).Value.ToString();
            }
            else
            {
                region_Info.USER_TYPE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString();
                region_Info.IsDistributor = "False";
                region_Info.IsOSM = "False";

            }
            region_Info.USER_TYPE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString();

            region_Info.COMPANY_ID = region_Info == null || region_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : region_Info.COMPANY_ID;
            region_Info.UNIT_ID = region_Info == null || region_Info.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : region_Info.UNIT_ID;
            region_Info.db_security = GetDbSecurityConnectionString();
            region_Info.db_sales = GetDbConnectionString();
            region_Info.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
            region_Info.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
            region_Info.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();

            int comp_id = region_Info == null || region_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : region_Info.COMPANY_ID;
            return await _service.LoadCustomer(GetDbConnectionString(), region_Info);

        }

        [HttpPost]
        public async Task<string> GetProductDataFiltered([FromBody] ProductFilterParameters paramsList)
        {
            int comp_id = paramsList == null || paramsList.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : paramsList.COMPANY_ID;
            string unit_id = paramsList == null || Convert.ToInt32(paramsList.UNIT_ID) == 0 ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value : paramsList.UNIT_ID;

            return await _service.GetProductDataFiltered(GetDbConnectionString(), comp_id, paramsList,Convert.ToInt32(unit_id));

        }

        [HttpPost]
        public async Task<string> LoadProductsData([FromBody] Order_Mst region_Info)
        {

            int comp_id = region_Info == null || region_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : region_Info.COMPANY_ID;
            string unit_id = region_Info == null || Convert.ToInt32(region_Info.UNIT_ID) == 0 ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value : region_Info.UNIT_ID.ToString();

            return await _service.LoadProducts(GetDbConnectionString(), comp_id.ToString(), region_Info.CUSTOMER_ID.ToString());

        }

        [HttpPost]
        public async Task<string> LoadProductsSpecific([FromBody] Order_Mst region_Info)
        {

            int comp_id = region_Info == null || region_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : region_Info.COMPANY_ID;
            string unit_id = region_Info == null || Convert.ToInt32(region_Info.UNIT_ID) == 0 ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value : region_Info.UNIT_ID.ToString();

            return await _service.LoadProductsSpecific(GetDbConnectionString(), comp_id.ToString(), region_Info.CUSTOMER_ID.ToString());

        }



        [HttpPost]
        public async Task<string> LoadData([FromBody] Order_Mst division_Info)
        {
            if(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString() == "Distributor")
            {
                division_Info.IsDistributor = "True";
                division_Info.IsOSM = "False";

                division_Info.CUSTOMER_CODE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.DistributorId).Value.ToString();
            }
            else if(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString() == "OSM")
            {
                division_Info.IsDistributor = "False";
                division_Info.IsOSM = "True";
                division_Info.CUSTOMER_CODE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.DistributorId).Value.ToString();
            }
            else
            {
                division_Info.USER_TYPE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString();
                division_Info.IsDistributor = "False";
                division_Info.IsOSM = "False";

            }
            division_Info.USER_TYPE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString();

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
        public async Task<string> LoadMonitoringData([FromBody] Order_Mst division_Info)
        {
            if (User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString() == "Distributor")
            {
                division_Info.IsDistributor = "True";
                division_Info.IsOSM = "False";

                division_Info.CUSTOMER_CODE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.DistributorId).Value.ToString();
            }
            else if (User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString() == "OSM")
            {
                division_Info.IsDistributor = "False";
                division_Info.IsOSM = "True";
                division_Info.CUSTOMER_CODE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.DistributorId).Value.ToString();
            }
            else
            {
                division_Info.USER_TYPE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString();
                division_Info.IsDistributor = "False";
                division_Info.IsOSM = "False";

            }
            division_Info.USER_TYPE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString();

            division_Info.COMPANY_ID = division_Info == null || division_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : division_Info.COMPANY_ID;
            division_Info.UNIT_ID = division_Info == null || division_Info.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : division_Info.UNIT_ID;
            division_Info.db_security = GetDbSecurityConnectionString();
            division_Info.db_sales = GetDbConnectionString();
            division_Info.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
            division_Info.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
            division_Info.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
            return await _service.LoadMonitoringData(division_Info);

        }
        [HttpPost]
        public async Task<string> LoadDataProcessed([FromBody] Order_Mst division_Info)
        {
            if (User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString() == "Distributor")
            {
                division_Info.IsDistributor = "True";
                division_Info.IsOSM = "False";

                division_Info.CUSTOMER_CODE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.DistributorId).Value.ToString();
            }
            else if (User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString() == "OSM")
            {
                division_Info.IsDistributor = "False";
                division_Info.IsOSM = "True";
                division_Info.CUSTOMER_CODE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.DistributorId).Value.ToString();
            }
            else
            {
                division_Info.USER_TYPE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString();
                division_Info.IsDistributor = "False";
                division_Info.IsOSM = "False";

            }
            division_Info.USER_TYPE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString();

            division_Info.COMPANY_ID = division_Info == null || division_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : division_Info.COMPANY_ID;
            division_Info.UNIT_ID = division_Info == null || division_Info.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : division_Info.UNIT_ID;
            division_Info.db_security = GetDbSecurityConnectionString();
            division_Info.db_sales = GetDbConnectionString();
            division_Info.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
            division_Info.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
            division_Info.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
            return await _service.LoadDataProcessed(division_Info);

        }


        [HttpPost]
        public async Task<string> LoadFilteredData([FromBody] OrderSKUFilterParameters model)
        {
            if (User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString() == "Distributor")
            {
                model.IsDistributor = "True";
                model.IsOSM = "False";

                model.CUSTOMER_CODE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.DistributorId).Value.ToString();
            }
            else if (User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString() == "OSM")
            {
                model.IsDistributor = "False";
                model.IsOSM = "True";
                model.CUSTOMER_CODE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.DistributorId).Value.ToString();
            }
            else
            {
                model.USER_TYPE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString();
                model.IsDistributor = "False";
                model.IsOSM = "False";

            }
            model.USER_TYPE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString();
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
        public async Task<string> LoadFilteredMonitoringData([FromBody] OrderSKUFilterParameters model)
        {
            if (User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString() == "Distributor")
            {
                model.IsDistributor = "True";
                model.IsOSM = "False";

                model.CUSTOMER_CODE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.DistributorId).Value.ToString();
            }
            else if (User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString() == "OSM")
            {
                model.IsDistributor = "False";
                model.IsOSM = "True";
                model.CUSTOMER_CODE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.DistributorId).Value.ToString();
            }
            else
            {
                model.USER_TYPE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString();
                model.IsDistributor = "False";
                model.IsOSM = "False";

            }
            model.USER_TYPE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString();

            model.COMPANY_ID = model == null || model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
            model.UNIT_ID = model == null? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : model.UNIT_ID;
            model.db_security = GetDbSecurityConnectionString();
            model.db_sales = GetDbConnectionString();
            model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
            model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
            model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
            return await _service.LoadFilteredMonitoringData(model);

        }

        [HttpPost]
        public async Task<string> LoadFilteredProcessedData([FromBody] OrderSKUFilterParameters model)
        {
            if (User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString() == "Distributor")
            {
                model.IsDistributor = "True";
                model.IsOSM = "False";

                model.CUSTOMER_CODE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.DistributorId).Value.ToString();
            }
            else if (User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString() == "OSM")
            {
                model.IsDistributor = "False";
                model.IsOSM = "True";
                model.CUSTOMER_CODE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.DistributorId).Value.ToString();
            }
            else
            {
                model.USER_TYPE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString();
                model.IsDistributor = "False";
                model.IsOSM = "False";

            }
            model.USER_TYPE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString();

            model.COMPANY_ID = model == null || model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
            model.UNIT_ID = model == null || model.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : model.UNIT_ID;
            model.db_security = GetDbSecurityConnectionString();
            model.db_sales = GetDbConnectionString();
            model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
            model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
            model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
            return await _service.LoadFilteredProcessedData(model);

        }

        [HttpPost]
        public async Task<string> LoadProductsSpecificType([FromBody] Order_Mst region_Info)
        {

            int comp_id = region_Info == null || region_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : region_Info.COMPANY_ID;
            string unit_id = region_Info == null || Convert.ToInt32(region_Info.UNIT_ID) == 0 ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value : region_Info.UNIT_ID.ToString();

            return await _service.LoadProductsSpecificType(GetDbConnectionString(), comp_id.ToString(), region_Info.distributor_product_type );

        }
        [HttpPost]
        public string LoadProductType([FromBody] Order_Mst region_Info)
        {
            return  _service.LoadProductType(GetDbConnectionString(), region_Info.CUSTOMER_ID.ToString());
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
        public async Task<string> Replacement_Master([FromBody] Order_Mst region_Info) => await _service.Replacement_Master(GetDbConnectionString(), region_Info.CUSTOMER_ID.ToString(),GetUnit(), region_Info.ORDER_MST_ID.ToString());
        [HttpPost]
        public async Task<string> Replacement_Dtl([FromBody] Order_Mst region_Info) => await _service.Replacement_Dtl(GetDbConnectionString(), region_Info.REPLACE_CLAIM_NO, region_Info.ORDER_UNIT_ID);
        [HttpPost]
        public async Task<string> Refurbishment_Master([FromBody] Order_Mst region_Info) => await _service.Refurbishment_Master(GetDbConnectionString(), region_Info.CUSTOMER_ID.ToString(), GetUnit());
        [HttpPost]
        public async Task<string> Refurbishment_Dtl([FromBody] Order_Mst region_Info) => await _service.Refurbishment_Dtl(GetDbConnectionString(), region_Info.REPLACE_CLAIM_NO);


        [HttpPost]
        public async Task<string> GetEditDataById([FromBody] Order_Mst mst)
        {
            Order_Mst order_Mst = new Order_Mst();
            if (mst.q != null && mst.q != "0" && mst.q != "")
            {
                if (User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString() == "Distributor")
                {
                    mst.IsDistributor = "True";
                }
                else
                {
                    mst.IsDistributor = "False";
                }
                int _Id = Convert.ToInt32(_comservice.Decrypt(mst.q));
                return await _service.GetEditDatabyId(GetDbConnectionString(), _Id,mst.IsDistributor);

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
                    if (User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString() == "Distributor")
                    {
                        model.IsDistributor = "True";
                        model.CUSTOMER_CODE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.DistributorId).Value.ToString();
                    }
                    else
                    {
                        model.IsDistributor = "False";
                    }
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
                    model.ORDER_UNIT_ID = model.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : model.UNIT_ID;

                    //model.DELIVERY_DATE = model.DELIVERY_DATE == null ? DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"): model.DELIVERY_DATE;

                    _result = JsonSerializer.Deserialize<Result>(await _service.AddOrUpdate(GetDbConnectionString(), model));
                }
                catch (Exception ex)
                {
                    _result.Status = ex.Message;
                }

            }


            return Json(_result);


        }

        [HttpPost]
        public async Task<JsonResult> Process_Order([FromBody] Order_Mst model)
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
                    model.ORDER_UNIT_ID  = model.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : model.UNIT_ID;
                    model.INVOICE_UNIT_ID = model.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : model.UNIT_ID;

                    _result.Key = await _service.Process_Order(GetDbConnectionString(), model);
                    _result.Status = "1";
                }
                catch (Exception ex)
                {
                    _result.Status = ex.Message;
                }

            }


            return Json(_result);


        }
        
        [HttpPost]
        public async Task<JsonResult> Delete_Order_full_Query([FromBody] Order_Mst model)
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
                    model.ORDER_UNIT_ID = model.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : model.UNIT_ID;
                    model.INVOICE_UNIT_ID = model.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : model.UNIT_ID;

                    _result.Key = await _service.Delete_Order_full(GetDbConnectionString(), model.ORDER_MST_ID);
                    _result.Status = "1";
                }
                catch (Exception ex)
                {
                    _result.Status = ex.Message;
                }

            }

            return Json(_result);


        }
        [HttpPost]
        public async Task<JsonResult> Delete_Processed_Order__Dtl([FromBody] Order_Mst model)
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
                    model.ORDER_UNIT_ID = model.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : model.UNIT_ID;
                    model.INVOICE_UNIT_ID = model.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : model.UNIT_ID;

                    _result.Key = await _service.Delete_Processed_Order(GetDbConnectionString(), model);
                    _result.Status = "1";
                }
                catch (Exception ex)
                {
                    _result.Status = ex.Message;
                }

            }

            return Json(_result);


        }

        [HttpPost]
        public async Task<string> Save_Final_Order([FromBody] Order_Mst model)
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
                    if (User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString() == "Distributor")
                    {
                        model.IsDistributor = "True";
                        model.CUSTOMER_CODE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.DistributorId).Value.ToString();
                    }
                    else
                    {
                        model.IsDistributor = "False";
                    }
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
                    model.ORDER_UNIT_ID = model.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : model.UNIT_ID;
                    model.INVOICE_UNIT_ID = model.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : model.UNIT_ID;

                    string res = await _service.Save_Final_Order(GetDbConnectionString(), model);
                    return res;
                }
                catch (Exception ex)
                {
                    _result.Status = ex.Message;
                }

            }


            return JsonSerializer.Serialize(_result);


        }


        [HttpPost]
        public async Task<JsonResult> Save_Final_Post_Order([FromBody] Order_Mst model)
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
                    if (User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString() == "Distributor")
                    {
                        model.IsDistributor = "True";
                        model.CUSTOMER_CODE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.DistributorId).Value.ToString();
                    }
                    else
                    {
                        model.IsDistributor = "False";
                    }

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
                    model.ORDER_UNIT_ID =  model.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : model.UNIT_ID;
                    model.INVOICE_UNIT_ID =  model.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : model.UNIT_ID;

                    _result.Key = await _service.Save_Final_Post_Order(GetDbConnectionString(), model);
                    _result.Status = "1";
                }
                catch (Exception ex)
                {
                    _result.Status = ex.Message;
                }

            }


            return Json(_result);


        }
        [HttpPost]
        public async Task<JsonResult> Update_Order_Confirmation_Status([FromBody] Order_Mst model)
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
                    if (User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString() == "Distributor")
                    {
                        model.IsDistributor = "True";
                        model.CUSTOMER_CODE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.DistributorId).Value.ToString();
                    }
                    else
                    {
                        model.USER_TYPE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString();

                        model.IsDistributor = "False";
                    }
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
                    model.ORDER_UNIT_ID = model.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : model.UNIT_ID;
                    model.INVOICE_UNIT_ID = model.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : model.UNIT_ID;

                    _result.Key = await _service.Save_Post_Confirmation_Order(GetDbConnectionString(), model);
                    _result.Status = "1";
                }
                catch (Exception ex)
                {
                    _result.Status = ex.Message;
                }

            }


            return Json(_result);


        }

        [HttpPost]
        public async Task<JsonResult> Cancel_Order_Confirmation_Status([FromBody] Order_Mst model)
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
                    if (User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString() == "Distributor")
                    {
                        model.IsDistributor = "True";
                        model.CUSTOMER_CODE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.DistributorId).Value.ToString();
                    }
                    else
                    {
                        model.USER_TYPE = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserType).Value.ToString();

                        model.IsDistributor = "False";
                    }
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
                    model.ORDER_UNIT_ID = model.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : model.UNIT_ID;
                    model.INVOICE_UNIT_ID = model.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : model.UNIT_ID;

                    _result.Key = await _service.Cancel_Post_Confirmation_Order(GetDbConnectionString(), model);
                    _result.Status = "1";
                }
                catch (Exception ex)
                {
                    _result.Status = ex.Message;
                }

            }


            return Json(_result);


        }

        [AuthorizeCheck]
        public IActionResult OrderMonitoring()
        {
            return View();
        }



        [HttpPost]
        public async Task<string> LoadProductPerfactOrderQty([FromBody] Order_Mst mst)
        {
            return await _service.LoadProductPerfactOrderQty(GetDbConnectionString(), mst);
        }
    }
}
