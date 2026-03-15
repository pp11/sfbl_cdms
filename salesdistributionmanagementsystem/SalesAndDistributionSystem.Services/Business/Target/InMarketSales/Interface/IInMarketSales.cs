using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.Entities.Target;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Target.InMarketSales.Interface
{
    public interface IInMarketSales
    {
        Task<string> LoadData(string db, int Company_Id);
        Task<string> AddOrUpdate(string db, IN_MARKET_SALES_MST model);
        Task<string> AddOrUpdateXlsx(string db, IN_MARKET_SALES_MST model);
        Task<IN_MARKET_SALES_MST> GetEditDataById(string db, int mst_id);
        DataTable Exportxlsx(string db, IN_MARKET_SALES_MST model);

    }
}
