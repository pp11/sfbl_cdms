using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface IRegionManager
    {
        Task<string> LoadData(string db,int Company_Id);
        Task<string> GetSearchableRegion(string db, int Company_Id, string region);
        Task<string> AddOrUpdate(string db, Region_Info model);
        Task<string> GenerateRegionCode(string db, string Company_Id);
    }
}
