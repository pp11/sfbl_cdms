using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface ISalesOrderManager
    {
        Task<string> AddOrUpdate(string db, Order_Mst model);
        Task<string> LoadCustomer(string db, Order_Mst model);
        Task<string> LoadProducts(string db, string Company_Id,string COMPANY_ID);
        Task<string> LoadProductsSpecific(string db, string Company_Id, string COMPANY_ID);
        Task<string> LoadProductsSpecificType(string db, string Company_Id, string type);

        string LoadProductType(string db, string Customer_Id);
        Task<string> LoadData(Order_Mst model);
        Task<string> LoadMonitoringData(Order_Mst model);
        Task<string> LoadDataProcessed(Order_Mst model);
        Task<decimal> LoadProductPrice(string db, string Company_Id, int SKU_ID, string Order_Type, int Customer_Id);
        Task<string> GetEditDatabyId(string db, int id,string dist_status);
        Task<string> Get_Customer_Balance(string db, string Company_Id, string Unit_Id, string Customer_Id);

        Task<string> LoadFilteredData(OrderSKUFilterParameters model);
        Task<string> LoadFilteredMonitoringData(OrderSKUFilterParameters model);
        Task<string> LoadFilteredProcessedData(OrderSKUFilterParameters model);
        Task<string> Process_Order(string db, Order_Mst model);
        Task<string> Delete_Processed_Order(string db, Order_Mst model);
        Task<string> Save_Final_Order(string db, Order_Mst model);
        Task<string> Save_Final_Post_Order(string db, Order_Mst model);
        Task<string> Save_Post_Confirmation_Order(string db, Order_Mst model);
        Task<string> Cancel_Post_Confirmation_Order(string db, Order_Mst model);
        Task<string> GetProductDataFiltered(string db, int Company_Id, ProductFilterParameters parameters, int Unit_Id);
        Task<string> Delete_Order_full(string db, int ORDER_MST_ID);
        Task<string> Replacement_Master(string db, string Customer_Id,string Unit_Id, string order_mst_id);
        Task<string> Replacement_Dtl(string db, string Claim_NO, int ORDER_UNIT_ID);
        Task<string> Refurbishment_Master(string db, string Customer_Id, string Unit_Id);
        Task<string> Refurbishment_Dtl(string db, string Claim_NO);
        Task<string> LoadProductPerfactOrderQty(string db, Order_Mst model);

    }
}
