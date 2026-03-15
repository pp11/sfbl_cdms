using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface IGiftItemManager
    {
        Task<string> LoadData(string db,int Company_Id);
        Task<string> AddOrUpdate(string db, Gift_Item_Info model);
        Task<string> GetSearchableGift_Item(string db, int Company_Id, string pack_Size) ;

    }
}
