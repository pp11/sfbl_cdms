using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager
{
    public interface IProductBonusManager
    {
        Task<string> LoadData(string db, int Company_Id);

        Task<string> LoadProductData(string db, int Company_Id);
        Task<string> LoadGroupData(string db, int Company_Id);
        Task<string> LoadBrandData(string db, int Company_Id);
        Task<string> LoadCategoryData(string db, int Company_Id);
        Task<string> LoadBaseProductData(string db, int Company_Id);
        int LoadNewBonusNo(string db);
        string LoadLocationTypes(string db, int Company_Id);

        Task<string> GetLocation_ByLocationType(string db, int Company_Id, string LocationType);
        Task<string> GetProductDataFiltered(string db, int Company_Id, ProductFilterParameters parameters);
        Task<string> AddOrUpdate(string db, int Company_Id, Product_Bonus_Mst bonus_Mst);
        Task<Product_Bonus_Mst> LoadEditDataByMasterId(string db, int Id);

    }
}
