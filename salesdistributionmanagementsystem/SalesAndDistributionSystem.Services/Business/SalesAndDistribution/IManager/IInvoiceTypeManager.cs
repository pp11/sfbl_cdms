using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface IInvoiceTypeManager
    {
        Task<string> LoadData(string db,int Company_Id);
        Task<string> AddOrUpdate(string db, Invoice_Type_Info model);
        Task<string> GenerateInvoiceTypeCode(string db, string Company_Id);
        Task<string> GetSearchableInvoiceType(string db, int Company_Id, string invoice_Type) ;

    }
}
