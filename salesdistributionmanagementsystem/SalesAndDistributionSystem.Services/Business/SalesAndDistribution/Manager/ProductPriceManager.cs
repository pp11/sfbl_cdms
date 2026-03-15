using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using NUnit.Framework.Constraints;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.Entities.Target;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public class ProductPriceManager : IProductPriceManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public ProductPriceManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        //-------------------Query Part ---------------------------------------------------

        string LoadData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY pp.Price_ID ASC) AS ROW_NO
                       ,pp.COMPANY_ID
                       ,pp.EMPLOYEE_PRICE
                       ,pp.GROSS_PROFIT
                       ,pp.MRP
                       ,TO_CHAR(pp.PRICE_EFFECT_DATE, 'DD/MM/YYYY') PRICE_EFFECT_DATE
                       ,TO_CHAR(pp.PRICE_ENTRY_DATE, 'DD/MM/YYYY') PRICE_ENTRY_DATE
                       ,pp.PRICE_ID
                       ,pp.SKU_CODE
                       ,pp.SKU_ID
                       ,pp.SPECIAL_PRICE
                       ,pp.SUPPLIMENTARY_TAX
                       ,pp.UNIT_ID
                       ,pp.UNIT_TP
                       ,pp.UNIT_VAT
                       ,P.SKU_NAME
                       From Product_Price_Info pp, Product_info p  Where p.SKU_CODE = pp.SKU_CODE AND  pp.COMPANY_ID = :param1";
        string loadBatchWiseStock_Query() => @"SELECT B.COMPANY_ID, B.UNIT_ID, U.UNIT_NAME, B.SKU_CODE, B.SKU_ID,B.UNIT_TP,B.BATCH_ID,B.BATCH_NO, NVL(B.MRP,0) MRP
