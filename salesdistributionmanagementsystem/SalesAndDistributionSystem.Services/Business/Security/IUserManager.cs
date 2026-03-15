using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.TableModels.Security;
using SalesAndDistributionSystem.Domain.Models.TableModels.User;
using SalesAndDistributionSystem.Domain.Models.ViewModels.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.User
{
    public interface IUserManager
    {
        Auth GetUserByEmailAndCompany(string db, string Email, int CompanyId);

        Auth GetUserByEmail(string db, string Email);
        Auth GetUserByUserId(string db, int userId);
        int GetCompanyIdByUserId(string db, int userId);

        bool IsValidUser(string db, string Email, string password, int CompanyId, string HashPass);
        string GetUserByCompanyJsonList(string db, string Company);
        Task<string> AddOrUpdate(string db, User_Info model, string ServerPath, string url);
        string GetUsers(string db);
        string GetEmployeesWithoutAccount(string db, int CompanyId);
        DataTable GetUsersByCompanyDataTable(string db, int CompanyId);
        string GetUsersByCompany(string db, int CompanyId);

        Task<string> LoadSearchableDefaultPages(string db, int companyId, string defaultpage);
        Task<string> LoadDefaultPages(string db, int companyId);
        Task<string> AddOrUpdateDefaultPage(string db, Default_Page model);
        Task<string> UpdateUserPassword(string db, PasswordChangeModel changeModel);
        User_Info IsVerified(string db, string UniquKey);
        Task<string> ForgetPasswordVerify(string db, PasswordChangeModel changeModel);
        List<User_Info> GetPasswords(string db);
        //im//
        Task<string> GetAllCodeAndDropdownListData(string db, string companyId, string unitId);
        //im//
        Task<string> ActivateUser(string db, int id);
        //im//
        Task<string> DeactivateUser(string db, int id);
    }
}
