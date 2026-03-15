using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.Inventory.Manager.IManager;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Inventory.Manager
{
    public class RequisitionReceiveManager : IRequisitionReceiveManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public RequisitionReceiveManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }
        string AddOrUpdate_AddQuery() => @"INSERT INTO DEPOT_REQUISITION_RCV_MST 
                                        (
                                            MST_ID
                                            ,REQUISITION_UNIT_ID
                                            ,ISSUE_UNIT_ID
                                        
                                            ,RECEIVE_NO
                                            ,RECEIVE_DATE
                                            ,ISSUE_NO
                                            ,ISSUE_DATE
                                            ,REQUISITION_NO
                                            ,ISSUE_AMOUNT
                                            ,RECEIVE_AMOUNT
                                            ,RECEIVE_BY
                                            ,STATUS
                                            ,COMPANY_ID
                                            ,REMARKS
                                            ,ENTERED_BY
                                            ,ENTERED_DATE
                                            ,ENTERED_TERMINAL
                                            ,DISPATCH_NO
                                            
                                       
                                         ) 
                                          VALUES ( :param1, :param2, :param3, :param4,TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM'),:param6,TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'),:param8,  :param9, :param10,:param11,:param12,:param13, :param14,:param15,TO_DATE(:param16, 'DD/MM/YYYY HH:MI:SS AM'), :param17,:param18)";

        string AddOrUpdate_AddQueryBatch() => @"INSERT INTO DEPOT_REQUISITION_RCV_BATCH 
                                        (
                                            RECEIVE_BATCH_ID
                                            ,DTL_ID
                                            ,MST_ID
                                            ,RECEIVE_NO
                                            ,RECEIVE_DATE
                                            ,ISSUE_NO
                                            ,REQUISITION_NO
                                            ,ISSUE_UNIT_ID
                                            ,SKU_ID
                                            ,SKU_CODE
                                            ,UNIT_TP
                                            ,BATCH_ID
                                            ,BATCH_NO
                                            ,RECEIVE_QTY
                                            ,RECEIVE_AMOUNT
                                            ,STATUS
                                            ,COMPANY_ID
                                            ,REMARKS
                                            ,ENTERED_BY
                                            ,ENTERED_DATE
                                            ,ENTERED_TERMINAL
                                         ) 
                                          VALUES ( :param1, :param2, :param3, :param4,TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM'),:param6,:param7, :param8, :param9, :param10,:param11,:param12,:param13, :param14,:param15, :param16,:param17,:param18,:param19,:param20,TO_DATE(:param21, 'DD/MM/YYYY HH:MI:SS AM'),:param22)";
        string AddOrUpdate_UpdateQuery() => @"UPDATE DEPOT_REQUISITION_RCV_MST SET  
                                                 REQUISITION_UNIT_ID= :param2
                                                ,ISSUE_UNIT_ID= :param3
                                       
                                                ,RECEIVE_NO= :param4
                                                ,RECEIVE_DATE= TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM')
                                                ,ISSUE_NO= :param6
                                                ,ISSUE_DATE= TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM')
                                                ,REQUISITION_NO= :param8
                                                ,ISSUE_AMOUNT= :param9
                                                ,RECEIVE_AMOUNT= :param10
                                                ,RECEIVE_BY= :param11
                                                ,STATUS= :param12
                                                ,COMPANY_ID= :param13
                                                ,REMARKS= :param14
                                                ,UPDATED_BY= :param15
                                                ,UPDATED_DATE= TO_DATE(:param16, 'DD/MM/YYYY HH:MI:SS AM')
                                                ,UPDATED_TERMINAL= :param17
                                                ,DISPATCH_NO=:param18
                                                WHERE MST_ID = :param1";
        string AddOrUpdate_AddQueryDTL() => @"INSERT INTO DEPOT_REQUISITION_RCV_DTL 
                    (   DTL_ID
                        ,MST_ID
                        ,SKU_ID
                        ,SKU_CODE
                        ,UNIT_TP
                        ,ISSUE_QTY
                        ,ISSUE_AMOUNT
                        ,RECEIVE_QTY
                        ,RECEIVE_AMOUNT
                        ,STATUS
                        ,COMPANY_ID
                        ,REMARKS
                        ,ENTERED_BY
                        ,ENTERED_DATE
                        ,ENTERED_TERMINAL
                   ) 
                    VALUES ( :param1, :param2, :param3, :param4, :param5, :param6, :param7, :param8, :param9,:param10,:param11,:param12, :param13,TO_DATE(:param14, 'DD/MM/YYYY HH:MI:SS AM'), :param15)";
        string AddOrUpdate_UpdateQueryDTL() => @"UPDATE DEPOT_REQUISITION_RCV_DTL SET  
                                                 SKU_ID= :param2
                                                ,SKU_CODE= :param3
                                                ,UNIT_TP= :param4
                                                ,ISSUE_QTY= :param5
                                                ,ISSUE_AMOUNT= :param6
                                                ,RECEIVE_QTY= :param7
                                                ,RECEIVE_AMOUNT= :param8
                                                ,STATUS= :param9
                                                ,COMPANY_ID= :param10
                                                ,REMARKS= :param11
                                                ,UPDATED_BY= :param12
                                                ,UPDATED_DATE=  TO_DATE(:param13, 'DD/MM/YYYY HH:MI:SS AM')
                                                ,UPDATED_TERMINAL= :param14
                                                WHERE DTL_ID = :param1";
        string LoadMasterData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO, M.MST_ID                                        
                                            ,M.REQUISITION_UNIT_ID
                                            ,M.ISSUE_UNIT_ID
                                            ,M.REQUISITION_NO
                                            ,TO_CHAR(M.RECEIVE_DATE, 'DD/MM/YYYY') RECEIVE_DATE
                                            ,M.ISSUE_NO
                                            ,M.RECEIVE_NO
                                            ,TO_CHAR(M.ISSUE_DATE, 'DD/MM/YYYY') ISSUE_DATE
                                      
                                            ,M.ISSUE_AMOUNT
                                            ,M.RECEIVE_AMOUNT
                                            ,M.RECEIVE_BY
                                            ,M.STATUS
                                            ,M.COMPANY_ID
                                            ,M.REMARKS
                                            ,M.ENTERED_BY
                                            ,M.ENTERED_DATE
                                            ,M.ENTERED_TERMINAL
                                            ,FN_UNIT_NAME(M.COMPANY_ID,M.ISSUE_UNIT_ID) ISSUE_UNIT_NAME
                                            ,FN_UNIT_NAME(M.COMPANY_ID,M.REQUISITION_UNIT_ID) REQUISITION_UNIT_NAME
            FROM DEPOT_REQUISITION_RCV_MST  M 
            where  M.COMPANY_ID = :param1 and M.REQUISITION_NO NOT IN (SELECT REQUISITION_NO FROM DEPOT_REQUISITION_RETURN_MST) ORDER BY M.MST_ID DESC";

        string LoadData_MasterById_Query() => @"
            SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO, M.MST_ID
                                            ,M.REQUISITION_UNIT_ID
                                            ,M.ISSUE_UNIT_ID
                                
                                            ,M.REQUISITION_NO
                                            ,TO_CHAR(M.RECEIVE_DATE, 'DD/MM/YYYY') RECEIVE_DATE
                                            ,M.ISSUE_NO
                                            ,M.RECEIVE_NO
                                            ,TO_CHAR(M.ISSUE_DATE, 'DD/MM/YYYY') ISSUE_DATE
                                      
                                            ,M.ISSUE_AMOUNT
                                            ,M.RECEIVE_AMOUNT
                                            ,M.RECEIVE_BY
                                            ,M.STATUS
                                            ,M.COMPANY_ID
                                            ,M.REMARKS
                                            ,M.ENTERED_BY
                                            ,M.ENTERED_DATE
                                            ,M.ENTERED_TERMINAL
                                            ,(SELECT  UNIT_NAME FROM COMPANY_INFO CI WHERE M.REQUISITION_UNIT_ID = CI.UNIT_ID AND ROWNUM = 1) REQUISITION_UNIT_NAME               
            FROM DEPOT_REQUISITION_RCV_MST  M 
            where M.MST_ID = :param1";

        string LoadData_DetailByMasterId_Query() => @" SELECT ROW_NUMBER () OVER (ORDER BY B.DTL_ID ASC) AS ROW_NO,
                                                       B.* ,
                                                       C.PACK_SIZE, 
 
                                                       B.RECEIVE_QTY RECEIVE_QTY 
                                                FROM DEPOT_REQUISITION_RCV_MST A, DEPOT_REQUISITION_RCV_DTL B,PRODUCT_INFO C
                                                WHERE A.MST_ID        = B.MST_ID
                                                AND   B.SKU_ID        = C.SKU_ID
                                                AND   A.MST_ID        = :param1";
        string LoadData_DetailWithStock_ByMasterId_Query() => @" SELECT  ROW_NUMBER () OVER (ORDER BY DTL_ID ASC) AS ROW_NO,COMPANY_ID,
                                                                   FN_COMPANY_NAME(COMPANY_ID) COMPANY_NAME,
                                                                   SKU_ID,
                                                                   SKU_CODE,
                                                                   FN_SKU_NAME(COMPANY_ID,SKU_ID) SKU_NAME,
                                                                   FN_PACK_SIZE(COMPANY_ID,SKU_ID) PACK_SIZE,       
                                                                   UNIT_TP,
                                                                   SUM(NVL(RECEIVE_QTY,0)) RECEIVE_QTY,
                                                                     SUM(NVL(RECEIVE_AMOUNT,0)) RECEIVE_AMOUNT,
                                                                   SUM(NVL(STOCK_QTY,0)) STOCK_QTY 
                                                            FROM
                                                               (   
                                                                SELECT A.COMPANY_ID,
                                                                       B.SKU_ID,
                                                                       B.SKU_CODE,
                                                                       B.UNIT_TP,
                                                                        B.DTL_ID,
                                                                       C.BATCH_ID,
                                                                       NVL(C.RECEIVE_QTY,0) RECEIVE_QTY,
                                                                        NVL(  C.RECEIVE_AMOUNT,0) RECEIVE_AMOUNT,
                                                                       NVL(D.PASSED_QTY,0) STOCK_QTY 
                                                                  FROM DEPOT_REQUISITION_RCV_MST A,
                                                                       DEPOT_REQUISITION_RCV_DTL B,
                                                                       DEPOT_REQUISITION_RCV_BATCH C,
                                                                       BATCH_WISE_STOCK D
                                                                 WHERE  A.MST_ID          = B.MST_ID 
                                                                 AND    B.DTL_ID          = C.DTL_ID
                                                                 AND    A.COMPANY_ID      = D.COMPANY_ID
                                                                 AND    A.REQUISITION_UNIT_ID = D.UNIT_ID
                                                                 AND    C.BATCH_ID        = D.BATCH_ID
                                                                 AND    C.SKU_ID          = D.SKU_ID 
                                                                 AND    A.MST_ID          = :param1   
   
                                                               )
                                                            GROUP BY COMPANY_ID,SKU_ID,SKU_CODE,UNIT_TP,DTL_ID";


        string LoadData_IssueBatch_Query() => @" select * from DEPOT_REQUISITION_ISSUE_BATCH where Requisition_No = :param1 and Sku_Id=:param2";

        string Get_LastReceive_no() => "SELECT  RECEIVE_NO  FROM DEPOT_REQUISITION_RCV_MST Where  MST_ID = (SELECT   NVL(MAX(MST_ID),0) MST_ID From DEPOT_REQUISITION_RCV_MST where COMPANY_ID = :param1 )";
        string GetRequisition_Batch_IdQuery() => "SELECT NVL(MAX(RECEIVE_BATCH_ID),0) + 1 RECEIVE_BATCH_ID  FROM DEPOT_REQUISITION_RCV_BATCH";
        string GetRequisition_Mst_IdQuery() => "SELECT NVL(MAX(MST_ID),0) + 1 MST_ID  FROM DEPOT_REQUISITION_RCV_MST";
        string GetRequisition_Rcv_Dtl_IdQuery() => "SELECT NVL(MAX(DTL_ID),0) + 1 DTL_ID  FROM DEPOT_REQUISITION_RCV_DTL";
        public async Task<string> GenerateReceiveCode(string db, string Company_Id)
        {
            try
            {
                string code;
                DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Get_LastReceive_no(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
                if (dataTable.Rows.Count > 0)
                {
                    string serial = (Convert.ToInt32(dataTable.Rows[0]["RECEIVE_NO"].ToString().Substring(4, 4)) + 1).ToString();
                    int serial_length = serial.Length;
                    code = DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM");
                    for (int i = 0; i < (CodeConstants.Requisition_No_CodeLength - (serial_length + 1)); i++)
                    {
                        code += "0";
                    }
                    code += serial;
                }
                else
                {
                    code = DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") + "0001";
                }
                return code;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> AddOrUpdate(string db, DEPOT_REQUISITION_RCV_MST model)
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

                    int dtlId = 0;
                    if (model.MST_ID == 0)
                    {

                        model.MST_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetRequisition_Mst_IdQuery(), _commonServices.AddParameter(new string[] { }));
                 
                        model.RECEIVE_NO = _commonServices.GenerateRequisitionCode(_configuration.GetConnectionString(db), "FN_GENERATE_REQ_RCV_NO", model.COMPANY_ID.ToString(), model.REQUISITION_UNIT_ID.ToString());

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(),
                         _commonServices.AddParameter(new string[] {model.MST_ID.ToString(),
                             model.REQUISITION_UNIT_ID.ToString(), model.ISSUE_UNIT_ID.ToString(),
                             model.RECEIVE_NO,model.RECEIVE_DATE,model.ISSUE_NO,model.ISSUE_DATE,model.REQUISITION_NO,model.ISSUE_AMOUNT.ToString(),
                             model.RECEIVE_AMOUNT.ToString(),model.RECEIVE_BY,model.STATUS, model.COMPANY_ID.ToString(), model.REMARKS,
                             model.ENTERED_BY, model.ENTERED_DATE,  model.ENTERED_TERMINAL,model.DISPATCH_NO
                            })));

                        if (model.requisitionRcvDtlList != null && model.requisitionRcvDtlList.Count > 0)
                        {
                            int batchId = 0;
                            int batchDtlId = 0;
                            dtlId = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetRequisition_Rcv_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { }));
                            batchDtlId = dtlId;
                            foreach (var item in model.requisitionRcvDtlList)
                            {
                               
                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQueryDTL(),
                               _commonServices.AddParameter(new string[] { dtlId.ToString(),model.MST_ID.ToString(),
                                     item.SKU_ID.ToString(),item.SKU_CODE,
                                     item.UNIT_TP.ToString(),  item.ISSUE_QTY.ToString(), item.ISSUE_AMOUNT.ToString(),
                                   item.RECEIVE_QTY.ToString(), item.RECEIVE_AMOUNT.ToString(),item.STATUS,
                                     item.COMPANY_ID.ToString(), item.REMARKS,  item.ENTERED_BY,  item.ENTERED_DATE,  item.ENTERED_TERMINAL
                                  })));
                                dtlId++;
                            }

                            //#region Add Rcv Batch
                          
                            //batchId = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetRequisition_Batch_IdQuery(), _commonServices.AddParameter(new string[] { }));

                            //foreach (var item in model.requisitionRcvDtlList)
                            //{
                            //    DEPOT_REQUISITION_ISSUE_BATCH _issueBatch = new DEPOT_REQUISITION_ISSUE_BATCH();
                                
                            //    _issueBatch = await GetIssueBatchByReqNoAndSkuId(db, model.REQUISITION_NO, Convert.ToInt32(item.SKU_ID));
                            //    if(_issueBatch != null)
                            //    {
                            
                            //        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQueryBatch(),
                            //      _commonServices.AddParameter(new string[] { batchId.ToString(),batchDtlId.ToString(),
                            //            model.MST_ID.ToString(),model.RECEIVE_NO,model.RECEIVE_DATE,model.ISSUE_NO,model.REQUISITION_NO,
                            //            model.ISSUE_UNIT_ID.ToString(),model.RECEIVE_UNIT_ID.ToString(),item.SKU_ID.ToString(),item.SKU_CODE,
                            //            item.UNIT_TP.ToString(),_issueBatch.BATCH_ID.ToString(),_issueBatch.BATCH_NO,item.RECEIVE_QTY.ToString(), item.RECEIVE_AMOUNT.ToString(),
                            //            item.STATUS,item.COMPANY_ID.ToString(), item.REMARKS,  item.ENTERED_BY,  item.ENTERED_DATE,  item.ENTERED_TERMINAL
                            //         })));
                            //        batchId++;
                            //        batchDtlId++;
                            //    }
                            //}
                            //#endregion
                        }

                    }
                    else
                    {
                        DEPOT_REQUISITION_RCV_MST depot_Requisition_issue_Mst = await LoadDetailData_ByMasterId_List(db, model.MST_ID);
                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] { model.MST_ID.ToString(),
                             model.REQUISITION_UNIT_ID.ToString(), model.ISSUE_UNIT_ID.ToString(),
                             model.RECEIVE_NO,model.RECEIVE_DATE,model.ISSUE_NO,model.ISSUE_DATE,model.REQUISITION_NO,model.ISSUE_AMOUNT.ToString(),
                             model.RECEIVE_AMOUNT.ToString(),model.RECEIVE_BY,model.STATUS, model.COMPANY_ID.ToString(), model.REMARKS,
                             model.UPDATED_BY, model.UPDATED_DATE,  model.UPDATED_TERMINAL,model.DISPATCH_NO})));
                        foreach (var item in model.requisitionRcvDtlList)
                        {
                            if (item.DTL_ID == 0)
                            {
                                //-------------Add new row on detail table--------------------

                                dtlId = dtlId == 0 ? _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetRequisition_Rcv_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { })) : (dtlId + 1);

                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQueryDTL(), _commonServices.AddParameter(new string[] { dtlId.ToString(),model.MST_ID.ToString(),
                                     item.SKU_ID.ToString(),item.SKU_CODE,
                                     item.UNIT_TP.ToString(),  item.ISSUE_QTY.ToString(), item.ISSUE_AMOUNT.ToString(),
                                   item.ISSUE_QTY.ToString(), item.ISSUE_AMOUNT.ToString(),item.STATUS,
                                     item.COMPANY_ID.ToString(), item.REMARKS,  item.ENTERED_BY,  item.ENTERED_DATE,  item.ENTERED_TERMINAL})));

                            }
                            else
                            {

                                //-------------Edit on detail table--------------------
                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_UpdateQueryDTL(), _commonServices.AddParameter(new string[] {  item.DTL_ID.ToString(),
                                     item.SKU_ID.ToString(),item.SKU_CODE,
                                     item.UNIT_TP.ToString(),  item.ISSUE_QTY.ToString(), item.ISSUE_AMOUNT.ToString(),
                                   item.ISSUE_QTY.ToString(), item.ISSUE_AMOUNT.ToString(),item.STATUS,
                                     item.COMPANY_ID.ToString(), item.REMARKS,  item.UPDATED_BY,  item.UPDATED_DATE,  item.UPDATED_TERMINAL })));

                            }

                        }

                 

                    }

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    return model.MST_ID.ToString();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public async Task<List<DEPOT_REQUISITION_RCV_DTL>> GetReceiveDetailListByMstId(string db, int MstId)
        {
            List<DEPOT_REQUISITION_RCV_DTL> _requisitionRcvDetailList = new List<DEPOT_REQUISITION_RCV_DTL>();
            DataTable dataTable_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DetailByMasterId_Query(), _commonServices.AddParameter(new string[] { MstId.ToString() }));
   
            for (int i = 0; i < dataTable_detail.Rows.Count; i++)
            {
                DEPOT_REQUISITION_RCV_DTL _requisition_Dtl = new DEPOT_REQUISITION_RCV_DTL();

                _requisition_Dtl.COMPANY_ID = Convert.ToInt32(dataTable_detail.Rows[i]["COMPANY_ID"]);
                _requisition_Dtl.MST_ID = Convert.ToInt32(dataTable_detail.Rows[i]["MST_ID"]);
                _requisition_Dtl.DTL_ID = Convert.ToInt32(dataTable_detail.Rows[i]["DTL_ID"]);

                _requisition_Dtl.SKU_ID = dataTable_detail.Rows[i]["SKU_ID"].ToString();
                _requisition_Dtl.SKU_CODE = dataTable_detail.Rows[i]["SKU_CODE"].ToString();
                _requisition_Dtl.UNIT_TP = Convert.ToDouble(dataTable_detail.Rows[i]["UNIT_TP"]);
                _requisition_Dtl.RECEIVE_QTY = Convert.ToInt32(dataTable_detail.Rows[i]["RECEIVE_QTY"]);
                _requisition_Dtl.RECEIVE_AMOUNT = Convert.ToDouble(dataTable_detail.Rows[i]["RECEIVE_AMOUNT"]);
                _requisition_Dtl.ISSUE_QTY = Convert.ToInt32(dataTable_detail.Rows[i]["ISSUE_QTY"]);
                _requisition_Dtl.ISSUE_AMOUNT = Convert.ToDouble(dataTable_detail.Rows[i]["ISSUE_AMOUNT"]);
                _requisition_Dtl.STATUS = dataTable_detail.Rows[i]["STATUS"].ToString();
                _requisition_Dtl.REMARKS = dataTable_detail.Rows[i]["REMARKS"].ToString();
                _requisition_Dtl.ENTERED_BY = dataTable_detail.Rows[i]["ENTERED_BY"].ToString();
                _requisition_Dtl.ENTERED_DATE = dataTable_detail.Rows[i]["ENTERED_DATE"].ToString();


                _requisition_Dtl.ROW_NO = Convert.ToInt32(dataTable_detail.Rows[i]["ROW_NO"]);
                _requisitionRcvDetailList.Add(_requisition_Dtl);
            }
            return _requisitionRcvDetailList;
        }

        public async Task<DEPOT_REQUISITION_ISSUE_BATCH>  GetIssueBatchByReqNoAndSkuId(string db, string ReqNo,int SkuId)
        {
            DEPOT_REQUISITION_ISSUE_BATCH _requisitionRcvDetailList = new DEPOT_REQUISITION_ISSUE_BATCH();
            DataTable dataTable_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_IssueBatch_Query(), _commonServices.AddParameter(new string[] { ReqNo, SkuId.ToString() }));

         
                DEPOT_REQUISITION_ISSUE_BATCH _requisition_Batch = new DEPOT_REQUISITION_ISSUE_BATCH();

                _requisition_Batch.COMPANY_ID = Convert.ToInt32(dataTable_detail.Rows[0]["COMPANY_ID"]);
                _requisition_Batch.MST_ID = Convert.ToInt32(dataTable_detail.Rows[0]["MST_ID"]);
                _requisition_Batch.DTL_ID = Convert.ToInt32(dataTable_detail.Rows[0]["DTL_ID"]);

                _requisition_Batch.SKU_ID = Convert.ToInt32(dataTable_detail.Rows[0]["SKU_ID"]);
                _requisition_Batch.SKU_CODE = dataTable_detail.Rows[0]["SKU_CODE"].ToString();
                _requisition_Batch.UNIT_TP = Convert.ToDouble(dataTable_detail.Rows[0]["UNIT_TP"]);
                _requisition_Batch.BATCH_ID = Convert.ToInt32(dataTable_detail.Rows[0]["BATCH_ID"]);
                _requisition_Batch.BATCH_NO = dataTable_detail.Rows[0]["BATCH_NO"].ToString();
                _requisition_Batch.ISSUE_QTY = Convert.ToInt32(dataTable_detail.Rows[0]["ISSUE_QTY"]);
                _requisition_Batch.ISSUE_AMOUNT = Convert.ToDouble(dataTable_detail.Rows[0]["ISSUE_AMOUNT"]);
                _requisition_Batch.STATUS = dataTable_detail.Rows[0]["STATUS"].ToString();
                _requisition_Batch.REMARKS = dataTable_detail.Rows[0]["REMARKS"].ToString();
             
                _requisition_Batch.ENTERED_DATE = dataTable_detail.Rows[0]["ENTERED_DATE"].ToString();

            return _requisition_Batch;
        }

        public Task<string> GetCustomerExistingSKUData(string db, int Company_Id, int Customer_Id)
        {
            throw new NotImplementedException();
        }

        public async Task<string> LoadData(string db, int Company_Id)
        {
            List<DEPOT_REQUISITION_RCV_MST> requisition_Mst_list = new List<DEPOT_REQUISITION_RCV_MST>();
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadMasterData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
            for (int i = 0; i < data.Rows.Count; i++)
            {
                DEPOT_REQUISITION_RCV_MST requisition_Mst = new DEPOT_REQUISITION_RCV_MST();
                requisition_Mst.ROW_NO = Convert.ToInt32(data.Rows[i]["ROW_NO"]);
                requisition_Mst.MST_ID = Convert.ToInt32(data.Rows[i]["MST_ID"]);
                requisition_Mst.ISSUE_UNIT_ID = Convert.ToInt32(data.Rows[i]["ISSUE_UNIT_ID"]);
                requisition_Mst.ISSUE_UNIT_NAME = data.Rows[i]["ISSUE_UNIT_NAME"].ToString();
                requisition_Mst.REQUISITION_UNIT_NAME = data.Rows[i]["REQUISITION_UNIT_NAME"].ToString();
                requisition_Mst.REQUISITION_UNIT_ID = Convert.ToInt32(data.Rows[0]["REQUISITION_UNIT_ID"]);
                requisition_Mst.REQUISITION_UNIT_NAME = data.Rows[i]["REQUISITION_UNIT_NAME"].ToString();
                requisition_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[i]["MST_ID"].ToString());
                requisition_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[i]["COMPANY_ID"]);
                requisition_Mst.REMARKS = data.Rows[i]["REMARKS"].ToString();
                requisition_Mst.RECEIVE_AMOUNT = Convert.ToDouble(data.Rows[i]["RECEIVE_AMOUNT"]);
                requisition_Mst.ISSUE_AMOUNT = Convert.ToDouble(data.Rows[i]["ISSUE_AMOUNT"]);

                requisition_Mst.STATUS = data.Rows[i]["STATUS"].ToString();

                requisition_Mst.RECEIVE_DATE = data.Rows[i]["RECEIVE_DATE"].ToString();
                requisition_Mst.RECEIVE_BY = data.Rows[i]["RECEIVE_BY"].ToString();
                requisition_Mst.ISSUE_DATE = data.Rows[i]["ISSUE_DATE"].ToString();
                requisition_Mst.REQUISITION_NO = data.Rows[i]["REQUISITION_NO"].ToString();
                requisition_Mst.ISSUE_NO = data.Rows[i]["ISSUE_NO"].ToString();
                requisition_Mst.RECEIVE_NO = data.Rows[i]["RECEIVE_NO"].ToString();

                requisition_Mst_list.Add(requisition_Mst);
            }
            return JsonSerializer.Serialize(requisition_Mst_list);
        }

        public async Task<string> LoadData_Master(string db, int Company_Id)
        {
            List<DEPOT_REQUISITION_RCV_MST> requisition_Mst_list = new List<DEPOT_REQUISITION_RCV_MST>();
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadMasterData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
            for (int i = 0; i < data.Rows.Count; i++)
            {
                DEPOT_REQUISITION_RCV_MST requisition_Mst = new DEPOT_REQUISITION_RCV_MST();
                requisition_Mst.ROW_NO = Convert.ToInt32(data.Rows[i]["ROW_NO"]);
                requisition_Mst.MST_ID = Convert.ToInt32(data.Rows[i]["MST_ID"]);
                requisition_Mst.ISSUE_UNIT_ID = Convert.ToInt32(data.Rows[i]["ISSUE_UNIT_ID"]);
                requisition_Mst.REQUISITION_UNIT_ID = Convert.ToInt32(data.Rows[i]["REQUISITION_UNIT_ID"]);
                requisition_Mst.REQUISITION_UNIT_NAME = data.Rows[i]["REQUISITION_UNIT_NAME"].ToString();
          
                requisition_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[i]["MST_ID"].ToString());
                requisition_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[i]["COMPANY_ID"]);
                requisition_Mst.REMARKS = data.Rows[i]["REMARKS"].ToString();
           
                requisition_Mst.ISSUE_AMOUNT = Convert.ToDouble(data.Rows[i]["ISSUE_AMOUNT"]);
                requisition_Mst.RECEIVE_AMOUNT = Convert.ToDouble(data.Rows[i]["RECEIVE_AMOUNT"]);

                requisition_Mst.STATUS = data.Rows[i]["STATUS"].ToString();

                requisition_Mst.RECEIVE_DATE = data.Rows[i]["RECEIVE_DATE"].ToString();
                requisition_Mst.ISSUE_DATE = data.Rows[i]["ISSUE_DATE"].ToString();
                requisition_Mst.REQUISITION_NO = data.Rows[i]["REQUISITION_NO"].ToString();
                requisition_Mst.ISSUE_NO = data.Rows[i]["ISSUE_NO"].ToString();
                requisition_Mst.RECEIVE_NO = data.Rows[i]["RECEIVE_NO"].ToString();

                requisition_Mst_list.Add(requisition_Mst);
            }
            return JsonSerializer.Serialize(requisition_Mst_list);
        }

        public async Task<DEPOT_REQUISITION_RCV_MST> LoadDetailData_ByMasterId_List(string db, int _Id)
        {
            DEPOT_REQUISITION_RCV_MST _requisition_Mst = new DEPOT_REQUISITION_RCV_MST();

            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_MasterById_Query(), _commonServices.AddParameter(new string[] { _Id.ToString() }));
            if (data.Rows.Count > 0)
            {
                _requisition_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[0]["COMPANY_ID"]);

                _requisition_Mst.ROW_NO = Convert.ToInt32(data.Rows[0]["ROW_NO"]);
                _requisition_Mst.MST_ID = Convert.ToInt32(data.Rows[0]["MST_ID"]);
                _requisition_Mst.ISSUE_UNIT_ID = Convert.ToInt32(data.Rows[0]["ISSUE_UNIT_ID"]);
                _requisition_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[0]["MST_ID"].ToString());
                _requisition_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[0]["COMPANY_ID"]);
                _requisition_Mst.REMARKS = data.Rows[0]["REMARKS"].ToString();
                _requisition_Mst.RECEIVE_AMOUNT = Convert.ToDouble(data.Rows[0]["RECEIVE_AMOUNT"]);
                _requisition_Mst.ISSUE_AMOUNT = Convert.ToDouble(data.Rows[0]["ISSUE_AMOUNT"]);
                _requisition_Mst.RECEIVE_BY = data.Rows[0]["RECEIVE_BY"].ToString();
                _requisition_Mst.ISSUE_DATE = data.Rows[0]["ISSUE_DATE"].ToString();
                _requisition_Mst.ISSUE_NO = data.Rows[0]["ISSUE_NO"].ToString();
                _requisition_Mst.RECEIVE_NO = data.Rows[0]["RECEIVE_NO"].ToString();
                _requisition_Mst.STATUS = data.Rows[0]["STATUS"].ToString();
                _requisition_Mst.RECEIVE_DATE = data.Rows[0]["RECEIVE_DATE"].ToString();
                _requisition_Mst.REQUISITION_NO = data.Rows[0]["REQUISITION_NO"].ToString();
                _requisition_Mst.REQUISITION_UNIT_ID = Convert.ToInt32(data.Rows[0]["REQUISITION_UNIT_ID"]); 
            

                DataTable dataTable_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DetailByMasterId_Query(), _commonServices.AddParameter(new string[] { _Id.ToString() }));
                _requisition_Mst.requisitionRcvDtlList = new List<DEPOT_REQUISITION_RCV_DTL>();
                for (int i = 0; i < dataTable_detail.Rows.Count; i++)
                {
                    DEPOT_REQUISITION_RCV_DTL _requisition_Dtl = new DEPOT_REQUISITION_RCV_DTL();

                    _requisition_Dtl.COMPANY_ID = Convert.ToInt32(dataTable_detail.Rows[i]["COMPANY_ID"]);
                    _requisition_Dtl.MST_ID = Convert.ToInt32(dataTable_detail.Rows[i]["MST_ID"]);
                    _requisition_Dtl.DTL_ID = Convert.ToInt32(dataTable_detail.Rows[i]["DTL_ID"]);

                    _requisition_Dtl.SKU_ID = dataTable_detail.Rows[i]["SKU_ID"].ToString();
                    _requisition_Dtl.SKU_CODE = dataTable_detail.Rows[i]["SKU_CODE"].ToString();
                    _requisition_Dtl.UNIT_TP = Convert.ToDouble(dataTable_detail.Rows[i]["UNIT_TP"]);
                    _requisition_Dtl.RECEIVE_QTY = Convert.ToInt32(dataTable_detail.Rows[i]["RECEIVE_QTY"]);
                    _requisition_Dtl.RECEIVE_AMOUNT = Convert.ToDouble(dataTable_detail.Rows[i]["RECEIVE_AMOUNT"]);
                    _requisition_Dtl.ISSUE_QTY = Convert.ToInt32(dataTable_detail.Rows[i]["ISSUE_QTY"]);
                    _requisition_Dtl.ISSUE_AMOUNT = Convert.ToDouble(dataTable_detail.Rows[i]["ISSUE_AMOUNT"]);
                    _requisition_Dtl.STATUS = dataTable_detail.Rows[i]["STATUS"].ToString();
                    _requisition_Dtl.REMARKS = dataTable_detail.Rows[i]["REMARKS"].ToString();
                    _requisition_Dtl.ENTERED_BY = dataTable_detail.Rows[i]["ENTERED_BY"].ToString();
                    _requisition_Dtl.ENTERED_DATE = dataTable_detail.Rows[i]["ENTERED_DATE"].ToString();


                    _requisition_Dtl.ROW_NO = Convert.ToInt32(dataTable_detail.Rows[i]["ROW_NO"]);
                    _requisition_Mst.requisitionRcvDtlList.Add(_requisition_Dtl);
                }
            }

            return _requisition_Mst;
        }

        public async Task<DEPOT_REQUISITION_RCV_MST> LoadDetailDataWithStock_ByMasterId_List(string db, int _Id)
        {
            DEPOT_REQUISITION_RCV_MST _requisition_Mst = new DEPOT_REQUISITION_RCV_MST();

            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_MasterById_Query(), _commonServices.AddParameter(new string[] { _Id.ToString() }));
            if (data.Rows.Count > 0)
            {
                _requisition_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[0]["COMPANY_ID"]);

                _requisition_Mst.ROW_NO = Convert.ToInt32(data.Rows[0]["ROW_NO"]);
                _requisition_Mst.MST_ID = Convert.ToInt32(data.Rows[0]["MST_ID"]);
                _requisition_Mst.ISSUE_UNIT_ID = Convert.ToInt32(data.Rows[0]["ISSUE_UNIT_ID"]);
                _requisition_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[0]["MST_ID"].ToString());
                _requisition_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[0]["COMPANY_ID"]);
                _requisition_Mst.REMARKS = data.Rows[0]["REMARKS"].ToString();
                _requisition_Mst.RECEIVE_AMOUNT = Convert.ToDouble(data.Rows[0]["RECEIVE_AMOUNT"]);
                _requisition_Mst.ISSUE_AMOUNT = Convert.ToDouble(data.Rows[0]["ISSUE_AMOUNT"]);
                _requisition_Mst.RECEIVE_BY = data.Rows[0]["RECEIVE_BY"].ToString();
                _requisition_Mst.ISSUE_DATE = data.Rows[0]["ISSUE_DATE"].ToString();
                _requisition_Mst.ISSUE_NO = data.Rows[0]["ISSUE_NO"].ToString();
                _requisition_Mst.RECEIVE_NO = data.Rows[0]["RECEIVE_NO"].ToString();
                _requisition_Mst.STATUS = data.Rows[0]["STATUS"].ToString();
                _requisition_Mst.RECEIVE_DATE = data.Rows[0]["RECEIVE_DATE"].ToString();
                _requisition_Mst.REQUISITION_NO = data.Rows[0]["REQUISITION_NO"].ToString();
                _requisition_Mst.REQUISITION_UNIT_ID = Convert.ToInt32(data.Rows[0]["REQUISITION_UNIT_ID"]);
      

                DataTable dataTable_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DetailWithStock_ByMasterId_Query(), _commonServices.AddParameter(new string[] { _Id.ToString() }));
                _requisition_Mst.requisitionRcvDtlList = new List<DEPOT_REQUISITION_RCV_DTL>();
                for (int i = 0; i < dataTable_detail.Rows.Count; i++)
                {
                    DEPOT_REQUISITION_RCV_DTL _requisition_Dtl = new DEPOT_REQUISITION_RCV_DTL();

                    _requisition_Dtl.COMPANY_ID = Convert.ToInt32(dataTable_detail.Rows[i]["COMPANY_ID"]);
      

                    _requisition_Dtl.SKU_ID = dataTable_detail.Rows[i]["SKU_ID"].ToString();
                    _requisition_Dtl.SKU_NAME = dataTable_detail.Rows[i]["SKU_NAME"].ToString();
                    _requisition_Dtl.SKU_CODE = dataTable_detail.Rows[i]["SKU_CODE"].ToString();
                    _requisition_Dtl.UNIT_TP = Convert.ToDouble(dataTable_detail.Rows[i]["UNIT_TP"]);
                    _requisition_Dtl.RECEIVE_QTY = Convert.ToInt32(dataTable_detail.Rows[i]["RECEIVE_QTY"]);
                    _requisition_Dtl.RECEIVE_AMOUNT = Convert.ToDouble(dataTable_detail.Rows[i]["RECEIVE_AMOUNT"]);
                    _requisition_Dtl.STOCK_QTY = Convert.ToInt32(dataTable_detail.Rows[i]["STOCK_QTY"]);

                    _requisition_Dtl.ROW_NO = Convert.ToInt32(dataTable_detail.Rows[i]["ROW_NO"]);
                    _requisition_Mst.requisitionRcvDtlList.Add(_requisition_Dtl);
                }
            }

            return _requisition_Mst;
        }

        public Task<string> LoadSKUPriceDtlData(string db, int Company_Id)
        {
            throw new NotImplementedException();
        }

        public Task<string> LoadSKUPriceMstData(string db, int Company_Id)
        {
            throw new NotImplementedException();
        }
    }
}
