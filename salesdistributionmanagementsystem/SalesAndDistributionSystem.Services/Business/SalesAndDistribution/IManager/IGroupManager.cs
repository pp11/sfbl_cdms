using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface IGroupManager
    {
        Task<string> LoadData(string db,int Company_Id);
        Task<string> AddOrUpdate(string db, Group_Info model);
        Task<string> GenerateGroupCode(string db, string Company_Id);
        Task<string> GetSearchableGroup(string db, int Company_Id, string grouop) ;

    }
}
