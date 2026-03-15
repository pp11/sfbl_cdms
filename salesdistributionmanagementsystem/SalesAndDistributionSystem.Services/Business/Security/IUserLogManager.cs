using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.TableModels.Security;
using SalesAndDistributionSystem.Domain.Models.TableModels.User;
using SalesAndDistributionSystem.Domain.Models.ViewModels.Security;
using SalesAndDistributionSystem.Domain.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Security
{
    public interface IUserLogManager
    {
        Task<string> LoadData(string db, string companyId);
        Task<string> Search(string db, int companyId, SearchModel model);
        Task<string> AddOrUpdate(string db, string activity_type, string activity_table, int CompanyId, int UnitId, int UserId, string terminal, string page_link, int tran_id, string dtl);
    }
}
