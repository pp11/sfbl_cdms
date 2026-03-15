using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Inventory.IManager
{
    public interface IRefurbishmentManager
    {
        Task<string> GetDistList(string db,string unitId);
        Task<string> GetManualDistList(string db, string searchKey);
        Task<string> GetManualProductList(string db, string companyId, string unitId, string searchKey);
        Task<string> GetDistListWhileEdit(string db,string mstId);
        Task<string> GetProductsByChallan(string db, string challanNo);
        Task<string> LoadDtlByMstId(string db, string mstId);
        Task<Result> AddOrUpdate(string db,RefurbishmentReceivingMst model);
        Task<Result> Approval(string db, RefurbishmentReceivingMst model);
        Task<string> GetMstList(string db, string companyId, string unitId, string fromDate, string toDate);
        Task<string> GetClaims(string db,string companyId,string unitId);
        Task<string> GetProductsByClaimNo(string db, string claimNo);
        Task<Result> AddOrUpdateFinalization(string db, RefurbishmentFinalizeMaster model);
        Task<string> GetFinalizationMstList(string db, string companyId, string unitId, string fromDate, string toDate);
        Task<string> GetClaimsWhileEdit(string db, string mstId);
        Task<string> loadFinalizationDtlByMstId(string db,string companyId, string unitId, string mstId);
        Task<Result> FinalizationApproval(string db, RefurbishmentFinalizeMaster model);
        Task<string> GetManualProductsWithStock(string db,string companyId, string unitId, string searchKey);


    }
}
