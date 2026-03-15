using SalesAndDistributionSystem.Domain.Models.TableModels.Security;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Security
{
    public interface IMenuCategoryManager
    {
        string LoadData(string db,int companyId);
        Task<string> AddOrUpdate(string db, Module_Info model);
        Task<string> ActivateMenuCategory(string db, int id);
        Task<string> DeactivateMenuCategory(string db, int id);
        Task<string> DeleteMenuCategory(string db, int id);

    }
}
