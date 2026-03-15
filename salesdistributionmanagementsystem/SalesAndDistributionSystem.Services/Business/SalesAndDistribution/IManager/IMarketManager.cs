using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface IMarketManager
    {
        Task<string> LoadData(string db,int Company_Id);
        Task<string> MarketDropDownDataData(string db,int Company_Id);
        Task<string> AddOrUpdate(string db, Market_Info model);
        Task<string> GenerateMarketCode(string db, string Company_Id);
        Task<string> GetSearchableMarket(string db, int Company_Id, string market) ;
        Task<string> LoadActiveMarkets(string db, int Company_Id);
        Task<string> GetDivitionToMarketRelation(string db/*, int ROW_NO*/);
    }
}
