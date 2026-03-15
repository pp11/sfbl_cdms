using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Inventory.IManager
{
    public interface IStockAdjustmentManager
    {

        Task<string> GetBatchList(string db, int company_id, int unit_id, int sku_id);

        Task<string> GetSearchData(string db, int company_id, int unit_id);

        Task<string> InsertOrUpdate(string db, Stock_Adjustment model);
    }
}
