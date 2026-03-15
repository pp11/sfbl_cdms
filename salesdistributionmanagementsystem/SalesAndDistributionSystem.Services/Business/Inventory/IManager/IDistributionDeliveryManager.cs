using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using SalesAndDistributionSystem.Domain.Models.ViewModels.Inventory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Inventory.IManager
{
    public interface IDistributionDeliveryManager
    {
        Task<string> GetDistributionRoutes(string db, int companyId, int unitId);
        Task<string> GetPendingInvoices(string db, int companyId, int unitId);
        Task<string> LoadData(string db, DistributionFilterParameter receive_info);
        Task<string> GetNonConfirmDeliveryList(string db, DistributionFilterParameter receive_info);
        Task<string> GetProductsByInvoice(string db, int companyId, int unitId, string invoiceNo);
        Task<string> GetEditDataById(string db, int companyId, string id);
        Task<string> GetGiftByInvoice(string db, int companyId, int unitId, string invoiceNo);
        Task<string> GetInvoiceProducts(string db, int companyId, int unitId, List<string> invoiceNos);
        Task<string> GetInvoiceGifts(string db, int companyId, int unitId, List<string> invoiceNos);
        Task<string> CustomerByRoute(string db, int companyId, int unitId, string routeId);
        Task<string> GetProductBatches(string db, string mst_id);
        Task<string> AddOrUpdate(string db, Depot_Customer_Dist_Mst model);

    }
}
