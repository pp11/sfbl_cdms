using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.ReportModels.Common.ProductReports;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    [Authorize]
    public class FGReceiveController : Controller
    {
        private readonly IFgReceiveFromProductionManager _service;
        private readonly ICommonServices _comservice;

        private readonly ILogger<FGReceiveController> _logger;

        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();

        public FGReceiveController(IFgReceiveFromProductionManager service, ILogger<FGReceiveController> logger, IConfiguration configuration, ICommonServices comservice)
        {
            _service = service;
            _logger = logger;
            _configuration = configuration;
            _comservice = comservice;
        }

        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");

        private string GetDbSecurityConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "Security");

        public string GetCompany() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString();

        public string GetUnit() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value.ToString();

        public async Task<string> LoadFGTransferData([FromBody] Fg_Reciving_From_Production receive_info)
        {
            int comp_id = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value);

            return await _service.LoadFGTransferData(GetDbConnectionString(), comp_id, receive_info.UNIT_ID);
        }

        [AuthorizeCheck]
        public IActionResult ReceiveFromProductionList()
        {
            _logger.LogInformation("ReceiveFromProductionList (FGReceive/ReceiveFromProductionList) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View();
        }

        [AuthorizeCheck]
        public IActionResult ReceiveFromProduction(string Id = "0")
        {
            Fg_Reciving_From_Production fg_Reciving_From_Production = new Fg_Reciving_From_Production();

            if (Id != "0" || Id != "")
            {
                fg_Reciving_From_Production.RECEIVE_ID_ENCRYPTED = Id;
            }
            _logger.LogInformation("ReceiveFromProduction (FGReceive/ReceiveFromProduction) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");

            return View(fg_Reciving_From_Production);
        }

        [HttpPost]
        public async Task<string> GetEditDataById([FromBody] Fg_Reciving_From_Production mst)
        {
            Fg_Reciving_From_Production fg_Reciving_From_Production = new Fg_Reciving_From_Production();
            if (mst.RECEIVE_ID_ENCRYPTED != null && mst.RECEIVE_ID_ENCRYPTED != "0" && mst.RECEIVE_ID_ENCRYPTED != "")
            {
                int _Id = Convert.ToInt32(_comservice.Decrypt(mst.RECEIVE_ID_ENCRYPTED));
                return await _service.LoadDetailDataById(GetDbConnectionString(), _Id);
            }
            return null;
        }

        [HttpPost]
        public async Task<string> LoadData([FromBody] ReportParams receive_info)
        {
            int comp_id = receive_info == null || receive_info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : receive_info.COMPANY_ID;
            return await _service.LoadData(GetDbConnectionString(), GetDbSecurityConnectionString(), comp_id, Convert.ToInt32(GetUnit()), receive_info.DATE_FROM, receive_info.DATE_TO);
        }

        [HttpPost]
        public async Task<string> LoadUnchekedData([FromBody] Fg_Reciving_From_Production receive_info)
        {
            int comp_id = receive_info == null || receive_info.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : receive_info.COMPANY_ID;
            return await _service.LoadUnchekedData(GetDbConnectionString(), GetDbSecurityConnectionString(), comp_id, Convert.ToInt32(GetUnit()));
        }

        [HttpPost]
        public async Task<string> GetLastMrp([FromBody] Fg_Reciving_From_Production rcv_info)
        {
            return await _service.GetLastMrp(GetDbConnectionString(), rcv_info.SKU_CODE, User.GetComapanyId(), User.GetUnitId());
        }

        [HttpPost]
        public async Task<JsonResult> AddOrUpdate([FromBody] Fg_Reciving_From_Production model)
        {
            string result = "";

            if (model == null)
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
                    if (model.RECEIVE_ID == 0)
                    {
                        model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        model.COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
                        model.RECEIVED_BY_ID = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value);
                    }
                    else
                    {
                        model.UPDATED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        if (model.IsChecked == "True")
                        {
                            model.CHECKED_BY_ID = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value);
                            model.CHECKED_BY_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        }
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
    }
}