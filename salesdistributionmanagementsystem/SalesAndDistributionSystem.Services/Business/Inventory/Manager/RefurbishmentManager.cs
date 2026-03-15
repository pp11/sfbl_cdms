using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using SalesAndDistributionSystem.Services.Business.Inventory.IManager;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Inventory.Manager
{
    public class RefurbishmentManager : IRefurbishmentManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public RefurbishmentManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;
        }
        public async Task<Result> AddOrUpdate(string db, RefurbishmentReceivingMst model)
        {
            Result result = new Result();
            try
            {
                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                if (model.MST_SLNO == 0)
                {
                    string insertMstQuery() => @"INSERT INTO REFURBISHMENT_RECEIVING_MST (MST_SLNO, CLAIM_NO, RECEIVE_DATE, RECEIVE_SHIFT, RECEIVE_CATEGORY, CUSTOMER_CODE, CUSTOMER_TYPE, CHALLAN_NUMBER, CHALLAN_DATE, SENDING_CARTON_QTY, SENDING_BAG_QTY, SENDING_TOTAL_AMOUNT, RECEIVE_CARTON_QTY, RECEIVE_BAG_QTY, RECEIVE_TOTAL_AMOUNT, TSO_CODE, TSO_NAME, DRIVER_CODE, DRIVER_NAME, VEHICLE_NO, RECEIVED_BY, REMARKS, ENTERED_BY, ENTERED_DATE, ENTERED_TERMINAL,COMPANY_ID,UNIT_ID,DRIVER_CONTACT_NO) 
                                                        VALUES ( :param1, :param2, TO_DATE ( :param3, 'DD/MM/YYYY HH:MI:SS AM'), :param4, :param5, :param6, :param7, :param8, TO_DATE ( :param9, 'DD/MM/YYYY HH:MI:SS AM'), :param10, :param11, :param12, :param13, :param14, :param15, :param16, :param17, :param18, :param19, :param20, :param21, :param22, :param23, TO_DATE ( :param24, 'DD/MM/YYYY HH:MI:SS AM'), :param25,:param26,:param27,:param28)";
                    string insertDtlQuery() => @"INSERT INTO STL_ERP_SDS.REFURBISHMENT_RECEIVING_DTL(DTL_SLNO, MST_SLNO, REFURBISHMENT_PRODUCT_STATUS, PRODUCT_CODE, CLAIM_QTY, RECEIVED_QTY, DISPUTE_QTY, TRADE_PRICE, REVISED_PRICE, EXPIRY_DATE, REMARKS, ENTERED_BY, ENTERED_DATE, ENTERED_TERMINAL,BATCH_NO) 
                                                        VALUES( :param1, :param2, :param3, :param4, :param5, :param6, :param7, :param8, :param9, TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'), :param11, :param12, TO_DATE(:param13, 'DD/MM/YYYY HH:MI:SS AM'), :param14,:param15)";
                    //-------------Add to master table--------------------
                    model.MST_SLNO = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), "SELECT NVL(MAX(MST_SLNO),0) + 1 MST_SLNO  FROM REFURBISHMENT_RECEIVING_MST", _commonServices.AddParameter(new string[] { }));
                    model.CLAIM_NO = _commonServices.GetMaximumNumber<string>(_configuration.GetConnectionString(db), "SELECT FN_GEN_REFURBISH_CLAIM_NO(1) CLAIM_NO FROM DUAL", _commonServices.AddParameter(new string[] { }));
                    int DTL_SLNO = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), "SELECT NVL(MAX(DTL_SLNO),0) + 1 DTL_SLNO  FROM REFURBISHMENT_RECEIVING_DTL", _commonServices.AddParameter(new string[] { }));
                    listOfQuery.Add(_commonServices.AddQuery(insertMstQuery(), _commonServices.AddParameter(new string[] { model.MST_SLNO.ToString(), model.CLAIM_NO, model.RECEIVE_DATE, model.RECEIVE_SHIFT, model.RECEIVE_CATEGORY, model.CUSTOMER_CODE, model.CUSTOMER_TYPE, model.CHALLAN_NUMBER, model.CHALLAN_DATE, model.SENDING_CARTON_QTY.ToString(), model.SENDING_BAG_QTY.ToString(), model.SENDING_TOTAL_AMOUNT.ToString(), model.RECEIVE_CARTON_QTY.ToString(), model.RECEIVE_BAG_QTY.ToString(), model.RECEIVE_TOTAL_AMOUNT.ToString(), model.TSO_CODE, model.TSO_NAME, model.DRIVER_CODE, model.DRIVER_NAME, model.VEHICLE_NO, model.RECEIVED_BY, model.REMARKS, model.ENTERED_BY, model.ENTERED_DATE, model.ENTERED_TERMINAL, model.COMPANY_ID, model.UNIT_ID, model.DRIVER_CONTACT_NO })));
                    foreach (var item in model.Details)
                    {
                        listOfQuery.Add(_commonServices.AddQuery(insertDtlQuery(),
                            _commonServices.AddParameter(new string[] { DTL_SLNO.ToString(), model.MST_SLNO.ToString(), item.REFURBISHMENT_PRODUCT_STATUS, item.PRODUCT_CODE, item.CLAIM_QTY.ToString(), item.RECEIVED_QTY.ToString(), item.DISPUTE_QTY.ToString(), item.TRADE_PRICE.ToString(), item.REVISED_PRICE.ToString(), item.EXPIRY_DATE, item.REMARKS, model.ENTERED_BY, model.ENTERED_DATE, model.ENTERED_TERMINAL, item.BATCH_NO })));
                        DTL_SLNO++;
                    }
                }
                else
                {
                    string chkApproved = _commonServices.GetMaximumNumber<string>(_configuration.GetConnectionString(db), "SELECT APPROVED_STATUS  FROM REFURBISHMENT_RECEIVING_MST WHERE MST_SLNO =:param1", _commonServices.AddParameter(new string[] {model.MST_SLNO.ToString() }));
                    if (chkApproved == "P")
                    {
                        string updateMstQuery() => @"UPDATE REFURBISHMENT_RECEIVING_MST SET
  RECEIVE_DATE = TO_DATE(:param3, 'DD/MM/YYYY HH:MI:SS AM'),
  RECEIVE_SHIFT = :param4,
  RECEIVE_CATEGORY = :param5,
  CUSTOMER_CODE = :param6,
  CUSTOMER_TYPE = :param7,
  CHALLAN_NUMBER = :param8,
  CHALLAN_DATE = TO_DATE(:param9, 'DD/MM/YYYY HH:MI:SS AM'),
  SENDING_CARTON_QTY = :param10,
  SENDING_BAG_QTY = :param11,
  SENDING_TOTAL_AMOUNT = :param12,
  RECEIVE_CARTON_QTY = :param13,
  RECEIVE_BAG_QTY = :param14,
  RECEIVE_TOTAL_AMOUNT = :param15,
  TSO_CODE = :param16,
  TSO_NAME = :param17,
  DRIVER_CODE = :param18,
  DRIVER_NAME = :param19,
  VEHICLE_NO = :param20,
  RECEIVED_BY = :param21,
  REMARKS = :param22,
  UPDATED_BY = :param23,
  UPDATED_DATE = TO_DATE(:param24, 'DD/MM/YYYY HH:MI:SS AM'),
  UPDATED_TERMINAL = :param25,
  DRIVER_CONTACT_NO = :param26
WHERE
   MST_SLNO = :param1 AND 
   CLAIM_NO = :param2";
                        string updateDtlQuery() => @"UPDATE STL_ERP_SDS.REFURBISHMENT_RECEIVING_DTL SET 
    REFURBISHMENT_PRODUCT_STATUS = :param3,
    PRODUCT_CODE = :param4,
    CLAIM_QTY = :param5,
    RECEIVED_QTY = :param6,
    DISPUTE_QTY = :param7,
    TRADE_PRICE = :param8,
    REVISED_PRICE = :param9,
    EXPIRY_DATE = TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'),
    REMARKS = :param11,
    UPDATED_BY = :param12,
    UPDATED_DATE = TO_DATE(:param13, 'DD/MM/YYYY HH:MI:SS AM'),
    UPDATED_TERMINAL = :param14,
    BATCH_NO= :param15
    WHERE DTL_SLNO = :param1 AND
      MST_SLNO = :param2";
                        string insertDtlQuery() => @"INSERT INTO STL_ERP_SDS.REFURBISHMENT_RECEIVING_DTL(DTL_SLNO, MST_SLNO, REFURBISHMENT_PRODUCT_STATUS, PRODUCT_CODE, CLAIM_QTY, RECEIVED_QTY, DISPUTE_QTY, TRADE_PRICE, REVISED_PRICE, EXPIRY_DATE, REMARKS, ENTERED_BY, ENTERED_DATE, ENTERED_TERMINAL,BATCH_NO) 
                                                        VALUES( :param1, :param2, :param3, :param4, :param5, :param6, :param7, :param8, :param9, TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'), :param11, :param12, TO_DATE(:param13, 'DD/MM/YYYY HH:MI:SS AM'), :param14, :param15)";
                        int DTL_SLNO = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), "SELECT NVL(MAX(DTL_SLNO),0) + 1 DTL_SLNO  FROM REFURBISHMENT_RECEIVING_DTL", _commonServices.AddParameter(new string[] { }));
                        listOfQuery.Add(_commonServices.AddQuery(updateMstQuery(), _commonServices.AddParameter(new string[] { model.MST_SLNO.ToString(), model.CLAIM_NO, model.RECEIVE_DATE, model.RECEIVE_SHIFT, model.RECEIVE_CATEGORY, model.CUSTOMER_CODE, model.CUSTOMER_TYPE, model.CHALLAN_NUMBER, model.CHALLAN_DATE, model.SENDING_CARTON_QTY.ToString(), model.SENDING_BAG_QTY.ToString(), model.SENDING_TOTAL_AMOUNT.ToString(), model.RECEIVE_CARTON_QTY.ToString(), model.RECEIVE_BAG_QTY.ToString(), model.RECEIVE_TOTAL_AMOUNT.ToString(), model.TSO_CODE, model.TSO_NAME, model.DRIVER_CODE, model.DRIVER_NAME, model.VEHICLE_NO, model.RECEIVED_BY, model.REMARKS, model.UPDATED_BY, model.UPDATED_DATE, model.UPDATED_TERMINAL,model.DRIVER_CONTACT_NO })));
                        foreach (var item in model.Details)
                        {
                            if (item.DTL_SLNO == 0)
                            {
                                item.DTL_SLNO = DTL_SLNO;
                                listOfQuery.Add(_commonServices.AddQuery(insertDtlQuery(),
                              _commonServices.AddParameter(new string[] { DTL_SLNO.ToString(), model.MST_SLNO.ToString(), item.REFURBISHMENT_PRODUCT_STATUS, item.PRODUCT_CODE, item.CLAIM_QTY.ToString(), item.RECEIVED_QTY.ToString(), item.DISPUTE_QTY.ToString(), item.TRADE_PRICE.ToString(), item.REVISED_PRICE.ToString(), item.EXPIRY_DATE, item.REMARKS, model.UPDATED_BY, model.UPDATED_DATE, model.UPDATED_TERMINAL, item.BATCH_NO })));
                                DTL_SLNO++;
                            }
                            else
                            {
                                listOfQuery.Add(_commonServices.AddQuery(updateDtlQuery(),
                               _commonServices.AddParameter(new string[] { item.DTL_SLNO.ToString(), model.MST_SLNO.ToString(), item.REFURBISHMENT_PRODUCT_STATUS, item.PRODUCT_CODE, item.CLAIM_QTY.ToString(), item.RECEIVED_QTY.ToString(), item.DISPUTE_QTY.ToString(), item.TRADE_PRICE.ToString(), item.REVISED_PRICE.ToString(), item.EXPIRY_DATE, item.REMARKS, model.UPDATED_BY, model.UPDATED_DATE, model.UPDATED_TERMINAL , item.BATCH_NO})));

                            }
                        }
                        string propertyList = string.Join(",", model.Details.Select(p => p.DTL_SLNO));
                        string deleteDtlQuery() => $"DELETE FROM REFURBISHMENT_RECEIVING_DTL WHERE MST_SLNO = :param1 AND DTL_SLNO NOT IN ({propertyList})";
                        listOfQuery.Add(_commonServices.AddQuery(deleteDtlQuery(), _commonServices.AddParameter(new string[] { model.MST_SLNO.ToString() })));
                    }
                    else
                    {
                        result.Status = "we regret to inform you that once you have confirmed your data, it cannot be updated or modified in our system";
                        result.Key = "";
                        result.Parent = "0";
                        return result;
                    }
                }
                await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                result.Key = model.CLAIM_NO;
                result.Parent = model.MST_SLNO.ToString();
                result.Status = "true";
                return result;
            }
            catch (Exception ex)
            {
                result.Status = ex.Message;
                result.Key = "";
                result.Parent = "0";
                return result;
            }
        }
        public async Task<Result> Approval(string db, RefurbishmentReceivingMst model)
        {
            Result result = new Result();
            try
            {
                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                static string approvalMstQuery() => @"UPDATE REFURBISHMENT_RECEIVING_MST SET APPROVED_STATUS = :param1,APPROVED_BY = :param2,APPROVED_DATE = TO_DATE ( :param3, 'DD/MM/YYYY HH:MI:SS AM') WHERE CLAIM_NO = :param4 AND MST_SLNO=:param5";
                listOfQuery.Add(_commonServices.AddQuery(approvalMstQuery(), _commonServices.AddParameter(new string[] { model.APPROVED_STATUS, model.APPROVED_BY, model.APPROVED_DATE, model.CLAIM_NO, model.MST_SLNO.ToString() })));
                await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                result.Key = model.CLAIM_NO;
                result.Parent = model.MST_SLNO.ToString();
                result.Status = "true";
                return result;
            }
            catch (Exception ex)
            {
                result.Status = ex.Message;
                result.Key = "";
                result.Parent = "0";
                return result;
            }
        }
        public async Task<string> GetDistList(string db,string unitId) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), @"SELECT DISTINCT CHALLAN_NO, TO_CHAR (CHALLAN_DATE, 'DD/MM/YYYY') CHALLAN_DATE, CUSTOMER_CODE, CUSTOMER_NAME, CUSTOMER_ADDRESS, TOTAL_AMOUNT, FACTORY_CODE, FACTORY_NAME, SEND_CARTON_QTY, SEND_BAG_QTY, DRIVER_CODE, DRIVER_NAME, DRIVER_CONTACT_NO, VEHICLE_NO FROM VW_REFURBISHMENT WHERE CASE WHEN FACTORY_CODE=3 THEN 2  ELSE FACTORY_CODE END = :param1 AND  CHALLAN_NO NOT IN (SELECT CHALLAN_NUMBER FROM REFURBISHMENT_RECEIVING_MST WHERE CUSTOMER_TYPE = 'Automation')", _commonServices.AddParameter(new string[] {unitId })));
        public async Task<string> GetManualDistList(string db, string searchKey) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), @"SELECT CUSTOMER_CODE, CUSTOMER_NAME,CUSTOMER_NAME || ' CODE:-' || CUSTOMER_CODE NAME_WITH_CODE, CUSTOMER_ADDRESS FROM CUSTOMER_INFO WHERE ROWNUM <= 50 AND CUSTOMER_STATUS = 'Active' AND CONCAT (UPPER (CUSTOMER_NAME), CUSTOMER_CODE) LIKE '%' || UPPER ( :param1) || '%' ORDER BY CUSTOMER_NAME ASC", _commonServices.AddParameter(new string[] { searchKey })));
        public async Task<string> GetManualProductList(string db,string companyId, string unitId, string searchKey) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), @"SELECT SKU_CODE, SKU_NAME, SKU_NAME || ' CODE:-' || SKU_CODE NAME_WITH_CODE, PACK_SIZE,FN_SKU_PRICE(SKU_ID , SKU_CODE   ,:param1 , :param2) TRADE_PRICE FROM PRODUCT_INFO WHERE     ROWNUM <= 50 AND PRODUCT_STATUS = 'Active' AND CONCAT (UPPER (SKU_NAME), SKU_CODE) LIKE '%' || UPPER ( :param3) || '%' ORDER BY SKU_NAME ASC", _commonServices.AddParameter(new string[] {companyId,unitId, searchKey })));
        public async Task<string> GetDistListWhileEdit(string db, string mstId) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), @"SELECT DISTINCT CHALLAN_NO,
                TO_CHAR(CHALLAN_DATE,'DD/MM/YYYY') CHALLAN_DATE,
                CUSTOMER_CODE,
                CUSTOMER_NAME,
                TOTAL_AMOUNT,
                FACTORY_CODE,
                FACTORY_NAME,
                SEND_CARTON_QTY,
                SEND_BAG_QTY,
                DRIVER_CODE,
                DRIVER_NAME,
                DRIVER_CONTACT_NO,
                VEHICLE_NO
  FROM VW_REFURBISHMENT
 WHERE CHALLAN_NO NOT IN (SELECT CHALLAN_NUMBER
                            FROM REFURBISHMENT_RECEIVING_MST WHERE CUSTOMER_TYPE='Automation')
