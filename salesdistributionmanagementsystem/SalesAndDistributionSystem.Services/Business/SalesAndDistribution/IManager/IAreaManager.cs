using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface IAreaManager
    {
        Task<string> LoadData(string db, int Company_Id);
        Task<string> AddOrUpdate(string db, Area_Info model);
        Task<string> GenerateAreaCode(string db, string Company_Id);
        Task<string> GetSearchableArea(string db, int Company_Id, string area);

    }
}
