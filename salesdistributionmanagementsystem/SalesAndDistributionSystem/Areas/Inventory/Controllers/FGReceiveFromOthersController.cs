using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.ReportModels.Common.ProductReports;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    [Authorize]

    public class FGReceiveFromOthersController : Controller
    {
        private readonly IFgReceiveFromOthersManager _service;
        private readonly ICommonServices _comservice;

        private readonly ILogger<FGReceiveFromOthersController> _logger;

        private readonly ServiceProvider Provider = new ServiceProvider();

        public FGReceiveFromOthersController(IFgReceiveFromOthersManager service,
            ILogger<FGReceiveFromOthersController> logger, 
            ICommonServices comservice)
        {
            _service = service;
            _logger = logger;
            _comservice = comservice;
        }
        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");
        private string GetDbSecurityConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "Security");

        public string GetCompany() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString();
        public string GetUnit() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value.ToString();

        public async Task<string> LoadFGTransferData([FromBody] Fg_Reciving_From_Others receive_info)
        {
            int comp_id =  Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value);

            return await _service.LoadFGTransferData(GetDbConnectionString(), comp_id,receive_info.UNIT_ID);
        }
        public async Task<string> Get_Refurbishment_SKU(string Ref_code)
        {
            return await _service.Get_Refurbishment_SKU(GetDbConnectionString(), Ref_code);
        }

        public async Task<string> Get_Refurbishment_SKU_ALL()
        {
            return await _service.Get_Refurbishment_SKU_ALL(GetDbConnectionString());
        }

        [AuthorizeCheck]
        public IActionResult ReceiveFromOthersList()
        {
            _logger.LogInformation("ReceiveFromOthersList (FGReceive/ReceiveFromOthersList) Page Has been accessed By " + User.GetUserName() + " ( ID= " + User.GetUserId());

            return View();
        }

        [AuthorizeCheck]
        public IActionResult ReceiveFromOthers(string Id = "0")
        {
            Fg_Reciving_From_Others fg_Reciving_From_Production = new Fg_Reciving_From_Others();

            if (Id != "0" || Id != "")
            {
                fg_Reciving_From_Production.RECEIVE_ID_ENCRYPTED = Id;
            }
            _logger.LogInformation("ReceiveFromProduction (FGReceive/ReceiveFromProduction) Page Has been accessed By " + User.GetUserName() + " ( ID= " + User.GetUserId() + " )");

            return View(fg_Reciving_From_Production);
        }
        [HttpPost]
        public async Task<string> GetEditDataById([FromBody] Fg_Reciving_From_Others mst)
        {
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

            int comp_id = receive_info == null || receive_info.COMPANY_ID == 0 ? User.GetComapanyId() : receive_info.COMPANY_ID;

            return await _service.LoadData(GetDbConnectionString(), GetDbSecurityConnectionString(), comp_id, Convert.ToInt32(GetUnit()), receive_info.DATE_FROM, receive_info.DATE_TO);

        }

        [HttpPost]
        public async Task<string> GetBatches([FromBody] Fg_Reciving_From_Others mst)
        {
            int comp_id = User.GetComapanyId();

            return await _service.GetBatches(GetDbConnectionString(), comp_id, mst.SKU_ID, mst.RECEIVE_TYPE);
        }

        [HttpPost]
        public async Task<string> GetApprovedList([FromBody] Fg_Reciving_From_Others receive_info)
        {
            int comp_id = receive_info == null || receive_info.COMPANY_ID == 0 ? User.GetComapanyId() : receive_info.COMPANY_ID;

            return await _service.GetApprovedList(GetDbConnectionString(), GetDbSecurityConnectionString(), comp_id, Convert.ToInt32(GetUnit()));
        }
        
        [HttpPost]
        public async Task<string> LoadUnchekedData([FromBody] ReportParams receive_info)
        {
            int comp_id = receive_info == null || receive_info.COMPANY_ID == 0 ? User.GetComapanyId() : receive_info.COMPANY_ID;
            receive_info.DATE_FROM = receive_info.DATE_FROM ?? "01/01/0001";
            receive_info.DATE_TO = receive_info.DATE_TO ?? "01/01/4023";
            return await _service.LoadUnchekedData(GetDbConnectionString(), GetDbSecurityConnectionString(), comp_id, Convert.ToInt32(GetUnit()), receive_info.DATE_FROM, receive_info.DATE_TO);
        }

        
        [HttpGet]
        public async Task<JsonResult> UpdateStatusToCancel(string  id)
        {
            Result _result = new Result();
            //string result = "";

           
                try
                {
                    
                    _result.Key = await _service.UpdateStatusToCancel(GetDbConnectionString(), id);
                    if (_result.Key == "1")
                    {
                        _result.Status = "Cancellation is done!";
                    }
                    else
                    {
                        _result.Status = "Sorry! This one can not be cancelled!!!";
                    }
                }
                catch (Exception ex)
                {
                    _result.Status = ex.Message;
                }

            

            return Json(_result);
        }

        [HttpPost]
        public async Task<JsonResult> AddOrUpdate([FromBody] Fg_Reciving_From_Others model)
        {
            Result _result = new Result();
            //string result = "";

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
                        if (model.IsChecked)
                        {
                            model.CHECKED_BY_ID = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value);
                            model.CHECKED_BY_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        }
                    }

                    _result.Key = await _service.AddOrUpdate(GetDbConnectionString(), model);
                    if (_result.Key=="0")
                    {
                        _result.Status = "This item has different UNIT TP and MRP under this batch no, You can't declare different MRP or UNIT TP for same batch";
                    }
                    else
                    {
                        _result.Status = "1";
                    }
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
