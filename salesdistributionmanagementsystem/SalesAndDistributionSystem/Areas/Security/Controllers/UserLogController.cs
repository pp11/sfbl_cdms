using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.Security;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Areas.Security.Controllers
{
    [Area("Security")]
    public class UserLogController : Controller
    {
        private readonly IUserLogManager _service;
        private readonly ICommonServices _comservice;
        private readonly ILogger<UserLogController> _logger;

        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();

        public UserLogController(IUserLogManager _service,
            ILogger<UserLogController> _logger,
            ICommonServices _comservice,
            IConfiguration _configuration)
        {
            this._service = _service;
            this._comservice = _comservice;
            this._logger = _logger;
            this._configuration = _configuration;
        }

        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "Security");

        [HttpGet]
        public Task<string> LoadData()
        {
            return _service.LoadData(GetDbConnectionString(), User.GetComapanyId().ToString());
        }

        [HttpPost]
        public async Task<string> Search([FromBody]SearchModel model)
        {
            if(!ModelState.IsValid)
            {
                var arr = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                return JsonConvert.SerializeObject(arr);
            }
            return await _service.Search(GetDbConnectionString(), User.GetComapanyId(), model);
        }

        [HttpPost]
        public async Task<string> ActivityLogData(SearchModel model)
        {
            model.USER_ID = User.GetUserId();
            model.FROM_DATE ??= DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            model.TO_DATE ??= DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            return await _service.Search(GetDbConnectionString(), User.GetComapanyId(), model);
        }

        public IActionResult ViewLogs()
        {
            return View();
        }

        public IActionResult ActivityLog()
        {
            return View();
        }
    }
}
