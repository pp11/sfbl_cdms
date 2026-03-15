using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.Entities.Target;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Target.IManager
{
    public interface ITargetManager
    {
        Task<string> LoadData(string db, int Company_Id);
        Task<string> GetCustWiseRemainingBnsRpt(string db, string DATE_FROM, string DATE_TO, string CUSTOMER_CODE, int COMPANY_ID, int UNIT_ID);
        Task<string> LockCustWiseRemainingBnsRpt(string db, string DATE_FROM, string DATE_TO, string CUSTOMER_CODE, int COMPANY_ID, int UNIT_ID, string ENTERED_BY,string ENTERED_TERMINAL);
        Task<string> GetEditDataById(string db, int Company_Id, string id);
        Task<string> AddOrUpdate(string db, CUSOTMER_TARGET_MST model);
        Task<string> AddOrUpdateAdjustment(string db, AdjustmentMst model);
        Task<string> AddTargetDelete(string db, CUSOTMER_TARGET_MST model);
        Task<string> AddList(string db, List<CUSOTMER_TARGET_MST> model);
        Task<List<CUSTOMER_TARGET_DTL>> GetTargetDetails(string db, List<CUSTOMER_TARGET_DTL> model);
    }
}
