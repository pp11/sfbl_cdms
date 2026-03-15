using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface ISalesReturnManager
    {
        Task<string> InvoicesLoad(string db, string Date_from,string Date_to, string CompanyId,string UnitId,string IsDistributor,string CUSTOMER_CODE);

        Task<string> FullReturn(string db,SalesReturnParameters model);
        Task<string> LoadSalesReturns(OrderSKUFilterParameters model);
        Task<string> LoadSalesReturnMst(string db, string Company_Id, string Invoice_Unit_id);
        Task<string> LoadReturnDetailsData(string db, string Mst_Id);
        Task<string> ProcessPartialReturn(string db, Process_data model);
        Task<string> SalesReturnPartial(string db, List<Process_data> model);
        Task<string> DeleteReturn(string db, Return_Mst model);
    }
}
