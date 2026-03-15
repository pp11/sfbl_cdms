using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using NUnit.Framework.Internal;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution.Manager
{
    public class SkuCommissionManager : ISkuCommissionManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;

        public SkuCommissionManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;
        }

        public async Task<string> GetCustomer(string db, int COMPANY_ID, string UNIT_ID, string SKU_ID)
        {
            var query = @"SELECT ROWNUM ROW_NO, M.CUSTOMER_ID, M.CUSTOMER_CODE, C.CUSTOMER_NAME, 'No' PRICE_FLAG, 
                            'Yes' COMMISSION_FLAG, 'PCT' COMMISSION_TYPE
                          FROM CUSTOMER_SKU_PRICE_MST M, CUSTOMER_INFO C
                         WHERE     M.CUSTOMER_ID = C.CUSTOMER_ID
                               AND TRUNC (SYSDATE) BETWEEN TRUNC (M.EFFECT_START_DATE)
                                                       AND TRUNC (M.EFFECT_END_DATE)
                               AND M.CUSTOMER_ID NOT IN (SELECT CUSTOMER_ID
                                                           FROM CUSTOMER_WISE_SKU_COMM_ADD_DTL D
                                                          WHERE     M.UNIT_ID = D.UNIT_ID
                                                                AND D.SKU_ID = :param1)
                               AND M.COMPANY_ID = :param2
                               AND M.UNIT_ID = :param3";

            var dt = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query,
                _commonServices.AddParameter(new string[] { SKU_ID, COMPANY_ID.ToString(), UNIT_ID }));

            return _commonServices.DataTableToJSON(dt);
        }
        public async Task<string> GetCustomerMarketwise(string db, int COMPANY_ID, string SKU_ID,string market_code, string customer_type, string customer_status)
        {
            var query = string.Format(@"SELECT DISTINCT 0 ROW_NO, M.CUSTOMER_ID, M.CUSTOMER_CODE, C.CUSTOMER_NAME, 'No' PRICE_FLAG, 
                            'Yes' COMMISSION_FLAG, 'PCT' COMMISSION_TYPE
                          FROM CUSTOMER_SKU_PRICE_MST M, CUSTOMER_INFO C, CUSTOMER_MARKET_MST MR, CUSTOMER_MARKET_DTL MDT
                         WHERE     M.CUSTOMER_ID = C.CUSTOMER_ID AND  MR.CUSTOMER_MARKET_MST_ID = MDT.CUSTOMER_MARKET_MST_ID AND MR.CUSTOMER_ID = M.CUSTOMER_ID AND C.CUSTOMER_STATUS=" + customer_status+@" AND C.CUSTOMER_TYPE_ID = "+ customer_type  + @"
                               AND TRUNC (SYSDATE) BETWEEN TRUNC (M.EFFECT_START_DATE)
                                                       AND TRUNC (M.EFFECT_END_DATE)
                               AND M.CUSTOMER_ID NOT IN (SELECT MD.CUSTOMER_ID
                                                           FROM CUSTOMER_SKU_PRICE_MST MD, CUSTOMER_SKU_PRICE_DTL DD
                                                          WHERE  MD.CUSTOMER_PRICE_MSTID = DD.CUSTOMER_PRICE_MSTID  AND
                                                          TRUNC (SYSDATE) BETWEEN TRUNC (MD.EFFECT_START_DATE)
                                                           AND TRUNC (MD.EFFECT_END_DATE)
                                                          AND DD.SKU_ID = :param1)
                                                          AND M.CUSTOMER_ID NOT IN 
                                                          (SELECT DTL.CUSTOMER_ID FROM CUSTOMER_WISE_SKU_COMM_ADD_MST MTR,CUSTOMER_WISE_SKU_COMM_ADD_DTL DTL
                                                           WHERE MTR.MST_ID = DTL.MST_ID AND NVL(MTR.IS_PROCESSED,'-') = '-'  AND DTL.SKU_ID = " + SKU_ID+@"
                                                           
                                                           )
                               AND M.COMPANY_ID = :param2 AND MDT.MARKET_ID in ({0})", market_code);

            var dt = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query,
                _commonServices.AddParameter(new string[] { SKU_ID, COMPANY_ID.ToString() }));

            return _commonServices.DataTableToJSON(dt);
        }
        public async Task<string> Add(string db, CUSTOMER_WISE_SKU_COMM_ADD_MST model)
        {
            List<QueryPattern> listOfQuery = new List<QueryPattern>();

            bool dtk = true;
            string cust_ids = "";
            int count = 0;

            var getMstIdQuery = @"SELECT NVL(MAX(MST_ID) + 1, 1) MST_ID FROM CUSTOMER_WISE_SKU_COMM_ADD_MST";
            model.MST_ID = await _commonServices.GetMaximumNumberAsyn<int>(_configuration.GetConnectionString(db), getMstIdQuery, _commonServices.AddParameter(new string[] { }));
            var lastDtlIdQuery = @"SELECT NVL(MAX(DTL_ID), 0) DTL_ID FROM CUSTOMER_WISE_SKU_COMM_ADD_DTL";
            var dtl_id = await _commonServices.GetMaximumNumberAsyn<int>(_configuration.GetConnectionString(db), lastDtlIdQuery, _commonServices.AddParameter(new string[] { }));
             model.SKU_CODE = await _commonServices.GetMaximumNumberAsyn<string>(_configuration.GetConnectionString(db), "Select SKU_CODE from Product_Info Where SKU_ID = :param1", _commonServices.AddParameter(new string[] { model.SKU_ID }));
            
            var mstAddQuery = @"INSERT INTO CUSTOMER_WISE_SKU_COMM_ADD_MST 
                (MST_ID, UNIT_ID, SKU_ID, SKU_CODE, COMPANY_ID, ENTERED_BY, ENTERED_DATE, ENTERED_TERMINAL) 
                VALUES (:param1, :param2, :param3, :param4, :param5, :param6, TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'), :param8)";

            listOfQuery.Add(_commonServices.AddQuery(mstAddQuery, _commonServices.AddParameter(new string[]
            {
                model.MST_ID.ToString(),
                model.UNIT_ID,
                model.SKU_ID,
                model.SKU_CODE,
                model.COMPANY_ID.ToString(),
                model.ENTERED_BY,
                model.ENTERED_DATE,
                model.ENTERED_TERMINAL
            })));

            foreach (var dtl in model.DETAILS)
            {
                if(count == 0)
                {
                    cust_ids = dtl.CUSTOMER_ID.ToString();
                }
                else
                {
                    cust_ids = cust_ids + ","+ dtl.CUSTOMER_ID.ToString();

                }

                var dtlAddQuery = @"INSERT INTO CUSTOMER_WISE_SKU_COMM_ADD_DTL 
                (DTL_ID, MST_ID, UNIT_ID, CUSTOMER_ID, CUSTOMER_CODE, SKU_ID, SKU_CODE, PRICE_FLAG, SKU_PRICE, 
                    COMMISSION_FLAG, COMMISSION_TYPE, COMMISSION_VALUE, ADD_COMMISSION1, ADD_COMMISSION2, COMPANY_ID,
                    ENTERED_BY, ENTERED_DATE, ENTERED_TERMINAL) 
                VALUES 
                 (:param1, :param2, :param3, :param4, :param5, :param6, :param7, :param8, :param9, :param10, 
                    :param11, :param12, :param13, :param14, :param15, :param16, TO_DATE(:param17, 'DD/MM/YYYY HH:MI:SS AM'), :param18)";

                listOfQuery.Add(_commonServices.AddQuery(dtlAddQuery, _commonServices.AddParameter(new string[]
                {
                    (++dtl_id).ToString(),
                    model.MST_ID.ToString(),
                    model.UNIT_ID,
                    dtl.CUSTOMER_ID.ToString(),
                    dtl.CUSTOMER_CODE,
                    model.SKU_ID,
                    model.SKU_CODE,
                    dtl.PRICE_FLAG,
                    model.SKU_PRICE.ToString(),
                    dtl.COMMISSION_FLAG,
                    dtl.COMMISSION_TYPE,
                    dtl.COMMISSION_VALUE,
                    dtl.ADD_COMMISSION1,
                    dtl.ADD_COMMISSION2,
                    model.COMPANY_ID.ToString(),
                    model.ENTERED_BY,
                    model.ENTERED_DATE,
                    model.ENTERED_TERMINAL
                })));
                count++;
            }


            string query = string.Format(@"SELECT DISTINCT MD.CUSTOMER_ID, C.CUSTOMER_NAME,C.CUSTOMER_CODE
                                                           FROM CUSTOMER_SKU_PRICE_MST MD, CUSTOMER_SKU_PRICE_DTL DD, CUSTOMER_INFO C
                                                          WHERE  MD.CUSTOMER_PRICE_MSTID = DD.CUSTOMER_PRICE_MSTID AND C.CUSTOMER_ID = MD.CUSTOMER_ID  AND
                                                          TRUNC (SYSDATE) BETWEEN TRUNC (MD.EFFECT_START_DATE)
                                                           AND TRUNC (MD.EFFECT_END_DATE)
                                                          AND DD.SKU_ID = {1}  AND MD.CUSTOMER_ID in ({0})", cust_ids, model.SKU_ID);

            DataTable dt = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query,
                        _commonServices.AddParameter(new string[] { }));

            if(dt.Rows.Count>0)
            {
                string errormsg = "Customer Exist Error: ";
                for(var i= 0; i<dt.Rows.Count;i++)
                {
                    errormsg = i==0? errormsg + dt.Rows[i]["CUSTOMER_NAME"]: errormsg +"," +dt.Rows[i]["CUSTOMER_NAME"];
                }

                return errormsg;
            }
            else
            {
                await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                return "1";
            }
        }

        public async Task<string> LoadData(string db, int company_id)
        {
            var query = @" SELECT ROWNUM ROW_NO,
       A.MST_ID,
       A.UNIT_ID,
       FN_UNIT_SHORT_NAME (A.COMPANY_ID, A.UNIT_ID) UNIT_NAME,
       A.SKU_ID,
       A.SKU_CODE,
       FN_SKU_NAME (A.COMPANY_ID, A.SKU_ID) SKU_NAME,
       A.COMPANY_ID,
       TO_CHAR(A.ENTERED_DATE, 'DD/MM/YYYY HH:MI:SS AM') ENTERED_DATE,
       USER_NAME ENTERED_BY,
       A.IS_PROCESSED,
       (SELECT COUNT(*) FROM CUSTOMER_WISE_SKU_COMM_ADD_DTL D WHERE  D.MST_ID =A.MST_ID GROUP BY D.MST_ID ) TOTAL_CUST
  FROM CUSTOMER_WISE_SKU_COMM_ADD_MST A, STL_ERP_SCS.USER_INFO B
 WHERE A.COMPANY_ID = :param1 AND A.ENTERED_BY = B.USER_ID
 ORDER BY FN_SKU_NAME (A.COMPANY_ID, A.SKU_ID), A.UNIT_ID ";

            return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[]
            {
                company_id.ToString()
            })));
        }

        public async Task<string> GetDetails(string db, int mst_id)
        {
            var query = @"SELECT ROWNUM ROW_NO, A.*, B.CUSTOMER_NAME
                  FROM CUSTOMER_WISE_SKU_COMM_ADD_DTL A, CUSTOMER_INFO B
                 WHERE A.CUSTOMER_ID = B.CUSTOMER_ID AND MST_ID = :param1";

            return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[]
            {
                mst_id.ToString()
            })));
        }

        public async Task<string> GetMarketWiseCustomers(string db, string Market_Code)
        {
            string query = string.Format(@"SELECT M.CUSTOMER_ID,M.CUSTOMER_CODE, D.MARKET_ID FROM CUSTOMER_MARKET_DTL D
              LEFT OUTER JOIN CUSTOMER_MARKET_MST M on M.CUSTOMER_MARKET_MST_ID = D.CUSTOMER_MARKET_MST_ID 
              WHERE MARKET_ID in ({0})",Market_Code);
            return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] {})));
        }

        public async Task<string> GetComissionDoneCustomers(string db, string Sku_id)
        {
            string query = string.Format(@"Select unique M.CUSTOMER_ID,M.CUSTOMER_CODE, D.SKU_CODE,D.SKU_ID , M.EFFECT_START_DATE,M.EFFECT_END_DATE
                          From CUSTOMER_SKU_PRICE_MST M, CUSTOMER_SKU_PRICE_DTL D 
                          WHERE M.CUSTOMER_PRICE_MSTID = D.CUSTOMER_PRICE_MSTID and (sysdate BETWEEN M.EFFECT_START_DATE AND M.EFFECT_END_DATE) AND D.SKU_ID = {0}", Sku_id);
            return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] { })));
        }
        public async Task Update(string db, CUSTOMER_WISE_SKU_COMM_ADD_DTL model)
        {
            var sql = @"UPDATE CUSTOMER_WISE_SKU_COMM_ADD_DTL
               SET PRICE_FLAG = :param2,
                   COMMISSION_FLAG = :param3,
                   COMMISSION_TYPE = :param4,
                   COMMISSION_VALUE = :param5,
                   ADD_COMMISSION1 = :param6,
                   ADD_COMMISSION2 = :param7,
                   UPDATED_BY = :param8,
                   UPDATED_DATE = TO_DATE(:param9, 'DD/MM/YYYY HH:MI:SS AM'),
                   UPDATED_TERMINAL = :param10
             WHERE DTL_ID = :param1";

            List<QueryPattern> listOfQuery = new List<QueryPattern>();
            listOfQuery.Add(_commonServices.AddQuery(sql, _commonServices.AddParameter(new string[]
                {
                    model.DTL_ID.ToString(),
                    model.PRICE_FLAG,
                    model.COMMISSION_FLAG,
                    model.COMMISSION_TYPE,
                    model.COMMISSION_VALUE,
                    model.ADD_COMMISSION1,
                    model.ADD_COMMISSION2,
                    model.UPDATED_BY,
                    model.UPDATED_DATE,
                    model.UPDATED_TERMINAL
                })));
            await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
        }

        public async Task DeleteMst(string db, int id)
        {
            var sql = @"DELETE FROM CUSTOMER_WISE_SKU_COMM_ADD_MST WHERE MST_ID = :param1";
            List<QueryPattern> listOfQuery = new List<QueryPattern>();
            listOfQuery.Add(_commonServices.AddQuery(sql, _commonServices.AddParameter(new string[] { id.ToString() })));

            await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
        }

        public async Task DeleteDtl(string db, int id)
        {
            var sql = @"DELETE FROM CUSTOMER_WISE_SKU_COMM_ADD_DTL WHERE DTL_ID = :param1";
            List<QueryPattern> listOfQuery = new List<QueryPattern>();
            listOfQuery.Add(_commonServices.AddQuery(sql, _commonServices.AddParameter(new string[] { id.ToString() })));

            await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
        }

        public async Task Process(string db, int id)
        {
            var sql = string.Format("begin PRC_CUSTOMER_WISE_SKU_COMM_ADD({0}); end;", id);
            List<QueryPattern> listOfQuery = new List<QueryPattern>();
            listOfQuery.Add(_commonServices.AddQuery(sql, _commonServices.AddParameter(new string[] { })));

            var updateMst = "UPDATE CUSTOMER_WISE_SKU_COMM_ADD_MST SET IS_PROCESSED = 'Yes' WHERE MST_ID = :param1";
            listOfQuery.Add(_commonServices.AddQuery(updateMst, _commonServices.AddParameter(new string[] { id.ToString() })));
            
            await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
        }
    }
}
