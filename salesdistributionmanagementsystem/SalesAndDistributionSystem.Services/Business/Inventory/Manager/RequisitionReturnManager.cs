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
    public class RequisitionReturnManager : IRequisitionReturnManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public RequisitionReturnManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        string AddOrUpdate_AddQuery() => @"INSERT INTO DEPOT_REQUISITION_RETURN_MST 
                                        (
                                            MST_ID
                                            ,RETURN_RCV_UNIT_ID
                                            ,RETURN_UNIT_ID
                                            ,RETURN_TYPE
                                            ,RETURN_NO
                                            ,RETURN_DATE
                                            ,RECEIVE_NO
                                            ,RECEIVE_DATE
                                            ,REQUISITION_NO
                                            ,RECEIVE_AMOUNT
                                            ,RETURN_AMOUNT
                                            ,RETURN_BY
                                            ,STATUS
                                            ,COMPANY_ID
                                            ,REMARKS
                                            ,ENTERED_BY
                                            ,ENTERED_DATE
                                            ,ENTERED_TERMINAL
                                     
                                       
                                         ) 
                                          VALUES ( :param1, :param2, :param3,:param4,:param5, TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM'), :param7,  TO_DATE(:param8, 'DD/MM/YYYY HH:MI:SS AM'), :param9,:param10,:param11,:param12, :param13,:param14, :param15,:param16,TO_DATE(:param17, 'DD/MM/YYYY HH:MI:SS AM'),:param18)";
        string LoadMasterDataForRcv_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO, MST_ID
                                                ,RETURN_RCV_UNIT_ID
                                                ,RETURN_UNIT_ID
                                                ,RETURN_TYPE
                                                ,RETURN_NO
                                                ,TO_CHAR(RETURN_DATE, 'DD/MM/YYYY') RETURN_DATE
                                                ,RECEIVE_NO
                                                 ,TO_CHAR(RECEIVE_DATE, 'DD/MM/YYYY') RECEIVE_DATE
                                                ,REQUISITION_NO
                                                ,RECEIVE_AMOUNT
                                                ,RETURN_AMOUNT
                                                ,RETURN_BY
                                                ,STATUS
                                                ,COMPANY_ID
                                                ,REMARKS
                                                ,ENTERED_BY
                                                ,ENTERED_DATE
                                                ,ENTERED_TERMINAL
                                                ,UPDATED_BY
                                                ,UPDATED_DATE
                                                ,UPDATED_TERMINAL
                                            ,(SELECT  UNIT_NAME FROM COMPANY_INFO CI WHERE M.RETURN_UNIT_ID = CI.UNIT_ID AND ROWNUM = 1) RETURN_UNIT_NAME
            FROM DEPOT_REQUISITION_RETURN_MST  M 
            where  M.COMPANY_ID = :param1 and RETURN_RCV_UNIT_ID = :param2  ORDER BY M.MST_ID DESC";
         string LoadMasterData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO, MST_ID
                                                ,RETURN_RCV_UNIT_ID
                                                ,RETURN_UNIT_ID
                                                ,RETURN_TYPE
                                                ,RETURN_NO
                                                ,TO_CHAR(RETURN_DATE, 'DD/MM/YYYY') RETURN_DATE
                                                ,RECEIVE_NO
                                                 ,TO_CHAR(RECEIVE_DATE, 'DD/MM/YYYY') RECEIVE_DATE
                                                ,REQUISITION_NO
                                                ,RECEIVE_AMOUNT
                                                ,RETURN_AMOUNT
                                                ,RETURN_BY
                                                ,STATUS
                                                ,COMPANY_ID
                                                ,REMARKS
                                                ,ENTERED_BY
                                                ,ENTERED_DATE
                                                ,ENTERED_TERMINAL
                                                ,UPDATED_BY
                                                ,UPDATED_DATE
                                                ,UPDATED_TERMINAL
                                            ,(SELECT  UNIT_NAME FROM COMPANY_INFO CI WHERE M.RETURN_UNIT_ID = CI.UNIT_ID AND ROWNUM = 1) RETURN_UNIT_NAME
            FROM DEPOT_REQUISITION_RETURN_MST  M 
            where  M.COMPANY_ID = :param1 and RETURN_UNIT_ID = :param2 ORDER BY M.MST_ID DESC";

        string LoadUnreturnedRequisitionData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO,                  M.MST_ID
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
            where  M.COMPANY_ID = :param1 
            and M.REQUISITION_UNIT_ID = :param2
            and M.REQUISITION_NO NOT IN (SELECT REQUISITION_NO FROM(
            SELECT REQUISITION_NO, SUM(RETURN_AMOUNT) RETURN_AMOUNT
            FROM DEPOT_REQUISITION_RETURN_MST
            GROUP BY REQUISITION_NO
        ) WHERE RETURN_AMOUNT < M.RECEIVE_AMOUNT) 
            ORDER BY M.MST_ID DESC";

        string AddOrUpdate_UpdateQuery() => @"UPDATE DEPOT_REQUISITION_RETURN_MST SET  
                                                   ,RETURN_RCV_UNIT_ID = :param2
                                                    ,RETURN_UNIT_ID= :param3
                                                    ,RETURN_TYPE= :param4
                                                    ,RETURN_NO= :param5
                                                    ,RETURN_DATE= TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM')
                                                    ,RECEIVE_NO= :param7
                                                    ,RECEIVE_DATE=  TO_DATE(:param8, 'DD/MM/YYYY HH:MI:SS AM')
                                                    ,REQUISITION_NO= :param9
                                                    ,RECEIVE_AMOUNT= :param10
                                                    ,RETURN_AMOUNT= :param11
                                                    ,RETURN_BY= :param12
                                                    ,STATUS= :param13
                                                    ,COMPANY_ID= :param14
                                                    ,REMARKS= :param15
                                                 
                                                    ,UPDATED_BY= :param16
                                                    ,UPDATED_DATE=  TO_DATE(:param17, 'DD/MM/YYYY HH:MI:SS AM')
                                                    ,UPDATED_TERMINAL= :param18
                                                WHERE MST_ID = :param1";

        string AddOrUpdate_UpdateQueryDTL() => @"UPDATE DEPOT_REQUISITION_RETURN_DTL SET  
                                                 SKU_ID= :param2
                                                ,SKU_CODE= :param3
                                                ,UNIT_TP= :param4
                                                ,RECEIVE_QTY= :param5
                                                ,RECEIVE_AMOUNT= :param6
                                                ,RETURN_QTY= :param7
                                                ,RETURN_AMOUNT= :param8
                                                ,STATUS= :param9
                                                ,COMPANY_ID= :param10
                                                ,REMARKS= :param11
                                                ,UPDATED_BY= :param12
                                                ,UPDATED_DATE=  TO_DATE(:param13, 'DD/MM/YYYY HH:MI:SS AM')
                                                ,UPDATED_TERMINAL= :param14
                                                WHERE DTL_ID = :param1";

        string AddOrUpdate_AddQueryDTL() => @"INSERT INTO DEPOT_REQUISITION_RETURN_DTL 
                    (DTL_ID
                    ,MST_ID
                    ,SKU_ID
                    ,SKU_CODE
                    ,UNIT_TP
                    ,RECEIVE_QTY
                    ,RECEIVE_AMOUNT
                    ,RETURN_QTY
                    ,RETURN_AMOUNT
                    ,STATUS
                    ,COMPANY_ID
                    ,REMARKS
                    ,ENTERED_BY
                    ,ENTERED_DATE
                    ,ENTERED_TERMINAL
               
                   ) 
                    VALUES ( :param1, :param2, :param3, :param4, :param5, :param6, :param7, :param8, :param9,:param10,:param11,:param12, :param13,TO_DATE(:param14, 'DD/MM/YYYY HH:MI:SS AM'), :param15)";

        string LoadData_MasterById_Query() => @"
            SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO, M.MST_ID
                                                ,M.RETURN_RCV_UNIT_ID
                                                , M.RETURN_UNIT_ID
                                                , M.RETURN_TYPE
                                                , M.RETURN_NO
                                                ,TO_CHAR(M.RETURN_DATE, 'DD/MM/YYYY') RETURN_DATE
                                                , M.RECEIVE_NO
                                                ,TO_CHAR(M.RECEIVE_DATE, 'DD/MM/YYYY') RECEIVE_DATE
                                                , M.REQUISITION_NO
                                                , M.RECEIVE_AMOUNT
                                                , M.RETURN_AMOUNT
                                                , M.RETURN_BY
                                                , M.STATUS
                                                , M.COMPANY_ID
                                                , M.REMARKS
                                                , M.ENTERED_BY
                                                , M.ENTERED_DATE
                                                , M.ENTERED_TERMINAL
                                                , M.UPDATED_BY
                                                , M.UPDATED_DATE
                                                , M.UPDATED_TERMINAL              
            FROM DEPOT_REQUISITION_RETURN_MST  M 
            where  M.MST_ID = :param1";

        string LoadData_DetailByMasterId_Query() => @" SELECT ROW_NUMBER () OVER (ORDER BY A.DTL_ID ASC) AS ROW_NO, A.DTL_ID,
                                                       A.MST_ID,       
                                                       A.SKU_ID,
                                                       A.SKU_CODE,
                                                       FN_SKU_NAME(A.COMPANY_ID,A.SKU_ID) SKU_NAME,
                                                       FN_PACK_SIZE(A.COMPANY_ID,A.SKU_ID) PACK_SIE,
                                                       A.UNIT_TP,
                                                       A.RECEIVE_QTY,
                                                       A.RECEIVE_AMOUNT,            
                                                       A.RETURN_QTY,
                                                       A.RETURN_AMOUNT,
                                                       B.STOCK_QTY,  
                                                       A.STATUS,
                                                       A.COMPANY_ID
                                                FROM
                                                    (
                                                    SELECT B.DTL_ID,
                                                           B.MST_ID,
                                                           A.RETURN_UNIT_ID,
                                                           A.COMPANY_ID,
                                                           B.SKU_ID,
                                                           B.SKU_CODE,
                                                           B.UNIT_TP,
                                                           B.RECEIVE_QTY,
                                                           B.RECEIVE_AMOUNT,       
                                                           B.RETURN_QTY,
                                                           B.RETURN_AMOUNT,
                                                           B.STATUS
           
                                                    FROM DEPOT_REQUISITION_RETURN_MST A,
                                                         DEPOT_REQUISITION_RETURN_DTL B
                                                    WHERE A.MST_ID = B.MST_ID 
                                                    AND   A.MST_ID = :param1
                                                   )A,
                                                   (
                                                    SELECT A.MST_ID,
                                                           A.COMPANY_ID,
                                                           A.RETURN_UNIT_ID,
                                                           A.SKU_ID,
                                                           A.SKU_CODE,
                                                           SUM(NVL(B.PASSED_QTY,0)) STOCK_QTY
                                                    FROM DEPOT_REQUISITION_RETURN_BATCH A, BATCH_WISE_STOCK B
                                                    WHERE A.COMPANY_ID=B.COMPANY_ID
                                                    AND   A.RETURN_UNIT_ID=B.UNIT_ID
                                                    AND   A.SKU_ID=B.SKU_ID
                                                    AND   A.BATCH_ID=B.BATCH_ID 
                                                    AND   A.MST_ID = :param1
                                                    GROUP BY A.MST_ID,A.COMPANY_ID,A.RETURN_UNIT_ID,A.SKU_ID,A.SKU_CODE 
                                                  )B
                                                WHERE A.MST_ID=B.MST_ID  
                                                AND   A.COMPANY_ID=B.COMPANY_ID
                                                AND   A.RETURN_UNIT_ID = B.RETURN_UNIT_ID
                                                AND   A.SKU_ID=B.SKU_ID
                                                ";
        string Get_LastRequisition_no() => "SELECT  RETURN_NO  FROM DEPOT_REQUISITION_RETURN_MST Where  MST_ID = (SELECT   NVL(MAX(MST_ID),0) MST_ID From DEPOT_REQUISITION_RETURN_MST where COMPANY_ID = :param1 )";
        string GetRequisition_Mst_IdQuery() => "SELECT NVL(MAX(MST_ID),0) + 1 MST_ID  FROM DEPOT_REQUISITION_RETURN_MST";
        string GetRequisition_Return_Dtl_IdQuery() => "SELECT NVL(MAX(DTL_ID),0) + 1 DTL_ID  FROM DEPOT_REQUISITION_RETURN_DTL";


        public async Task<string> GenerateReturnCode(string db, string Company_Id)
        {
            try
            {
                string code;
                DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Get_LastRequisition_no(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
                if (dataTable.Rows.Count > 0)
                {
                    string serial = (Convert.ToInt32(dataTable.Rows[0]["RETURN_NO"].ToString().Substring(4, 4)) + 1).ToString();
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
        public async Task<string> AddOrUpdate(string db, DEPOT_REQUISITION_RETURN_MST model)
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

                        model.RETURN_NO = _commonServices.GenerateRequisitionCode(_configuration.GetConnectionString(db), "FN_GENERATE_REQ_RETURN_NO", model.COMPANY_ID.ToString(), model.RETURN_UNIT_ID.ToString());

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(),
                         _commonServices.AddParameter(new string[] {model.MST_ID.ToString(),
                             model.RETURN_RCV_UNIT_ID.ToString(),model.RETURN_UNIT_ID.ToString(),
                             model.RETURN_TYPE,model.RETURN_NO,model.RETURN_DATE,model.RECEIVE_NO,
                             model.RECEIVE_DATE, model.REQUISITION_NO.ToString(),model.RECEIVE_AMOUNT.ToString(),model.RETURN_AMOUNT.ToString(),
                             model.RETURN_BY,model.STATUS, model.COMPANY_ID.ToString(), model.REMARKS,
                             model.ENTERED_BY, model.ENTERED_DATE,  model.ENTERED_TERMINAL
                            })));

                        if (model.requisitionReturnDtlList != null && model.requisitionReturnDtlList.Count > 0)
                        {
                            dtlId = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetRequisition_Return_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { }));
                            foreach (var item in model.requisitionReturnDtlList)
                            {


                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQueryDTL(),
                               _commonServices.AddParameter(new string[] { dtlId.ToString(),model.MST_ID.ToString(),
                                     item.SKU_ID.ToString(),item.SKU_CODE,
                                     item.UNIT_TP.ToString(),item.RECEIVE_QTY.ToString(), item.RECEIVE_AMOUNT.ToString(),item.RETURN_QTY.ToString(), item.RETURN_AMOUNT.ToString(), item.STATUS,
                                     item.COMPANY_ID.ToString(), item.REMARKS,  item.ENTERED_BY,  item.ENTERED_DATE,  item.ENTERED_TERMINAL
                                  })));
                                dtlId++;
                            }
                        }

                    }
                    else
                    {
                        DEPOT_REQUISITION_RETURN_MST depot_Requisition_issue_Mst = await LoadDetailData_ByMasterId_List(db, model.MST_ID);
                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] { model.MST_ID.ToString(),
                              model.RETURN_RCV_UNIT_ID.ToString(),model.RETURN_UNIT_ID.ToString(),
                             model.RETURN_TYPE,model.RETURN_NO,model.RETURN_DATE,model.RECEIVE_NO,
                             model.RECEIVE_DATE, model.REQUISITION_NO.ToString(),model.RECEIVE_AMOUNT.ToString(),model.RETURN_AMOUNT.ToString(),
                             model.RETURN_BY,model.STATUS, model.COMPANY_ID.ToString(), model.REMARKS,
                               model.UPDATED_BY.ToString(), model.UPDATED_DATE.ToString(),
                             model.UPDATED_TERMINAL})));

                        foreach (var item in model.requisitionReturnDtlList)
                        {
                            if (item.DTL_ID == 0)
                            {
                                //-------------Add new row on detail table--------------------

                                dtlId = dtlId == 0 ? _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetRequisition_Return_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { })) : (dtlId + 1);

                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQueryDTL(), _commonServices.AddParameter(new string[] {dtlId.ToString(),model.MST_ID.ToString(),
                                     item.SKU_ID.ToString(),item.SKU_CODE,
                                     item.UNIT_TP.ToString(),item.RECEIVE_QTY.ToString(), item.RECEIVE_AMOUNT.ToString(),item.RETURN_QTY.ToString(), item.RETURN_AMOUNT.ToString(), item.STATUS,
                                     item.COMPANY_ID.ToString(), item.REMARKS,  item.ENTERED_BY,  item.ENTERED_DATE,  item.ENTERED_TERMINAL})));

                            }
                            else
                            {

                                //-------------Edit on detail table--------------------
                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_UpdateQueryDTL(), _commonServices.AddParameter(new string[] { item.DTL_ID.ToString(),
                                     item.SKU_ID.ToString(),item.SKU_CODE,
                                     item.UNIT_TP.ToString(),item.RECEIVE_QTY.ToString(), item.RECEIVE_AMOUNT.ToString(),item.RETURN_QTY.ToString(), item.RETURN_AMOUNT.ToString(), item.STATUS,
                                     item.COMPANY_ID.ToString(), item.REMARKS,model.UPDATED_BY.ToString(), model.UPDATED_DATE.ToString(),
                                        model.UPDATED_TERMINAL })));

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

        public Task<string> GetCustomerExistingSKUData(string db, int Company_Id, int Customer_Id)
        {
            throw new NotImplementedException();
        }

        public async Task<string> LoadData(string db, int Company_Id,string unitId)
        {
            List<DEPOT_REQUISITION_RETURN_MST> requisition_Mst_list = new List<DEPOT_REQUISITION_RETURN_MST>();
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadMasterData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), unitId.ToString() }));
            for (int i = 0; i < data.Rows.Count; i++)
            {
                DEPOT_REQUISITION_RETURN_MST requisition_Mst = new DEPOT_REQUISITION_RETURN_MST();
                requisition_Mst.ROW_NO = Convert.ToInt32(data.Rows[i]["ROW_NO"]);
                requisition_Mst.MST_ID = Convert.ToInt32(data.Rows[i]["MST_ID"]);
    
                requisition_Mst.RETURN_UNIT_NAME = data.Rows[i]["RETURN_UNIT_NAME"].ToString();
                requisition_Mst.RETURN_UNIT_ID = data.Rows[i]["RETURN_UNIT_ID"].ToString();
                requisition_Mst.RETURN_RCV_UNIT_ID = Convert.ToInt32(data.Rows[0]["RETURN_RCV_UNIT_ID"]);
                requisition_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[i]["MST_ID"].ToString());
                requisition_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[i]["COMPANY_ID"]);
                requisition_Mst.REMARKS = data.Rows[i]["REMARKS"].ToString();
                requisition_Mst.RECEIVE_AMOUNT = Convert.ToDouble(data.Rows[i]["RECEIVE_AMOUNT"]);
                requisition_Mst.RETURN_AMOUNT = Convert.ToDouble(data.Rows[i]["RETURN_AMOUNT"]);
                requisition_Mst.STATUS = data.Rows[i]["STATUS"].ToString();
                requisition_Mst.RECEIVE_DATE = data.Rows[i]["RECEIVE_DATE"].ToString();
                requisition_Mst.RETURN_BY = data.Rows[i]["RETURN_BY"].ToString();
                requisition_Mst.RETURN_DATE = data.Rows[i]["RETURN_DATE"].ToString();
                requisition_Mst.REQUISITION_NO = data.Rows[i]["REQUISITION_NO"].ToString();
                requisition_Mst.RETURN_NO = data.Rows[i]["RETURN_NO"].ToString();

                requisition_Mst_list.Add(requisition_Mst);
            }
            return JsonSerializer.Serialize(requisition_Mst_list);
        }
        public async Task<string> LoadDataForRcv(string db, int Company_Id,string unitId)
        {
            List<DEPOT_REQUISITION_RETURN_MST> requisition_Mst_list = new List<DEPOT_REQUISITION_RETURN_MST>();
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadMasterDataForRcv_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), unitId.ToString() }));
            for (int i = 0; i < data.Rows.Count; i++)
            {
                DEPOT_REQUISITION_RETURN_MST requisition_Mst = new DEPOT_REQUISITION_RETURN_MST();
                requisition_Mst.ROW_NO = Convert.ToInt32(data.Rows[i]["ROW_NO"]);
                requisition_Mst.MST_ID = Convert.ToInt32(data.Rows[i]["MST_ID"]);
    
                requisition_Mst.RETURN_UNIT_NAME = data.Rows[i]["RETURN_UNIT_NAME"].ToString();
                requisition_Mst.RETURN_UNIT_ID = data.Rows[i]["RETURN_UNIT_ID"].ToString();
                requisition_Mst.RETURN_RCV_UNIT_ID = Convert.ToInt32(data.Rows[0]["RETURN_RCV_UNIT_ID"]);
                requisition_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[i]["MST_ID"].ToString());
                requisition_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[i]["COMPANY_ID"]);
                requisition_Mst.REMARKS = data.Rows[i]["REMARKS"].ToString();
                requisition_Mst.RECEIVE_AMOUNT = Convert.ToDouble(data.Rows[i]["RECEIVE_AMOUNT"]);
                requisition_Mst.RETURN_AMOUNT = Convert.ToDouble(data.Rows[i]["RETURN_AMOUNT"]);
                requisition_Mst.STATUS = data.Rows[i]["STATUS"].ToString();
                requisition_Mst.RECEIVE_DATE = data.Rows[i]["RECEIVE_DATE"].ToString();
                requisition_Mst.RETURN_BY = data.Rows[i]["RETURN_BY"].ToString();
                requisition_Mst.RETURN_DATE = data.Rows[i]["RETURN_DATE"].ToString();
                requisition_Mst.REQUISITION_NO = data.Rows[i]["REQUISITION_NO"].ToString();
                requisition_Mst.RETURN_NO = data.Rows[i]["RETURN_NO"].ToString();

                requisition_Mst_list.Add(requisition_Mst);
            }
            return JsonSerializer.Serialize(requisition_Mst_list);
        }

        public async Task<string> UnreturnedRequisition(string db, int Company_Id, int unit_id)
        {
            List<DEPOT_REQUISITION_RCV_MST> requisition_Mst_list = new List<DEPOT_REQUISITION_RCV_MST>();
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), 
                LoadUnreturnedRequisitionData_Query(),
                _commonServices.AddParameter(new string[] { Company_Id.ToString(), unit_id.ToString() }));
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

        public Task<string> LoadData_Master(string db, int Company_Id)
        {
            throw new NotImplementedException();
        }

        public async Task<DEPOT_REQUISITION_RETURN_MST> LoadDetailData_ByMasterId_List(string db, int _Id)
        {
            DEPOT_REQUISITION_RETURN_MST _requisition_Mst = new DEPOT_REQUISITION_RETURN_MST();

            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_MasterById_Query(), _commonServices.AddParameter(new string[] { _Id.ToString() }));
            if (data.Rows.Count > 0)
            {
                _requisition_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[0]["COMPANY_ID"]);

                _requisition_Mst.ROW_NO = Convert.ToInt32(data.Rows[0]["ROW_NO"]);
                _requisition_Mst.MST_ID = Convert.ToInt32(data.Rows[0]["MST_ID"]);
   
                _requisition_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[0]["MST_ID"].ToString());
                _requisition_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[0]["COMPANY_ID"]);
                _requisition_Mst.REMARKS = data.Rows[0]["REMARKS"].ToString();
                _requisition_Mst.RETURN_AMOUNT = Convert.ToDouble(data.Rows[0]["RETURN_AMOUNT"]);
                _requisition_Mst.RECEIVE_AMOUNT = Convert.ToDouble(data.Rows[0]["RECEIVE_AMOUNT"]);
                _requisition_Mst.RETURN_BY = data.Rows[0]["RETURN_BY"].ToString();
                _requisition_Mst.RETURN_DATE = data.Rows[0]["RETURN_DATE"].ToString();
                _requisition_Mst.RECEIVE_NO = data.Rows[0]["RECEIVE_NO"].ToString();
                _requisition_Mst.RETURN_NO = data.Rows[0]["RETURN_NO"].ToString();
                _requisition_Mst.STATUS = data.Rows[0]["STATUS"].ToString();
                _requisition_Mst.RETURN_DATE = data.Rows[0]["RETURN_DATE"].ToString();
                _requisition_Mst.RECEIVE_DATE = data.Rows[0]["RECEIVE_DATE"].ToString();
                _requisition_Mst.REQUISITION_NO = data.Rows[0]["REQUISITION_NO"].ToString();
                _requisition_Mst.RETURN_RCV_UNIT_ID = Convert.ToInt32(data.Rows[0]["RETURN_RCV_UNIT_ID"]) ;
                _requisition_Mst.RETURN_UNIT_ID = data.Rows[0]["RETURN_UNIT_ID"].ToString(); ;

                _requisition_Mst.RETURN_TYPE = data.Rows[0]["RETURN_TYPE"].ToString(); ;

                DataTable dataTable_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DetailByMasterId_Query(), _commonServices.AddParameter(new string[] { _Id.ToString() }));
                _requisition_Mst.requisitionReturnDtlList = new List<DEPOT_REQUISITION_RETURN_DTL>();
                for (int i = 0; i < dataTable_detail.Rows.Count; i++)
                {
                    DEPOT_REQUISITION_RETURN_DTL _requisition_Dtl = new DEPOT_REQUISITION_RETURN_DTL();

                    _requisition_Dtl.COMPANY_ID = Convert.ToInt32(dataTable_detail.Rows[i]["COMPANY_ID"]);
                    _requisition_Dtl.MST_ID = Convert.ToInt32(dataTable_detail.Rows[i]["MST_ID"]);
                    _requisition_Dtl.DTL_ID = Convert.ToInt32(dataTable_detail.Rows[i]["DTL_ID"]);

                    _requisition_Dtl.SKU_ID = dataTable_detail.Rows[i]["SKU_ID"].ToString();
                    _requisition_Dtl.SKU_NAME = dataTable_detail.Rows[i]["SKU_NAME"].ToString();
                    _requisition_Dtl.SKU_CODE = dataTable_detail.Rows[i]["SKU_CODE"].ToString();
                    _requisition_Dtl.UNIT_TP = Convert.ToDouble(dataTable_detail.Rows[i]["UNIT_TP"]);
                    _requisition_Dtl.RETURN_QTY = Convert.ToInt32(dataTable_detail.Rows[i]["RETURN_QTY"]);
                    _requisition_Dtl.RETURN_AMOUNT = Convert.ToDouble(dataTable_detail.Rows[i]["RETURN_AMOUNT"]);
                    _requisition_Dtl.RECEIVE_AMOUNT = Convert.ToDouble(dataTable_detail.Rows[i]["RECEIVE_AMOUNT"]);
                    _requisition_Dtl.RECEIVE_QTY = Convert.ToInt32(dataTable_detail.Rows[i]["RECEIVE_QTY"]);
                    _requisition_Dtl.RETURN_QTY = Convert.ToInt32(dataTable_detail.Rows[i]["RETURN_QTY"]);
            
                    _requisition_Dtl.STATUS = dataTable_detail.Rows[i]["STATUS"].ToString();




                    _requisition_Dtl.ROW_NO = Convert.ToInt32(dataTable_detail.Rows[i]["ROW_NO"]);
                    _requisition_Mst.requisitionReturnDtlList.Add(_requisition_Dtl);
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
