using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Inventory.Manager.IManager
{
   
    public interface IStockTransferManager
    {
        Task<string> LoadProductData(string db, int Company_Id,string Unit_Id);
        Task<string> LoadData(string db, int Company_Id, string unitId, string date_from, string date_to);
        Task<string> LoadReceivableTransfer(string db, int Company_Id, string unitId);
        Task<string> LoadRcvUnitStock(string db, int unitId, int skuId);
        Task<string> AddOrUpdate(string db, DEPOT_STOCK_TRANSFER_MST model);
        Task<string> LoadSKUPriceDtlData(string db, int Company_Id);
        Task<string> LoadSKUPriceMstData(string db, int Company_Id);
        Task<string> GetCustomerExistingSKUData(string db, int Company_Id, int Customer_Id);
        Task<string> LoadData_Master(string db, int Company_Id);
        Task<DEPOT_STOCK_TRANSFER_MST> LoadDetailData_ByMasterId_List(string db, int _Id);


    }
}
