using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager
{
    
    public interface ICustomerPriceInfoManager
    {
        Task<string> LoadData(string db, int Company_Id);
        Task<string> AddOrUpdate(string db, Customer_SKU_Price_Mst model);
        Task<string> LoadSKUPriceDtlData(string db, int Company_Id);
        Task<string> LoadSKUPriceMstData(string db, int Company_Id);
        Task<string> GetCustomerExistingSKUData(string db, int Company_Id, int Customer_Id);
        Task<string> LoadData_Master(string db, int Company_Id);
        Task<Customer_SKU_Price_Mst> LoadDetailData_ByMasterId_List(string db,int _Id);

        Task<string> LoadSKUPriceDtlDataRestrict(string db, string Company_Id,int cust_id, List<int> sku_id, string start_date,List<string> Cust_Ids);

    }
}
