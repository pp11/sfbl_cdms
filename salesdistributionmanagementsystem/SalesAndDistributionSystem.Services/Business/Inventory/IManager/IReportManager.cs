using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.ReportModels.Common.ProductReports;
using SalesAndDistributionSystem.Domain.Models.TableModels.Company;
using SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Inventory
{
    public interface IReportManager
    {
        string GetReportTemplate(DataSet dataSet, string headerTitle, string companyName, string IsLogoAppllicable, string IsCompanyApplicable, Company_Info company_Info, ProductReportParameters parameters);
        Task<ReportDataGenerator> GeneratePDF(string reportId, string db, string dbsecurity, int Company_Id, string IsLogoApplicable, string IsCompanyApplicable, ProductReportParameters reportParameters, int user_Id, string Security_db);
        Task<DataSet> GenerateExcel(string reportId, string db, string dbsecurity, int Company_Id, ProductReportParameters reportParameters);
        Task<string> ReceiveBatches(string db, ReportParams reportParams);
        Task<string> GetBatchesFromStock(string db, ReportParams reportParams);
        Task<string> GetTransferNotes(string db, ReportParams reportParams);
        Task<string> GetTransferNo(string db, ReportParams reportParams);
        Task<string> GetTransferRcvNo(string db, ReportParams reportParams);
        Task<string> GetChallans(string db, ReportParams reportParams);
        Task<string> GetGiftChallans(string db, ReportParams reportParams);
        Task<string> GetCustomers(string db, ReportParams reportParams);
        //distribution reports data
        Task<string> GetDistributionNumbers(string db, ReportParams reportParams);
        Task<string> GetDistributionDeliveryNumbers(string db, ReportParams reportParams);
        Task<DataTable> DistributionAndSkuWiseSalesReport(string db, ReportParams reportParams);
        Task<DataTable> MonthlyPrimarySalesReport(string db, ReportParams reportParams);
        Task<DataTable> MonthlyPrimarySalesUnitReport(string db, ReportParams reportParams);
        Task<string> GetDivitionToMarketRelation(string db);
        Task<DataTable> GetUnitNameById(string db, string unit_id, int company_id);
    }
}
