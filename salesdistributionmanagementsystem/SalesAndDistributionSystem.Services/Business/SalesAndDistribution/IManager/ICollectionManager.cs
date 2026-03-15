using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface ICollectionManager
    {
        Task<string> LoadData(string db, string Company_Id,string Unit_Id);
        Task<string> AddOrUpdate(string db, Collection_Mst model);
        Task<string> GetEditDataById(string db,int Id, int unit_id);
        Task<string> LoadBranch(string db);
        Task<string> LoadCollectionMode(string db);
        Task<string> LoadCustomerDaywiseBalance(string db, string customerId,string unit_id);
        Task<string> LoadCustomerDaywiseBalanceV2(string db, string customerId, string unit_id);
        Task<string> Update_Approval(string db, Collection_Mst model);

    }
}
