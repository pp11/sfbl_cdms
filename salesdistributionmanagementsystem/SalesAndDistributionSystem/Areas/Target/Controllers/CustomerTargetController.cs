using Microsoft.AspNetCore.Mvc;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.Entities.Target;
using System.Linq;
using System.Threading.Tasks;
using System;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.Target.IManager;
using SalesAndDistributionSystem.Services.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Common;
using DocumentFormat.OpenXml.Office2010.Excel;
using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using DocumentFormat.OpenXml.EMMA;
using ClosedXML.Excel;
using System.IO;
using System.Data;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Vml;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Authorization;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.AspNet.SignalR.Json;

namespace SalesAndDistributionSystem.Areas.Target.Controllers
{
    [Area("Target")]
    [Authorize]
    public class CustomerTargetController : Controller
    {
        private readonly ITargetManager _service;
        private readonly ICommonServices _comservice;
        private readonly ILogger<CustomerTargetController> _logger;

        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();

        public CustomerTargetController(ITargetManager service, ICommonServices comservice, ILogger<CustomerTargetController> logger, IConfiguration configuration)
        {
            _service = service;
            _comservice = comservice;
            _logger = logger;
            _configuration = configuration;
        }


        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");

        [AuthorizeCheck]
        public IActionResult CustomerNationalTarget(string id = "")
        {
            var model = new CUSOTMER_TARGET_MST();
            if (id != "")
            {
                model.MST_ID_ENCRYPTED = id;
            }
            return View(model);
        }

        [AuthorizeCheck]
        public IActionResult CustomerNationalTargetList()
        {
            return View();
        }
        [AuthorizeCheck]
        public IActionResult DeleteTarget()
        {
            return View();
        }

        [AuthorizeCheck]
        public IActionResult TargetProcess()
        {
            return View();
        }
        public IActionResult CustWiseRemainingBnsRpt()
        {
            return View();
        }

        
        public Task<string> GetTargetList()
        {
            return _service.LoadData(GetDbConnectionString(), User.GetComapanyId());
        }

        [HttpPost]
        public async Task<string> GetEditDataById([FromBody] CUSOTMER_TARGET_MST model)
        {
            var companyId = model.COMPANY_ID != 0 ? model.COMPANY_ID : User.GetComapanyId();
            if (!string.IsNullOrWhiteSpace(model.MST_ID_ENCRYPTED))
            {
                string id = _comservice.Decrypt(model.MST_ID_ENCRYPTED);
                return await _service.GetEditDataById(GetDbConnectionString(), companyId, id);
            }
            return null;
        }

        [HttpPost]
        public async Task<JsonResult> AddOrUpdate([FromBody] CUSOTMER_TARGET_MST model)
        {
            Result _result = new Result();
            //string result = "";

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

                    _result.Key = await _service.AddOrUpdate(GetDbConnectionString(), model);
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
        public async Task<string> ProcessExcel()
        {
            Result _result = new Result();
            try
            {

                var file = Request.Form.Files[0];
                var dt = await file.ToDataTable();
                var dtl = dt.ToList<CUSTOMER_TARGET_DTL>();

                dtl = await _service.GetTargetDetails(GetDbConnectionString(), dtl);
                _result.Status = "0";

                _result.Key = JsonConvert.SerializeObject(dtl);
            }
            catch(Exception exp)
            {
                _result.Status = "1";
                _result.Key = exp.Message;
            }
            return JsonConvert.SerializeObject(_result);
        }

        [HttpPost]
        public async Task<JsonResult> SaveFromExcel([FromBody] CUSOTMER_TARGET_MST model)
        {
            Result _result = new Result();
            //string result = "";

            if (!ModelState.IsValid)
            {
                var arr = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                return new JsonResult(arr);
            }
            else
            {
                List<CUSOTMER_TARGET_MST> targetList = new List<CUSOTMER_TARGET_MST>();

                var customers = model.TARGET_DTLs.Select(e => new { e.CUSTOMER_ID, e.CUSTOMER_CODE }).Distinct();
                foreach (var customer in customers)
                {
                    if (customer.CUSTOMER_ID == null || customer.CUSTOMER_ID == "00")
                    {
                        _result.Status = "Market Code not found";
                    }
                    var mst = new CUSOTMER_TARGET_MST
                    {
                        MST_ID = 0,
                        YEAR = model.YEAR,
                        MONTH_CODE = model.MONTH_CODE,
                        CUSTOMER_ID = customer.CUSTOMER_ID,
                        CUSTOMER_CODE = customer.CUSTOMER_CODE,
                        ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value,
                        ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"),
                        ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString(),
                        COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID,
                        TARGET_DTLs = model.TARGET_DTLs.Where(e => e.CUSTOMER_ID == customer.CUSTOMER_ID)
                                    .ToList()
                    };

                    targetList.Add(mst);
                }

                _result.Status = await _service.AddList(GetDbConnectionString(), targetList);
                return new JsonResult(_result);
            }
        }

        [AuthorizeCheck]
        public IActionResult TargetDelete()
        {
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> AddTargetDelete([FromBody] CUSOTMER_TARGET_MST model)
        {
            Result _result = new Result();
            //string result = "";

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
                    if (model.CUSTOMER_CODE == "admincandeletetarget")
                    {
                        if (model.MST_ID == 0)
                        {
                            model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                            model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                            model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                            model.COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
                        }
                        _result.Key = await _service.AddTargetDelete(GetDbConnectionString(), model);
                        _result.Status = "1";
                    }
                    else
                    {
                        _result.Status = "Please Enter Valid Security Password";
                    }
                }
                catch (Exception ex)
                {
                    _result.Status = ex.Message;
                }
            }
            return Json(_result);
        }
        public Task<string> GetCustWiseRemainingBnsRpt(string DATE_FROM, string DATE_TO, string CUSTOMER_CODE, int UNIT_ID)
        {
            return _service.GetCustWiseRemainingBnsRpt(GetDbConnectionString(), DATE_FROM, DATE_TO, CUSTOMER_CODE,  User.GetComapanyId(), UNIT_ID);
        }
        public Task<string> LockCustWiseRemainingBnsRpt(string DATE_FROM, string DATE_TO, string CUSTOMER_CODE, int UNIT_ID)
        {
            string ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
            string ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
            return _service.LockCustWiseRemainingBnsRpt(GetDbConnectionString(), DATE_FROM, DATE_TO, CUSTOMER_CODE, User.GetComapanyId(), UNIT_ID, ENTERED_BY, ENTERED_BY);
        }
        
        [HttpPost]
        public async Task<JsonResult> AddOrUpdateAdjustment([FromBody] AdjustmentMst model)
        {
            Result _result = new Result();
            //string result = "";

            if (!ModelState.IsValid)
            {
                
                var arr = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                _result.Key = "0";
                _result.Status = "Data Bind Faild, Contact with SIL";
            }
            else
            {
                try
                {
                    model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                    model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                    model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                    model.COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;

                    _result.Key = "1";
                    _result.Status = await _service.AddOrUpdateAdjustment(GetDbConnectionString(), model);
                }
                catch (Exception ex)
                {
                    _result.Key = "0";
                    _result.Status = ex.Message;
                }
            }

            return Json(_result);
        }

    }
}