UNION ALL
SELECT DISTINCT R.CHALLAN_NO,
                TO_CHAR(R.CHALLAN_DATE,'DD/MM/YYYY') CHALLAN_DATE,
                R.CUSTOMER_CODE,
                R.CUSTOMER_NAME,
                R.TOTAL_AMOUNT,
                R.FACTORY_CODE,
                R.FACTORY_NAME,
                R.SEND_CARTON_QTY,
                R.SEND_BAG_QTY,
                R.DRIVER_CODE,
                R.DRIVER_NAME,
                R.DRIVER_CONTACT_NO,
                R.VEHICLE_NO
  FROM VW_REFURBISHMENT R, REFURBISHMENT_RECEIVING_MST M
 WHERE     R.CHALLAN_NO = M.CHALLAN_NUMBER
       AND R.CUSTOMER_CODE = M.CUSTOMER_CODE
       AND M.MST_SLNO = :param1", _commonServices.AddParameter(new string[] { mstId })));
        public async Task<string> GetProductsByChallan(string db, string challanNo) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), @"SELECT  CHALLAN_NO, CHALLAN_DATE,PRODUCT_CODE, PRODUCT_NAME, PACK_SIZE, BATCH_NO,TP TRADE_PRICE,TP REVISED_PRICE, SUM(QTY) CLAIM_QTY,SUM(QTY) RECEIVE_QTY,SUM(AMOUNT) AMOUNT,0 RECEIVED_QTY,0 DISPUTE_QTY FROM VW_REFURBISHMENT WHERE CHALLAN_NO = :param1 GROUP BY  CHALLAN_NO, CHALLAN_DATE,PRODUCT_CODE, PRODUCT_NAME, PACK_SIZE, BATCH_NO,TP ", _commonServices.AddParameter(new string[] { challanNo })));
        public async Task<string> LoadDtlByMstId(string db, string mstId) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), @"SELECT DTL_SLNO, MST_SLNO, REFURBISHMENT_PRODUCT_STATUS, PRODUCT_CODE, FN_PACK_SIZE_BY_CODE(1,PRODUCT_CODE) PACK_SIZE, FN_SKU_NAME_BY_CODE(1,PRODUCT_CODE) PRODUCT_NAME, CLAIM_QTY, RECEIVED_QTY, DISPUTE_QTY, TRADE_PRICE, REVISED_PRICE, TO_CHAR(EXPIRY_DATE,'DD/MM/YYYY') EXPIRY_DATE, REMARKS, REVISED_PRICE*RECEIVED_QTY AS VALUE, BATCH_NO FROM REFURBISHMENT_RECEIVING_DTL WHERE MST_SLNO = :param1", _commonServices.AddParameter(new string[] { mstId })));
        public async Task<string> GetMstList(string db, string companyId, string unitId, string fromDate, string toDate) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), @"SELECT M.MST_SLNO, M.CLAIM_NO, TO_CHAR (M.RECEIVE_DATE, 'DD/MM/YYYY') RECEIVE_DATE, M.RECEIVE_SHIFT, M.RECEIVE_CATEGORY, M.CUSTOMER_CODE, C.CUSTOMER_NAME, C.CUSTOMER_ADDRESS, M.CUSTOMER_TYPE, M.CHALLAN_NUMBER, TO_CHAR (M.CHALLAN_DATE, 'DD/MM/YYYY') CHALLAN_DATE, M.SENDING_CARTON_QTY, M.SENDING_BAG_QTY, M.SENDING_TOTAL_AMOUNT, M.RECEIVE_CARTON_QTY, M.RECEIVE_BAG_QTY, M.RECEIVE_TOTAL_AMOUNT, M.TSO_CODE, M.TSO_NAME, M.DRIVER_CODE, M.DRIVER_NAME, M.DRIVER_CONTACT_NO, M.VEHICLE_NO, M.RECEIVED_BY, M.REMARKS, FN_USER_NAME (1, M.ENTERED_BY) ENTERED_BY, TO_CHAR (M.ENTERED_DATE, 'DD/MM/YYYY HH12:MI:SS PM') ENTERED_DATE, M.APPROVED_STATUS, CASE WHEN M.APPROVED_STATUS = 'A' THEN 'Approved' ELSE 'Pending' END AS APPROVED_MGS FROM REFURBISHMENT_RECEIVING_MST M, CUSTOMER_INFO C WHERE     M.COMPANY_ID = :param1 AND M.UNIT_ID = :param2 AND M.CUSTOMER_CODE = C.CUSTOMER_CODE AND TRUNC (M.ENTERED_DATE) BETWEEN TO_DATE ( :param3, 'DD/MM/YYYY') AND TO_DATE ( :param4, 'DD/MM/YYYY') ORDER BY M.MST_SLNO DESC", _commonServices.AddParameter(new string[] {companyId,unitId, fromDate, toDate })));
        public async Task<string> GetClaims(string db,string companyId, string unitId) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), @"SELECT TO_CHAR (M.RECEIVE_DATE, 'DD/MM/YYYY') RECEIVE_DATE, M.RECEIVE_CATEGORY, M.CUSTOMER_TYPE, M.CLAIM_NO, M.CUSTOMER_CODE, CI.CUSTOMER_NAME, CI.CUSTOMER_ADDRESS, M.CHALLAN_NUMBER, TO_CHAR (M.CHALLAN_DATE, 'DD/MM/YYYY') CHALLAN_DATE FROM REFURBISHMENT_RECEIVING_MST M, CUSTOMER_INFO CI WHERE     M.COMPANY_ID = :param1 AND M.UNIT_ID = :param2 AND M.CUSTOMER_CODE = CI.CUSTOMER_CODE AND CLAIM_NO NOT IN (SELECT CLAIM_NO FROM REFURBISHMENT_FINALIZE_MST) AND M.APPROVED_STATUS = 'A' ORDER BY M.MST_SLNO DESC", _commonServices.AddParameter(new string[] {companyId,unitId })));
        public async Task<string> GetProductsByClaimNo(string db, string claimNo) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), @"SELECT D.DTL_SLNO, D.MST_SLNO, D.REFURBISHMENT_PRODUCT_STATUS, D.PRODUCT_CODE, FN_PACK_SIZE_BY_CODE ( M.COMPANY_ID, D.PRODUCT_CODE) PACK_SIZE, FN_SKU_NAME_BY_CODE ( M.COMPANY_ID, D.PRODUCT_CODE) PRODUCT_NAME, D.CLAIM_QTY, D.RECEIVED_QTY, D.DISPUTE_QTY, D.TRADE_PRICE, D.REVISED_PRICE, TO_CHAR (D.EXPIRY_DATE, 'DD/MM/YYYY') EXPIRY_DATE, D.REMARKS, D.REVISED_PRICE * D.RECEIVED_QTY AS VALUE, FN_CURRENT_STOCK_QTY_BY_CODE ( M.COMPANY_ID,M.UNIT_ID, D.PRODUCT_CODE)  PASSED_QTY FROM REFURBISHMENT_RECEIVING_DTL D, REFURBISHMENT_RECEIVING_MST M WHERE     M.MST_SLNO = D.MST_SLNO AND M.CLAIM_NO = :param1 AND M.APPROVED_STATUS = 'A' AND D.REFURBISHMENT_PRODUCT_STATUS = 'Refurbishment'", _commonServices.AddParameter(new string[] { claimNo })));
        public async Task<Result> AddOrUpdateFinalization(string db, RefurbishmentFinalizeMaster model)
        {
            Result result = new Result();
            try
            {
                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                if (model.MST_SLNO == 0)
                {
                    string insertMstQuery() => @"INSERT INTO REFURBISHMENT_FINALIZE_MST(
  MST_SLNO,
  FINALIZE_DATE,
  FINALIZE_SHIFT,
  CUSTOMER_CODE,
  CLAIM_NO,
  RECEIVE_CATEGORY,
  TOTAL_AMOUNT,
  REMARKS,
  ENTERED_BY,
  ENTERED_DATE,
  ENTERED_TERMINAL,
  COMPANY_ID,
  UNIT_ID
)
VALUES
(
  :param1,
  TO_DATE(:param2, 'DD/MM/YYYY HH:MI:SS AM'),
  :param3,
  :param4,
  :param5,
  :param6,
  :param7,
  :param8,
  :param9,
  TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'),
  :param11,:param12,:param13)";
                    string insertDtlQuery() => @"INSERT INTO REFURBISHMENT_FINALIZE_DTL(
  DTL_SLNO,
  MST_SLNO,
  PRODUCT_CODE,
  TRADE_PRICE,
  PROD_QTY,
  AMOUNT,
  REMARKS,
  ENTERED_BY,
  ENTERED_DATE,
  ENTERED_TERMINAL
)
VALUES
(
  :param1,
  :param2, 
  :param3,
  :param4, 
  :param5,
  :param6,
  :param7,
  :param8,
  TO_DATE(:param9, 'DD/MM/YYYY HH:MI:SS AM'),
  :param10)";
                    string insertDtlRcvQuery() => @"INSERT INTO STL_ERP_SDS.REFURBISHMENT_FINALIZE_RECV(
  DTL_SLNO,
  MST_SLNO,
  PRODUCT_CODE,
  TRADE_PRICE,
  PROD_QTY,
  AMOUNT,
  REMARKS,
  ENTERED_BY,
  ENTERED_DATE,
  ENTERED_TERMINAL
)
VALUES
(
  :param1,
  :param2, 
  :param3, 
  :param4,
  :param5,
  :param6,
  :param7, 
  :param8, 
  TO_DATE(:param9, 'DD/MM/YYYY HH:MI:SS AM'),
  :param10)";
                   //-------------Add to master table--------------------
                    model.MST_SLNO = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), "SELECT NVL(MAX(MST_SLNO),0) + 1 MST_SLNO  FROM REFURBISHMENT_FINALIZE_MST", _commonServices.AddParameter(new string[] { }));
                    int DTL_SLNO = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), "SELECT NVL(MAX(DTL_SLNO),0) + 1 DTL_SLNO  FROM REFURBISHMENT_FINALIZE_DTL", _commonServices.AddParameter(new string[] { }));
                    int DTL_SLNO_RCV = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), "SELECT NVL(MAX(DTL_SLNO),0) + 1 DTL_SLNO  FROM REFURBISHMENT_FINALIZE_RECV", _commonServices.AddParameter(new string[] { }));
                    listOfQuery.Add(_commonServices.AddQuery(insertMstQuery(), _commonServices.AddParameter(new string[] { model.MST_SLNO.ToString(), model.FINALIZE_DATE, model.FINALIZE_SHIFT, model.CUSTOMER_CODE, model.CLAIM_NO, model.RECEIVE_CATEGORY, model.TOTAL_AMOUNT.ToString(), model.REMARKS, model.ENTERED_BY, model.ENTERED_DATE, model.ENTERED_TERMINAL,model.COMPANY_ID, model.UNIT_ID})));
                    foreach (var item in model.Details)
                    {
                        listOfQuery.Add(_commonServices.AddQuery(insertDtlQuery(),_commonServices.AddParameter(new string[] { DTL_SLNO.ToString(), model.MST_SLNO.ToString(), item.PRODUCT_CODE, item.REVISED_PRICE.ToString(), item.PROD_QTY.ToString(), item.AMOUNT.ToString(), item.REMARKS, model.ENTERED_BY, model.ENTERED_DATE, model.ENTERED_TERMINAL })));
                        DTL_SLNO++;
                    }
                    foreach (var item in model.RcvDetails)
                    {
                        listOfQuery.Add(_commonServices.AddQuery(insertDtlRcvQuery(),
                            _commonServices.AddParameter(new string[] { DTL_SLNO_RCV.ToString(), model.MST_SLNO.ToString(), item.PRODUCT_CODE, item.REVISED_PRICE.ToString(), item.RECEIVED_QTY.ToString(), item.VALUE.ToString(), item.REMARKS, model.ENTERED_BY, model.ENTERED_DATE, model.ENTERED_TERMINAL })));
                        DTL_SLNO_RCV++;
                    }
                }
                else
                {
                    string chkApproved = _commonServices.GetMaximumNumber<string>(_configuration.GetConnectionString(db), "SELECT APPROVED_STATUS  FROM REFURBISHMENT_FINALIZE_MST WHERE MST_SLNO =:param1", _commonServices.AddParameter(new string[] { model.MST_SLNO.ToString() }));
                    if(chkApproved =="P") { 
                    string updateMstQuery() => @"UPDATE STL_ERP_SDS.REFURBISHMENT_FINALIZE_MST
SET FINALIZE_DATE = TO_DATE(:param2, 'DD/MM/YYYY HH:MI:SS AM'),
    FINALIZE_SHIFT = :param3,
    CUSTOMER_CODE = :param4,
    CLAIM_NO = :param5,
    RECEIVE_CATEGORY = :param6,
    TOTAL_AMOUNT = :param7,
    REMARKS = :param8,
    UPDATED_BY = :param9,
    UPDATED_DATE = TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'),
    UPDATED_TERMINAL = :param11
WHERE MST_SLNO = :param1";
                    string updateDtlQuery() => @"UPDATE STL_ERP_SDS.REFURBISHMENT_FINALIZE_DTL
SET PRODUCT_CODE = :param3,
    TRADE_PRICE = :param4,
    PROD_QTY = :param5,
    AMOUNT = :param6,
    REMARKS = :param7,
    UPDATED_BY = :param8,
    UPDATED_DATE = TO_DATE(:param9, 'DD/MM/YYYY HH:MI:SS AM'),
    UPDATED_TERMINAL = :param10
WHERE DTL_SLNO = :param1
  AND MST_SLNO = :param2";
                    string insertDtlQuery() => @"INSERT INTO REFURBISHMENT_FINALIZE_DTL(
  DTL_SLNO,
  MST_SLNO,
  PRODUCT_CODE,
  TRADE_PRICE,
  PROD_QTY,
  AMOUNT,
  REMARKS,
  ENTERED_BY,
  ENTERED_DATE,
  ENTERED_TERMINAL
)
VALUES
(
  :param1,
  :param2, 
  :param3,
  :param4, 
  :param5,
  :param6,
  :param7,
  :param8,
  TO_DATE(:param9, 'DD/MM/YYYY HH:MI:SS AM'),
  :param10)";
                    int DTL_SLNO = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), "SELECT NVL(MAX(DTL_SLNO),0) + 1 DTL_SLNO  FROM REFURBISHMENT_RECEIVING_DTL", _commonServices.AddParameter(new string[] { }));
                    listOfQuery.Add(_commonServices.AddQuery(updateMstQuery(), _commonServices.AddParameter(new string[] { model.MST_SLNO.ToString(), model.FINALIZE_DATE, model.FINALIZE_SHIFT, model.CUSTOMER_CODE, model.CLAIM_NO, model.RECEIVE_CATEGORY, model.TOTAL_AMOUNT.ToString(), model.REMARKS, model.UPDATED_BY, model.UPDATED_DATE, model.UPDATED_TERMINAL })));
                    foreach (var item in model.Details)
                    {
                        if (item.DTL_SLNO == 0)
                        {
                            item.DTL_SLNO = DTL_SLNO;
                            listOfQuery.Add(_commonServices.AddQuery(insertDtlQuery(), _commonServices.AddParameter(new string[] { DTL_SLNO.ToString(), model.MST_SLNO.ToString(), item.PRODUCT_CODE, item.REVISED_PRICE.ToString(), item.PROD_QTY.ToString(), item.AMOUNT.ToString(), item.REMARKS, model.UPDATED_BY, model.UPDATED_DATE, model.UPDATED_TERMINAL })));
                            DTL_SLNO++;
                        }
                        else
                        {
                            listOfQuery.Add(_commonServices.AddQuery(updateDtlQuery(),
                           _commonServices.AddParameter(new string[] { item.DTL_SLNO.ToString(), model.MST_SLNO.ToString(), item.PRODUCT_CODE, item.REVISED_PRICE.ToString(), item.PROD_QTY.ToString(), item.AMOUNT.ToString(), item.REMARKS, model.UPDATED_BY, model.UPDATED_DATE, model.UPDATED_TERMINAL })));

                        }
                    }
                    string propertyList = string.Join(",", model.Details.Select(p => p.DTL_SLNO));
                    string deleteDtlQuery() => $"DELETE FROM REFURBISHMENT_FINALIZE_DTL WHERE MST_SLNO = :param1 AND DTL_SLNO NOT IN ({propertyList})";
                    listOfQuery.Add(_commonServices.AddQuery(deleteDtlQuery(), _commonServices.AddParameter(new string[] { model.MST_SLNO.ToString() })));
                    }
                    else
                    {
                        result.Status = "we regret to inform you that once you have confirmed your data, it cannot be updated or modified in our system";
                        result.Key = "";
                        result.Parent = "0";
                        return result;
                    }
                }

                await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                result.Key = model.CLAIM_NO;
                result.Parent = model.MST_SLNO.ToString();
                result.Status = "true";
                return result;
            }
            catch (Exception ex)
            {
                result.Status = ex.Message;
                result.Key = "";
                result.Parent = "0";
                return result;
            }
        }
        public async Task<string> GetFinalizationMstList(string db, string companyId, string unitId,string fromDate, string toDate) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), @"SELECT F.MST_SLNO, F.CUSTOMER_CODE, F.CLAIM_NO, F.RECEIVE_CATEGORY, F.TOTAL_AMOUNT, F.REMARKS, F.ENTERED_BY, F.ENTERED_DATE, TO_CHAR (F.FINALIZE_DATE, 'DD/MM/YYYY') FINALIZE_DATE, FN_CUS_NAME_BY_CODE (F.COMPANY_ID, F.CUSTOMER_CODE) CUSTOMER_NAME, F.APPROVED_STATUS, CASE WHEN F.APPROVED_STATUS = 'A' THEN 'Approved' ELSE 'Pending' END AS APPROVED_MGS, TO_CHAR (M.RECEIVE_DATE, 'DD/MM/YYYY') RECEIVE_DATE, M.CUSTOMER_TYPE, CI.DB_LOCATION_NAME CUSTOMER_ADDRESS, M.CHALLAN_NUMBER, TO_CHAR (M.CHALLAN_DATE, 'DD/MM/YYYY') CHALLAN_DATE FROM REFURBISHMENT_FINALIZE_MST F, REFURBISHMENT_RECEIVING_MST M, CUSTOMER_INFO CI WHERE     F.CLAIM_NO = M.CLAIM_NO AND M.CUSTOMER_CODE = CI.CUSTOMER_CODE AND TRUNC (F.ENTERED_DATE) BETWEEN TO_DATE ( :param1, 'DD/MM/YYYY') AND TO_DATE ( :param2, 'DD/MM/YYYY') AND F.COMPANY_ID = :param3 AND F.UNIT_ID = :param4", _commonServices.AddParameter(new string[] { fromDate, toDate,companyId,unitId })));
        public async Task<string> GetClaimsWhileEdit(string db, string mstId) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), @"SELECT TO_CHAR (M.RECEIVE_DATE, 'DD/MM/YYYY') RECEIVE_DATE,
       M.RECEIVE_CATEGORY,
       M.CUSTOMER_TYPE,
       M.CLAIM_NO,
       M.CUSTOMER_CODE,
       CI.CUSTOMER_NAME,
       CI.DB_LOCATION_NAME CUSTOMER_ADDRESS,
       M.CHALLAN_NUMBER,
       TO_CHAR (M.CHALLAN_DATE, 'DD/MM/YYYY') CHALLAN_DATE
  FROM REFURBISHMENT_RECEIVING_MST M, CUSTOMER_INFO CI
 WHERE     M.CUSTOMER_CODE = CI.CUSTOMER_CODE
       AND CLAIM_NO NOT IN (SELECT CLAIM_NO FROM REFURBISHMENT_FINALIZE_MST)
       AND M.APPROVED_STATUS = 'A'
