using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
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
    public class RequisitionReturnReceiveManager : IRequisitionReturnReceiveManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public RequisitionReturnReceiveManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }
        string AddOrUpdate_AddQuery() => @"INSERT INTO DEPOT_REQUISITION_RET_RCV_MST 
                                        (
                                             MST_ID
                                            ,RETURN_UNIT_ID
                                            ,RETURN_RCV_UNIT_ID
                                            ,RET_RCV_NO
                                            ,RET_RCV_DATE
                                            ,RETURN_NO
                                            ,RETURN_DATE
                                            ,REQUISITION_NO
                                            ,RETURN_AMOUNT
                                            ,RET_RCV_AMOUNT
                                            ,RET_RCV_BY
                                            ,STATUS
                                            ,COMPANY_ID
                                            ,REMARKS
                                            ,ENTERED_BY
                                            ,ENTERED_DATE
                                            ,ENTERED_TERMINAL
                                       
                                         ) 
                                          VALUES ( :param1, :param2, :param3, :param4,TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM'),:param6, TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'),:param8, :param9, :param10,:param11,:param12,:param13, :param14,:param15,TO_DATE(:param16, 'DD/MM/YYYY HH:MI:SS AM'),:param17)";

        string AddOrUpdate_AddQueryDTL() => @"INSERT INTO DEPOT_REQUISITION_RET_RCV_DTL 
                    (  DTL_ID
                        ,MST_ID
                        ,SKU_ID
                        ,SKU_CODE
                        ,UNIT_TP
                        ,RETURN_QTY
                        ,RETURN_AMOUNT
                        ,RET_RCV_QTY
                        ,RET_RCV_AMOUNT
                        ,STATUS
                        ,COMPANY_ID
                        ,REMARKS
                        ,ENTERED_BY
                        ,ENTERED_DATE
                        ,ENTERED_TERMINAL
                     
                   ) 
                    VALUES ( :param1, :param2, :param3, :param4, :param5, :param6, :param7, :param8, :param9,:param10,:param11,:param12, :param13,TO_DATE(:param14, 'DD/MM/YYYY HH:MI:SS AM'), :param15)";

        string LoadData_MasterById_Query() => @"
            SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO,M.MST_ID
                                                    ,M.RETURN_UNIT_ID
                                                    ,M.RETURN_RCV_UNIT_ID
                                                    ,M.RET_RCV_NO
                                                    ,TO_CHAR(M.RET_RCV_DATE, 'DD/MM/YYYY') RET_RCV_DATE
                                                    ,M.RETURN_NO
                                                    ,TO_CHAR(M.RETURN_DATE, 'DD/MM/YYYY') RETURN_DATE
                                                    ,M.REQUISITION_NO
                                                    ,M.RETURN_AMOUNT
                                                    ,M.RET_RCV_AMOUNT
                                                    ,M.RET_RCV_BY
                                                    ,M.STATUS
                                                    ,M.COMPANY_ID
                                                    ,M.REMARKS
                                                    ,M.ENTERED_BY
                                                    ,M.ENTERED_DATE
                                                    ,M.ENTERED_TERMINAL

                                            ,(SELECT  UNIT_NAME FROM COMPANY_INFO CI WHERE M.RETURN_RCV_UNIT_ID = CI.UNIT_ID AND ROWNUM = 1) RET_RCV_UNIT_NAME               
            FROM DEPOT_REQUISITION_RET_RCV_MST  M 
            where M.MST_ID = :param1";

        string AddOrUpdate_UpdateQuery() => @"UPDATE DEPOT_REQUISITION_RCV_MST SET  
                                                    RETURN_UNIT_ID= :param2
                                                    ,RETURN_RCV_UNIT_ID= :param3
                                                    ,RET_RCV_NO= :param4
                                                    ,RET_RCV_DATE=  TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM')
                                                    ,RETURN_NO= :param6
                                                    ,RETURN_DATE=  TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM')
                                                    ,REQUISITION_NO= :param8
                                                    ,RETURN_AMOUNT= :param9
                                                    ,RET_RCV_AMOUNT= :param10
                                                    ,RET_RCV_BY= :param11
                                                    ,STATUS= :param12
                                                    ,COMPANY_ID= :param13
                                                    ,REMARKS= :param14
                                          
                                                    ,UPDATED_BY= :param15
                                                    ,UPDATED_DATE= TO_DATE(:param16, 'DD/MM/YYYY HH:MI:SS AM')
                                                    ,UPDATED_TERMINAL= :param17
                                                    WHERE MST_ID = :param1";

        string AddOrUpdate_UpdateQueryDTL() => @"UPDATE DEPOT_REQUISITION_RET_RCV_DTL SET  
                                                 SKU_ID= :param2
                                                ,SKU_CODE= :param3
                                                ,UNIT_TP= :param4
                                                ,RETURN_QTY= :param5
                                                ,RETURN_AMOUNT= :param6
                                                ,RET_RCV_QTY= :param7
                                                ,RET_RCV_AMOUNT= :param8
                                                ,STATUS= :param9
                                                ,COMPANY_ID= :param10
                                                ,REMARKS= :param11
                                                ,UPDATED_BY= :param12
                                                ,UPDATED_DATE=  TO_DATE(:param13, 'DD/MM/YYYY HH:MI:SS AM')
                                                ,UPDATED_TERMINAL= :param14
                                                WHERE DTL_ID = :param1";

        string LoadData_DetailByMasterId_Query() => @" SELECT ROW_NUMBER () OVER (ORDER BY B.DTL_ID ASC) AS ROW_NO,
                                                       B.* ,
                                                       C.PACK_SIZE, 
 
                                                       B.RET_RCV_QTY RET_RCV_QTY 
                                                FROM DEPOT_REQUISITION_RET_RCV_MST A, DEPOT_REQUISITION_RET_RCV_DTL B,PRODUCT_INFO C
                                                WHERE A.MST_ID        = B.MST_ID
                                                AND   B.SKU_ID        = C.SKU_ID
                                                AND   A.MST_ID        = :param1";

        string LoadMasterData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO,M.MST_ID
                                                    ,M.RETURN_UNIT_ID
                                                    ,M.RETURN_RCV_UNIT_ID
                                                    ,M.RET_RCV_NO
                                                    ,TO_CHAR(M.RET_RCV_DATE, 'DD/MM/YYYY') RET_RCV_DATE
                                                    ,M.RETURN_NO
                                                    ,TO_CHAR(M.RETURN_DATE, 'DD/MM/YYYY') RETURN_DATE
                                                    ,M.REQUISITION_NO
                                                    ,M.RETURN_AMOUNT
                                                    ,M.RET_RCV_AMOUNT
                                                    ,M.RET_RCV_BY
                                                    ,M.STATUS
                                                    ,M.COMPANY_ID
                                                    ,M.REMARKS
                                                    ,M.ENTERED_BY
                                                    ,M.ENTERED_DATE
                                                    ,M.ENTERED_TERMINAL
                                            ,(SELECT  UNIT_NAME FROM COMPANY_INFO CI WHERE M.RETURN_RCV_UNIT_ID = CI.UNIT_ID AND ROWNUM = 1) RET_RCV_UNIT_NAME
            FROM DEPOT_REQUISITION_RET_RCV_MST  M 
            where  M.COMPANY_ID = :param1 and M.RETURN_RCV_UNIT_ID = :param2 ORDER BY M.MST_ID DESC";
        string LoadData_IssueBatch_Query() => @" select * from DEPOT_REQUISITION_ISSUE_BATCH where Requisition_No = :param1 and Sku_Id=:param2";

        string Get_LastReturnReceive_no() => "SELECT  RET_RCV_NO  FROM DEPOT_REQUISITION_RET_RCV_MST Where  MST_ID = (SELECT   NVL(MAX(MST_ID),0) MST_ID From DEPOT_REQUISITION_RET_RCV_MST where COMPANY_ID = :param1 )";
        string GetRequisition_Batch_IdQuery() => "SELECT NVL(MAX(RECEIVE_BATCH_ID),0) + 1 RECEIVE_BATCH_ID  FROM DEPOT_REQUISITION_RCV_BATCH";
        string GetRequisition_Mst_IdQuery() => "SELECT NVL(MAX(MST_ID),0) + 1 MST_ID  FROM DEPOT_REQUISITION_RET_RCV_MST";
        string GetRequisition_Rcv_Dtl_IdQuery() => "SELECT NVL(MAX(DTL_ID),0) + 1 DTL_ID  FROM DEPOT_REQUISITION_RET_RCV_DTL";
        public async Task<string> GenerateReturnReceiveCode(string db, string Company_Id)
        {
            try
            {
                string code;
                DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Get_LastReturnReceive_no(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
                if (dataTable.Rows.Count > 0)
                {
                    string serial = (Convert.ToInt32(dataTable.Rows[0]["RET_RCV_NO"].ToString().Substring(4, 4)) + 1).ToString();
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
        public async Task<string> AddOrUpdate(string db, DEPOT_REQUISITION_RET_RCV_MST model)
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

                  
                        model.RET_RCV_NO = _commonServices.GenerateRequisitionCode(_configuration.GetConnectionString(db), "FN_GENERATE_REQ_RET_RCV_NO", model.COMPANY_ID.ToString(), model.RETURN_UNIT_ID.ToString());
                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(),
                         _commonServices.AddParameter(new string[] {model.MST_ID.ToString(),
                             model.RETURN_UNIT_ID.ToString(), model.RETURN_RCV_UNIT_ID.ToString(),model.RET_RCV_NO.ToString(),
                             model.RET_RCV_DATE,model.RETURN_NO,model.RETURN_DATE,model.REQUISITION_NO,model.RETURN_AMOUNT.ToString(),
                             model.RET_RCV_AMOUNT.ToString(),model.RET_RCV_BY,model.STATUS, model.COMPANY_ID.ToString(), model.REMARKS,
                             model.ENTERED_BY, model.ENTERED_DATE,  model.ENTERED_TERMINAL
                            })));

                        if (model.requisitionRetRcvDtlList != null && model.requisitionRetRcvDtlList.Count > 0)
                        {
                            int batchId = 0;
                            int batchDtlId = 0;
                            dtlId = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetRequisition_Rcv_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { }));
                            batchDtlId = dtlId;
                            foreach (var item in model.requisitionRetRcvDtlList)
                            {

                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQueryDTL(),
                               _commonServices.AddParameter(new string[] { dtlId.ToString(),model.MST_ID.ToString(),
                                     item.SKU_ID.ToString(),item.SKU_CODE,
                                     item.UNIT_TP.ToString(),  item.RETURN_QTY.ToString(), item.RETURN_AMOUNT.ToString(),
                                   item.RET_RCV_QTY.ToString(), item.RET_RCV_AMOUNT.ToString(),item.STATUS,
                                     item.COMPANY_ID.ToString(), item.REMARKS,  item.ENTERED_BY,  item.ENTERED_DATE,  item.ENTERED_TERMINAL
                                  })));
                                dtlId++;
                            }

                     
                        }

                    }
                    else
                    {
                        DEPOT_REQUISITION_RET_RCV_MST depot_Requisition_issue_Mst = await LoadDetailData_ByMasterId_List(db, model.MST_ID);
                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] { model.MST_ID.ToString(),
                             model.RETURN_UNIT_ID.ToString(), model.RETURN_RCV_UNIT_ID.ToString(),model.RET_RCV_NO.ToString(),
                             model.RET_RCV_DATE,model.RETURN_NO,model.RETURN_DATE,model.REQUISITION_NO,model.RETURN_AMOUNT.ToString(),
                             model.RET_RCV_AMOUNT.ToString(),model.RET_RCV_BY,model.STATUS, model.COMPANY_ID.ToString(), model.REMARKS,
                              model.UPDATED_BY, model.UPDATED_DATE,  model.UPDATED_TERMINAL})));
                        foreach (var item in model.requisitionRetRcvDtlList)
                        {
                            if (item.DTL_ID == 0)
                            {
                                //-------------Add new row on detail table--------------------

                                dtlId = dtlId == 0 ? _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetRequisition_Rcv_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { })) : (dtlId + 1);

                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQueryDTL(), _commonServices.AddParameter(new string[] {dtlId.ToString(),model.MST_ID.ToString(),
                                     item.SKU_ID.ToString(),item.SKU_CODE,
                                     item.UNIT_TP.ToString(),  item.RETURN_QTY.ToString(), item.RETURN_AMOUNT.ToString(),
                                   item.RET_RCV_QTY.ToString(), item.RET_RCV_AMOUNT.ToString(),item.STATUS,
                                     item.COMPANY_ID.ToString(), item.REMARKS,item.ENTERED_BY,  item.ENTERED_DATE,  item.ENTERED_TERMINAL})));

                            }
                            else
                            {

                                //-------------Edit on detail table--------------------
                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_UpdateQueryDTL(), _commonServices.AddParameter(new string[] {  item.DTL_ID.ToString(),
                                     item.SKU_ID.ToString(),item.SKU_CODE,
                                     item.UNIT_TP.ToString(),  item.RETURN_QTY.ToString(), item.RETURN_AMOUNT.ToString(),
                                   item.RET_RCV_QTY.ToString(), item.RET_RCV_AMOUNT.ToString(),item.STATUS,
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


        public async Task<DEPOT_REQUISITION_RET_RCV_MST> LoadDetailData_ByMasterId_List(string db, int _Id)
        {
            DEPOT_REQUISITION_RET_RCV_MST _requisition_Mst = new DEPOT_REQUISITION_RET_RCV_MST();

            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_MasterById_Query(), _commonServices.AddParameter(new string[] { _Id.ToString() }));
            if (data.Rows.Count > 0)
            {
                _requisition_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[0]["COMPANY_ID"]);

                _requisition_Mst.ROW_NO = Convert.ToInt32(data.Rows[0]["ROW_NO"]);
                _requisition_Mst.MST_ID = Convert.ToInt32(data.Rows[0]["MST_ID"]);
                _requisition_Mst.RETURN_UNIT_ID = data.Rows[0]["RETURN_UNIT_ID"].ToString();
                _requisition_Mst.RETURN_RCV_UNIT_ID = Convert.ToInt32(data.Rows[0]["RETURN_RCV_UNIT_ID"]);
                _requisition_Mst.RET_RCV_UNIT_NAME = data.Rows[0]["RET_RCV_UNIT_NAME"].ToString();
                _requisition_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[0]["MST_ID"].ToString());
                _requisition_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[0]["COMPANY_ID"]);
                _requisition_Mst.REMARKS = data.Rows[0]["REMARKS"].ToString();
                _requisition_Mst.RETURN_AMOUNT = Convert.ToDouble(data.Rows[0]["RETURN_AMOUNT"]);
                _requisition_Mst.RET_RCV_AMOUNT = Convert.ToDouble(data.Rows[0]["RET_RCV_AMOUNT"]);
                _requisition_Mst.RET_RCV_BY = data.Rows[0]["RET_RCV_BY"].ToString();
                _requisition_Mst.RETURN_DATE = data.Rows[0]["RETURN_DATE"].ToString();
                _requisition_Mst.RETURN_NO = data.Rows[0]["RETURN_NO"].ToString();
                _requisition_Mst.RET_RCV_NO = data.Rows[0]["RET_RCV_NO"].ToString();
                _requisition_Mst.STATUS = data.Rows[0]["STATUS"].ToString();
                _requisition_Mst.RET_RCV_DATE = data.Rows[0]["RET_RCV_DATE"].ToString();
                _requisition_Mst.REQUISITION_NO = data.Rows[0]["REQUISITION_NO"].ToString();

                _requisition_Mst.RETURN_RCV_UNIT_ID = Convert.ToInt32(data.Rows[0]["RETURN_RCV_UNIT_ID"]);

                DataTable dataTable_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DetailByMasterId_Query(), _commonServices.AddParameter(new string[] { _Id.ToString() }));
                _requisition_Mst.requisitionRetRcvDtlList = new List<DEPOT_REQUISITION_RET_RCV_DTL>();
                for (int i = 0; i < dataTable_detail.Rows.Count; i++)
                {
                    DEPOT_REQUISITION_RET_RCV_DTL _requisition_Dtl = new DEPOT_REQUISITION_RET_RCV_DTL();

                    _requisition_Dtl.COMPANY_ID = Convert.ToInt32(dataTable_detail.Rows[i]["COMPANY_ID"]);
                    _requisition_Dtl.MST_ID = Convert.ToInt32(dataTable_detail.Rows[i]["MST_ID"]);
                    _requisition_Dtl.DTL_ID = Convert.ToInt32(dataTable_detail.Rows[i]["DTL_ID"]);

                    _requisition_Dtl.SKU_ID = dataTable_detail.Rows[i]["SKU_ID"].ToString();
                    _requisition_Dtl.SKU_CODE = dataTable_detail.Rows[i]["SKU_CODE"].ToString();
                    _requisition_Dtl.UNIT_TP = Convert.ToInt32(dataTable_detail.Rows[i]["UNIT_TP"]);
                    _requisition_Dtl.RETURN_QTY = Convert.ToInt32(dataTable_detail.Rows[i]["RETURN_QTY"]);
                    _requisition_Dtl.RETURN_AMOUNT = Convert.ToDouble(dataTable_detail.Rows[i]["RETURN_AMOUNT"]);
                    _requisition_Dtl.RET_RCV_QTY = Convert.ToInt32(dataTable_detail.Rows[i]["RET_RCV_QTY"]);
                    _requisition_Dtl.RET_RCV_AMOUNT = Convert.ToDouble(dataTable_detail.Rows[i]["RET_RCV_AMOUNT"]);
                    _requisition_Dtl.STATUS = dataTable_detail.Rows[i]["STATUS"].ToString();
                    _requisition_Dtl.REMARKS = dataTable_detail.Rows[i]["REMARKS"].ToString();
                    _requisition_Dtl.ENTERED_BY = dataTable_detail.Rows[i]["ENTERED_BY"].ToString();
                    _requisition_Dtl.ENTERED_DATE = dataTable_detail.Rows[i]["ENTERED_DATE"].ToString();


                    _requisition_Dtl.ROW_NO = Convert.ToInt32(dataTable_detail.Rows[i]["ROW_NO"]);
                    _requisition_Mst.requisitionRetRcvDtlList.Add(_requisition_Dtl);
                }
            }

            return _requisition_Mst;
        }

        public Task<string> GetCustomerExistingSKUData(string db, int Company_Id, int Customer_Id)
        {
            throw new NotImplementedException();
        }

        public async Task<string> LoadData(string db, int Company_Id,string unitId)
        {
            List<DEPOT_REQUISITION_RET_RCV_MST> requisition_Mst_list = new List<DEPOT_REQUISITION_RET_RCV_MST>();
         
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadMasterData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), unitId }));
            for (int i = 0; i < data.Rows.Count; i++)
            {
                DEPOT_REQUISITION_RET_RCV_MST requisition_Mst = new DEPOT_REQUISITION_RET_RCV_MST();
                requisition_Mst.ROW_NO = Convert.ToInt32(data.Rows[i]["ROW_NO"]);
                requisition_Mst.MST_ID = Convert.ToInt32(data.Rows[i]["MST_ID"]);
                requisition_Mst.RETURN_UNIT_ID = data.Rows[i]["RETURN_UNIT_ID"].ToString();
                requisition_Mst.RET_RCV_UNIT_NAME = data.Rows[i]["RET_RCV_UNIT_NAME"].ToString();
                requisition_Mst.RETURN_RCV_UNIT_ID = Convert.ToInt32(data.Rows[0]["RETURN_RCV_UNIT_ID"]);

                requisition_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[i]["MST_ID"].ToString());
                requisition_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[i]["COMPANY_ID"]);
                requisition_Mst.REMARKS = data.Rows[i]["REMARKS"].ToString();
                requisition_Mst.RET_RCV_AMOUNT = Convert.ToDouble(data.Rows[i]["RET_RCV_AMOUNT"]);
                requisition_Mst.RETURN_AMOUNT = Convert.ToDouble(data.Rows[i]["RETURN_AMOUNT"]);
                requisition_Mst.RET_RCV_DATE = data.Rows[0]["RET_RCV_DATE"].ToString();

                requisition_Mst.STATUS = data.Rows[i]["STATUS"].ToString();
                requisition_Mst.RETURN_DATE = data.Rows[i]["RETURN_DATE"].ToString();
                requisition_Mst.RET_RCV_BY = data.Rows[i]["RET_RCV_BY"].ToString();
                requisition_Mst.RETURN_DATE = data.Rows[i]["RETURN_DATE"].ToString();
                requisition_Mst.REQUISITION_NO = data.Rows[i]["REQUISITION_NO"].ToString();
                requisition_Mst.RETURN_NO = data.Rows[i]["RETURN_NO"].ToString();
                requisition_Mst.RET_RCV_NO = data.Rows[i]["RET_RCV_NO"].ToString();

                requisition_Mst_list.Add(requisition_Mst);
            }
            return JsonSerializer.Serialize(requisition_Mst_list);
        }
    

        public Task<string> LoadData_Master(string db, int Company_Id)
        {
            throw new NotImplementedException();
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
