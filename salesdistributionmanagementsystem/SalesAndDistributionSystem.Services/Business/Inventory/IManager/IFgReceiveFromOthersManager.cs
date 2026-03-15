using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager
{
    public interface IFgReceiveFromOthersManager
    {
        Task<string> LoadUnchekedData(string db, string db_security, int Company_Id, int Unit_Id, string DATE_FROM = "01/01/0001", string DATE_TO = "01/01/4023");
        Task<string> GetApprovedList(string db, string db_security, int Company_Id, int Unit_Id);
        Task<string> LoadData(string db, string db_security, int Company_Id, int Unit_Id, string date_from, string date_to);
        Task<string> GetBatches(string db, int Company_Id, int sku_id, string RECEIVE_TYPE);
        Task<string> AddOrUpdate(string db, Fg_Reciving_From_Others model);
        Task<string> LoadFGTransferData(string db, int Company_Id, int UnitNo);
        Task<string> Get_Refurbishment_SKU(string db, string Ref_code);
        Task<string> Get_Refurbishment_SKU_ALL(string db);
        Task<string> LoadDetailDataById(string db, int Id);
        Task<string> UpdateStatusToCancel(string db, string id);

    }
}
