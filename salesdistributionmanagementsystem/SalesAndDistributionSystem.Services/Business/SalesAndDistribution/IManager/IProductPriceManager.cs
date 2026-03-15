using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.Entities.Target;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface IProductPriceManager
    {
        Task<string> LoadData(string db,int Company_Id);      
        Task<string> BatchPriceLoadData(string db,int Company_Id);      
        Task<string> loadBatchWiseStock(string db, BATCH_PRICE_MST model);
        Task<string> loadBatchWiseStockTPUpdate(string db, BATCH_PRICE_MST model);

        Task<string> AddOrUpdate(string db, Product_Price_Info model);
        Task<string> AddOrUpdateBatchPrice(string db, BATCH_PRICE_MST model);
        Task<string> AddOrUpdateTP(string db, BATCH_PRICE_MST model);

        Task<string> GetSearchableProductPrice(string db, int Company_Id, string product_Price);
        Task<decimal> UnitWiseSkuPrice(string db, int Company_Id, int Unit_id, int sku_id, string sku_code);
        Task<decimal> SkuPrice(string db, int Company_Id, int Unit_id, int sku_id, string sku_code);
        Task<BATCH_PRICE_MST> GetEditDataById(string db, int mst_id);

    }
}
