using SalesAndDistributionSystem.Domain.Models.TableModels.Security;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Security
{
    public interface IUserMenuConfigManager
    {
        Task<string> GetSearchableUsers(string db, int companyId, string user_name);
        Task<string> GetSearchableCentralUsers(string db, string user_Name);
        Task<string> UserMenuConfigSelectionList(string db, int companyId, int roleId);
        Task<string> AddUserMenuConfiguration(string db, List<Menu_User_Configuration> model);

    }
}
