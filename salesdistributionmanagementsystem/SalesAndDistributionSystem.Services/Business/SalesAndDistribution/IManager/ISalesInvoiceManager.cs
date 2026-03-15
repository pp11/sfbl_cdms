using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface ISalesInvoiceManager
    {
        Task<string> LoadPostableData(OrderSKUFilterParameters model);
        Task<string> AllPostOrder(string db, int Company_Id, int Unit_id, List<string> Order_id, string db_security, int enter_by, string terminal);

        Task<string> SinglePostOrder(string db, int Company_Id, int Unit_id, string Order_id, string db_security, int enter_by, string terminal);
        Task<string> CancelOrder(string db, string Company_Id, string Unit_id, string Order_id);
        Task<string> Load_PreInvoice_Data(string db, string OrderId, string OrderDate, string CustomerId, string CustomerCode, string OrderUnitId, string InvoiceUnitId, string Company_Id);
        Task<string> UpdateOrderRevisedQty(string db, Order_Mst _Mst);
        Task<string> Load_Sales_Invoice_Mst_data(OrderSKUFilterParameters model);
        Task<string> LoadInvoiceDetailsData(string db, string Mst_Id);
        Task<string> LoadSearchableInvoice(string db, string Unit_Id, string Company_Id, string Invoice_No);
        Task<string> LoadSearchableInvoiceByCustomer(string db, string Unit_Id, string Company_Id, string Invoice_No, string CUSTOMER_ID);

        Task<string> DeleteInvoice(string db, string InvoiceNo, int Company_Id, int Unit_Id, string db_security, int enter_by, string terminal);
        Task<string> LoadSearchableInvoiceInDate(string db, string Unit_Id, string Company_Id, string Invoice_No, string invoice_date, string q);
        Task<string> LoadSearchableOrderInDate(string db, string Unit_Id, string Company_Id, string Invoice_No, string invoice_date, string q);
        Task<string> LoadMaxMinInvoiceInDate(string db, string Unit_Id, string Company_Id, string invoice_date);
        Task<string> LoadMaxMinOrderInDate(string db, string Unit_Id, string Company_Id, string invoice_date);
    }
}
