using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager
{
  
    public interface ICreditInfoManager
    {
        Task<string> LoadData(string db, int Company_Id);
        Task<string> AddOrUpdate(string db, Credit_Info model);
        Task<string> LoadCreditInfoData(string db, int Company_Id);

        Task<string> GetCustomerExistingCreditInfoData(string db, int Company_Id, int Customer_Id);
        Task<string> LoadData_Master(string db, int Company_Id);
        Task<Credit_Info> LoadCreditInfoDataList(string db, int _Id);

    }
}
