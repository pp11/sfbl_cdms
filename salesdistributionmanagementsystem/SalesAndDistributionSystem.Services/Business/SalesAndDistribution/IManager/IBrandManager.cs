using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface IBrandManager
    {
        Task<string> LoadData(string db,int Company_Id);
        Task<string> AddOrUpdate(string db, Brand_Info model);
        Task<string> GenerateBrandCode(string db, string Company_Id);
        Task<string> GetSearchableBrand(string db, int Company_Id, string brand) ;

    }
}
