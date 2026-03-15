using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface ITerritoryManager
    {
        Task<string> LoadData(string db,int Company_Id);
        Task<string> AddOrUpdate(string db, Territory_Info model);
        Task<string> GenerateTerritoryCode(string db, string Company_Id);
        Task<string> GetSearchableTerritory(string db, int Company_Id, string territory) ;

    }
}
