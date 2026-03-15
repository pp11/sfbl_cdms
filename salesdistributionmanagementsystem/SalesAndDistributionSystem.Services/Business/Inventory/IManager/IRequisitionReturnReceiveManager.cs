using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Inventory.Manager.IManager
{
   
    public interface IRequisitionReturnReceiveManager
    {
        Task<string> LoadData(string db, int Company_Id,string unitId);
        Task<string> AddOrUpdate(string db, DEPOT_REQUISITION_RET_RCV_MST model);
        Task<string> LoadSKUPriceDtlData(string db, int Company_Id);
        Task<string> LoadSKUPriceMstData(string db, int Company_Id);
        Task<string> GetCustomerExistingSKUData(string db, int Company_Id, int Customer_Id);
        Task<string> LoadData_Master(string db, int Company_Id);
        Task<DEPOT_REQUISITION_RET_RCV_MST> LoadDetailData_ByMasterId_List(string db, int _Id);
    }
}
