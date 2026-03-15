using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.TableModels.Company;
using SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public interface IReportManager
    {
        string GetReportTemplate(DataSet dataSet, string headerTitle, string companyName, string IsLogoAppllicable, string IsCompanyApplicable, Domain.Models.TableModels.Company.Company_Info company_Info);
        Task<ReportDataGenerator> GeneratePDF(string reportId, string db, int Company_Id, string IsLogoApplicable, string IsCompanyApplicable, ProductReportParameters reportParameters, int user_Id, string Security_db);
        Task<DataSet> GenerateExcel(string reportId, string db, int Company_Id, ProductReportParameters reportParameters);

    }
}
