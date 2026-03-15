using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager
{
    public interface ICollectionReverseManager
    {
        Task<string> GetTransactions(string db,string batch_no);
        Task<string> Save(string db, COLLECTION_REVERSE model);
        Task<string> LoadData(string db, string Company_Id, string Unit_Id);

    }
}
