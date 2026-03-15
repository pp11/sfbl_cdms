using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.Common.IManager;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager;
using SalesAndDistributionSystem.Services.Business.Security;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Area.Common.Controllers
{
    [Area("Common")]
    public class CProductController : Controller
    {
        private readonly ICProductManager _service;
        private readonly ILogger<CProductController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ICommonServices _common;

        private readonly ServiceProvider Provider = new ServiceProvider();

        public CProductController(ICProductManager service, ILogger<CProductController> logger, IConfiguration configuration, ICommonServices common)
        {
            _service = service;
            _logger = logger;
            _configuration = configuration;
            _common = common;
        }
        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");
       
        public string GetCompany() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString();
       
        [HttpPost]
        public async Task<string> GetProductDataFiltered([FromBody] ProductFilterParameters paramsList)
        {
            int comp_id = paramsList == null || paramsList.COMPANY_ID == 0 ? Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value) : paramsList.COMPANY_ID;
            return await _service.GetProductDataFiltered(GetDbConnectionString(), comp_id, paramsList);

        }
       
    }
}
