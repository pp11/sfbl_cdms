using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface IPrimaryProductManager
    {
        Task<string> LoadData(string db,int Company_Id);
        Task<string> AddOrUpdate(string db, Primary_Product_Info model);
        Task<string> GeneratePrimaryProductCode(string db, string Company_Id);
        Task<string> GetSearchablePrimaryProduct(string db, int Company_Id, string primary_Product) ;

    }
}
