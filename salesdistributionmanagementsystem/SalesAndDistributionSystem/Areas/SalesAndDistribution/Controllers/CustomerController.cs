using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution;
using SalesAndDistributionSystem.Services.Business.Security;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Areas.SalesAndDistribution.Controllers
{
    [Area("SalesAndDistribution")]
    public class CustomerController : Controller
    {
        private readonly ICustomerManager _service;
        private readonly ILogger<CustomerController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();
        public CustomerController(ICustomerManager service, ILogger<CustomerController> logger, IConfiguration configuration)
        {
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }
        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");
        public string GetCompany() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString();

        [AuthorizeCheck]
        public IActionResult CustomerInfo()
        {
            _logger.LogInformation("CustomerInfo (Customer/CustomerInfo) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View();
        }
        [HttpPost]
        public async Task<string> LoadData([FromBody] Customer_Info customer_Info)
        {
            int comp_id = customer_Info == null || customer_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : customer_Info.COMPANY_ID;
            return await _service.LoadData(GetDbConnectionString(), comp_id);
        }


        [HttpPost]
        public async Task<string> LoadCustomerDataByType([FromBody] Customer_Info customer_Info)
        {
            int comp_id = customer_Info == null || customer_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : customer_Info.COMPANY_ID;
            return await _service.LoadCustomerDataByType(GetDbConnectionString(), comp_id, customer_Info.CUSTOMER_TYPE_ID);

        }
        [HttpGet]
        public async Task<string> LoadActiveCustomerData(int COMPANY_ID = 0)
        {
            int comp_id = COMPANY_ID == null || COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : COMPANY_ID;
            return await _service.LoadActiveCustomerData(GetDbConnectionString(), comp_id);
        }

        [HttpGet]
        public async Task<string> LoadCustomerDropdownData(int COMPANY_ID)
        {
            int comp_id = COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : COMPANY_ID;
            return await _service.LoadCustomerDropdownData(GetDbConnectionString(), comp_id);

        }

        [HttpPost]
        public async Task<string> GetCustomerRoute([FromBody] Customer_Info customer_Info)
        {
            return await _service.GetCustomerRoutes(GetDbConnectionString(), customer_Info.CUSTOMER_ID);
        }

        [HttpPost]
        public async Task<string> GetSearchableCustomer([FromBody] Customer_Info customer_Info)
        {

            int comp_id = customer_Info == null || customer_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : customer_Info.COMPANY_ID;
            return await _service.GetSearchableCustomer(GetDbConnectionString(), comp_id, customer_Info.CUSTOMER_NAME);

        }
        
        [HttpPost]
        public async Task<JsonResult> AddOrUpdate([FromBody] Customer_Info model)
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
                    if (model.CUSTOMER_ID == 0)
                    {
                        model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        model.OPENING_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
                        model.CLOSING_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                    }
                    else
                    {
                        model.UPDATED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
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
        public async Task<JsonResult> AddOrUpdateDistProduct([FromBody] Customer_Info model)
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
                    if (model.CUSTOMER_ID == 0)
                    {
                        model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        model.OPENING_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
                        model.CLOSING_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                    }
                    else
                    {
                        model.UPDATED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                    }

                    result = await _service.AddOrUpdate_Dist_Product_Type(GetDbConnectionString(), model);

                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }

            }


            return Json(result);


        }

        //---------------Other Methods --------------------
        [HttpPost]
        public async Task<string> GenerateCustomerCode([FromBody] Customer_Info customer_Info)
        {
            int comp_id = customer_Info == null || customer_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : customer_Info.COMPANY_ID;
            return await _service.GenerateCustomerCode(GetDbConnectionString(), comp_id.ToString(), User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyName).Value);

        }
    }
}
