using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution;
using SalesAndDistributionSystem.Services.Business.Security;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Areas.SalesAndDistribution.Controllers
{
    [Area("SalesAndDistribution")]
    [Authorize]
    public class OrderAdjustmentController : Controller
    {
        private readonly IOrderAdjustmentManager _service;
        private readonly ILogger<OrderAdjustmentController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();

        public OrderAdjustmentController(IOrderAdjustmentManager service, ILogger<OrderAdjustmentController> logger, IConfiguration configuration)
        {
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }
        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");

        public string GetCompany() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString();

        [AuthorizeCheck]
        public IActionResult Adjustment(Order_Adjustment order_Adjustment)
        {
            _logger.LogInformation("Adjustment (OrderAdjustment/Adjustment) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View(order_Adjustment);
        }

        [AuthorizeCheck]
        public IActionResult DebitCreditAdj(DEBIT_CREDIT_ADJ model)
        {
            _logger.LogInformation("Adjustment (OrderAdjustment/Adjustment) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");
            return View(model);
        }


        [HttpPost]
        public async Task<string> LoadData([FromBody] Order_Adjustment area_Info)
        {

            int comp_id = area_Info == null || area_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : area_Info.COMPANY_ID;
            return await _service.LoadData(GetDbConnectionString(), comp_id);

        }

        [HttpPost]
        public async Task<string> LoadDebitCreditAdjData([FromBody] DEBIT_CREDIT_ADJ area_Info)
        {

            int comp_id = area_Info == null || area_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : area_Info.COMPANY_ID;
            return await _service.LoadDebitCreditAdjData(GetDbConnectionString(), comp_id);

        }

        [HttpPost]
        public async Task<string> LoadDataByOrderId([FromBody] Order_Adjustment area_Info)
        {

            int comp_id = area_Info == null || area_Info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : area_Info.COMPANY_ID;
            return await _service.LoadDataByOrderId(GetDbConnectionString(), comp_id, area_Info.ORDER_MST_ID);

        }


        [HttpPost]
        public async Task<string> AddOrUpdate([FromBody] Order_Adjustment model)
        {
            Result result = new Result();

            if (model == null)
            {
                result.Status = "No Changes Found!";
            }
            else
            {
                try
                {
                    if (model.ID == 0)
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

                    return await _service.AddOrUpdate(GetDbConnectionString(), model);

                }
                catch (Exception ex)
                {
                    result.Status = ex.Message;
                }

            }


            return JsonSerializer.Serialize(result);


        }

        [HttpPost]
        public async Task<string> AddOrUpdateDebitCreditAdj([FromBody] DEBIT_CREDIT_ADJ model)
        {
            Result result = new Result();

            if (!ModelState.IsValid)
            {
                result.Status = "No Changes Found!";
            }
            else
            {
                try
                {
                    if (model.ID == 0)
                    {
                        model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        model.COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
                    }
                    return await _service.AddOrUpdateDebitCreditAdj(GetDbConnectionString(), model);
                }
                catch (Exception ex)
                {
                    result.Status = ex.Message;
                }

            }


            return JsonSerializer.Serialize(result);


        }

        [HttpPost]
        public async Task<string> PostDebitCreditAdj([FromBody] DEBIT_CREDIT_ADJ model)
        {
            Result result = new Result();

            if (!ModelState.IsValid)
            {
                result.Status = "No Changes Found!";
            }
            else
            {
                try
                {
                    model.POSTED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                    model.POSTED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                    model.POSTED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                    return await _service.PostDebitCreditAdj(GetDbConnectionString(), model);
                }
                catch (Exception ex)
                {
                    result.Status = ex.Message;
                }

            }


            return JsonSerializer.Serialize(result);


        }

    }
}
