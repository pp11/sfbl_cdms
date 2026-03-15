using SalesAndDistributionSystem.Domain.Models.TableModels.Security;
using SalesAndDistributionSystem.Domain.Models.ViewModels.Security;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Security
{
    public interface IRoleManager
    {
        string LoadData(string db, int companyId);

        Task<string> AddOrUpdate(string db, Role_Info model);
        Task<string> ActivateRole(string db, int id);
        Task<string> DeactivateRole(string db, int id);
        Task<string> GetSearchableRoles(string db, int companyId, string role_name);

        Task<string> RoleMenuConfigSelectionList(string db, int companyId, int roleId);
        Task<string> AddRoleMenuConfiguration(string db, List<Role_Menu_Configuration> model);
        Task<string> RoleUserConfigSelectionList(string db, int companyId, int userId);
        Task<string> RoleCentralUserConfigSelectionList(string db, int userId);
        Task<string> AddRoleUserConfiguration(string db, List<Role_User_Configuration> model);

    }
}
