using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface IAdjustmentInfoManager
    {
        Task<string> InsertOrUpdate(string db, Adjustment_Info model);

        Task<string> SearchData(string db, int Company_Id);

    }
}
