using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Areas.SalesAndDistribution.Controllers;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.Entities.Target;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution;
using SalesAndDistributionSystem.Services.Business.Target.InMarketSales;
using SalesAndDistributionSystem.Services.Business.Target.InMarketSales.Interface;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Areas.Target.Controllers
{
    [Area("Target")]
    public class InMarketSalesController : Controller
    {

        private readonly IInMarketSales _service;
        private readonly ICommonServices _comservice;
        private readonly ILogger<SalesOrderController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();

        public InMarketSalesController(ICommonServices comservice, IInMarketSales service, ILogger<SalesOrderController> logger, IConfiguration configuration)
        {
            _comservice = comservice;
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }
        private string GetDbSecurityConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "Security");
        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");

        public IActionResult Index(string Id = "0")
        {
            IN_MARKET_SALES_MST _Mst = new IN_MARKET_SALES_MST();
            if (Id != "0" || Id != "")
            {
                _Mst.MST_ID_ENCRYPTED = Id;

            }
            _logger.LogInformation("In Market Sales  InsertOrEdit (Target/InMarketSales/index) Page Has been accessed By " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserName).Value.ToString() : "Unknown User" + " ( ID= " + User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value != null ? User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString() : "");
            return View(_Mst);
        }

        [HttpPost]
        public async Task<string> AddOrUpdate([FromBody] IN_MARKET_SALES_MST model)
        {
            if (!ModelState.IsValid)
            {
                var arr = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
            }
            Result result = new Result();

            if (model == null)
            {
                result.Status = "No Data Found To Insert Or Update!";
            }
            else
            {
                try
                {
                    model.UNIT_ID = model.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : model.UNIT_ID;
                    model.db_security = GetDbSecurityConnectionString();
                    model.db_sales = GetDbConnectionString();
                    model.UNIT_ID = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value);
                    Random rnd = new Random();
                    int num = rnd.Next();
                    model.COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
                    if (model.MST_ID == 0)
                    {
                        model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        if (model.IN_MARKET_SALES_DTL != null && model.IN_MARKET_SALES_DTL.Count > 0)
                        {
                            foreach (var item in model.IN_MARKET_SALES_DTL)
                            {
                                item.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                                item.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy");
                                item.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                            }
                        }
                    }
                    else
                    {
                        model.UPDATED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                        model.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                        model.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        foreach (var item in model.IN_MARKET_SALES_DTL)
                        {
                            item.UPDATED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                            item.UPDATED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                            item.UPDATED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                            item.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                            item.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                            item.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        }
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
        public async Task<string> AddOrUpdateXlsx([FromBody] IN_MARKET_SALES_MST model)
        {
            if (!ModelState.IsValid)
            {
                var arr = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
            }
            Result result = new Result();

            if (model == null)
            {
                result.Status = "No Data Found To Insert Or Update!";
            }
            else
            {
                try
                {
                    model.UNIT_ID = model.UNIT_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value) : model.UNIT_ID;
                    model.db_security = GetDbSecurityConnectionString();
                    model.db_sales = GetDbConnectionString();
                    model.UNIT_ID = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UnitId)?.Value);
                    model.COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : model.COMPANY_ID;
                    model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                    model.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                    model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                    if (model.IN_MARKET_SALES_DTL != null && model.IN_MARKET_SALES_DTL.Count > 0)
                    {
                        foreach (var item in model.IN_MARKET_SALES_DTL)
                        {
                            item.ENTERED_BY = User.Claims.FirstOrDefault(c => c.Type == ClaimsType.UserId)?.Value;
                            item.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy");
                            item.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                        }
                    }
                    return await _service.AddOrUpdateXlsx(GetDbConnectionString(), model);
                }
                catch (Exception ex)
                {
                    result.Status = ex.Message;
                }
            }
            return JsonSerializer.Serialize(result);
        }

        [HttpPost]
        public async Task<string> LoadData(int CompanyId)
        {
            int comp_id = CompanyId == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : CompanyId;
            return await _service.LoadData(GetDbConnectionString(), comp_id);
        }

        [HttpPost]
        public async Task<string> GetEditDataById([FromBody] Depot_Requisition_Raise_Mst mst)
        {
            IN_MARKET_SALES_MST in_market_sales_mst = new IN_MARKET_SALES_MST();
            if (mst.q != null && mst.q != "0" && mst.q != "")
            {
                int _Id = Convert.ToInt32(_comservice.Decrypt(mst.q));
                in_market_sales_mst = await _service.GetEditDataById(GetDbConnectionString(), _Id);
            }
            return JsonSerializer.Serialize(in_market_sales_mst);
        }

        [HttpGet]
        public IActionResult Export(string MONTH_CODE,string MARKET_CODE, string YEAR, string ENTRY_DATE )
        {
            IN_MARKET_SALES_MST model = new IN_MARKET_SALES_MST()
            {
                MONTH_CODE = MONTH_CODE,
                MARKET_CODE = MARKET_CODE,
                YEAR = YEAR,
                ENTRY_DATE = ENTRY_DATE
            };
            using (XLWorkbook wb = new XLWorkbook())
            {
                var dt = _service.Exportxlsx(GetDbConnectionString(), model);
                wb.Worksheets.Add(dt, "in_market_sales.xls");

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "in_market_sales.xls");
                }
            }
        }

    }
}
