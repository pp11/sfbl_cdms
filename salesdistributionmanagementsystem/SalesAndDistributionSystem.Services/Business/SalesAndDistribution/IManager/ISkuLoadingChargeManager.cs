using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface ISkuLoadingChargeManager
    {
        Task<string> AddOrUpdate(string db, SkuLoadingCharge model);
        Task<string> SearchData(string db, int Company_Id);
    }



}