FROM FG_RECEIVING_FROM_PRODUCTION B 
LEFT OUTER JOIN STL_ERP_SCS.COMPANY_INFO U ON U.COMPANY_ID= B.COMPANY_ID AND U.UNIT_ID=B.UNIT_ID
WHERE B.COMPANY_ID= :param1 AND B.SKU_ID= :param2 AND B.BATCH_PRICE_REVIEW_STATUS='Yes'
UNION
SELECT B.COMPANY_ID, B.UNIT_ID, U.UNIT_NAME, B.SKU_CODE, B.SKU_ID,B.UNIT_TP,B.BATCH_ID,B.BATCH_NO, NVL(B.MRP,0) MRP
FROM FG_RECEIVING_FROM_OTHERS B 
LEFT OUTER JOIN STL_ERP_SCS.COMPANY_INFO U ON U.COMPANY_ID= B.COMPANY_ID AND U.UNIT_ID=B.UNIT_ID
WHERE B.COMPANY_ID= :param1 AND B.SKU_ID= :param2 AND B.BATCH_PRICE_REVIEW_STATUS='Yes'
";
        string LoadSearchableData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY Price_ID ASC) AS ROW_NO
                       ,COMPANY_ID
                       ,EMPLOYEE_PRICE
                       ,GROSS_PROFIT
                       ,MRP
                       ,TO_CHAR(PRICE_EFFECT_DATE, 'DD/MM/YYYY') PRICE_EFFECT_DATE
                       ,TO_CHAR(PRICE_ENTRY_DATE, 'DD/MM/YYYY') PRICE_ENTRY_DATE
                       ,PRICE_ID
                       ,SKU_CODE
                       ,SKU_ID
                       ,SPECIAL_PRICE
                       ,SUPPLIMENTARY_TAX
                       ,UNIT_ID
                       ,UNIT_TP
                       ,UNIT_VAT
                       From Product_Price_Info Where COMPANY_ID = :param1 AND upper(SKU_CODE) Like '%' || upper(:param2) || '%'";

        string AddOrUpdate_AddQuery() => @"INSERT INTO Product_Price_Info 
                                         (COMPANY_ID, EMPLOYEE_PRICE, GROSS_PROFIT, MRP,PRICE_EFFECT_DATE, PRICE_ENTRY_DATE,PRICE_ID,SKU_CODE,SKU_ID,SUPPLIMENTARY_TAX,UNIT_ID,SPECIAL_PRICE,UNIT_TP,UNIT_VAT,  Entered_By, Entered_Date,ENTERED_TERMINAL ) 
                                         VALUES ( :param1, :param2, :param3, :param4,TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM') ,TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM'), :param7, :param8, :param9,:param10 ,:param11,:param12,:param13,:param14,:param15,TO_DATE(:param16, 'DD/MM/YYYY HH:MI:SS AM'),:param17)";
        string AddOrUpdateBatchPriceMst_Query() => @"INSERT INTO BATCH_PRICE_MST  (COMPANY_ID,ENTERED_BY,ENTERED_DATE,ENTERED_TERMINAL,ENTRY_DATE,MST_ID,SKU_CODE,SKU_ID,UNIT_ID) 
                                         VALUES ( :param1, :param2, TO_DATE(:param3, 'DD/MM/YYYY HH:MI:SS AM'), :param4,TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM') ,:param6, :param7, :param8, :param9)";
        //string AddOrUpdateBatchPriceDtl_Query() => @"INSERT INTO BATCH_PRICE_DTL  (BATCH_ID,BATCH_NO,COMPANY_ID,DTL_ID,EMPLOYEE_PRICE,ENTERED_BY,ENTERED_DATE,ENTERED_TERMINAL,GROSS_PROFIT,MRP,MST_ID,SKU_CODE,SKU_ID,SPECIAL_PRICE,SUPPLIMENTARY_TAX,UNIT_ID,UNIT_TP,UNIT_VAT) VALUES ( :param1, :param2, :param3, :param4,TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM') ,TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM'), :param7, :param8, :param9,:param10 ,:param11,:param12,:param13,:param14,:param15,TO_DATE(:param16, 'DD/MM/YYYY HH:MI:SS AM'),:param17,:param18)";
        string AddOrUpdateBatchPriceDtl_Query() => @"INSERT INTO BATCH_PRICE_DTL  (BATCH_ID,BATCH_NO,COMPANY_ID,DTL_ID,ENTERED_BY,ENTERED_DATE,ENTERED_TERMINAL,MRP,MST_ID,SKU_CODE,SKU_ID,UNIT_ID,UNIT_TP) VALUES ( :param1, :param2, :param3, :param4,:param5,TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM') , :param7, :param8, :param9,:param10 ,:param11,:param12,:param13)";
        string UpdateBatchPriceMst_Query() => @"UPDATE BATCH_PRICE_MST SET ENTRY_DATE=TO_DATE(:param1, 'DD/MM/YYYY HH:MI:SS AM'),UPDATED_BY=:param2,UPDATED_DATE=TO_DATE(:param3, 'DD/MM/YYYY HH:MI:SS AM'),UPDATED_TERMINAL=:param4 WHERE MST_ID =:param5";
        string UpdateBatchPriceDtl_Query() => @"UPDATE BATCH_PRICE_DTL SET MRP=:param1,UNIT_TP=:param2, UPDATED_BY=:param3, UPDATED_DATE=TO_DATE(:param4, 'DD/MM/YYYY HH:MI:SS AM'),UPDATED_TERMINAL=:param5 WHERE DTL_ID =:param6";
        string AddOrUpdate_UpdateQuery() => @"UPDATE Product_Price_Info SET 
                            COMPANY_ID = :param2
                           ,EMPLOYEE_PRICE = :param3
                           ,GROSS_PROFIT = :param4
                           ,MRP = :param5
                           ,PRICE_EFFECT_DATE =  TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM')
                           ,PRICE_ENTRY_DATE = TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM')
                           ,SKU_CODE = :param8
                           ,SKU_ID = :param9
                           ,SPECIAL_PRICE = :param10
                           ,SUPPLIMENTARY_TAX = :param11
                           ,UNIT_ID = :param12
                           ,UNIT_TP = :param13
                           ,UNIT_VAT = :param14
                           ,UPDATED_BY = :param15
                           ,UPDATED_DATE = TO_DATE(:param16, 'DD/MM/YYYY HH:MI:SS AM')
                           ,UPDATED_TERMINAL = :param17
                           Where PRICE_ID = :param1";
        string GetNewProductPrice_Info_IdQuery() => "SELECT NVL(MAX(PRICE_ID),0) + 1 PRICE_ID  FROM PRODUCT_PRICE_INFO";
        string BATCH_PRICE_MST_IDENTITY_QUERY() => "SELECT NVL(MAX(MST_ID),0) + 1 MST_ID  FROM BATCH_PRICE_MST";
        string BATCH_PRICE_DTL_IDENTITY_QUERY() => "SELECT NVL(MAX(DTL_ID),0) + 1 DTL_ID  FROM BATCH_PRICE_DTL";
        string Get_LastProductPrice_Ino() => "SELECT  PRICE_ID  FROM PRODUCT_PRICE_INFO Where  PRICE_ID = (SELECT   NVL(MAX(PRICE_ID),0) PRICE_ID From PRODUCT_PRICE_INFO where COMPANY_ID = :param1 )";
        string LOAD_MST_DATA_BY_ID_QUERY() => "SELECT COMPANY_ID,ENTERED_BY,ENTERED_DATE,ENTERED_TERMINAL,TO_CHAR(ENTRY_DATE,'DD/MM/YYYY') ENTRY_DATE,MST_ID,SKU_CODE,SKU_ID,UNIT_ID,UPDATED_BY,UPDATED_DATE,UPDATED_TERMINAL FROM BATCH_PRICE_MST M WHERE M.MST_ID=:param1\r\n";
        string LOAD_DTL_DATA_BY_ID_QUERY() => @"SELECT D.*,C.UNIT_NAME  FROM BATCH_PRICE_DTL D
LEFT OUTER JOIN STL_ERP_SCS.COMPANY_INFO C ON C.COMPANY_ID=D.COMPANY_ID AND D.UNIT_ID=C.UNIT_ID
WHERE D.MST_ID=:param1";

        //---------- Method Execution Part ------------------------------------------------
        public async Task<string> AddOrUpdate(string db, Product_Price_Info model)
        {
            if (model == null)
            {
                return "No data provided to insert!!!!";

            }
            else
            {
                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                try
                {

                    if (model.PRICE_ID == 0)
                    {

                        model.PRICE_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewProductPrice_Info_IdQuery(), _commonServices.AddParameter(new string[] { }));
                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(),
                            _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(),
                               model.EMPLOYEE_PRICE.ToString(), model.GROSS_PROFIT.ToString(), model.MRP.ToString(),
                               model.PRICE_EFFECT_DATE, model.PRICE_ENTRY_DATE,model.PRICE_ID.ToString(),model.SKU_CODE.ToString(), model.SKU_ID.ToString(),
                               model.SUPPLIMENTARY_TAX.ToString(),model.UNIT_ID.ToString(),model.SPECIAL_PRICE.ToString(),
                               model.UNIT_TP.ToString(),model.UNIT_VAT.ToString(),model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL })));

                    }
                    else
                    {

                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] {
                               model.PRICE_ID.ToString(),model.COMPANY_ID.ToString(),
                               model.EMPLOYEE_PRICE.ToString(), model.GROSS_PROFIT.ToString(), model.MRP.ToString(),
                               model.PRICE_EFFECT_DATE, model.PRICE_ENTRY_DATE,model.SKU_CODE.ToString(), model.SKU_ID.ToString(),
                               model.SPECIAL_PRICE.ToString(),model.SUPPLIMENTARY_TAX.ToString(),model.UNIT_ID.ToString(),
                               model.UNIT_TP.ToString(),model.UNIT_VAT.ToString(),model.UPDATED_BY.ToString(), model.UPDATED_DATE,model.UPDATED_TERMINAL })));

                    }

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    return "1";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
        public async Task<string> LoadData(string db, int Company_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));
        public async Task<string> GetSearchableProductPrice(string db, int Company_Id, string ProductPrice) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSearchableData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), ProductPrice })));

        public async Task<decimal> UnitWiseSkuPrice(string db, int Company_Id, int Unit_id, int sku_id, string sku_code)
        {
            //FN_UNIT_WISE_SKU_PRICE(vSKU_ID NUMBER, vSKU_CODE VARCHAR2, vCOMPANY_ID NUMBER, vUNIT_ID NUMBER)
            var query = @"SELECT FN_UNIT_WISE_SKU_PRICE(:param1, :param2, :param3, :param4) price FROM DUAL";

            var dt = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] { sku_id.ToString(), sku_code, Company_Id.ToString(), Unit_id.ToString() }));

            return Convert.ToDecimal(dt.Rows[0]["price"]);
        }

        public async Task<decimal> SkuPrice(string db, int Company_Id, int Unit_id, int sku_id, string sku_code)
        {
            //FN_UNIT_WISE_SKU_PRICE(vSKU_ID NUMBER, vSKU_CODE VARCHAR2, vCOMPANY_ID NUMBER, vUNIT_ID NUMBER)
            var query = @"SELECT FN_SKU_PRICE(:param1, :param2, :param3, :param4) price FROM DUAL";

            var dt = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] { sku_id.ToString(), sku_code, Company_Id.ToString(), Unit_id.ToString() }));

            return Convert.ToDecimal(dt.Rows[0]["price"]);
        }

        public async Task<string> loadBatchWiseStock(string db, BATCH_PRICE_MST model)
        {
            string query = loadBatchWiseStock_Query();
            if (model.UNIT_ID_MULTIPLE !=null && !model.UNIT_ID_MULTIPLE.Contains("ALL"))
            {
                StringBuilder inClause = new StringBuilder(" AND B.UNIT_ID IN (");
                var arr = String.Join(",", model.UNIT_ID_MULTIPLE);
                inClause.AppendLine(arr);
                inClause.AppendLine(")");
                query = query + inClause;
            }
            query = query + "ORDER BY UNIT_ID, BATCH_ID";
            return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), model.SKU_ID })));
        }
        public async Task<string> loadBatchWiseStockTPUpdate(string db, BATCH_PRICE_MST model)
        {
            string query = @"SELECT SKU_ID,
         SKU_CODE,
         BATCH_ID,
         BATCH_NO,
         NVL(MAX (UNIT_TP),0) UNIT_TP,
         NVL(MAX (MRP),0) MRP
    FROM BATCH_WISE_STOCK B
   WHERE B.COMPANY_ID = :param1 AND B.SKU_ID = :param2 AND B.BATCH_NO = :param3
GROUP BY SKU_ID,
         SKU_CODE,
         BATCH_ID,
         BATCH_NO";
            return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), model.SKU_ID, model.BATCH_NO.Trim() })));
        }
        public async Task<string> AddOrUpdateBatchPrice(string db, BATCH_PRICE_MST model)
        {
            if (model == null)
            {
                return "No data provided to insert!!!!";
            }
            else
            {
                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                try
                {
                    if (model.MST_ID == 0)
                    {
                        int dtlID = 0;
                        model.MST_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), BATCH_PRICE_MST_IDENTITY_QUERY(), _commonServices.AddParameter(new string[] { }));
                        dtlID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), BATCH_PRICE_DTL_IDENTITY_QUERY(), _commonServices.AddParameter(new string[] { }));
                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdateBatchPriceMst_Query(), _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), model.ENTERED_BY, model.ENTERED_DATE, model.ENTERED_TERMINAL, model.ENTRY_DATE, model.MST_ID.ToString(), model.SKU_CODE, model.SKU_ID, model.UNIT_ID.ToString() })));

                        foreach (var item in model.BATCH_PRICE_DTL_LIST)
                        {
                            listOfQuery.Add(_commonServices.AddQuery(AddOrUpdateBatchPriceDtl_Query(), _commonServices.AddParameter(new string[] { item.BATCH_ID.ToString(), item.BATCH_NO, model.COMPANY_ID.ToString(), dtlID.ToString(), model.ENTERED_BY, model.ENTERED_DATE, model.ENTERED_TERMINAL, item.MRP.ToString(), model.MST_ID.ToString(), model.SKU_CODE, model.SKU_ID.ToString(), item.UNIT_ID.ToString(), item.UNIT_TP.ToString() })));
                            ++dtlID;
                        }
                    }
                    else
                    {
                        listOfQuery.Add(_commonServices.AddQuery(UpdateBatchPriceMst_Query(), _commonServices.AddParameter(new string[] { model.ENTRY_DATE, model.UPDATED_BY, model.UPDATED_DATE, model.UPDATED_TERMINAL, model.MST_ID.ToString() })));

                        foreach (var item in model.BATCH_PRICE_DTL_LIST)
                        {
                            listOfQuery.Add(_commonServices.AddQuery(UpdateBatchPriceDtl_Query(), _commonServices.AddParameter(new string[] { item.MRP.ToString(), item.UNIT_TP.ToString(), model.UPDATED_BY, model.UPDATED_DATE, model.UPDATED_TERMINAL, item.DTL_ID.ToString() })));

                        }

                    }

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    return "1";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
        public async Task<string> AddOrUpdateTP(string db, BATCH_PRICE_MST model)
        {
            if (model == null)
            {
                return "No data provided to insert!!!!";
            }
            else
            {
                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                try
                {
                    foreach (var item in model.BATCH_PRICE_DTL_LIST)
                    {

                        listOfQuery.Add(_commonServices.AddQuery(@"INSERT INTO TP_UPDATE_HISTORY (
    SKU_CODE,
    BATCH_NO,
    UNIT_TP,
    MRP,
    OLD_UNIT_TP,
    OLD_MRP,
    ENTERED_BY,
    ENTERED_DATE,
    ENTERED_TERMINAL
) VALUES (
    :param1,
    :param2,
    :param3,
    :param4,
    :param5,
    :param6,
    :param7,
    TO_DATE(:param8, 'DD/MM/YYYY HH:MI:SS AM'),
    :param9
)",
                           _commonServices.AddParameter(new string[] { model.SKU_CODE, model.BATCH_NO.Trim(), item.UNIT_TP.ToString(), item.MRP.ToString(),"","",model.ENTERED_BY,model.ENTERED_DATE, model.ENTERED_TERMINAL  })));

                        listOfQuery.Add(_commonServices.AddQuery(@"UPDATE BATCH_WISE_STOCK B SET UNIT_TP =:param1 , MRP=:param2 
WHERE BATCH_NO=:param3 AND SKU_CODE=:param4",
                            _commonServices.AddParameter(new string[] { item.UNIT_TP.ToString(), item.MRP.ToString(), model.BATCH_NO.Trim(), model.SKU_CODE })));
                        listOfQuery.Add(_commonServices.AddQuery(@"UPDATE FG_RECEIVING_FROM_production B
SET UNIT_TP =:param1 , MRP=:param2 
WHERE BATCH_NO=:param3 AND SKU_CODE=:param4",
                               _commonServices.AddParameter(new string[] { item.UNIT_TP.ToString(), item.MRP.ToString(), model.BATCH_NO.Trim(), model.SKU_CODE })));
                        listOfQuery.Add(_commonServices.AddQuery(@"

UPDATE FG_RECEIVING_FROM_OTHERS  B
SET UNIT_TP =:param1 , MRP=:param2 
WHERE BATCH_NO=:param3 AND SKU_CODE=:param4",
                                                   _commonServices.AddParameter(new string[] { item.UNIT_TP.ToString(), item.MRP.ToString(), model.BATCH_NO.Trim(), model.SKU_CODE })));
                    }

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    return "1";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public async Task<string> BatchPriceLoadData(string db, int Company_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), @"SELECT MST_ID,
       TO_CHAR(ENTRY_DATE,'DD/MM/YYYY HH:MI AM') ENTRY_DATE,
       SKU_ID,
       SKU_CODE,
       FN_SKU_NAME(COMPANY_ID,SKU_ID) SKU_NAME,
       COMPANY_ID,
       UNIT_ID,
       ENTERED_BY,
       TO_CHAR(ENTERED_DATE,'DD/MM/YYYY HH:MI AM') ENTERED_DATE,
       ENTERED_TERMINAL,
       UPDATED_BY,
      TO_CHAR(UPDATED_DATE,'DD/MM/YYYY HH:MI AM') UPDATED_DATE,
       UPDATED_TERMINAL FROM BATCH_PRICE_MST WHERE COMPANY_ID =:param1", _commonServices.AddParameter(new string[] { Company_Id.ToString() })));
        public async Task<BATCH_PRICE_MST> GetEditDataById(string db, int mst_id)
        {
            BATCH_PRICE_MST batch_price_mst = new BATCH_PRICE_MST();
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LOAD_MST_DATA_BY_ID_QUERY(), _commonServices.AddParameter(new string[] { mst_id.ToString() }));
            if (data.Rows.Count > 0)
            {
                batch_price_mst.COMPANY_ID = Convert.ToInt32(data.Rows[0]["COMPANY_ID"]);
                batch_price_mst.ENTRY_DATE = data.Rows[0]["ENTRY_DATE"].ToString();
                batch_price_mst.MST_ID = Convert.ToInt32(data.Rows[0]["MST_ID"]);
                batch_price_mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[0]["MST_ID"].ToString());
                batch_price_mst.SKU_CODE = (data.Rows[0]["SKU_CODE"]).ToString();
                batch_price_mst.SKU_ID = data.Rows[0]["SKU_ID"].ToString();
                batch_price_mst.UNIT_ID = Convert.ToInt32(data.Rows[0]["UNIT_ID"]);

                DataTable dataTable_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LOAD_DTL_DATA_BY_ID_QUERY(), _commonServices.AddParameter(new string[] { mst_id.ToString() }));
                batch_price_mst.BATCH_PRICE_DTL_LIST = new List<BATCH_PRICE_DTL>();
                for (int i = 0; i < dataTable_detail.Rows.Count; i++)
                {
                    BATCH_PRICE_DTL batch_price_dtl = new BATCH_PRICE_DTL();
                    batch_price_dtl.DTL_ID = Convert.ToInt32(dataTable_detail.Rows[i]["DTL_ID"]);
                    batch_price_dtl.UNIT_TP = Convert.ToDecimal(dataTable_detail.Rows[i]["UNIT_TP"]);
                    batch_price_dtl.MRP = Convert.ToDecimal(dataTable_detail.Rows[i]["MRP"]);
                    batch_price_dtl.BATCH_ID = Convert.ToInt32(dataTable_detail.Rows[i]["BATCH_ID"]);
                    batch_price_dtl.BATCH_NO = dataTable_detail.Rows[i]["BATCH_NO"].ToString();
                    batch_price_dtl.UNIT_ID = Convert.ToInt32(dataTable_detail.Rows[i]["UNIT_ID"]);
                    batch_price_dtl.UNIT_NAME = (dataTable_detail.Rows[i]["UNIT_NAME"]).ToString();
                    //batch_price_dtl.UNIT_NAME = dataTable_detail.Rows[i]["UNIT_NAME"].ToString();


                    batch_price_mst.BATCH_PRICE_DTL_LIST.Add(batch_price_dtl);
                }
            }
            return batch_price_mst;
        }

    }
}




