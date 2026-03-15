using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface IDivisionRegionRelationManager
    {
        Task<string> AddOrUpdate(string db, Division_Region_Mst model);
        Task<string> LoadData_Master(string db, int Company_Id);
        Task<string> LoadData_MasterById(string db, int Id);
        Task<string> LoadData_DetailById(string db, int Id);
        Task<string> LoadData_DetailByMasterId(string db, int Id);
        Task<Division_Region_Mst> LoadDetailData_ByMasterId_List(string db, int Id);
        Task<string> Existing_Division_Load(string db, int Id);
        Task<string> Existing_Region_Load(string db, int Id);



    }
}
