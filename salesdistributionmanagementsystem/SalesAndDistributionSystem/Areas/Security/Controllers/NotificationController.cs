using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Reporting.WebForms;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Models.ReportModels.Common.ProductReports;
using SalesAndDistributionSystem.Domain.Models.TableModels.Company;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.Company;
using SalesAndDistributionSystem.Services.Business.Security;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Areas.Security.Company.Controllers
{
    [Area("Security")]
    public class NotificationController : Controller
    {
        private readonly INotificationManager _service;
        private readonly ILogger<NotificationController> _logger;
        private readonly IConfiguration _configuration;
        public NotificationController(INotificationManager service, ILogger<NotificationController> logger, IConfiguration configuration)
        {
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }
 
        private string GetDbConnectionString() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.DbSpecifier).Value.ToString();

        public string GetCompany() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString();
        public string GetCompanyName() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyName).Value.ToString();
        public string GetUser() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UserId).Value.ToString();

        public string GetUnit() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value.ToString();
        public string GetUnitName() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitName).Value.ToString();


       
        [HttpGet]
        public async Task<string> NotificationLoad()
        {
            Notification model = new Notification();
            model.USER_ID = Convert.ToInt32(GetUser());
            var data = await _service.NotificationLoad(GetDbConnectionString(), model);
            return data;
        }

        [HttpGet]
        public async Task<string> LoadData( ReportParams model)
        {
            model.USER_NAME = GetUser(); // UserId
            var data = await _service.LoadData(GetDbConnectionString(), model);
            return data;
        }



        [HttpPost]
        public async Task<string> Notification_Permitted_Users([FromBody] Notification model)
        {
            model.COMPANY_ID = model.COMPANY_ID == 0? Convert.ToInt32(GetCompany()):model.COMPANY_ID;
            model.UNIT_ID = model.UNIT_ID == 0 ? Convert.ToInt32(GetUnit()) : model.UNIT_ID;

            return await _service.Notification_Permitted_Users(GetDbConnectionString(), model.NOTIFICATION_POLICY_ID,model.UNIT_ID,model.COMPANY_ID);
        }
        [HttpGet]
        public async Task<string> UpdateNotificationViewStatus(int ID)
        {
            Notification model = new Notification();
            model.ID = ID;
            return await _service.UpdateNotificationViewStatus(GetDbConnectionString(), model);
        }
        [HttpPost]
        public async Task<string> UpdateNotificationViewStatusByUser([FromBody] Notification model)
        {
            model = new Notification();
            model.USER_ID =Convert.ToInt32(GetUser());
            return await _service.UpdateNotificationViewStatusByUser(GetDbConnectionString(), model);
        }

        [HttpPost]
        public async Task<JsonResult> AddOrUpdate([FromBody] Notification model)
        {
            string result;

            if (model == null)
            {
                result = "No Changes Found!";
            }
            else
            {
                try
                {
                    model.COMPANY_ID = Convert.ToInt32(GetCompany());
                    model.UNIT_ID = Convert.ToInt32(GetUnit());
                    result = await _service.AddOrUpdateNotification(GetDbConnectionString(), model);

                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }

            }


            return Json(result);


        }

        public IActionResult List()
        {
            _logger.LogInformation("DistributionDeliveryList (Inventory/DistributionDelivery/DeliveryList) Page Has been accessed By " + User.GetUserName() + " ( ID= " + User.GetUserId() + " )");
            return View();
        }


    }
}
