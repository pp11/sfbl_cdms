using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager
{
    public interface ISkuCommissionManager
    {
        Task<string> LoadData(string db, int company_id);
        Task<string> GetDetails(string db, int mst_id);
        Task<string> GetCustomer(string db, int COMPANY_ID, string UNIT_ID, string SKU_ID);
        Task<string> GetCustomerMarketwise(string db, int COMPANY_ID, string SKU_ID,string market_code, string customer_type, string customer_status);

        Task<string> Add(string db, CUSTOMER_WISE_SKU_COMM_ADD_MST model);
        Task Update(string db, CUSTOMER_WISE_SKU_COMM_ADD_DTL model);
        Task<string> GetMarketWiseCustomers(string db, string Market_Code);
        Task<string> GetComissionDoneCustomers(string db, string Sku_id);
        Task DeleteMst(string db, int id);
        Task DeleteDtl(string db, int id);
        Task Process(string db, int id);
    }
}
