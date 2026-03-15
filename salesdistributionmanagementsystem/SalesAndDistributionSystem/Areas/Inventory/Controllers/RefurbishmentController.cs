using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.Inventory.IManager;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    [Authorize]
    public class RefurbishmentController : Controller
    {
        private readonly IRefurbishmentManager _service;
        private readonly ServiceProvider Provider = new ServiceProvider();
        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");
        public RefurbishmentController(IRefurbishmentManager service)
        {
            _service = service;
        }
        #region  Refurbishment Receiving
        [HttpGet]
        [AuthorizeCheck]
        public IActionResult Receiving()
        {
            return View();
        }
        [HttpPost]
        public async Task<string> AddOrUpdate([FromBody] RefurbishmentReceivingMst model)
        {
            Result result = new Result();
            try
            {
                if (model != null)
                {
                    model.COMPANY_ID = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value;
                    model.UNIT_ID= User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value;
                    if (model.MST_SLNO == 0)
                    {
                        model.ENTERED_BY = User.GetUserId();
                        model.RECEIVED_BY = User.GetUserId();
                        model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                    }
                    else
                    {
                        model.UPDATED_BY = User.GetUserId();
                        model.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                    }
                    result = await _service.AddOrUpdate(GetDbConnectionString(), model);
                }
                else
                {
                    result.Status = "No Data Found ! Please contact your software department!";
                    result.Key = "";
                    result.Parent = "0";
                }
               
            }
            catch (Exception ex)
            {
                result.Status = ex.Message;
                result.Key = "";
                result.Parent = "0";
            }
            return JsonSerializer.Serialize(result);
        }
        [HttpPost]
        public async Task<string> Approval([FromBody] RefurbishmentReceivingMst model)
        {
            Result result = new Result();
            try
            {
                if (model != null)
                {

                    if (model.MST_SLNO != 0)
                    {
                        model.APPROVED_BY = User.GetUserId();
                        model.APPROVED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.APPROVED_STATUS = "A";
                        result = await _service.Approval(GetDbConnectionString(), model);
                    }
                    else {
                        result.Status = "No Data Found" ;
                        result.Key = "";
                        result.Parent = "0";
                    }
                }
            }
            catch (Exception ex)
            {
                result.Status = ex.Message;
                result.Key = "";
                result.Parent = "0";
            }
            return JsonSerializer.Serialize(result);
        }
        [HttpGet]
        public async Task<string> GetDistList()
        {
            string unitId = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value;
            return await _service.GetDistList(GetDbConnectionString(), unitId);
        }
        [HttpGet]
        public async Task<string> GetManualDistList(string searchKey)
        {
            return await _service.GetManualDistList(GetDbConnectionString(), searchKey);
        }
        [HttpGet]
        public async Task<string> GetManualProductList(string searchKey)
        {
            string companyId = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value;
            string unitId = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value;
            return await _service.GetManualProductList(GetDbConnectionString(), companyId,unitId, searchKey);
        }
        [HttpGet]
        public async Task<string> GetDistListWhileEdit(string mstId)
        {
            return await _service.GetDistListWhileEdit(GetDbConnectionString(), mstId);
        }
        [HttpGet]
        public async Task<string> GetProductsByChallan(string challanNo)
        {
            return await _service.GetProductsByChallan(GetDbConnectionString(), challanNo);
        }
        [HttpGet]
        public async Task<string> LoadDtlByMstId(string mstId)
        {
            return await _service.LoadDtlByMstId(GetDbConnectionString(), mstId);
        }
        [HttpGet]
        public async Task<string> GetMstList(string  fromDate, string toDate)
        {
            string companyId = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value;
            string unitId = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value;
            return await _service.GetMstList(GetDbConnectionString(), companyId, unitId,fromDate, toDate);
        }
        #endregion
        #region Refurbishment Finalization
        [HttpGet]
        [AuthorizeCheck]
        public IActionResult Finalization()
        {
            return View();
        }
        [HttpGet]
        public async Task<string> GetClaims()
        {
            string companyId = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value;
            string unitId = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value;
            return await _service.GetClaims(GetDbConnectionString(), companyId, unitId);
        }
        [HttpGet]
        public async Task<string> GetProductsByClaimNo(string db, string claimNo)
        {
            return await _service.GetProductsByClaimNo(GetDbConnectionString(), claimNo);
        }
        [HttpPost]
        public async Task<string> AddOrUpdateFinalization([FromBody] RefurbishmentFinalizeMaster model)
        {
            Result result = new Result();
            try
            {
                if (model != null)
                {
                    model.COMPANY_ID = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value;
                    model.UNIT_ID = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value;
                    if (model.MST_SLNO == 0)
                    {
                        model.ENTERED_BY = User.GetUserId();
                        model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                    }
                    else
                    {
                        model.UPDATED_BY = User.GetUserId();
                        model.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                    }
                    result = await _service.AddOrUpdateFinalization(GetDbConnectionString(), model);
                }
                else
                {
                    result.Status = "No Data Found ! Please contact your software department!";
                    result.Key = "";
                    result.Parent = "0";
                }

            }
            catch (Exception ex)
            {
                result.Status = ex.Message;
                result.Key = "";
                result.Parent = "0";
            }
            return JsonSerializer.Serialize(result);
        }
        [HttpGet]
        public async Task<string> GetFinalizationMstList(string fromDate, string toDate)
        {
            string companyId = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value;
            string unitId = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value;
            return await _service.GetFinalizationMstList(GetDbConnectionString(), companyId, unitId ,fromDate, toDate);
        }
        [HttpGet]
        public async Task<string> GetClaimsWhileEdit(string mstId)
        {
            return await _service.GetClaimsWhileEdit(GetDbConnectionString(), mstId);
        }
        [HttpGet]
        public async Task<string> loadFinalizationDtlByMstId(string mstId)
        {
            string companyId = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value;
            string unitId = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value;
            return await _service.loadFinalizationDtlByMstId(GetDbConnectionString(), companyId, unitId, mstId);
        }
        [HttpPost]
        public async Task<string> FinalizationApproval([FromBody] RefurbishmentFinalizeMaster model)
        {
            Result result = new Result();
            try
            {
                if (model != null)
                {

                    if (model.MST_SLNO != 0)
                    {
                        model.APPROVED_BY = User.GetUserId();
                        model.APPROVED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.APPROVED_STATUS = "A";
                        result = await _service.FinalizationApproval(GetDbConnectionString(), model);
                    }
                    else
                    {
                        result.Status = "No Data Found";
                        result.Key = "";
                        result.Parent = "0";
                    }
                }
            }
            catch (Exception ex)
            {
                result.Status = ex.Message;
                result.Key = "";
                result.Parent = "0";
            }
            return JsonSerializer.Serialize(result);
        }
        [HttpGet]
        public async Task<string> GetManualProductsWithStock(string searchKey)
        {
            string companyId = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value;
            string unitId = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value;
            return await _service.GetManualProductsWithStock(GetDbConnectionString(), companyId, unitId,searchKey);
        }
        #endregion
    }
}
