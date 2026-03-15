using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface IDriverManager
    {
        Task<string> LoadData(string db, int Company_Id);
        Task<string> AddOrUpdate(string db, Driver_Info model);
        Task<string> GetSearchableDriver(string db, int Company_Id, string driver);
    }
}
