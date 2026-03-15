using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Inventory.IManager
{
   
    public interface IStockTransferRcvManager
    {

        Task<DEPOT_STOCK_TRANS_RCV_MST> LoadDetailData_ByMasterId_List(string db, int _Id);
        Task<string> LoadData(string db, int Company_Id, string unitId, string date_from, string date_to);

        Task<string> AddOrUpdate(string db, DEPOT_STOCK_TRANS_RCV_MST model);
        Task<string> LoadDispatchedTransferData(string db, int Company_Id, string unitId);
        Task<string> LoadDispatchedTransferDtl(string db, DEPOT_STOCK_TRANSFER_MST model);
    }
}
