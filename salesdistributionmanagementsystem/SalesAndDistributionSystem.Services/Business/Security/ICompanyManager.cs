using SalesAndDistributionSystem.Domain.Models.TableModels.Company;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Company
{
    public interface ICompanyManager
    {
        Task<List<Company_Info>> GetCompanyList(string db);
        Task<Company_Info> GetCompanyById(string db, int id);
        Task<string> GetUnitInfo(string db, int companyId, int unitId);
        Task<string> AddOrUpdate(string db, Company_Info model);
        Task<string> GetCompanyJsonList(string db);
        Task<string> GetUnitJsonList(string db);
        Task<string> GetUnitByCompanyId(string db, int Company_Id);
        Task<string> AddOrUpdateUnit(string db, Company_Info model);
        Task<string> ActivateUnit(string db, int id);
        Task<string> DeactivateUnit(string db, int id);
        Task<string> GetCompanyAndItsUnit(string db, string companyId);

    }
}
