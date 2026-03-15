using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface IDistributorProductRelation
    {
        Task<string> AddOrUpdate(string db, Distributor_Product_Mst model);
        Task<string> LoadData_Master(string db, int Company_Id);
        Task<string> LoadData_MasterById(string db, int Id);
        Task<string> LoadData_DetailById(string db, int Id);
        Task<string> LoadData_DetailByMasterId(string db, int Id);
        Task<Distributor_Product_Mst> LoadDetailData_ByMasterId_List(string db, int Id);

    }
}
