using SalesAndDistributionSystem.Domain.Models.ReportModels.Common.ProductReports;
using SalesAndDistributionSystem.Domain.Models.TableModels.Company;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Company
{
    public interface INotificationManager
    {
        Task<string> NotificationLoad(string db, Notification model);
        Task<string> LoadData(string db, ReportParams model);
        Task<string> UpdateNotificationViewStatus(string db, Notification model);
        Task<string> UpdateNotificationViewStatusByUser(string db, Notification model);

        Task<string> AddOrUpdateNotification(string db, Notification model);
        Task<string> AddOrderNotification(string db,string db_sales, int NotificationPolicyId, string NotificationBody, int CompanyId, int UnitId);
        Task<string> Notification_Permitted_Users(string db, int Policy_Id, int Unit_Id, int Company_Id);
    }
}
