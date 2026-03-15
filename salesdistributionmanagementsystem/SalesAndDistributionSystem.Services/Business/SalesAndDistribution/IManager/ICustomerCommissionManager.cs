using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface ICustomerCommissionManager
    {
        Task<string> AddOrUpdate(string db, CustomerCommission model);
        Task<string> SearchData(string db, int Company_Id);
    }
}
