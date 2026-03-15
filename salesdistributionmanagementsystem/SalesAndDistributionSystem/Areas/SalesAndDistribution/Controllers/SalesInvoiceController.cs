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
using System.Text.Json;
using System.Threading.Tasks;
using ServiceProvider = SalesAndDistributionSystem.Common.ServiceProvider;

namespace SalesAndDistributionSystem.Areas.SalesAndDistribution.Controllers
{
    [Area("SalesAndDistribution")]
    [Authorize]
    public class SalesInvoiceController : Controller
    {
        private readonly ISalesInvoiceManager _service;
        private readonly ICommonServices _comservice;

        private readonly ILogger<SalesInvoiceController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();

        public SalesInvoiceController(ICommonServices comservice, ISalesInvoiceManager service, ILogger<SalesInvoiceController> logger, IConfiguration configuration)
        {
            _comservice = comservice;
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }
        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");
        private string GetDbSecurityConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "Security");
        public string GetUser() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString();

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
            _logger.LogInformation(" List(SalesInvoice/List) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View();
        }

        [AuthorizeCheck]
        public IActionResult InvoiceDetails(string q)
        {
            Invoice_Mst _Mst = new Invoice_Mst();

            if (q!=null&& q!="")
            {
                _Mst.MST_ID_ENCRYPTED = q;
                
            }
            _logger.LogInformation("InvoiceDetails (SalesInvoice/InvoiceDetails) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View(_Mst);
        }

        [HttpPost]
        public async Task<string> LoadPostableData([FromBody] OrderSKUFilterParameters model)
        {

            model.COMPANY_ID = model == null || model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
            model.UNIT_ID = model == null || model.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : model.UNIT_ID;
            model.db_security = GetDbSecurityConnectionString();
            model.db_sales = GetDbConnectionString();
            model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
            model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
            model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
            return await _service.LoadPostableData(model);

        }
        
        [HttpPost]
        public async Task<string> AllPostOrder([FromBody] Order_Mst mst)
        {
            int comp_id = mst == null || mst.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : mst.COMPANY_ID;

            return await _service.AllPostOrder(GetDbConnectionString(), comp_id, Convert.ToInt32(GetUnit()), mst.ORDER_NO_LIST,GetDbSecurityConnectionString(), Convert.ToInt32(GetUser()), HttpContext.Connection.RemoteIpAddress.ToString());
        }
        [HttpPost]
        public async Task<string> SinglePostOrder([FromBody] Order_Mst mst)
        {
            int comp_id = mst == null || mst.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : mst.COMPANY_ID;

            return await _service.SinglePostOrder(GetDbConnectionString(), comp_id, Convert.ToInt32(GetUnit()), mst.ORDER_MST_ID.ToString(),GetDbSecurityConnectionString(),Convert.ToInt32(GetUser()), HttpContext.Connection.RemoteIpAddress.ToString());
        }
        [HttpPost]
        public async Task<string> CancelOrder([FromBody] Order_Mst mst)
        {
            int comp_id = mst == null || mst.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : mst.COMPANY_ID;

            var data = await _service.CancelOrder(GetDbConnectionString(), comp_id.ToString(), GetUnit(), mst.ORDER_MST_ID.ToString());
            return null;
        }
        [HttpPost]
        public async Task<string> DeleteInvoice([FromBody] Invoice_Mst mst) => await _service.DeleteInvoice(GetDbConnectionString(), mst.INVOICE_NO, Convert.ToInt32(GetCompany()), Convert.ToInt32(GetUnit()),GetDbSecurityConnectionString(), Convert.ToInt32(GetUser()), HttpContext.Connection.RemoteIpAddress.ToString());
      
        [HttpPost]
        public async Task<string> LoadPreInvoiceData([FromBody] Order_Mst region_Info)
        {

            int comp_id = region_Info == null || region_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : region_Info.COMPANY_ID;
            return   await _service.Load_PreInvoice_Data(GetDbConnectionString(), region_Info.ORDER_MST_ID.ToString(), region_Info.ORDER_DATE, region_Info.CUSTOMER_ID.ToString(), region_Info.CUSTOMER_CODE, region_Info.ORDER_UNIT_ID.ToString(), region_Info.INVOICE_UNIT_ID.ToString(), comp_id.ToString());
        }

        [HttpPost]
        public async Task<string> UpdateOrderRevisedQty([FromBody] Order_Mst region_Info)
        {

            int comp_id = region_Info == null || region_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : region_Info.COMPANY_ID;
            return await _service.UpdateOrderRevisedQty(GetDbConnectionString(), region_Info);
        }
        [HttpPost]
        public async Task<string> Load_Sales_Invoice_Mst_data([FromBody] OrderSKUFilterParameters model)
        {
            model.COMPANY_ID = model == null || model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
            model.UNIT_ID = model == null || model.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : model.UNIT_ID;
            model.db_security = GetDbSecurityConnectionString();
            model.db_sales = GetDbConnectionString();
            model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
            model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
            model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();

            return await _service.Load_Sales_Invoice_Mst_data(model);
        }

        [HttpPost]
        public async Task<string> LoadInvoiceDetailsData([FromBody] Invoice_Mst mst)
        {
            Order_Mst order_Mst = new Order_Mst();
            if (mst.q != null && mst.q != "0" && mst.q != "")
            {
                return await _service.LoadInvoiceDetailsData(GetDbConnectionString(), _comservice.Decrypt(mst.q));
            }
            return null;
        }
        [HttpPost]
        public async Task<string> LoadSearchableInvoice([FromBody] Invoice_Mst mst) => await _service.LoadSearchableInvoice(GetDbConnectionString(), GetCompany(),GetUnit(),mst.INVOICE_NO);

        [HttpPost]
        public async Task<string> LoadInvoicesByCustomer([FromBody] Invoice_Mst mst) => await _service.LoadSearchableInvoiceByCustomer(GetDbConnectionString(), GetCompany(),GetUnit(),mst.INVOICE_NO, mst.CUSTOMER_ID.ToString());

        [HttpPost]
        public async Task<string> LoadSearchableInvoiceInDate([FromBody] Invoice_Mst mst) => await _service.LoadSearchableInvoiceInDate(GetDbConnectionString(),  mst.INVOICE_UNIT_ID.ToString(), mst.COMPANY_ID, mst.INVOICE_NO, mst.INVOICE_DATE, mst.q);


        [HttpPost]
        public async Task<string> LoadSearchableOrderInDate([FromBody] Invoice_Mst mst) => await _service.LoadSearchableOrderInDate(GetDbConnectionString(), mst.INVOICE_UNIT_ID.ToString(), mst.COMPANY_ID, mst.INVOICE_NO, mst.INVOICE_DATE, mst.q);


        [HttpPost]
        public async Task<string> LoadMaxMinInvoiceInDate([FromBody] Invoice_Mst mst) => await _service.LoadMaxMinInvoiceInDate(GetDbConnectionString(),  mst.INVOICE_UNIT_ID.ToString(), mst.COMPANY_ID, mst.INVOICE_DATE);
        public async Task<string> LoadMaxMinOrderInDate([FromBody] Invoice_Mst mst) => await _service.LoadMaxMinOrderInDate(GetDbConnectionString(), mst.INVOICE_UNIT_ID.ToString(), mst.COMPANY_ID, mst.INVOICE_DATE);



    }
}
