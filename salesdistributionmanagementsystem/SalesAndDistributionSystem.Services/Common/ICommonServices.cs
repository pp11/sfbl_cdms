using SalesAndDistributionSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Common
{
    public interface ICommonServices
    {
        QueryPattern AddQuery(string query, Dictionary<string, string> parametes);

        string DataTableToJSON(DataTable table);

        string DataSetToJSON(DataSet ds);

        DataRow GetDataRow(string conString, string query, Dictionary<string, string> param = null);
        DataSet GetDataSet(string connString, string query, Dictionary<string, string> param);
        DataTable GetDataTable(string conString, string query, Dictionary<string, string> param = null);
        T GetMaxNumNonParaQuery<T>(string conneString, string query);

        T GetMaximumNumber<T>(string conneString, string query, Dictionary<string, string> param);
        bool SaveChanges(string conString, List<QueryPattern> queryPatterns);
        Dictionary<string, string> AddParameter(string[] values);

        Task<DataTable> GetDataTableAsyn(string connString, string query, Dictionary<string, string> param = null);
        Task<T> ProcedureCallAsyn<T>(string connString, string query, Dictionary<string, string> param = null);
        Task<T> ProcedureCallOptimizeAsync<T>(string connString, string query, Dictionary<string, string> param = null);
        Task<T> PreExecuteProcedureCallAsyn<T>(string connString, string query, Dictionary<string, string> param = null);
        Task<DataSet> GetDataSetAsyn(string connString, string query, Dictionary<string, string> param);
        Task<DataRow> GetDataRowAsyn(string connString, string query, Dictionary<string, string> param = null);
        Task<bool> SaveChangesAsyn(string connString, List<QueryPattern> queryPatterns);
        Task<int> SaveChangesOptimizeAsync(string connString, List<QueryPattern> queryPatterns);
        Task<T> GetMaxNumNonParaQueryAsyn<T>(string connString, string query);
        Task<T> GetMaximumNumberAsyn<T>(string connString, string query, Dictionary<string, string> param);
        string ReturnExtension(string fileExtension);
        bool DataSave(string connString, string query, Dictionary<string, string> param = null);
        string Encrypt(string pstrText);
        string OptimizeEncrypt(string pstrText);

        string Decrypt(string pstrText);
        string DecryptNew(string pstrText, int id);

        List<T> ConvertDataTableToList<T>(DataTable dt);
        DataTable ListToDataTable<T>(List<T> items);
        public string GenerateRequisitionCode(string conString, string fn_name, string companyId, string unitId);
        public string GenerateRequisitionCode(string conString, string fn_name, string companyId, string unitId, string customerId);
        Task<DataSet> GetDataSetForMultiQueryAsync(string connString, string[] queries);
        Task<DataSet> GetDataSetForMultiQueryWithParamAsync(string connString, List<string> queries, List<Dictionary<string, string>> parametersList);
        string InsertQuery<T>(T obj, string tableName);
        string InsertQuerySkipProperties<T>(T obj, string tableName, params Expression<Func<T, object>>[] skipProperties);
        string InsertQueryIncludedProperties<T>(T obj, string tableName, params Expression<Func<T, object>>[] includedProperties);
        Dictionary<string, string> InsertParameter(object obj);
        Dictionary<string, string> InsertParameterSkipProperties<T>(T obj, params Expression<Func<T, object>>[] skipProperties);
        Dictionary<string, string> InsertParameterIncludedProperties<T>(T obj, params Expression<Func<T, object>>[] includedProperties);
        string UpdateQuery<T>(T obj, string tableName, params Expression<Func<T, object>>[] whereProperties);
        string UpdateQueryIncludedProperties<T>(T obj, string tableName, Expression<Func<T, object>>[] updateProperties, params Expression<Func<T, object>>[] whereProperties);
        public string UpdateQueryExludedProperties<T>(T obj, string tableName, Expression<Func<T, object>>[] excludedProperties, params Expression<Func<T, object>>[] whereProperties);
        Dictionary<string, string> UpdateParameterIncludedProperties<T>(T obj, Expression<Func<T, object>>[] updateProperties, params Expression<Func<T, object>>[] whereProperties);
        Dictionary<string, string> UpdateParameter<T>(T obj, params Expression<Func<T, object>>[] whereProperties);
        public Dictionary<string, string> UpdateParameterExcludedProperties<T>(T obj, Expression<Func<T, object>>[] excludeProperties, params Expression<Func<T, object>>[] whereProperties);
        string SelectQuery<T>(string tableName, params Expression<Func<T, bool>>[] whereProperties) where T : class;
        string SelectQueryComplex<T>(
            string tableName,
            Expression<Func<T, bool>>[] whereProperties = null,
            Expression<Func<T, object>>[] skipProperties = null,
            Expression<Func<T, object>>[] includedProperties = null) where T : class;
        Task<T> Ridwan<T>(string connString, List<QueryPattern> queryPatterns);
        List<T> ConvertToList<T>(DataTable dataTable) where T : class, new();

    }
}
