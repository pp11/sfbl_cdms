using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface ISalesOrderRationingManager
    {
        Task<string> AddOrUpdate(string db, List<SalesOrderRationing> model);
        Task<string> LoadRationingData(OrderSKUFilterParameters model);
    }
}
