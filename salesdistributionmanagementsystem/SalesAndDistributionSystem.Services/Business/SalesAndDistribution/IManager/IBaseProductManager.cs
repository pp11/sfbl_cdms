using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface IBaseProductManager
    {
        Task<string> LoadData(string db,int Company_Id);
        Task<string> AddOrUpdate(string db, Base_Product_Info model);
        Task<string> GenerateBase_ProductCode(string db, string Company_Id);
        Task<string> GetSearchableBase_Product(string db, int Company_Id, string base_Product) ;

    }
}
