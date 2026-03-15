using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface IAreaTerritoryRelationManager
    {
        Task<string> AddOrUpdate(string db, Area_Territory_Mst model);
        Task<string> LoadData_Master(string db, int Company_Id);
        Task<string> LoadData_MasterById(string db, int Id);
        Task<string> LoadData_DetailById(string db, int Id);
        Task<string> LoadData_DetailByMasterId(string db, int Id);
        Task<Area_Territory_Mst> LoadDetailData_ByMasterId_List(string db, int Id);

        Task<string> Existing_Area_Load(string db, int Id);
        Task<string> Existing_Territory_Load(string db, int Id);

    }
}
