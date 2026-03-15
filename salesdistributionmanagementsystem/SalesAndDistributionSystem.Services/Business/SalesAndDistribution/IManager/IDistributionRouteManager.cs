using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager
{
    public interface IDistributionRouteManager
    {
        public Task<string> AddOrUpdate(string db, Distribution_Route_Info model);
        public Task<string> LoadData(string db, int companyId);
    }
}
