using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface IMeasuringUnitManager
    {
        Task<string> LoadData(string db,int Company_Id);
        Task<string> AddOrUpdate(string db, Measuring_Unit_Info model);
        Task<string> GetSearchableMeasuringUnit(string db, int Company_Id, string measuring_Unit) ;

    }
}
