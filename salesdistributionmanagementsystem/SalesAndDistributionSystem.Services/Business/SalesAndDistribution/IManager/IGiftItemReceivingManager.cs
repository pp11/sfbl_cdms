using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface IGiftItemReceivingManager
    {
        Task<string> LoadData(string db, int Company_Id, int unit_id);
        Task<string> AddOrUpdate(string db, Gift_Item_Receiving model);

    }
}
