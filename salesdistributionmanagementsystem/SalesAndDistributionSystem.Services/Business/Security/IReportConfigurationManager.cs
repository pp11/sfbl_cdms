using SalesAndDistributionSystem.Domain.Models.Entities.Security;
using SalesAndDistributionSystem.Domain.Models.ReportModels.Common.ProductReports;
using SalesAndDistributionSystem.Domain.Models.TableModels.Security;
using SalesAndDistributionSystem.Domain.Models.ViewModels.Security;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Security
{
    public interface IReportConfigurationManager
    {
        Task<string> LoadData(string db, int companyId);
        Task<string> GetReturnInvoiceNumbers(string db, ReportParams reportParams);
        Task<string> AddOrUpdate(string db,Report_Configuration model);
        Task<string> ActivateReport(string db, int id);
        Task<string> DeactivateReport(string db, int id);

        //Role Report Configuration-----------------------------------------------------------
        Task<string> GetSearchableRoles(string db, int companyId, string role_name);

        Task<string> RoleReportConfigSelectionList(string db, int companyId, int roleId);
        Task<string> AddRoleReportConfiguration(string db, List<Role_Report_Configuration> model);
      
        
        //Report User Configuration-----------------------------
        Task<string> GetSearchableUsers(string db, int companyId, string user_name);
        Task<string> GetSearchableCentralUsers(string db, string user_Name);
        Task<string> UserReportConfigSelectionList(string db, int companyId, int roleId);
        Task<string> AddUserReportConfiguration(string db, List<Report_User_Configuration> model);

        //Report Permission----------------------------------------
        Task<List<ReportPermission>> LoadReportPermissionData(string db, int Company_Id, int User_Id);
        ReportPermission IsReportPermitted(int reportId, List<ReportPermission> permissions);

    }
}
