using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace SalesAndDistributionSystem.Services.Business.Inventory.Manager
{
    public interface IBatchFreezingManager
    {
        Task<string> GetBatchList(string db, int company_id, int unit_id, int sku_id);

        Task<string> GetSkuList(string db, int company_id,int unit_id);

        Task<string> GetDtlData(string db, int mst_id);

        Task<string> GetMstData(string db, int company_id, int unit_id);

        Task<string> InsertOrUpdate(string db, BatchFreezingMst model);

    }
}
