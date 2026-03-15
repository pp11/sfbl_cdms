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
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ServiceProvider = SalesAndDistributionSystem.Common.ServiceProvider;

namespace SalesAndDistributionSystem.Areas.SalesAndDistribution.Controllers
{
    [Area("SalesAndDistribution")]
    [Authorize]

    public class SalesReturnController : Controller
    {
        private readonly ISalesReturnManager _service;
        private readonly ICommonServices _comservice;

        private readonly ILogger<SalesReturnController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();

        public SalesReturnController(ICommonServices comservice, ISalesReturnManager service, ILogger<SalesReturnController> logger, IConfiguration configuration)
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
        public IActionResult List()
        {
            _logger.LogInformation("Region Area Relation List(RegionAreaRelation/List) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View();
        }
        [AuthorizeCheck]
        public IActionResult SalesReturnDetails(string q)
        {
            Return_Mst _Mst = new Return_Mst();

            if (q != null && q != "")
            {
                _Mst.MST_ID_ENCRYPTED = q;

            }
            _logger.LogInformation("SalesReturnDetails (SalesReturn/SalesReturnDetails) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View(_Mst);
        }

       

       
       
        [HttpPost]
        public async Task<string> DeleteReturn([FromBody] Return_Mst model)
        {
            model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
            model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
            model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
            model.COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
            model.UNIT_ID = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value);
            model.db_security = GetDbSecurityConnectionString();
            model.db_sales = GetDbConnectionString();
            return await _service.DeleteReturn(GetDbConnectionString(), model);
        }

        [HttpPost]
        public async Task<string> FullReturn([FromBody] SalesReturnParameters model)
        {
            model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
            model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
            model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
            model.COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
            model.UNIT_ID = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value);
            model.db_security = GetDbSecurityConnectionString();
            model.db_sales = GetDbConnectionString();
            var data = await _service.FullReturn(GetDbConnectionString(), model);
            return data;
        }

        [HttpPost]
        public async Task<string> LoadSalesReturnMst([FromBody] OrderSKUFilterParameters model)
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
                model.COMPANY_ID = model == null || model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
                model.UNIT_ID = model == null || model.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : model.UNIT_ID;
                model.db_security = GetDbSecurityConnectionString();
                model.db_sales = GetDbConnectionString();
                model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();

                var data = await _service.LoadSalesReturns(model);
                return data;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }

        [HttpPost]
        public async Task<string> LoadInvoiceDetailsData([FromBody] Invoice_Mst mst)
        {
            Order_Mst order_Mst = new Order_Mst();
            if (mst.q != null && mst.q != "0" && mst.q != "")
            {
                return await _service.LoadReturnDetailsData(GetDbConnectionString(), _comservice.Decrypt(mst.q));
            }
            return null;
        }

        [HttpPost]
        public async Task<string> InvoicesLoad([FromBody] OrderSKUFilterParameters model)
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
            return await _service.InvoicesLoad(GetDbConnectionString(), model.DATE_FROM,model.DATE_TO, GetCompany(), GetUnit(),model.IsDistributor,model.CUSTOMER_CODE);
            
        }



        [HttpPost]
        public async Task<string> ProcessPartialReturnList([FromBody] List<Process_data> model)
        {
            Result result = new Result();
            try
            {
                foreach (var item in model)
                {
                    await this.ProcessPartialReturn(item);
                }
                result.Status = "1";
            }
            catch (Exception ex)
            {
                result.Status = ex.Message;
            }
            return JsonSerializer.Serialize(result);
        }

        [HttpPost]
        public async Task<string> ProcessPartialReturn([FromBody] Process_data model)
        {
            Result result = new Result();
            try
            {
                model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                model.COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
                model.UNIT_ID = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value);
                model.db_security = GetDbSecurityConnectionString();
                model.db_sales = GetDbConnectionString();
                var data = await _service.ProcessPartialReturn(GetDbConnectionString(), model);
                result.Status = "1";
            }
            catch (Exception ex)
            {
                result.Status  = ex.Message;
            }
               return JsonSerializer.Serialize(result);
        }
        [HttpPost]
        public async Task<string> SalesReturnPartial([FromBody] List<Process_data> model)
        {
            Result result = new Result();
            try
            {
                //var result_ =await this.ProcessPartialReturnList(model);

                 //Result result1 =  JsonSerializer.Deserialize<Result>(result_);
                //if(result1.Status == "1")
                //{
                    model[0].ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                    model[0].ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                    model[0].ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                    model[0].COMPANY_ID = model[0].COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model[0].COMPANY_ID;

                    model[0].UNIT_ID = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value);
                    model[0].db_security = GetDbSecurityConnectionString();
                    model[0].db_sales = GetDbConnectionString();
                    var data = await _service.SalesReturnPartial(GetDbConnectionString(), model);

                //}

                
                result.Status = data;
            }
            catch (Exception ex)
            {
                result.Status = ex.Message;
            }
            return JsonSerializer.Serialize(result);
        }
    }
}
