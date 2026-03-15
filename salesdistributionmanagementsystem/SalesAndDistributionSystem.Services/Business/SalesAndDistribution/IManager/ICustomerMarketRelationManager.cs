using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface ICustomerMarketRelationManager
    {
        Task<string> AddOrUpdate(string db, Customer_Market_Mst model);
        Task<string> LoadData_Master(string db, int Company_Id);
        Task<string> LoadData_MasterById(string db, int Id);
        Task<string> LoadData_DetailById(string db, int Id);
        Task<string> LoadData_DetailByMasterId(string db, int Id);
        Task<Customer_Market_Mst> LoadDetailData_ByMasterId_List(string db, int Id);
        Task<string> Existing_Customer_Load(string db, int Id);
        Task<string> Existing_Market_Load(string db, int Id);



    }
}
