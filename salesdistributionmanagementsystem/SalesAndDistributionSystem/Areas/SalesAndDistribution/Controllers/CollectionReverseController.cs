using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Areas.SalesAndDistribution.Controllers
{
    [Area("SalesAndDistribution")]
    [Authorize]
    public class CollectionReverseController : Controller
    {
        private readonly ICollectionReverseManager _service;

        private readonly ILogger<CollectionController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();

        public CollectionReverseController(ICollectionReverseManager service, 
            ILogger<CollectionController> logger, IConfiguration configuration)
        {
            this._service = service;
            this._logger = logger;
            this._configuration = configuration;
        }

        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");

        [AuthorizeCheck]
        public IActionResult InsertOrEdit()
        {
            return View();
        }

        public async Task<string> GetTransactions(string batch_no)
        {
            return await _service.GetTransactions(GetDbConnectionString(), batch_no);
        }

        [HttpPost]
        public async Task<Result> Save([FromBody]COLLECTION_REVERSE model)
        {
            if(!ModelState.IsValid)
            {
                var err = ModelState.ErrorCount;
            }

            var result = new Result();
            try
            {
                model.REVERSE_DATE = DateTime.Now.ToString("dd/MM/yyyy");
                model.COMPANY_ID = User.GetComapanyId();
                model.ENTERED_BY = User.GetUserId();
                model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                model.ENTERED_TERMINAL = HttpContext.Connection.RemoteIpAddress.ToString();
                result.Status = await _service.Save(GetDbConnectionString(), model);
            }
            catch (Exception ex) 
            {
                result.Status = ex.Message;
            }
            return result;
        }

        public IActionResult List()
        {
            return View();
        }

        [HttpPost]
        public async Task<string> LoadData()
        {
            string comp_id = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value;
            string unit_id = User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value;
            return await _service.LoadData(GetDbConnectionString(), comp_id, unit_id);

        }
    }
}
