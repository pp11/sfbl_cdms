using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface IDivisionManager
    {
        Task<string> LoadData(Division_Info Company_Id);
        Task<string> LoadActiveDivisionData(Division_Info division_Info);

        Task<string> AddOrUpdate( Division_Info model);
        Task<string> GenerateDivisionCode(Division_Info division_Info);
        Task<string> GetSearchableDivision(Division_Info division_Info) ;

    }
}
