using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface IVehicleManager
    {
        Task<string> LoadData(string db, int Company_Id);
        Task<string> GetVehicleJsonList(string db, int Company_Id);
        Task<string> AddOrUpdate(string db, Vehicle_Info model);
        Task<string> GetSearchableVehicle(string db, int Company_Id, string vehicle);
        Task<string> GetMeasuingUnit(string db, int Company_Id, string measuring_Unit);
    }
}
