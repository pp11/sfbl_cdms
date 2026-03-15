using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Common.IManager
{
    public interface ICProductManager
    {
        Task<string> LoadData(string db, int Company_Id);

        Task<string> LoadProductData(string db, int Company_Id);
        Task<string> LoadGroupData(string db, int Company_Id);
        Task<string> LoadBrandData(string db, int Company_Id);
        Task<string> LoadCategoryData(string db, int Company_Id);
        Task<string> LoadBaseProductData(string db, int Company_Id);

        Task<string> GetProductDataFiltered(string db, int Company_Id, ProductFilterParameters parameters);

    }
}
