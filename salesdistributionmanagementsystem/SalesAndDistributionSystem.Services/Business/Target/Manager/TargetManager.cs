using Glimpse.Core.ClientScript;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using Org.BouncyCastle.Asn1.Cmp;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.Entities.Target;
using SalesAndDistributionSystem.Services.Business.Target.IManager;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Target.Manager
{
    public class TargetManager : ITargetManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;

        public TargetManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;
        }

        string Get_LastMST_ID() => "SELECT NVL(MAX(MST_ID),0) MST_ID  FROM CUSTOMER_TARGET_MST";
        string Get_NewDTL_ID() => "SELECT NVL(MAX(DTL_ID),0) + 1 DTL_ID  FROM CUSTOMER_TARGET_DTL";
        string TargetAddQuery() => @"INSERT INTO CUSTOMER_TARGET_MST (MST_ID,
            YEAR,
            MONTH_CODE,
            CUSTOMER_ID,
            CUSTOMER_CODE,
            COMPANY_ID,
            ENTERED_DATE,
            ENTERED_BY,
            ENTERED_TERMINAL,
            UNIT_ID) VALUES
            (:param1, :param2, :param3, :param4, :param5, :param6, 
                TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'), :param8, :param9, :param10)";

        string TargetUpdateQuery() => @"UPDATE CUSTOMER_TARGET_MST SET
            YEAR = :param2,
            MONTH_CODE = :param3,
            CUSTOMER_ID = :param4,
            CUSTOMER_CODE = :param5,
            COMPANY_ID = :param6,
            UPDATED_DATE = TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'),
            UPDATED_BY = :param8,
            UPDATED_TERMINAL = :param9,
            UNIT_ID = :param10
            WHERE MST_ID = :param1";

        string TargetDetailAddQuery() => @"INSERT INTO CUSTOMER_TARGET_DTL (MST_ID,
            DTL_ID,
            SKU_ID,
            SKU_CODE,
            MRP,
            TARGET_QTY,
            TARGET_VALUE,
            AVG_PER_DAY_TARGET_QTY,
            DISCOUNT_VALUE,
            NET_VALUE,
            PREVIOUS_TARGET_QTY,
            UNIT_TP,
            ENTERED_DATE,
            ENTERED_BY,
            ENTERED_TERMINAL,
            COMPANY_ID) VALUES
            (:param1, :param2, :param3, :param4, :param5, :param6, 
                :param7, :param8, :param9, :param10, :param11, :param12, 
                TO_DATE(:param13, 'DD/MM/YYYY HH:MI:SS AM'), :param14, :param15, :param16)";

        string TargetDetailUpdateQuery() => @"UPDATE CUSTOMER_TARGET_DTL SET
            SKU_ID=:param2,
            SKU_CODE=:param3,
            MRP=:param4,
            TARGET_QTY=:param5,
            TARGET_VALUE=:param6,
            AVG_PER_DAY_TARGET_QTY=:param7,
            DISCOUNT_VALUE=:param8,
            NET_VALUE=:param9,
            PREVIOUS_TARGET_QTY=:param10,
            UNIT_TP=:param11,
            UPDATED_DATE=TO_DATE(:param12, 'DD/MM/YYYY HH:MI:SS AM'),
            UPDATED_BY=:param13,
            UPDATED_TERMINAL=:param14,
            COMPANY_ID=:param15
            WHERE DTL_ID=:param1";

        public async Task<string> LoadData(string db, int Company_Id)
        {
            var query = @"SELECT ROW_NUMBER() OVER(ORDER BY T.CUSTOMER_ID ASC) AS ROW_NO,
                T.MST_ID,
                T.YEAR,
                T.CUSTOMER_CODE,
                TO_CHAR(TO_DATE(MONTH_CODE, 'MM'), 'MONTH') AS MONTH,
                T.CUSTOMER_ID,
                M.CUSTOMER_NAME,
                T.COMPANY_ID,
                T.UNIT_ID,
                T.ENTERED_DATE,
                '' MST_ID_ENCRYPTED
                FROM CUSTOMER_TARGET_MST T, CUSTOMER_INFO M
                Where T.COMPANY_ID = :param1
                AND T.CUSTOMER_ID = M.CUSTOMER_ID
                ORDER BY T.YEAR DESC, T.MONTH_CODE DESC";

            var data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] { Company_Id.ToString() }));

            for (var i = 0; i < data.Rows.Count; i++)
            {
                data.Rows[i]["MST_ID_ENCRYPTED"] = _commonServices.Encrypt(data.Rows[i]["MST_ID"].ToString());
            }

            DataSet ds = new DataSet();
            ds.Tables.Add(data);

            return _commonServices.DataSetToJSON(ds);
        }

        public string GetTargetMstById_Query() => @"SELECT * FROM CUSTOMER_TARGET_MST 
                        WHERE COMPANY_ID = :param1 AND MST_ID = :param2";
        public string GetTargetDtlByMstId_Query() => @"SELECT T.*, FN_PACK_SIZE(COMPANY_ID, SKU_ID) PACK_SIZE 
                FROM CUSTOMER_TARGET_DTL T
                WHERE COMPANY_ID = :param1 AND MST_ID = :param2";
        public string DeleteTargetDtl_Query() => @"DELETE FROM CUSTOMER_TARGET_MST WHERE DTL_ID = :param1";

        private async Task<CUSOTMER_TARGET_MST> GetDataById(string db, int companyId, string id)
        {
            var dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db),
                GetTargetMstById_Query(),
                _commonServices.AddParameter(new string[] { companyId.ToString(), id }));

            var mst = dataTable.Rows[0].ToObject<CUSOTMER_TARGET_MST>();

            var details = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db),
                GetTargetDtlByMstId_Query(),
                _commonServices.AddParameter(new string[] { companyId.ToString(), id }));
            mst.TARGET_DTLs = details.ToList<CUSTOMER_TARGET_DTL>();
            return mst;
        }

        private void AddTarget(string db, CUSOTMER_TARGET_MST model, ref int last_mst_id, ref int new_dtl_id, ref List<QueryPattern> listOfQuery)
        {
            listOfQuery.Add(_commonServices.AddQuery(TargetAddQuery(), _commonServices.AddParameter(new string[]
            {
                       (++last_mst_id).ToString(),
                        model.YEAR.ToString(),
                        model.MONTH_CODE,
                        model.CUSTOMER_ID.ToString(),
                        model.CUSTOMER_CODE,
                        model.COMPANY_ID.ToString(),
                        model.ENTERED_DATE,
                        model.ENTERED_BY,
                        model.ENTERED_TERMINAL,
                        model.UNIT_ID.ToString(),
            })));

            if (model.TARGET_DTLs.Count < 1)
            {
                throw new Exception("Empty detail information!");
            }
            foreach (var detail in model.TARGET_DTLs)
            {
                detail.DTL_ID = new_dtl_id++;
                if (string.IsNullOrEmpty(detail.SKU_ID) || detail.SKU_ID == "0")
                {
                    throw new Exception("SKU ID not found");
                }
                listOfQuery.Add(_commonServices.AddQuery(TargetDetailAddQuery(), _commonServices.AddParameter(new string[]
                {
                            last_mst_id.ToString(),
                            detail.DTL_ID.ToString(),
                            detail.SKU_ID.ToString(),
                            detail.SKU_CODE,
                            detail.MRP.ToString(),
                            detail.TARGET_QTY.ToString(),
                            detail.TARGET_VALUE.ToString(),
                            detail.AVG_PER_DAY_TARGET_QTY.ToString(),
                            detail.DISCOUNT_VALUE.ToString(),
                            detail.NET_VALUE.ToString(),
                            detail.PREVIOUS_TARGET_QTY.ToString(),
                            detail.UNIT_TP.ToString(),
                            model.ENTERED_DATE,
                            model.ENTERED_BY,
                            model.ENTERED_TERMINAL,
                            model.COMPANY_ID.ToString(),
                })));
            }
        }
        public async Task<string> AddOrUpdate(string db, CUSOTMER_TARGET_MST model)
        {
            if (model == null)
            {
                return "No data provided to insert!!!!";
            }
            else
            {
                var new_dtl_id = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db),
                            Get_NewDTL_ID(), _commonServices.AddParameter(new string[] { }));
                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                if (model.MST_ID == 0)
                {
                    var MST_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), Get_LastMST_ID(), _commonServices.AddParameter(new string[] { }));
                    AddTarget(db, model, ref MST_ID, ref new_dtl_id, ref listOfQuery);
                }
                else
                {
                    var oldMaster = await GetDataById(db, model.COMPANY_ID, model.MST_ID.ToString());

                    listOfQuery.Add(_commonServices.AddQuery(TargetUpdateQuery(), _commonServices.AddParameter(new string[]
                    {
                        model.MST_ID.ToString(),
                        model.YEAR.ToString(),
                        model.MONTH_CODE,
                        model.CUSTOMER_ID.ToString(),
                        model.CUSTOMER_CODE,
                        model.COMPANY_ID.ToString(),
                        model.UPDATED_DATE,
                        model.UPDATED_BY,
                        model.UPDATED_TERMINAL,
                        model.UNIT_ID.ToString(),
                    })));
                    //UPDATE DETAILS
                    foreach (var detail in model.TARGET_DTLs)
                    {
                        if (detail.DTL_ID == 0) //new target_dtl
                        {
                            listOfQuery.Add(_commonServices.AddQuery(TargetDetailAddQuery(), _commonServices.AddParameter(new string[]
                            {
                                model.MST_ID.ToString(),
                                new_dtl_id++.ToString(),
                                detail.SKU_ID.ToString(),
                                detail.SKU_CODE,
                                detail.MRP.ToString(),
                                detail.TARGET_QTY.ToString(),
                                detail.TARGET_VALUE.ToString(),
                                detail.AVG_PER_DAY_TARGET_QTY.ToString(),
                                detail.DISCOUNT_VALUE.ToString(),
                                detail.NET_VALUE.ToString(),
                                detail.PREVIOUS_TARGET_QTY.ToString(),
                                detail.UNIT_TP.ToString(),
                                model.UPDATED_DATE,
                                model.UPDATED_BY,
                                model.UPDATED_TERMINAL,
                                model.COMPANY_ID.ToString(),
                            })));
                        }
                        else
                        {
                            listOfQuery.Add(_commonServices.AddQuery(TargetDetailUpdateQuery(), _commonServices.AddParameter(new string[]
                                {
                                    detail.DTL_ID.ToString(),
                                    detail.SKU_ID.ToString(),
                                    detail.SKU_CODE,
                                    detail.MRP.ToString(),
                                    detail.TARGET_QTY.ToString(),
                                    detail.TARGET_VALUE.ToString(),
                                    detail.AVG_PER_DAY_TARGET_QTY.ToString(),
                                    detail.DISCOUNT_VALUE.ToString(),
                                    detail.NET_VALUE.ToString(),
                                    detail.PREVIOUS_TARGET_QTY.ToString(),
                                    detail.UNIT_TP.ToString(),
                                    model.UPDATED_DATE,
                                    model.UPDATED_BY,
                                    model.UPDATED_TERMINAL,
                                    model.COMPANY_ID.ToString()
                                })));
                        }
                        //delete extra target_details
                        foreach (var oldDetail in oldMaster.TARGET_DTLs)
                        {
                            var exits = model.TARGET_DTLs.Any(e => e.DTL_ID == oldDetail.DTL_ID);
                            if (!exits)
                            {
                                //delete
                                listOfQuery.Add(_commonServices.AddQuery(DeleteTargetDtl_Query(), _commonServices.AddParameter(new string[] { oldDetail.DTL_ID.ToString() })));
                            }
                        }
                    }
                }

                await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                model.MST_ID_ENCRYPTED = _commonServices.Encrypt(model.MST_ID.ToString());
                return model.MST_ID_ENCRYPTED;
            }
        }

        public async Task<string> AddTargetDelete(string db, CUSOTMER_TARGET_MST model)
        {
            if (model == null)
            {
                return "No data provided to insert!!!!";
            }
            else
            {
                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                listOfQuery.Add(_commonServices.AddQuery(@"DELETE CUSTOMER_TARGET_DTL WHERE MST_ID IN (SELECT MST_ID FROM CUSTOMER_TARGET_MST WHERE YEAR=:param1 AND MONTH_CODE =:param2)", _commonServices.AddParameter(new string[] { model.YEAR, model.MONTH_CODE })));
                listOfQuery.Add(_commonServices.AddQuery(@"DELETE CUSTOMER_TARGET_MST WHERE YEAR=:param1 AND MONTH_CODE =:param2", _commonServices.AddParameter(new string[] { model.YEAR, model.MONTH_CODE })));
                listOfQuery.Add(_commonServices.AddQuery(@"INSERT INTO TARGET_DELETE_HISTORY (
    YEAR,
    MONTH_CODE,
    ENTERED_BY,
    ENTERED_DATE,
    ENTERED_TERMINAL
) VALUES (
    :param1,
    :param2,
    :param3,
    TO_DATE(:param4, 'DD/MM/YYYY HH:MI:SS AM'),
    :param5
)", _commonServices.AddParameter(new string[] { model.YEAR, model.MONTH_CODE, model.ENTERED_BY, model.ENTERED_DATE, model.ENTERED_TERMINAL })));


                await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                model.MST_ID_ENCRYPTED = _commonServices.Encrypt(model.MST_ID.ToString());
                return model.MST_ID_ENCRYPTED;
            }
        }

        public async Task<string> GetEditDataById(string db, int Company_Id, string id)
        {
            var mst = await GetDataById(db, Company_Id, id);
            return JsonConvert.SerializeObject(mst);
        }

        public async Task<List<CUSTOMER_TARGET_DTL>> GetTargetDetails(string db, List<CUSTOMER_TARGET_DTL> model)
        {
            var sl = 0;
            foreach (var dtl in model)
            {
                var sql = "SELECT SKU_ID, SKU_NAME, PACK_SIZE, MRP, UNIT_TP FROM VW_PRODUCT_PRICE WHERE SKU_CODE=:param1";
                var dt = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db),
                sql,
                _commonServices.AddParameter(new string[] { dtl.SKU_CODE }));

                if (dt.Rows.Count > 0)
                {
                    dtl.ROW_NO = ++sl;
                    dtl.SKU_ID = dt.Rows[0]["SKU_ID"].ToString();
                    dtl.SKU_NAME = dt.Rows[0]["SKU_NAME"].ToString();
                    dtl.MRP = decimal.TryParse(dt.Rows[0]["MRP"].ToString(), out decimal temp) ? temp : (decimal?)null;
                    dtl.UNIT_TP = decimal.TryParse(dt.Rows[0]["UNIT_TP"].ToString(), out temp) ? temp : (decimal?)null;
                    dtl.PACK_SIZE = dt.Rows[0]["PACK_SIZE"].ToString();
                    dtl.TARGET_VALUE = dtl.TARGET_QTY * (decimal)dtl.UNIT_TP;
                }

                var sql2 = "SELECT CUSTOMER_ID, CUSTOMER_NAME, CUSTOMER_CODE FROM CUSTOMER_INFO WHERE CUSTOMER_CODE=:param1";
                var dt2 = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db),
                sql2,
                _commonServices.AddParameter(new string[] { dtl.CUSTOMER_CODE }));

                if (dt2.Rows.Count > 0)
                {
                    dtl.CUSTOMER_ID = dt2.Rows[0]["CUSTOMER_ID"].ToString();
                    dtl.CUSTOMER_NAME = dt2.Rows[0]["CUSTOMER_NAME"].ToString();
                    dtl.CUSTOMER_CODE = dt2.Rows[0]["CUSTOMER_CODE"].ToString();
                }
            }

            return model;
        }

        public async Task<string> AddList(string db, List<CUSOTMER_TARGET_MST> model)
        {

            var new_dtl_id = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db),
                        Get_NewDTL_ID(), _commonServices.AddParameter(new string[] { }));
            List<QueryPattern> listOfQuery = new List<QueryPattern>();

            try
            {
                var MST_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), Get_LastMST_ID(), _commonServices.AddParameter(new string[] { }));
                foreach (var item in model)
                {
                    AddTarget(db, item, ref MST_ID, ref new_dtl_id, ref listOfQuery);
                }
                await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                return "1";
            }
            catch (Exception exp)
            {
                return exp.Message;
            }
        }

        public async Task<string> GetCustWiseRemainingBnsRpt(string db, string DATE_FROM, string DATE_TO, string CUSTOMER_CODE, int COMPANY_ID, int UNIT_ID)
        {
            if (CUSTOMER_CODE == null || CUSTOMER_CODE == "null" || CUSTOMER_CODE == "undefined" || CUSTOMER_CODE == "" || CUSTOMER_CODE == "''" || CUSTOMER_CODE == " ")
            {
                CUSTOMER_CODE = "ALL";
            }
            string Query = @"begin  :param1 := FN_CUST_WISE_REMAINING_BNS_RPT(TO_DATE(:param2, 'DD/MM/YYYY'), TO_DATE(:param3, 'DD/MM/YYYY'), :param4, :param5,:param6); end;";
            DataTable dt = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Query, _commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), DATE_FROM, DATE_TO, CUSTOMER_CODE, UNIT_ID.ToString(), COMPANY_ID.ToString(), }));
            return _commonServices.DataTableToJSON(dt);


        }

        public async Task<string> LockCustWiseRemainingBnsRpt(string db, string DATE_FROM, string DATE_TO, string CUSTOMER_CODE, int COMPANY_ID, int UNIT_ID, string ENTERED_BY, string ENTERED_TERMINAL)
        {
            if (CUSTOMER_CODE == null || CUSTOMER_CODE == "null" || CUSTOMER_CODE == "undefined" || CUSTOMER_CODE == "" || CUSTOMER_CODE == "''" || CUSTOMER_CODE == " ")
            {
                CUSTOMER_CODE = "ALL";
            }
            string Query = @"begin SP_LOCK_CUST_WISE_REM_BNS(TO_DATE(:param1, 'DD/MM/YYYY'), TO_DATE(:param2, 'DD/MM/YYYY'), :param3, :param4,:param5,:param6,:param7,:param8); end;";
            string result = await _commonServices.ProcedureCallAsyn<string>(_configuration.GetConnectionString(db), Query, _commonServices.AddParameter(new string[] { DATE_FROM, DATE_TO, CUSTOMER_CODE, UNIT_ID.ToString(), COMPANY_ID.ToString(), ENTERED_BY, ENTERED_TERMINAL, OracleDbType.Varchar2.ToString() }));
            return result;
        }

        public async Task<string> AddOrUpdateAdjustment(string db, AdjustmentMst model)
        {
            if (model == null || model.Adjustment.Count<=0)
            {
                return "No data provided to insert !";
            }
            else
            {
                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                foreach (var detail in model.Adjustment)
                {
                    DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), @"SELECT M.CUSTOMER_CODE,
       M.CUSTOMER_ID,
       M.EFFECT_START_DATE,
       M.EFFECT_END_DATE
FROM   REM_BONUS_ADJ M
WHERE  M.CUSTOMER_ID = :param1 AND M.UNIT_ID=:param2 
AND    ( 
            (TO_DATE(:param3, 'DD/MM/YYYY') BETWEEN M.EFFECT_START_DATE AND M.EFFECT_END_DATE) 
         OR (TO_DATE(:param4, 'DD/MM/YYYY') BETWEEN M.EFFECT_START_DATE AND M.EFFECT_END_DATE)
         OR (M.EFFECT_START_DATE BETWEEN TO_DATE(:param3, 'DD/MM/YYYY') AND TO_DATE(:param4, 'DD/MM/YYYY'))
         OR (M.EFFECT_END_DATE BETWEEN TO_DATE(:param3, 'DD/MM/YYYY') AND TO_DATE(:param4, 'DD/MM/YYYY'))
       )
      
", _commonServices.AddParameter(new string[] { detail.CUSTOMER_ID.ToString(), model.UNIT_ID, detail.EFFECT_START_DATE, detail.EFFECT_END_DATE }));
                    if (dataTable.Rows.Count > 0 )
                    {
                        throw new Exception("Adjustment already given in this date period for customer code: " + detail.CUSTOMER_CODE);
                    }
                    if(string.IsNullOrWhiteSpace(detail.CUSTOMER_CODE) || string.IsNullOrWhiteSpace(detail.EFFECT_START_DATE) || string.IsNullOrWhiteSpace(detail.EFFECT_END_DATE) || detail.AMOUNT<=0)
                    {
                        throw new Exception("Data not valid");

                    }

                    listOfQuery.Add(_commonServices.AddQuery(
                        "INSERT INTO REM_BONUS_ADJ ( CUSTOMER_ID, CUSTOMER_CODE, COMPANY_ID, UNIT_ID, AMOUNT, EFFECT_START_DATE, EFFECT_END_DATE, REMARKS, ENTERED_BY, ENTERED_DATE, ENTERED_TERMINAL ) VALUES ( :param1, :param2, :param3, :param4, :param5, TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM'), TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'), :param8, :param9, TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'), :param11 )", 
                        _commonServices.AddParameter(new string[] { detail.CUSTOMER_ID.ToString(),
                            detail.CUSTOMER_CODE, 
                            model.COMPANY_ID.ToString(),
                            model.UNIT_ID,
                            detail.AMOUNT.ToString(),
                            detail.EFFECT_START_DATE,
                            detail.EFFECT_END_DATE,
                            detail.REMARKS,
                            model.ENTERED_BY,
                            model.ENTERED_DATE,
                            model.ENTERED_TERMINAL
                            })));
                }
                await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                return "Data Save Successully";
            }
        }
    }
}


