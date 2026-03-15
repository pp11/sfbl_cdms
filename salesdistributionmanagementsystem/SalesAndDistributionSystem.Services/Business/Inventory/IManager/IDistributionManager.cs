using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Inventory.IManager
{

    public interface IDistributionManager
    {

        Task<DEPOT_DISPATCH_MST> LoadDistributionDetailData_ByMasterId(string db, int _Id);
        Task<string> LoadData(string db, int companyId);
        Task<string> LoadDistributionReqData(string db, int companyId,int unit_id);
        Task<string> LoadDistributionProductDataByReqId(string db, int CompanyId, int ReqId);
        Task<string> LoadShipperDtlData(string db, int CompanyId,int Mst_Id);
        Task<string> LoadDispatchBatchData(string db, int CompanyId, int Mst_Id);
        Task<string> GetPendingRequisition(string db, int companyId, int unitId);
        Task<string> GetPendingStock(string db, int companyId, int unitId);
        Task<string> GetProductsByRequisition(string db, int companyId, int unitId, string RequisitionNo);
        Task<string> GetProductsByStock(string db, int companyId, int unitId, string RequisitionNo);
        Task<string> AddOrUpdate(string db, DEPOT_DISPATCH_MST model);
    }
}
