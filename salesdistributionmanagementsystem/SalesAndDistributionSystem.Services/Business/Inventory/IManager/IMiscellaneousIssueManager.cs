using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager
{
    public interface IMiscellaneousIssueManager
    {
        Task<string> LoadProductData(string db, int Company_Id,int Unit_id);

        //Task<string> LoadSKUBatchData(string db, int Company_Id, int Unit_Id, int Sku_Id);
        Task<string> AddOrUpdate(string db, Miscellaneous_Issue_Mst model);
        Task<string> LoadData(string db, int Company_Id,string date_from,string date_to);

        Task<string> LoadData_Master(string db, int Company_Id);
        Task<Miscellaneous_Issue_Mst> LoadDetailData_ByMasterId_List(string db, int _Id);
    }
}
