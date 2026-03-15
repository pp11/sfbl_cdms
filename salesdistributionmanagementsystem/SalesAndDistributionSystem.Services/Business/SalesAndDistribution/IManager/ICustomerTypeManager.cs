using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface ICustomerTypeManager
    {
        Task<string> LoadData(string db,int Company_Id);
        Task<string> AddOrUpdate(string db, Customer_Type_Info model);
        Task<string> GenerateCustomerTypeCode(string db, string Company_Id);
        Task<string> GetSearchableCustomerType(string db, int Company_Id, string customer_type) ;

    }
}
