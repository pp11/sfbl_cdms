using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using SalesAndDistributionSystem.Domain.Models.ViewModels.Inventory;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.Inventory.IManager;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    [Authorize]
    public class DistributionDeliveryController : Controller
    {
        private readonly IDistributionDeliveryManager _service;
        private readonly ICommonServices _comservice;
        private readonly ILogger<DistributionDeliveryController> _logger;

        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();


        public DistributionDeliveryController(IDistributionDeliveryManager _service,
            ILogger<DistributionDeliveryController> _logger,
            ICommonServices _comservice,
            IConfiguration _configuration)
        {
            this._service = _service;
            this._comservice = _comservice;
            this._logger = _logger;
            this._configuration = _configuration;
        }

        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");

        [HttpGet]
        public async Task<string> GetDistributionRoutes()
        {
            return await _service.GetDistributionRoutes(GetDbConnectionString(), User.GetComapanyId(), User.GetUnitId());
        }

        [HttpGet]
        public async Task<string> GetPendingInvoices()
        {
            return await _service.GetPendingInvoices(GetDbConnectionString(), User.GetComapanyId(), User.GetUnitId());
        }

        [HttpGet]
        public async Task<string> GetPendingInvoicesByUnit(int unitId)
        {
            return await _service.GetPendingInvoices(GetDbConnectionString(), User.GetComapanyId(), unitId);
        }

        [HttpGet]
        public async Task<string> GetProductsByInvoice(string id)
        {
            return await _service.GetProductsByInvoice(GetDbConnectionString(), User.GetComapanyId(), User.GetUnitId(), id);
        }


        [HttpPost]
        public async Task<string> GetProductsByInvoices([FromBody] List<string> Invoices)
        {
            if (Invoices == null) return null;
            return await _service.GetInvoiceProducts(GetDbConnectionString(), User.GetComapanyId(), User.GetUnitId(), Invoices);
        }
        [HttpPost]
        public async Task<string> GetProductsByGifts([FromBody] List<string> Invoices)
        {
            if (Invoices == null) return null;
            return await _service.GetInvoiceGifts(GetDbConnectionString(), User.GetComapanyId(), User.GetUnitId(), Invoices);
        }

        [HttpGet]
        public async Task<string> GetGiftByInvoice(string id)
        {
            return await _service.GetGiftByInvoice(GetDbConnectionString(), User.GetComapanyId(), User.GetUnitId(), id);
        }

        [HttpGet]
        public async Task<string> CustomerByRoute(string id)
        {
            return await _service.CustomerByRoute(GetDbConnectionString(), User.GetComapanyId(), User.GetUnitId(), id);
        }

        [HttpPost]
        public async Task<string> GetProductBatches([FromBody] Depot_Customer_Dist_Mst mst)
        {
            return await _service.GetProductBatches(GetDbConnectionString(), mst.MST_ID_ENCRYPTED);
        }

        [HttpPost]
        public async Task<JsonResult> AddOrUpdate([FromBody] Depot_Customer_Dist_Mst model)
        {
            var result = new Result();

            if (!ModelState.IsValid)
            {
                var arr = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                return new JsonResult(arr);
            }
            else if (model.Invoices.Count <= 0)
            {
                result.Status = "No Invoice Found";
                return Json(result);
            }
            else
            {
                try
                {
                    if (model.MST_ID == 0)
                    {
                        model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        model.COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
                    }
                    else
                    {
                        model.UPDATED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                    }

                    if (model.COMPANY_ID <= 0)
                    {
                        result.Status = "please reload or login";

                    }

                    result.Key = await _service.AddOrUpdate(GetDbConnectionString(), model);
                    result.Status = "1";
                    result.Parent = _comservice.Encrypt(model.MST_ID.ToString());
                    result.CodeNo = model.DISTRIBUTION_NO;
                }
                catch (Exception ex)
                {
                    result.Status = ex.Message;
                }
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<string> LoadData([FromBody] DistributionFilterParameter receive_info)
        {
            receive_info.COMPANY_ID = receive_info.COMPANY_ID != 0 ? receive_info.COMPANY_ID : User.GetComapanyId();
            receive_info.INVOICE_UNIT_ID = User.GetUnitId();
            return await _service.LoadData(GetDbConnectionString(), receive_info);
        }

        [HttpPost]
        public async Task<string> GetNonConfirmDeliveryList([FromBody] DistributionFilterParameter receive_info)
        {
            receive_info.COMPANY_ID = receive_info.COMPANY_ID != 0 ? receive_info.COMPANY_ID : User.GetComapanyId();
            receive_info.INVOICE_UNIT_ID = User.GetUnitId();
            return await _service.GetNonConfirmDeliveryList(GetDbConnectionString(), receive_info);
        }

        [HttpPost]
        public async Task<string> GetEditDataById([FromBody] Depot_Customer_Dist_Mst model)
        {
            var companyId = model.COMPANY_ID != 0 ? model.COMPANY_ID : User.GetComapanyId();
            if (!string.IsNullOrWhiteSpace(model.MST_ID_ENCRYPTED))
            {
                string id = _comservice.Decrypt(model.MST_ID_ENCRYPTED);
                return await _service.GetEditDataById(GetDbConnectionString(), companyId, id);
            }
            return null;
        }

        [AuthorizeCheck]
        public IActionResult Delivery(string id = "")
        {
            var model = new Depot_Customer_Dist_Mst();
            if (id != "")
            {
                model.MST_ID_ENCRYPTED = id;
            }
            _logger.LogInformation("DistributionDelivery (Inventory/DistributionDelivery/Delivery) Page Has been accessed By " + User.GetUserName() + " ( ID= " + User.GetUserId() + " )");
            return View(model);
        }

        [AuthorizeCheck]
        public IActionResult DeliveryList()
        {
            _logger.LogInformation("DistributionDeliveryList (Inventory/DistributionDelivery/DeliveryList) Page Has been accessed By " + User.GetUserName() + " ( ID= " + User.GetUserId() + " )");
            return View();
        }

        [AuthorizeCheck]
        public IActionResult NonConfirmedList()
        {
            _logger.LogInformation("DistributionNonConfirmedList (Inventory/DistributionDelivery/NonConfirmedList) Page Has been accessed By " + User.GetUserName() + " ( ID= " + User.GetUserId() + " )");
            return View();
        }
    }
}
