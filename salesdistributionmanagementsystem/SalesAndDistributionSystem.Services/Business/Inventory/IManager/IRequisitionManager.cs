using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager
{
   
    public interface IRequisitionManager
    {
        Task<string> LoadData(string db, int Company_Id, int unit_id);
        Task<string> LoadDataForIssue(string db, int Company_Id,int unit_id);
        Task<string> LoadDataBetweenDate(string db, int Company_Id,string Unit_id,string date_form, string date_to, string MST_ID);
        Task<string> LoadProductWeightData(string db, int Company_Id, int Sku_Id, int ReqQty);
        Task<string> LoadProductData(string db, int Company_Id, int Unit_Id);
        Task<string> GetProductDataFiltered(string db, int Company_Id, ProductFilterParameters parameters, int Unit_Id);
        Task<string> AddOrUpdate(string db, Depot_Requisition_Raise_Mst model);
        Task<string> LoadSKUPriceDtlData(string db, int Company_Id);
        Task<string> LoadSKUPriceMstData(string db, int Company_Id);
        Task<string> GetCustomerExistingSKUData(string db, int Company_Id, int Customer_Id);
        Task<string> LoadData_Master(string db, int Company_Id);
        Task<Depot_Requisition_Raise_Mst> LoadDetailData_ByMasterId_List(string db, int _Id);
        Task<int> LoadIssuePendingRequisitionCount(string db, int Company_Id, int Unit_Id);
    }
}
