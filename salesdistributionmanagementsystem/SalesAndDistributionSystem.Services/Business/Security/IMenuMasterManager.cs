using SalesAndDistributionSystem.Domain.Models.TableModels.Security;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Security
{
    public interface IMenuMasterManager
    {
        string LoadData(string db, int companyId);
        Task<string> AddOrUpdate(string db,Menu_Configuration model);
        Task<string> ActivateMenu(string db, int id);
        Task<string> DeactivateMenu(string db, int id);
        Task<string> DeleteMenu(string db,int id);

        //im//
        Task<string> GetAllCodeAndDropdownListData(string db, string companyId, string unitId);
    }
}
