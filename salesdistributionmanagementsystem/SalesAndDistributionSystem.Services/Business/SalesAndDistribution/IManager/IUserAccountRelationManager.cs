using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface IUserAccountRelationManager
    {
        Task<string> AddOrUpdate(string db, User_Account_Relation_Mst model);
        Task<string> LoadData_Master(string db, int Company_Id);
        Task<string> LoadData_MasterById(string db, int Id);
        Task<string> LoadData_DetailById(string db, int Id);
        Task<string> LoadData_DetailByMasterId(string db, int Id);
        Task<User_Account_Relation_Mst> LoadDetailData_ByMasterId_List(string db, int Id);
        Task<string> Existing_User_Load(string db, int Id);
        Task<string> Existing_Account_Load(string db, int Id);



    }
}
