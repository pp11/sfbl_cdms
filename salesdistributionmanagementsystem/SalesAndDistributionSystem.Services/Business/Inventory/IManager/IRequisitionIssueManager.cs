using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager
{
   
    public interface IRequisitionIssueManager
    {
        Task<string> LoadData(string db, int Company_Id, int unit_id);
        Task<string> AddOrUpdate(string db, DEPOT_REQUISITION_ISSUE_MST model);
        Task<string> LoadProductData(string db, int Company_Id,int Unit_Id);
        Task<string> LoadSKUPriceDtlData(string db, int Company_Id);
        Task<string> LoadSKUPriceMstData(string db, int Company_Id);
        Task<string> GetCustomerExistingSKUData(string db, int Company_Id, int Customer_Id);
        Task<string> LoadData_Master(string db, int Company_Id);
        Task<DEPOT_REQUISITION_ISSUE_MST> LoadDetailData_ByMasterId_List(string db, int _Id);
        Task<List<DEPOT_REQUISITION_ISSUE_BATCH>> LoadBatchDetailData_ByMasterId_List(string db, int _Id);
    }
}
