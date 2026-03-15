using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface IProductTypeManager
    {
        Task<string> LoadData(string db,int Company_Id);
        Task<string> AddOrUpdate(string db, Product_Type_Info model);
        Task<string> GenerateProductTypeCode(string db, string Company_Id);
        Task<string> GetSearchableProductType(string db, int Company_Id, string product_Type) ;

    }
}
