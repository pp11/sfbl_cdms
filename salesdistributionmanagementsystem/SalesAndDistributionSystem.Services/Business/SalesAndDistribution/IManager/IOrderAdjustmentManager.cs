using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface IOrderAdjustmentManager
    {
        Task<string> LoadDataByOrderId(string db, int Company_Id,int Order_Id);

        Task<string> LoadData(string db, int Company_Id);
        Task<string> LoadDebitCreditAdjData(string db, int Company_Id);
        Task<string> AddOrUpdate(string db, Order_Adjustment model);
        Task<string> AddOrUpdateDebitCreditAdj(string db, DEBIT_CREDIT_ADJ model);
        Task<string> PostDebitCreditAdj(string db, DEBIT_CREDIT_ADJ model);

        
    }
}
