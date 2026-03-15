using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager
{
    public interface IDashboardManager
    {
        //im//
        Task<DataTable> GetChartAreaData(string db);
        //im//
        Task<DataTable> GetChartBarData(string db);
        //im//
        Task<DataTable> GetChartPieData(string db);
        //im//
        Task<string> DashBoardData(string db);
        //im//
        Task<string> GetSbuData(string db);
        

    }
}
