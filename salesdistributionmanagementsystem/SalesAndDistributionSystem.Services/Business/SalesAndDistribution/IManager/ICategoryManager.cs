using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager
{
    public interface ICategoryManager
    {
        Task<string> LoadData(string db,int Company_Id);
        Task<string> AddOrUpdate(string db, Category_Info model);
        Task<string> GenerateCategoryCode(string db, string Company_Id);
        Task<string> GetSearchableCategory(string db, int Company_Id, string category) ;

    }
}
