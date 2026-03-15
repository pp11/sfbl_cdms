using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface IProductManager
    {
        Task<string> LoadData(string db,int Company_Id);
        Task<string> LoadSkuCodeData(string db,int Company_Id);
        Task<string> LoadPackSizeData(string db,int Company_Id);
        Task<string> LoadProductPrimaryData(string db, int Company_Id);

        Task<string> LoadProductdropdownData(string db,int Company_Id);
        Task<string> LoadDataFromView(string db,int Company_Id);
        Task<string> LoadDropDownDataFromView(string db,int Company_Id);
        Task<string> LoadFilteredData(Price_Dtl_Param Price_Dt_Info);
        Task<string> AddOrUpdate(string db, Product_Info model);
        Task<string> GetSearchableProduct(string db, int Company_Id, string product) ;
        Task<string> GenerateProductCode(string db, string Company_Id);
        Task<DataTable> LoadDataTable(string db, int Company_Id);
        Task<string> LoadProductByProductCode(string db, int Company_Id, string product_code);
        Task<string> LoadProductSegmentInfo(string db);
        Task<string> AddOSkuDepoRelation(string db, SKU_DEPOT_RELATION model);
        Task<string> DeleteSkuDepoRelation(string db, string sku_depot_id);
        Task<string> LoadSKU_DEPOTData(string db, int Company_Id);
    }
}
