using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager
{
    public interface IFgReceiveFromProductionManager
    {
        Task<string> LoadUnchekedData(string db, string db_security, int Company_Id, int Unit_Id);
        
        Task<string> LoadData(string db,string db_security, int Company_Id, int Unit_Id,string date_from, string date_to);
        Task<string> AddOrUpdate(string db, Fg_Reciving_From_Production model);
        Task<string> LoadFGTransferData(string db, int Company_Id, int UnitNo);
        public  Task<string> LoadDetailDataById(string db, int Id);
        public  Task<string> GetLastMrp(string db, string sku_code, int company_id, int unit_id);
            
    }
}
