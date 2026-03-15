using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.Company;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager;
using SalesAndDistributionSystem.Services.Business.Security;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution.Manager
{
    public class RequisitionIssueManager : IRequisitionIssueManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        private readonly INotificationManager _NotificationManager;
        private readonly IUserLogManager _logManager; 
        public RequisitionIssueManager(ICommonServices commonServices, IConfiguration configuration, INotificationManager NotificationManager, IUserLogManager logManager)
        {
            _commonServices = commonServices;
            _configuration = configuration;
            _NotificationManager = NotificationManager;
            _logManager = logManager;
        }
        string AddOrUpdate_AddQuery() => @"INSERT INTO DEPOT_REQUISITION_ISSUE_MST 
                                        (
                                            MST_ID
                                            ,REQUISITION_UNIT_ID
                                            ,ISSUE_UNIT_ID
                                            ,ISSUE_NO
                                            ,ISSUE_DATE
                                            ,REQUISITION_NO
                                            ,REQUISITION_DATE
                                            ,REQUISITION_AMOUNT
                                            ,ISSUE_AMOUNT
                                            ,ISSUE_BY
                                            ,STATUS
                                            ,COMPANY_ID
                                            ,REMARKS
                                            ,ENTERED_BY
                                            ,ENTERED_DATE
                                            ,ENTERED_TERMINAL
                                            ,TOTAL_WEIGHT
                                            ,TOTAL_VOLUME
                                         ) 
                                          VALUES ( :param1, :param2, :param3, :param4,TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM'),:param6, TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'), :param8, :param9, :param10,:param11,:param12,:param13, :param14,TO_DATE(:param15, 'DD/MM/YYYY HH:MI:SS AM'), :param16, :param17, :param18)";
        string AddOrUpdate_UpdateQuery() => @"UPDATE DEPOT_REQUISITION_ISSUE_MST SET  
                                                 REQUISITION_UNIT_ID= :param2
                                                ,ISSUE_UNIT_ID= :param3
                                                ,ISSUE_NO= :param4
                                                ,ISSUE_DATE= TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM')
                                                ,REQUISITION_NO= :param6
                                                ,REQUISITION_DATE= TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM')
                                                ,REQUISITION_AMOUNT= :param8
                                                ,ISSUE_AMOUNT= :param9
                                                ,ISSUE_BY= :param10
                                                ,STATUS= :param11
                                                ,COMPANY_ID= :param12
                                                ,REMARKS= :param13
                                                ,UPDATED_BY= :param14
                                                ,UPDATED_DATE= TO_DATE(:param15, 'DD/MM/YYYY HH:MI:SS AM')
                                                ,UPDATED_TERMINAL= :param16
                                                ,TOTAL_WEIGHT= :param17
                                                ,TOTAL_VOLUME= :param18
                                                WHERE MST_ID = :param1";
        string AddOrUpdate_AddQueryDTL() => @"INSERT INTO DEPOT_REQUISITION_ISSUE_DTL 
                    (DTL_ID
                        ,MST_ID
                        ,SKU_ID
                        ,SKU_CODE
                        ,UNIT_TP
                        ,REQUISITION_QTY
                        ,REQUISITIO_AMOUNT
                        ,ISSUE_QTY
                        ,ISSUE_AMOUNT
                        ,STATUS
                        ,COMPANY_ID
                        ,REMARKS
                        ,ENTERED_BY
                        ,ENTERED_DATE
                        ,ENTERED_TERMINAL
                        ,SHIPPER_QTY
                        ,NO_OF_SHIPPER
                        ,LOOSE_QTY
                        ,SHIPPER_WEIGHT
                        ,TOTAL_SHIPPER_WEIGHT
                        ,LOOSE_WEIGHT
                        ,TOTAL_WEIGHT

                        ,SHIPPER_VOLUME
                        ,TOTAL_SHIPPER_VOLUME
                        ,LOOSE_VOLUME
                        ,TOTAL_VOLUME
                        ,PER_PACK_VOLUME
                        ,PER_PACK_WEIGHT
                   ) 
                    VALUES ( :param1, :param2, :param3, :param4, :param5, :param6, :param7, :param8, :param9,:param10,:param11,:param12, :param13,TO_DATE(:param14, 'DD/MM/YYYY HH:MI:SS AM'), :param15,
       :param16, :param17, :param18, :param19, :param20, :param21, :param22, :param23, :param24, :param25, :param26, :param27, :param28)";

        string AddOrUpdate_UpdateQueryDTL() => @"UPDATE DEPOT_REQUISITION_ISSUE_DTL SET  
                                                 SKU_ID= :param2
                                                ,SKU_CODE= :param3
                                                ,UNIT_TP= :param4
                                                ,REQUISITION_QTY= :param5
                                                ,REQUISITIO_AMOUNT= :param6
                                                ,ISSUE_QTY= :param7
                                                ,ISSUE_AMOUNT= :param8
                                                ,STATUS= :param9
                                                ,COMPANY_ID= :param10
                                                ,REMARKS= :param11
                                                ,UPDATED_BY= :param12
                                                ,UPDATED_DATE=  TO_DATE(:param13, 'DD/MM/YYYY HH:MI:SS AM')
                                                ,UPDATED_TERMINAL= :param14
                                                ,SHIPPER_QTY= :param15
                                                ,NO_OF_SHIPPER= :param16
                                                ,LOOSE_QTY= :param17
                                                ,SHIPPER_WEIGHT= :param18
                                                ,TOTAL_SHIPPER_WEIGHT= :param19
                                                ,LOOSE_WEIGHT= :param20
                                                ,TOTAL_WEIGHT= :param21

                                                ,SHIPPER_VOLUME= :param22
                                                ,TOTAL_SHIPPER_VOLUME= :param23
                                                ,LOOSE_VOLUME= :param24
                                                ,TOTAL_VOLUME= :param25
                                                ,PER_PACK_VOLUME= :param26
                                                ,PER_PACK_WEIGHT= :param27
                                                WHERE DTL_ID = :param1";
        string LoadData_DetailByMasterId_Query() => @" SELECT ROW_NUMBER () OVER (ORDER BY B.DTL_ID ASC) AS ROW_NO,
                                                       B.* ,
                                                       C.PACK_SIZE, 
                                                       NVL((SELECT STOCK_QTY FROM VW_SKU_WISE_STOCK WHERE COMPANY_ID=A.COMPANY_ID AND UNIT_ID=A.ISSUE_UNIT_ID AND SKU_ID=B.SKU_ID),0) ISSUE_STOCK_QTY,
                                                        NVL((SELECT STOCK_QTY FROM VW_SKU_WISE_STOCK WHERE COMPANY_ID=A.COMPANY_ID AND UNIT_ID=A.REQUISITION_UNIT_ID AND SKU_ID=B.SKU_ID),0) STOCK_QTY,
                                                       B.REQUISITION_QTY ISSUE_QTY 
                                                FROM DEPOT_REQUISITION_ISSUE_MST A, DEPOT_REQUISITION_ISSUE_DTL B,PRODUCT_INFO C
                                                WHERE A.MST_ID        = B.MST_ID
                                                AND   B.SKU_ID        = C.SKU_ID
                                                AND   A.MST_ID        = :param1";

        string LoadData_MasterById_Query() => @"
            SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO,M.MST_ID                                       
                                            ,M.REQUISITION_UNIT_ID
                                            ,M.ISSUE_UNIT_ID
                                            ,M.ISSUE_NO
                                            ,TO_CHAR(M.ISSUE_DATE, 'DD/MM/YYYY') ISSUE_DATE
                                            ,M.REQUISITION_NO
                                            ,TO_CHAR(M.REQUISITION_DATE, 'DD/MM/YYYY') REQUISITION_DATE
                                            ,M.REQUISITION_AMOUNT
                                            ,M.ISSUE_AMOUNT
                                            ,M.ISSUE_BY
                                            ,M.STATUS
                                            ,M.COMPANY_ID
                                            ,M.REMARKS
                                            ,M.ENTERED_BY
                                            ,M.ENTERED_DATE
                                            ,M.ENTERED_TERMINAL    
                                            ,M.TOTAL_WEIGHT
                                            ,M.TOTAL_VOLUME
            FROM DEPOT_REQUISITION_ISSUE_MST  M 
            where MST_ID = :param1";

        string LoadBatchData_MasterById_Query() => @"
            SELECT ROW_NUMBER() OVER(ORDER BY MST_ID ASC) AS ROW_NO,ISSUE_BATCH_ID
                                            ,DTL_ID
                                            ,MST_ID
                                            ,REQUISITION_NO
                                            ,ISSUE_NO
                                            ,ISSUE_DATE
                                            ,COMPANY_ID
                                            ,REQUISITION_UNIT_ID
                                            ,ISSUE_UNIT_ID
                                            ,SKU_ID
                                            ,FN_SKU_NAME(COMPANY_ID,SKU_ID) SKU_NAME
                                            ,SKU_CODE
                                            ,UNIT_TP
                                            ,BATCH_ID
                                            ,BATCH_NO
                                            ,ISSUE_QTY
                                            ,ISSUE_AMOUNT                
            FROM DEPOT_REQUISITION_ISSUE_BATCH  
            where MST_ID = :param1";

        string LoadMasterData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO, M.MST_ID                                        
                                            ,M.REQUISITION_UNIT_ID
                                            ,M.ISSUE_UNIT_ID
                                            ,M.REQUISITION_NO
                                            ,TO_CHAR(M.REQUISITION_DATE, 'DD/MM/YYYY') REQUISITION_DATE
                                            ,M.ISSUE_NO
                                            ,M.ISSUE_DATE
                                            ,M.REQUISITION_AMOUNT
                                            ,M.ISSUE_AMOUNT
                                            ,M.ISSUE_BY
                                            ,M.STATUS
                                            ,M.COMPANY_ID
                                            ,M.REMARKS
                                            ,M.ENTERED_BY
                                            ,M.ENTERED_DATE
                                            ,M.ENTERED_TERMINAL
                                            ,M.TOTAL_WEIGHT
                                            ,M.TOTAL_VOLUME
                                            ,(SELECT  UNIT_NAME FROM COMPANY_INFO CI WHERE M.REQUISITION_UNIT_ID = CI.UNIT_ID AND ROWNUM = 1) REQUISITION_UNIT_NAME
            FROM DEPOT_REQUISITION_ISSUE_MST  M 
            where  M.COMPANY_ID = :param1 and M.ISSUE_UNIT_ID = :param2 and M.REQUISITION_NO NOT IN (SELECT REQUISITION_NO FROM DEPOT_REQUISITION_RCV_MST) ORDER BY M.MST_ID DESC";


        string LoadProductData_Query(int unitid) => string.Format(@"SELECT SKU_ID,
                                           SKU_CODE,
                                           SKU_NAME,
                                           PACK_SIZE,
                                           FN_SKU_PRICE(SKU_ID,SKU_CODE,COMPANY_ID,{0} ) UNIT_TP,
                                           FN_CURRENT_STOCK_QTY(COMPANY_ID ,:param1 ,SKU_ID) STOCK_QTY
                                    FROM Product_Info
                                    WHERE COMPANY_ID=:param2
                                    ", unitid);
        string DeleteArea_Requisition_Dtl_IdQuery() => "DELETE  FROM DEPOT_REQUISITION_ISSUE_DTL WHere DTL_ID = :param1";  

        string GetRequisition_Mst_IdQuery() => "SELECT NVL(MAX(MST_ID),0) + 1 MST_ID  FROM DEPOT_REQUISITION_ISSUE_MST";
        string GetRequisition_Issue_Dtl_IdQuery() => "SELECT NVL(MAX(DTL_ID),0) + 1 DTL_ID  FROM DEPOT_REQUISITION_ISSUE_DTL";
        string Get_LastRequisition_no() => "SELECT  ISSUE_NO  FROM DEPOT_REQUISITION_ISSUE_MST Where  MST_ID = (SELECT   NVL(MAX(MST_ID),0) MST_ID From DEPOT_REQUISITION_ISSUE_MST where COMPANY_ID = :param1 )";
        string Load_DistributionPendingRequisition_Count_Query() => @"SELECT 
   COUNT(MST_ID)  as ROWCOUNT
FROM DEPOT_REQUISITION_ISSUE_MST Where ISSUE_NO NOT IN (SELECT ISSUE_NO FROM DEPOT_DISPATCH_REQUISITION )
and COMPANY_ID = :param1 and ISSUE_UNIT_ID = :param2";
        public async Task<int> Load_DistributionPendingRequisition_Count(string db, int Company_Id, int Unit_Id)
        {
            return await _commonServices.GetMaximumNumberAsyn<int>(_configuration.GetConnectionString(db), Load_DistributionPendingRequisition_Count_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), Unit_Id.ToString() }));
        }
        public async Task<string> AddOrUpdate(string db, DEPOT_REQUISITION_ISSUE_MST model)
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
                    bool status_action = true;
                    string dtl_list = "";
                    int dtlId = 0;
                    if (model.MST_ID == 0)
                    {

                        model.MST_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetRequisition_Mst_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        model.ISSUE_NO = _commonServices.GenerateRequisitionCode(_configuration.GetConnectionString(db), "FN_GENERATE_REQ_ISSUE_NO", model.COMPANY_ID.ToString(), model.UNIT_ID.ToString());
                        
                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(),
                         _commonServices.AddParameter(new string[] {model.MST_ID.ToString(),
                             model.REQUISITION_UNIT_ID.ToString(), model.ISSUE_UNIT_ID.ToString(),model.ISSUE_NO,model.ISSUE_DATE,model.REQUISITION_NO,
                             model.REQUISITION_DATE, model.REQUISITION_AMOUNT.ToString(),model.ISSUE_AMOUNT.ToString(),model.ISSUE_BY,model.STATUS, model.COMPANY_ID.ToString(), model.REMARKS,
                             model.ENTERED_BY, model.ENTERED_DATE,  model.ENTERED_TERMINAL,model.TOTAL_WEIGHT.ToString(),model.TOTAL_VOLUME.ToString()
                            })));

                        if (model.requisitionIssueDtlList != null && model.requisitionIssueDtlList.Count > 0)
                        {
                            dtlId = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetRequisition_Issue_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { }));
                            foreach (var item in model.requisitionIssueDtlList)
                            {


                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQueryDTL(),
                               _commonServices.AddParameter(new string[] { dtlId.ToString(),model.MST_ID.ToString(),
                                     item.SKU_ID.ToString(),item.SKU_CODE,
                                     item.UNIT_TP.ToString(), item.REQUISITION_QTY.ToString(), item.REQUISITION_AMOUNT.ToString(),
                                     item.ISSUE_QTY.ToString(), item.ISSUE_AMOUNT.ToString(),item.STATUS,
                                     item.COMPANY_ID.ToString(), item.REMARKS,  item.ENTERED_BY,  item.ENTERED_DATE.ToString(),  item.ENTERED_TERMINAL,item.SHIPPER_QTY.ToString(),
                                            item.NO_OF_SHIPPER.ToString(),item.LOOSE_QTY.ToString(),item.SHIPPER_WEIGHT.ToString(),item.TOTAL_SHIPPER_WEIGHT.ToString(),item.LOOSE_WEIGHT.ToString(),item.TOTAL_WEIGHT.ToString(),
                                            item.SHIPPER_VOLUME.ToString(),item.TOTAL_SHIPPER_VOLUME.ToString(),item.LOOSE_VOLUME.ToString(),item.TOTAL_VOLUME.ToString(),item.PER_PACK_VOLUME.ToString(),item.PER_PACK_WEIGHT.ToString()
                                  })));
                                dtl_list += "," + dtlId;

                                dtlId++;
                            }
                        }
                        status_action = false;

                    }
                    else
                    {
                        DEPOT_REQUISITION_ISSUE_MST depot_Requisition_issue_Mst = await LoadDetailData_ByMasterId_List(db, model.MST_ID);
                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] { model.MST_ID.ToString(),
                             model.REQUISITION_UNIT_ID.ToString(), model.ISSUE_UNIT_ID.ToString(),
                                model.ISSUE_NO,
                             model.ISSUE_DATE,model.REQUISITION_NO,
                             model.REQUISITION_DATE, model.REQUISITION_AMOUNT.ToString(), model.ISSUE_AMOUNT.ToString(),model.ISSUE_BY,model.STATUS, model.COMPANY_ID.ToString(), model.REMARKS,
                             model.UPDATED_BY.ToString(), model.UPDATED_DATE.ToString(),
                             model.UPDATED_TERMINAL.ToString(),model.TOTAL_WEIGHT.ToString(),model.TOTAL_VOLUME.ToString()})));
                        foreach (var item in model.requisitionIssueDtlList)
                        {
                            if (item.DTL_ID == 0)
                            {
                                //-------------Add new row on detail table--------------------

                                dtlId = dtlId == 0 ? _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetRequisition_Issue_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { })) : (dtlId + 1);

                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQueryDTL(), _commonServices.AddParameter(new string[] {dtlId.ToString(),model.MST_ID.ToString(),
                                     item.SKU_ID.ToString(),item.SKU_CODE,
                                     item.UNIT_TP.ToString(), item.REQUISITION_QTY.ToString(), item.REQUISITION_AMOUNT.ToString(),
                                     item.ISSUE_QTY.ToString(), item.ISSUE_AMOUNT.ToString(),item.STATUS,
                                     item.COMPANY_ID.ToString(), item.REMARKS,  item.ENTERED_BY,  item.ENTERED_DATE,  item.ENTERED_TERMINAL,item.SHIPPER_QTY.ToString(),
                                            item.NO_OF_SHIPPER.ToString(),item.LOOSE_QTY.ToString(),item.SHIPPER_WEIGHT.ToString(),item.TOTAL_SHIPPER_WEIGHT.ToString(),item.LOOSE_WEIGHT.ToString(),item.TOTAL_WEIGHT.ToString(),
                                            item.SHIPPER_VOLUME.ToString(),item.TOTAL_SHIPPER_VOLUME.ToString(),item.LOOSE_VOLUME.ToString(),item.TOTAL_VOLUME.ToString(),item.PER_PACK_VOLUME.ToString(),item.PER_PACK_WEIGHT.ToString()})));
                                dtl_list += "," + dtlId;

                            }
                            else
                            {     
                                               
                                //-------------Edit on detail table--------------------
                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_UpdateQueryDTL(), _commonServices.AddParameter(new string[] { item.DTL_ID.ToString(),
                                     item.SKU_ID.ToString(),item.SKU_CODE,
                                     item.UNIT_TP.ToString(), item.REQUISITION_QTY.ToString(), item.REQUISITION_AMOUNT.ToString(),
                                    item.ISSUE_QTY.ToString(), item.ISSUE_AMOUNT.ToString(),item.STATUS,
                                     item.COMPANY_ID.ToString(), item.REMARKS,item.UPDATED_BY,item.UPDATED_DATE,item.UPDATED_TERMINAL,item.SHIPPER_QTY.ToString(),
                                            item.NO_OF_SHIPPER.ToString(),item.LOOSE_QTY.ToString(),item.SHIPPER_WEIGHT.ToString(),item.TOTAL_SHIPPER_WEIGHT.ToString(),item.LOOSE_WEIGHT.ToString(),item.TOTAL_WEIGHT.ToString(),
                                            item.SHIPPER_VOLUME.ToString(),item.TOTAL_SHIPPER_VOLUME.ToString(),item.LOOSE_VOLUME.ToString(),item.TOTAL_VOLUME.ToString(),item.PER_PACK_VOLUME.ToString(),item.PER_PACK_WEIGHT.ToString() })));
                                dtl_list += "," + item.DTL_ID;

                            }

                        }

                        foreach (var item in depot_Requisition_issue_Mst.requisitionIssueDtlList)
                        {
                            bool status = true;

                            foreach (var updateditem in model.requisitionIssueDtlList)
                            {
                                if (item.DTL_ID == updateditem.DTL_ID)
                                {
                                    status = false;
                                }
                            }
                            if (status)
                            {
                                //-------------Delete row from detail table--------------------

                                listOfQuery.Add(_commonServices.AddQuery(DeleteArea_Requisition_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { item.DTL_ID.ToString() })));
                                dtl_list += ",Deleted: " + item.DTL_ID;

                            }

                        }

                    }

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);

                    string st = status_action == false ? "Add" : "Edit";

                    await _logManager.AddOrUpdate(model.db_security, st, "DEPOT_REQUISITION_RAISE_MST", model.COMPANY_ID, model.UNIT_ID, Convert.ToInt32(model.ENTERED_BY), model.ENTERED_TERMINAL, "/Inventory/Requisition/Requisition", model.MST_ID, dtl_list);
                    int distribution_Pending = await this.Load_DistributionPendingRequisition_Count(db, model.COMPANY_ID, Convert.ToInt32(model.ISSUE_UNIT_ID));

                    //Notification Add---------------

                    await _NotificationManager.AddOrderNotification(model.db_security, model.db_sales, 4, "New Requisition Issued. " + model.REQUISITION_NO + " is  ready for Distribution. Pending Requisition for Distribution: " + distribution_Pending, model.COMPANY_ID, Convert.ToInt32(model.ISSUE_UNIT_ID));

                    return model.MST_ID.ToString();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public async Task<string> LoadProductData(string db, int Company_Id,int Unit_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadProductData_Query(Unit_Id), _commonServices.AddParameter(new string[] { Unit_Id.ToString(),Company_Id.ToString() })));
        public async Task<DEPOT_REQUISITION_ISSUE_MST> LoadDetailData_ByMasterId_List(string db, int _Id)
        {
            DEPOT_REQUISITION_ISSUE_MST _requisition_Mst = new DEPOT_REQUISITION_ISSUE_MST();

            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_MasterById_Query(), _commonServices.AddParameter(new string[] { _Id.ToString() }));
            if (data.Rows.Count > 0)
            {
                _requisition_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[0]["COMPANY_ID"]);

                _requisition_Mst.ROW_NO = Convert.ToInt32(data.Rows[0]["ROW_NO"]);
                _requisition_Mst.MST_ID = Convert.ToInt32(data.Rows[0]["MST_ID"]);
                _requisition_Mst.ISSUE_UNIT_ID = data.Rows[0]["ISSUE_UNIT_ID"].ToString();
                _requisition_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[0]["MST_ID"].ToString());
                _requisition_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[0]["COMPANY_ID"]);
                _requisition_Mst.REMARKS = data.Rows[0]["REMARKS"].ToString();
                _requisition_Mst.REQUISITION_AMOUNT = Convert.ToDouble(data.Rows[0]["REQUISITION_AMOUNT"]);
                _requisition_Mst.ISSUE_AMOUNT = Convert.ToDouble(data.Rows[0]["ISSUE_AMOUNT"]);
                _requisition_Mst.ISSUE_BY = data.Rows[0]["ISSUE_BY"].ToString();
                _requisition_Mst.ISSUE_DATE = data.Rows[0]["ISSUE_DATE"].ToString();
                _requisition_Mst.ISSUE_NO = data.Rows[0]["ISSUE_NO"].ToString();
                _requisition_Mst.STATUS = data.Rows[0]["STATUS"].ToString();
                _requisition_Mst.REQUISITION_DATE = data.Rows[0]["REQUISITION_DATE"].ToString();
                _requisition_Mst.REQUISITION_NO = data.Rows[0]["REQUISITION_NO"].ToString();
                _requisition_Mst.REQUISITION_UNIT_ID = data.Rows[0]["REQUISITION_UNIT_ID"].ToString(); ;
                _requisition_Mst.TOTAL_WEIGHT = Convert.ToDouble(data.Rows[0]["TOTAL_WEIGHT"]);
                _requisition_Mst.TOTAL_VOLUME = Convert.ToDouble(data.Rows[0]["TOTAL_VOLUME"]);

                DataTable dataTable_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DetailByMasterId_Query(), _commonServices.AddParameter(new string[] { _Id.ToString() }));
                _requisition_Mst.requisitionIssueDtlList = new List<DEPOT_REQUISITION_ISSUE_DTL>();
                for (int i = 0; i < dataTable_detail.Rows.Count; i++)
                {
                    DEPOT_REQUISITION_ISSUE_DTL _requisition_Dtl = new DEPOT_REQUISITION_ISSUE_DTL();

                    _requisition_Dtl.COMPANY_ID = Convert.ToInt32(dataTable_detail.Rows[i]["COMPANY_ID"]);
                    _requisition_Dtl.MST_ID = Convert.ToInt32(dataTable_detail.Rows[i]["MST_ID"]);
                    _requisition_Dtl.DTL_ID = Convert.ToInt32(dataTable_detail.Rows[i]["DTL_ID"]);

                    _requisition_Dtl.SKU_ID = dataTable_detail.Rows[i]["SKU_ID"].ToString();
                    _requisition_Dtl.SKU_CODE = dataTable_detail.Rows[i]["SKU_CODE"].ToString();
                    _requisition_Dtl.UNIT_TP = Convert.ToInt32(dataTable_detail.Rows[i]["UNIT_TP"]);
                    _requisition_Dtl.REQUISITION_QTY = Convert.ToInt32(dataTable_detail.Rows[i]["REQUISITION_QTY"]);
                    _requisition_Dtl.REQUISITION_AMOUNT = Convert.ToDouble(dataTable_detail.Rows[i]["REQUISITIO_AMOUNT"]);
                    _requisition_Dtl.ISSUE_QTY = Convert.ToInt32(dataTable_detail.Rows[i]["ISSUE_QTY"]);
                    _requisition_Dtl.STOCK_QTY = Convert.ToInt32(dataTable_detail.Rows[i]["STOCK_QTY"]);
                    _requisition_Dtl.ISSUE_STOCK_QTY = Convert.ToInt32(dataTable_detail.Rows[i]["ISSUE_STOCK_QTY"]);
                    _requisition_Dtl.ISSUE_AMOUNT = Convert.ToDouble(dataTable_detail.Rows[i]["ISSUE_AMOUNT"]);
                    _requisition_Dtl.STATUS = dataTable_detail.Rows[i]["STATUS"].ToString();
                    _requisition_Dtl.REMARKS = dataTable_detail.Rows[i]["REMARKS"].ToString();
                    _requisition_Dtl.ENTERED_BY = dataTable_detail.Rows[i]["ENTERED_BY"].ToString();
                    _requisition_Dtl.ENTERED_DATE = dataTable_detail.Rows[i]["ENTERED_DATE"].ToString();
                                    _requisition_Dtl.NO_OF_SHIPPER = Convert.ToInt32(dataTable_detail.Rows[i]["NO_OF_SHIPPER"]);
                    _requisition_Dtl.LOOSE_QTY = Convert.ToInt32(dataTable_detail.Rows[i]["LOOSE_QTY"]);
                    _requisition_Dtl.LOOSE_VOLUME = Convert.ToDouble(dataTable_detail.Rows[i]["LOOSE_VOLUME"]);
                    _requisition_Dtl.LOOSE_WEIGHT = Convert.ToDouble(dataTable_detail.Rows[i]["LOOSE_WEIGHT"]);
                    _requisition_Dtl.SHIPPER_WEIGHT = Convert.ToDouble(dataTable_detail.Rows[i]["SHIPPER_WEIGHT"]);
                    _requisition_Dtl.TOTAL_VOLUME = Convert.ToDouble(dataTable_detail.Rows[i]["TOTAL_VOLUME"]);
                    _requisition_Dtl.TOTAL_WEIGHT = Convert.ToDouble(dataTable_detail.Rows[i]["TOTAL_WEIGHT"]);


                    _requisition_Dtl.SHIPPER_QTY = Convert.ToInt32(dataTable_detail.Rows[i]["SHIPPER_QTY"]);
                    _requisition_Dtl.SHIPPER_WEIGHT = Convert.ToDouble(dataTable_detail.Rows[i]["SHIPPER_WEIGHT"]);
                    _requisition_Dtl.SHIPPER_VOLUME = Convert.ToDouble(dataTable_detail.Rows[i]["SHIPPER_VOLUME"]);

                    _requisition_Dtl.ROW_NO = Convert.ToInt32(dataTable_detail.Rows[i]["ROW_NO"]);
                    _requisition_Mst.requisitionIssueDtlList.Add(_requisition_Dtl);
                }
            }

            return _requisition_Mst;
        }


        public async Task<List<DEPOT_REQUISITION_ISSUE_BATCH>> LoadBatchDetailData_ByMasterId_List(string db, int _Id)
        {
            List<DEPOT_REQUISITION_ISSUE_BATCH> _issueBatchList = new List<DEPOT_REQUISITION_ISSUE_BATCH>();
            DataTable dataTable_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadBatchData_MasterById_Query(), _commonServices.AddParameter(new string[] { _Id.ToString() }));
            //DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadBatchData_MasterById_Query(), _commonServices.AddParameter(new string[] { _Id.ToString() }));
            if (dataTable_detail.Rows.Count > 0)
            {
             
                for (int i = 0; i < dataTable_detail.Rows.Count; i++)
                {
                    DEPOT_REQUISITION_ISSUE_BATCH _batch_dtail = new DEPOT_REQUISITION_ISSUE_BATCH();

                    _batch_dtail.COMPANY_ID = Convert.ToInt32(dataTable_detail.Rows[i]["COMPANY_ID"]);
                    _batch_dtail.MST_ID = Convert.ToInt32(dataTable_detail.Rows[i]["MST_ID"]);
                    _batch_dtail.DTL_ID = Convert.ToInt32(dataTable_detail.Rows[i]["DTL_ID"]);

                    _batch_dtail.SKU_ID = Convert.ToInt32(dataTable_detail.Rows[i]["SKU_ID"]);
                    _batch_dtail.SKU_NAME = dataTable_detail.Rows[i]["SKU_NAME"].ToString();
                    _batch_dtail.SKU_CODE = dataTable_detail.Rows[i]["SKU_CODE"].ToString();
                    _batch_dtail.UNIT_TP = Convert.ToInt32(dataTable_detail.Rows[i]["UNIT_TP"]);
                    _batch_dtail.ISSUE_QTY = Convert.ToInt32(dataTable_detail.Rows[i]["ISSUE_QTY"]);
                    _batch_dtail.ISSUE_AMOUNT = Convert.ToDouble(dataTable_detail.Rows[i]["ISSUE_AMOUNT"]);
                    _batch_dtail.BATCH_ID = Convert.ToInt32(dataTable_detail.Rows[i]["BATCH_ID"]);
                    _batch_dtail.REQUISITION_NO = dataTable_detail.Rows[i]["REQUISITION_NO"].ToString(); ;
                    _batch_dtail.ISSUE_AMOUNT = Convert.ToDouble(dataTable_detail.Rows[i]["ISSUE_AMOUNT"]);
                    _batch_dtail.ISSUE_NO = dataTable_detail.Rows[i]["ISSUE_NO"].ToString();
                    _batch_dtail.ISSUE_DATE = dataTable_detail.Rows[i]["ISSUE_DATE"].ToString();
                    _batch_dtail.ISSUE_UNIT_ID = dataTable_detail.Rows[i]["ISSUE_UNIT_ID"].ToString();
                    _batch_dtail.ISSUE_AMOUNT = Convert.ToDouble(dataTable_detail.Rows[i]["ISSUE_AMOUNT"]);
                    //_batch_dtail.STATUS = dataTable_detail.Rows[i]["STATUS"].ToString();
                    //_batch_dtail.REMARKS = dataTable_detail.Rows[i]["REMARKS"].ToString();
                    _batch_dtail.BATCH_NO = dataTable_detail.Rows[i]["BATCH_NO"].ToString();
                    //_batch_dtail.ENTERED_DATE = dataTable_detail.Rows[i]["ENTERED_DATE"].ToString();


                    _batch_dtail.ROW_NO = Convert.ToInt32(dataTable_detail.Rows[i]["ROW_NO"]);
                    _issueBatchList.Add(_batch_dtail);
                }
            }

            return _issueBatchList;
        }
        public async Task<string> GenerateIssueCode(string db, string Company_Id)
        {
            try
            {
                string code;
                DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Get_LastRequisition_no(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
                if (dataTable.Rows.Count > 0)
                {
                    string serial = (Convert.ToInt32(dataTable.Rows[0]["ISSUE_NO"].ToString().Substring(4, 4)) + 1).ToString();
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
        public Task<string> GetCustomerExistingSKUData(string db, int Company_Id, int Customer_Id)
        {
            throw new NotImplementedException();
        }

        public async Task<string> LoadData(string db, int Company_Id,int unit_id)
        { 
            List<DEPOT_REQUISITION_ISSUE_MST> requisition_Mst_list = new List<DEPOT_REQUISITION_ISSUE_MST>();

            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadMasterData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), unit_id.ToString() }));

            for (int i = 0; i < data.Rows.Count; i++)
            {
                DEPOT_REQUISITION_ISSUE_MST requisition_Mst = new DEPOT_REQUISITION_ISSUE_MST();
                requisition_Mst.ROW_NO = Convert.ToInt32(data.Rows[i]["ROW_NO"]);
                requisition_Mst.MST_ID = Convert.ToInt32(data.Rows[i]["MST_ID"]);
                requisition_Mst.ISSUE_UNIT_ID = data.Rows[i]["ISSUE_UNIT_ID"].ToString();
                requisition_Mst.REQUISITION_UNIT_NAME = data.Rows[i]["REQUISITION_UNIT_NAME"].ToString();
                requisition_Mst.REQUISITION_UNIT_ID = data.Rows[0]["REQUISITION_UNIT_ID"].ToString(); 
                requisition_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[i]["MST_ID"].ToString());
                requisition_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[i]["COMPANY_ID"]);
                requisition_Mst.REMARKS = data.Rows[i]["REMARKS"].ToString();
                requisition_Mst.REQUISITION_AMOUNT = Convert.ToDouble(data.Rows[i]["REQUISITION_AMOUNT"]);
                requisition_Mst.ISSUE_AMOUNT = Convert.ToDouble(data.Rows[i]["ISSUE_AMOUNT"]);

                requisition_Mst.STATUS = data.Rows[i]["STATUS"].ToString();

                requisition_Mst.REQUISITION_DATE = data.Rows[i]["REQUISITION_DATE"].ToString();
                requisition_Mst.ISSUE_DATE = data.Rows[i]["ISSUE_DATE"].ToString();
                requisition_Mst.REQUISITION_NO = data.Rows[i]["REQUISITION_NO"].ToString();
                requisition_Mst.ISSUE_NO = data.Rows[i]["ISSUE_NO"].ToString();
                requisition_Mst.TOTAL_WEIGHT = Convert.ToDouble(data.Rows[i]["TOTAL_WEIGHT"]);
                requisition_Mst.TOTAL_VOLUME = Convert.ToDouble(data.Rows[i]["TOTAL_VOLUME"]);

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
