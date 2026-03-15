using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface IRepClaimManager
    {
        Task<string> AddOrUpdate(string db, Order_Mst model);
        Task<string> LoadCustomer(string db, string Company_Id);
        Task<string> LoadProducts(string db, string Company_Id);
        Task<string> LoadData(Order_Mst model);
        Task<decimal> LoadProductPrice(string db, string Company_Id, int SKU_ID, string Order_Type, int Customer_Id);
        Task<string> GetEditDatabyId(string db, int id);
        Task<string> Get_Customer_Balance(string db, string Company_Id, string Unit_Id, string Customer_Id);

        Task<string> LoadFilteredData(OrderSKUFilterParameters model);
    }
}
