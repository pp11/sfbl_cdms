using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface ISupplierManager
    {
        Task<string> LoadData(string db, int Company_Id);
        Task<string> AddOrUpdate(string db, Supplier_Info model);
        Task<string> GetSearchableSupplier(string db, int Company_Id, string supplier);
        Task<string> GetSupplierByType(string db, int Company_Id, string supplierType);
    }
}