UNION ALL
SELECT TO_CHAR (M.RECEIVE_DATE, 'DD/MM/YYYY') RECEIVE_DATE,
       M.RECEIVE_CATEGORY,
       M.CUSTOMER_TYPE,
       M.CLAIM_NO,
       M.CUSTOMER_CODE,
       CI.CUSTOMER_NAME,
       CI.DB_LOCATION_NAME CUSTOMER_ADDRESS,
       M.CHALLAN_NUMBER,
       TO_CHAR (M.CHALLAN_DATE, 'DD/MM/YYYY') CHALLAN_DATE
  FROM REFURBISHMENT_RECEIVING_MST M, CUSTOMER_INFO CI, REFURBISHMENT_FINALIZE_MST F
 WHERE     M.CUSTOMER_CODE = CI.CUSTOMER_CODE AND F.CLAIM_NO= M.CLAIM_NO
       AND M.APPROVED_STATUS = 'A'
       AND F.MST_SLNO=:param1", _commonServices.AddParameter(new string[] { mstId })));
        public async Task<string> loadFinalizationDtlByMstId(string db,string companyId,string unitId, string mstId) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), @"SELECT FN_PACK_SIZE_BY_CODE (:param1, PRODUCT_CODE) PACK_SIZE, FN_SKU_NAME_BY_CODE (:param2, PRODUCT_CODE) PRODUCT_NAME, DTL_SLNO, MST_SLNO, PRODUCT_CODE, TRADE_PRICE, TRADE_PRICE AS REVISED_PRICE, PROD_QTY, AMOUNT, REMARKS,FN_CURRENT_STOCK_QTY_BY_CODE ( :param3,:param4, PRODUCT_CODE) PASSED_QTY FROM REFURBISHMENT_FINALIZE_DTL WHERE MST_SLNO = :param5", _commonServices.AddParameter(new string[] { companyId, companyId,companyId, unitId, mstId })));
        public async Task<Result> FinalizationApproval(string db, RefurbishmentFinalizeMaster model)
        {
            Result result = new Result();
            try
            {
                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                static string approvalMstQuery() => @"UPDATE REFURBISHMENT_FINALIZE_MST SET APPROVED_STATUS = :param1,APPROVED_BY = :param2,APPROVED_DATE = TO_DATE ( :param3, 'DD/MM/YYYY HH:MI:SS AM') WHERE CLAIM_NO = :param4 AND MST_SLNO=:param5";
                listOfQuery.Add(_commonServices.AddQuery(approvalMstQuery(), _commonServices.AddParameter(new string[] { model.APPROVED_STATUS, model.APPROVED_BY, model.APPROVED_DATE, model.CLAIM_NO, model.MST_SLNO.ToString() })));
                await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                result.Key = model.CLAIM_NO;
                result.Parent = model.MST_SLNO.ToString();
                result.Status = "true";
                return result;
            }
            catch (Exception ex)
            {
                result.Status = ex.Message;
                result.Key = "";
                result.Parent = "0";
                return result;
            }
        }
        public async Task<string> GetManualProductsWithStock(string db,string companyId, string unitId, string searchKey) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), @"
  SELECT P.SKU_CODE,
         P.SKU_NAME,
         P.SKU_NAME || ' CODE:-' || P.SKU_CODE NAME_WITH_CODE,
         P.PACK_SIZE,
         FN_SKU_PRICE (P.SKU_ID,
                       P.SKU_CODE,
                       :param1,
                       :param2)
            TRADE_PRICE,
         FN_SKU_PRICE (P.SKU_ID,
                       P.SKU_CODE,
                       :param3,
                       :param4)
            REVISED_PRICE,
         SUM (NVL (B.PASSED_QTY, 0)) PASSED_QTY
    FROM PRODUCT_INFO P, BATCH_WISE_STOCK B
   WHERE     CONCAT (UPPER (P.SKU_NAME), P.SKU_CODE) LIKE
                '%' || UPPER ( :param5) || '%'
         AND PRODUCT_STATUS = 'Active'
         AND P.SKU_CODE = B.SKU_CODE
         AND B.COMPANY_ID = :param6
         AND B.UNIT_ID = :param7
GROUP BY P.SKU_ID,
         P.SKU_CODE,
         P.SKU_NAME,
         P.PACK_SIZE
  HAVING SUM (NVL (B.PASSED_QTY, 0)) > 0
ORDER BY P.SKU_NAME ASC", _commonServices.AddParameter(new string[] {companyId, unitId, companyId, unitId,searchKey, companyId, unitId })));
    }
}
