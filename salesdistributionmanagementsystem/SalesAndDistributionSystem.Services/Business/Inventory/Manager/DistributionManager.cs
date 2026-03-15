using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.Company;
using SalesAndDistributionSystem.Services.Business.Inventory.IManager;
using SalesAndDistributionSystem.Services.Business.Security;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Inventory.Manager
{
    public class DistributionManager : IDistributionManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        private readonly IUserLogManager _logManager;
        private readonly INotificationManager _NotificationManager;

        public DistributionManager(ICommonServices commonServices, IConfiguration configuration, INotificationManager NotificationManager, IUserLogManager logManager)
        {
            _commonServices = commonServices;
            _configuration = configuration;
            _NotificationManager = NotificationManager;
            _logManager = logManager;
        }

        private string AddOrUpdate_AddQuery() => @"INSERT INTO DEPOT_DISPATCH_MST
                                        (
                                           MST_ID
                                            ,DISPATCH_NO
                                            ,DISPATCH_DATE
                                            ,VECHILE_NO
                                            ,VECHILE_DESCRIPTION
                                            ,VECHILE_VOLUME
                                            ,VECHILE_WEIGHT
                                            ,DISPATCH_VOLUME
                                            ,DISPATCH_WEIGHT
                                            ,DRIVER_ID
                                            ,DISPATCH_BY
                                            ,DISPATCH_UNIT_ID
                                            ,COMPANY_ID
                                            ,REMARKS
                                            ,ENTERED_BY
                                            ,ENTERED_DATE
                                            ,ENTERED_TERMINAL
                                            ,DISPATCH_TYPE

                                         )
                                          VALUES ( :param1, :param2,TO_DATE(:param3, 'DD/MM/YYYY HH:MI:SS AM'), :param4,:param5 ,:param6, :param7, :param8, :param9, :param10, :param11,:param12,:param13,:param14, :param15,TO_DATE(:param16, 'DD/MM/YYYY HH:MI:SS AM') , :param17, :param18)";

        private string AddOrUpdate_UpdateQuery() => @"UPDATE DEPOT_DISPATCH_MST SET

                                                DISPATCH_NO= :param2
                                                ,DISPATCH_DATE= TO_DATE(:param3, 'DD/MM/YYYY HH:MI:SS AM')
                                                ,VECHILE_NO= :param4
                                                ,VECHILE_DESCRIPTION= :param5
                                                ,VECHILE_VOLUME= :param6
                                                ,VECHILE_WEIGHT= :param7
                                                ,DISPATCH_VOLUME= :param8
                                                ,DISPATCH_WEIGHT= :param9
                                                ,DRIVER_ID= :param10
                                                ,DISPATCH_BY= :param11
                                                ,DISPATCH_UNIT_ID= :param12
                                                ,COMPANY_ID= :param13
                                                ,REMARKS= :param14
                                                ,UPDATED_BY= :param15
                                                ,UPDATED_DATE= TO_DATE(:param16, 'DD/MM/YYYY HH:MI:SS AM')
                                                ,UPDATED_TERMINAL= :param17
                                                ,DISPATCH_TYPE= :param18
                                                WHERE MST_ID = :param1";

        private string AddOrUpdate_AddQueryIssue() => @"INSERT INTO DEPOT_DISPATCH_REQUISITION
                    (DISPATCH_REQ_ID
                        ,MST_ID
                        ,REQUISITION_NO
                        ,REQUISITION_DATE
                        ,REQUISITION_UNIT_ID
                        ,ISSUE_NO
                        ,ISSUE_DATE
                        ,ISSUE_UNIT_ID
                        ,DISPATCH_UNIT_ID
                        ,COMPANY_ID
                        ,ENTERED_BY
                        ,ENTERED_DATE
                        ,ENTERED_TERMINAL

                   )
                    VALUES ( :param1, :param2, :param3,TO_DATE(:param4, 'DD/MM/YYYY HH:MI:SS AM'), :param5, :param6,TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'), :param8, :param9,:param10,:param11,TO_DATE(:param12, 'DD/MM/YYYY HH:MI:SS AM'), :param13)";

        private string AddOrUpdate_UpdateQueryIssue() => @"UPDATE DEPOT_DISPATCH_REQUISITION SET
                                                    REQUISITION_NO= :param2
                                                    ,REQUISITION_DATE=  TO_DATE(:param3, 'DD/MM/YYYY HH:MI:SS AM')
                                                    ,REQUISITION_UNIT_ID= :param4
                                                    ,ISSUE_NO= :param5
                                                    ,ISSUE_DATE=   TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM')
                                                    ,ISSUE_UNIT_ID= :param7
                                                    ,DISPATCH_UNIT_ID= :param8
                                                    ,COMPANY_ID= :param9
                                                    ,UPDATED_BY= :param10
                                                    ,UPDATED_DATE=  TO_DATE(:param11, 'DD/MM/YYYY HH:MI:SS AM')
                                                    ,UPDATED_TERMINAL= :param12
                                                    WHERE DISPATCH_REQ_ID = :param1";

        private string AddOrUpdate_AddQueryProduct() => @"INSERT INTO DEPOT_DISPATCH_PRODUCT
                    (   DISPATCH_PRODUCT_ID
                        ,DISPATCH_REQ_ID
                        ,REQUISITION_NO
                        ,COMPANY_ID
                        ,DISPATCH_UNIT_ID
                        ,DISPATCH_DATE
                        ,MST_ID
                        ,SKU_ID
                        ,SKU_CODE
                        ,UNIT_TP
                        ,ISSUE_QTY
                        ,DISPATCH_QTY
                        ,DISPATCH_AMOUNT
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
                        ,ENTERED_BY
                        ,ENTERED_DATE
                        ,ENTERED_TERMINAL
                        ,DISPATCH_NO
                        ,ISSUE_AMOUNT
                        ,ISSUE_NO
                        ,DISPATCH_TYPE
                   )
                    VALUES ( :param1, :param2, :param3,:param4,:param5, TO_DATE( :param6, 'DD/MM/YYYY HH:MI:SS AM'),:param7,:param8,:param9, :param10,:param11,:param12,:param13,:param14,:param15,:param16,:param17,:param18,:param19,:param20,:param21,:param22,:param23,:param24,:param25,:param26,:param27,TO_DATE(:param28, 'DD/MM/YYYY HH:MI:SS AM'),:param29,:param30,:PARAM31,:param32, :param33)";

        private string AddOrUpdate_UpdateQueryProduct() => @"UPDATE DEPOT_DISPATCH_PRODUCT SET
                                                    DISPATCH_REQ_ID= :param2
                                                     ,REQUISITION_NO= :param3
                                                    ,COMPANY_ID= :param4
                                                    ,DISPATCH_UNIT_ID= :param5
                                                    ,DISPATCH_DATE= TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM')
                                                    ,MST_ID= :param7
                                                    ,SKU_ID= :param8
                                                    ,SKU_CODE= :param9
                                                    ,UNIT_TP= :param10
                                                    ,ISSUE_QTY= :param11
                                                    ,DISPATCH_QTY= :param12

                                                    ,DISPATCH_AMOUNT= :param13
                                                    ,SHIPPER_QTY= :param14
                                                    ,NO_OF_SHIPPER= :param15
                                                    ,LOOSE_QTY= :param16
                                                    ,SHIPPER_WEIGHT= :param17
                                                    ,TOTAL_SHIPPER_WEIGHT= :param18
                                                    ,LOOSE_WEIGHT= :param19
                                                    ,TOTAL_WEIGHT= :param20

                                                    ,SHIPPER_VOLUME= :param21
                                                    ,TOTAL_SHIPPER_VOLUME= :param22
                                                    ,LOOSE_VOLUME= :param23
                                                    ,TOTAL_VOLUME= :param24
                                                    ,PER_PACK_VOLUME= :param25
                                                    ,PER_PACK_WEIGHT= :param26
                                                    ,UPDATED_BY= :param27
                                                    ,UPDATED_DATE=  TO_DATE(:param28, 'DD/MM/YYYY HH:MI:SS AM')
                                                    ,UPDATED_TERMINAL= :param29
                                                    ,DISPATCH_NO=:param30

                                                    ,ISSUE_AMOUNT=:param31
                                                     ,ISSUE_NO=:param32
                                                    WHERE DISPATCH_PRODUCT_ID = :param1";

        private string LoadBatchData_MasterById_Query() => @"
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

        private string LoadMasterData_Query() => @"SELECT ROW_NUMBER () OVER (ORDER BY M.MST_ID DESC) AS ROW_NO,
         M.MST_ID,
         M.DISPATCH_NO,
         TO_CHAR (M.DISPATCH_DATE, 'DD/MM/YYYY') DISPATCH_DATE,
         NVL (M.VECHILE_NO, '') AS VEHICLE_NO,
         M.VECHILE_DESCRIPTION,
         M.VECHILE_VOLUME,
         M.VECHILE_WEIGHT,
         M.DISPATCH_VOLUME,
         M.DISPATCH_WEIGHT,
         M.DRIVER_ID,
         I.USER_NAME DISPATCH_BY,
         M.DISPATCH_UNIT_ID,
         M.COMPANY_ID,
         M.REMARKS,
         M.ENTERED_BY,
         M.ENTERED_DATE,
         M.ENTERED_TERMINAL,
        M.DISPATCH_TYPE
    FROM DEPOT_DISPATCH_MST M, STL_ERP_SCS.USER_INFO I
   WHERE I.USER_ID= M.ENTERED_BY AND M.COMPANY_ID = :param1
";

        private string LoadData_MasterById_Query() => @"
            SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO,M.MST_ID

                                            ,M.DISPATCH_NO
                                            , TO_CHAR(M.DISPATCH_DATE, 'DD/MM/YYYY') DISPATCH_DATE
                                            ,M.VECHILE_NO
                                            ,M.VECHILE_DESCRIPTION
                                            ,M.VECHILE_VOLUME
                                            ,M.VECHILE_WEIGHT
                                            ,M.DISPATCH_VOLUME
                                            ,M.DISPATCH_WEIGHT
                                            ,M.DRIVER_ID
                                            ,M.DISPATCH_BY
                                            ,M.DISPATCH_UNIT_ID
                                            ,M.COMPANY_ID
                                            ,M.REMARKS
                                            ,M.ENTERED_BY
                                            ,M.ENTERED_DATE
                                            ,M.ENTERED_TERMINAL
                                            ,M.DISPATCH_TYPE
            FROM DEPOT_DISPATCH_MST  M
            where M.MST_ID = :param1";

        private string LoadData_DistributionReq_Query() => @" SELECT ROW_NUMBER () OVER (ORDER BY A.DISPATCH_REQ_ID ASC) AS ROW_NO,
                                                       A.*
                                                      ,TO_CHAR(A.REQUISITION_DATE, 'DD/MM/YYYY') REQUISITION_DATE_FORMATED
                                                        ,TO_CHAR(A.ISSUE_DATE, 'DD/MM/YYYY') ISSUE_DATE_FORMATED
                                                        ,FN_UNIT_NAME(A.COMPANY_ID,A.ISSUE_UNIT_ID) ISSUE_UNIT_NAME
                                                        ,FN_UNIT_NAME(A.COMPANY_ID,A.REQUISITION_UNIT_ID) REQUISITION_UNIT_NAME

                                                FROM DEPOT_DISPATCH_REQUISITION A
                                                WHERE A.MST_ID = :param1";

        private string LoadData_All_DistributionReq_Query() => @" SELECT ROW_NUMBER () OVER (ORDER BY A.DISPATCH_REQ_ID ASC) AS ROW_NO,
                                                       A.*
                                                      ,TO_CHAR(A.REQUISITION_DATE, 'DD/MM/YYYY') REQUISITION_DATE_FORMATED
                                                        ,TO_CHAR(A.ISSUE_DATE, 'DD/MM/YYYY') ISSUE_DATE_FORMATED
                                                        ,FN_UNIT_NAME(A.COMPANY_ID,A.ISSUE_UNIT_ID) ISSUE_UNIT_NAME
                                                        ,FN_UNIT_NAME(A.COMPANY_ID,A.REQUISITION_UNIT_ID) REQUISITION_UNIT_NAME
                                                       ,B.DISPATCH_NO
                                                FROM DEPOT_DISPATCH_REQUISITION A
                                                left join DEPOT_DISPATCH_MST B on B.MST_ID = A.MST_ID
                                                  where B.DISPATCH_TYPE='Requisition' AND A.COMPANY_ID=:param1 and A.REQUISITION_UNIT_ID=:param2 AND B.DISPATCH_NO not in(select NVL(DISPATCH_NO,0) from DEPOT_REQUISITION_RCV_MST)
                                                ";

        private string LoadData_All_DistributionProduct_Query() => @" SELECT ROW_NUMBER () OVER (ORDER BY A.DISPATCH_PRODUCT_ID ASC) AS ROW_NO,
                                                       A.*
                                                       ,FN_SKU_NAME(A.COMPANY_ID,A.SKU_ID) SKU_NAME
                                                       ,TO_CHAR(A.DISPATCH_DATE, 'DD/MM/YYYY') DISPATCH_DATE_FORMATED
                                                FROM DEPOT_DISPATCH_PRODUCT A
                                                WHERE A.DISPATCH_REQ_ID = :param1";

        private string LoadData_DistributionProduct_Query() => @" SELECT ROW_NUMBER () OVER (ORDER BY A.DISPATCH_PRODUCT_ID ASC) AS ROW_NO,
                                                       A.*

                                                       ,FN_SKU_NAME(A.COMPANY_ID,A.SKU_ID) SKU_NAME
                                                        ,TO_CHAR(A.DISPATCH_DATE, 'DD/MM/YYYY') DISPATCH_DATE_FORMATED
                                                        ,(SELECT SUM(DISPATCH_QTY) FROM DEPOT_DISPATCH_PRODUCT WHERE SKU_CODE = A.SKU_CODE AND REQUISITION_NO=A.REQUISITION_NO) TOTALDISQTY
                                                FROM DEPOT_DISPATCH_PRODUCT A
                                                WHERE A.DISPATCH_REQ_ID = :param1";

        private string LoadData_Shipper_Dtl_Query() => @" SELECT ROW_NUMBER () OVER (ORDER BY A.DISPATCH_SHIPPER_DTL_ID ASC) AS ROW_NO,
                                                       A.*
                                                       ,FN_SKU_NAME(A.COMPANY_ID,A.SKU_ID) SKU_NAME

                                                FROM DEPOT_DISPATCH_SHIPPER_DTL A
                                                WHERE A.Company_Id =  :param1 and A.MST_ID = :param2";

        private string LoadData_Batch_Dtl_Query() => @" SELECT ROW_NUMBER () OVER (ORDER BY A.DISPATCH_PROD_BATCH_ID ASC) AS ROW_NO,
                                                       A.*
                                                       ,FN_SKU_NAME(A.COMPANY_ID,A.SKU_ID) SKU_NAME

                                                FROM DEPOT_DISPATCH_PRODUCT_BATCH A
                                                WHERE A.Company_Id =  :param1 and A.MST_ID = :param2";

        private string Insert_Shipper_Dtl_Query() => @"
                      begin
                            PRC_DEPOT_DISPATCH_SHIPPER_DTL(:param1);
                      end;";

        private string Delete_Request_Dtl_IdQuery() => "DELETE  FROM DEPOT_DISPATCH_REQUISITION WHere DISPATCH_REQ_ID = :param1";

        private string Delete_Product_Dtl_IdQuery() => "DELETE  FROM DEPOT_DISPATCH_PRODUCT WHere DISPATCH_PRODUCT_ID = :param1";

        private string DeleteArea_Requisition_Dtl_IdQuery() => "DELETE  FROM DEPOT_DISPATCH_REQUISITION WHere DISPATCH_REQ_ID = :param1";

        private string GetDistribution_Mst_IdQuery() => "SELECT NVL(MAX(MST_ID),0) + 1 MST_ID  FROM DEPOT_DISPATCH_MST";

        private string Get_LastDistribution_no() => "SELECT  DISPATCH_NO  FROM DEPOT_DISPATCH_MST Where  MST_ID = (SELECT   NVL(MAX(MST_ID),0) MST_ID From DEPOT_DISPATCH_MST where COMPANY_ID = :param1 )";

        private string GetRequisition_Issue_IdQuery() => "SELECT NVL(MAX(DISPATCH_REQ_ID),0) + 1 DISPATCH_REQ_ID  FROM DEPOT_DISPATCH_REQUISITION";

        private string GetRequisition_Product_IdQuery() => "SELECT NVL(MAX(DISPATCH_PRODUCT_ID),0) + 1 DISPATCH_PRODUCT_ID  FROM DEPOT_DISPATCH_PRODUCT";

        private string LoadPendingReceiveCount_Query() => @"SELECT
   COUNT(MST_ID)  as ROWCOUNT
FROM DEPOT_DISPATCH_REQUISITION Where ISSUE_NO NOT IN (SELECT ISSUE_NO FROM DEPOT_REQUISITION_RCV_MST )
and COMPANY_ID = :param1 and REQUISITION_UNIT_ID = :param2";

        public async Task<int> LoadPendingReceiveCount(string db, int Company_Id, int Unit_Id)
        {
            return await _commonServices.GetMaximumNumberAsyn<int>(_configuration.GetConnectionString(db), LoadPendingReceiveCount_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), Unit_Id.ToString() }));
        }

        public async Task<string> AddOrUpdate(string db, DEPOT_DISPATCH_MST model)
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
                    int DISPATCH_REQ_ID = 0;
                    int DISPATCH_REQ_PRODUCT_ID = 0;
                    List<Pending_Receive_Add> req_receive_pending = new List<Pending_Receive_Add>();
                    if (model.MST_ID == 0)
                    {
                        var (flagInsert, mgsInsert) = await CheckDelivaryQty(db, model);
                        if (flagInsert)
                        {
                            model.MST_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetDistribution_Mst_IdQuery(), _commonServices.AddParameter(new string[] { }));
                            model.DISPATCH_NO = _commonServices.GenerateRequisitionCode(_configuration.GetConnectionString(db), "FN_GENERATE_DISPATCH_NO", model.COMPANY_ID.ToString(), model.UNIT_ID.ToString());
                            listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(), _commonServices.AddParameter(new string[] {model.MST_ID.ToString(),
                             model.DISPATCH_NO.ToString(), model.DISPATCH_DATE.ToString(),model.VEHICLE_NO,model.VEHICLE_DESCRIPTION,model.VECHILE_VOLUME.ToString(),
                             model.VECHILE_WEIGHT.ToString(), model.DISPATCH_VOLUME.ToString(), model.DISPATCH_WEIGHT.ToString(), model.DRIVER_ID.ToString(),model.DISPATCH_BY,model.DISPATCH_UNIT_ID.ToString(), model.COMPANY_ID.ToString(), model.REMARKS,
                             model.ENTERED_BY, model.ENTERED_DATE,  model.ENTERED_TERMINAL,model.DISPATCH_TYPE
                            })));
                            #region Distribution Request Add
                            if (model.requisitionIssueDtlList != null && model.requisitionIssueDtlList.Count > 0)
                            {
                                DISPATCH_REQ_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetRequisition_Issue_IdQuery(), _commonServices.AddParameter(new string[] { }));
                                DISPATCH_REQ_PRODUCT_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetRequisition_Product_IdQuery(), _commonServices.AddParameter(new string[] { }));
                                foreach (var item in model.requisitionIssueDtlList)
                                {
                                    //item.REQUISITION_DATE = Convert.ToDateTime(item.REQUISITION_DATE).ToString("dd/MM/yyyy hh:mm:ss");
                                    //item.ISSUE_DATE = Convert.ToDateTime(item.ISSUE_DATE).ToString("dd/MM/yyyy hh:mm:ss");

                                    listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQueryIssue(),
                                   _commonServices.AddParameter(new string[] { DISPATCH_REQ_ID.ToString(),model.MST_ID.ToString(),
                                     item.REQUISITION_NO,item.REQUISITION_DATE,
                                     item.REQUISITION_UNIT_ID.ToString(), item.ISSUE_NO.ToString(), item.ISSUE_DATE.ToString(),
                                     item.ISSUE_UNIT_ID.ToString(),item.DISPATCH_UNIT_ID.ToString(),
                                     item.COMPANY_ID.ToString(),  item.ENTERED_BY,  item.ENTERED_DATE,  item.ENTERED_TERMINAL
                                      })));

                                    #region Distribution Product Add

                                    if (item.requisitionProductDtlList != null && item.requisitionProductDtlList.Count > 0)
                                    {
                                        foreach (var product in item.requisitionProductDtlList)
                                        {
                                            listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQueryProduct(),
                                                _commonServices.AddParameter(new string[] {
                                                DISPATCH_REQ_PRODUCT_ID.ToString(), DISPATCH_REQ_ID.ToString(),
                                                product.REQUISITION_NO.ToString(), product.COMPANY_ID.ToString(),
                                                product.DISPATCH_UNIT_ID.ToString(), product.DISPATCH_DATE,
                                                model.MST_ID.ToString(), product.SKU_ID.ToString(),
                                                product.SKU_CODE.ToString(), product.UNIT_TP.ToString(),
                                                product.ISSUE_QTY.ToString(), product.DISPATCH_QTY.ToString(),
                                                product.DISPATCH_AMOUNT.ToString(), product.SHIPPER_QTY.ToString(),
                                                product.NO_OF_SHIPPER.ToString(), product.LOOSE_QTY.ToString(),
                                                product.SHIPPER_WEIGHT.ToString(), product.TOTAL_SHIPPER_WEIGHT.ToString(),
                                                product.LOOSE_WEIGHT.ToString(), product.TOTAL_WEIGHT.ToString(),
                                                product.SHIPPER_VOLUME.ToString(), product.TOTAL_SHIPPER_VOLUME.ToString(),
                                                product.LOOSE_VOLUME.ToString(), product.TOTAL_VOLUME.ToString(),
                                                product.PER_PACK_VOLUME.ToString(),product.PER_PACK_WEIGHT.ToString(),
                                                product.ENTERED_BY.ToString(),  product.ENTERED_DATE,
                                                product.ENTERED_TERMINAL, model.DISPATCH_NO, product.ISSUE_AMOUNT.ToString(),
                                                product.ISSUE_NO.ToString(), model.DISPATCH_TYPE
                                            })));

                                            DISPATCH_REQ_PRODUCT_ID++;
                                        }
                                    }

                                    #endregion Distribution Product Add

                                    dtl_list += "," + DISPATCH_REQ_ID;
                                    DISPATCH_REQ_ID++;
                                    Pending_Receive_Add add_data = new Pending_Receive_Add();
                                    add_data.REQUISITION_UNIT_ID = item.REQUISITION_UNIT_ID;
                                    add_data.ISSUE_NO = item.ISSUE_NO;
                                    req_receive_pending.Add(add_data);
                                }
                            }

                            #endregion Distribution Request Add
                        }
                        else
                        {
                            throw new Exception(mgsInsert);
                        }
                    }
                    else
                    {
                        //DEPOT_REQUISITION_ISSUE_MST depot_Requisition_issue_Mst = await LoadDetailData_ByMasterId_List(db, model.MST_ID);
                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] { model.MST_ID.ToString(),
                             model.DISPATCH_NO.ToString(), model.DISPATCH_DATE.ToString(),model.VEHICLE_NO,model.VEHICLE_DESCRIPTION,model.VECHILE_VOLUME.ToString(),
                             model.VECHILE_WEIGHT.ToString(), model.DISPATCH_VOLUME.ToString(), model.DISPATCH_WEIGHT.ToString(), model.DRIVER_ID.ToString(),model.DISPATCH_BY,model.DISPATCH_UNIT_ID.ToString(),
                             model.COMPANY_ID.ToString(), model.REMARKS,
                             model.UPDATED_BY.ToString(), model.UPDATED_DATE.ToString(),
                             model.UPDATED_TERMINAL.ToString(),model.DISPATCH_TYPE})));
                        foreach (var item in model.requisitionIssueDtlList)
                        {
                            // item.REQUISITION_DATE = Convert.ToDateTime(item.REQUISITION_DATE).ToString("dd/MM/yyyy hh:mm:ss");
                            //item.ISSUE_DATE = Convert.ToDateTime(item.ISSUE_DATE).ToString("dd/MM/yyyy hh:mm:ss");

                            if (item.DISPATCH_REQ_ID == 0)
                            {
                                //-------------Add new row on detail table--------------------

                                DISPATCH_REQ_ID = DISPATCH_REQ_ID == 0 ? _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetRequisition_Issue_IdQuery(), _commonServices.AddParameter(new string[] { })) : (DISPATCH_REQ_ID + 1);
                                DISPATCH_REQ_PRODUCT_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetRequisition_Product_IdQuery(), _commonServices.AddParameter(new string[] { }));
                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQueryIssue(),
                                  _commonServices.AddParameter(new string[] {DISPATCH_REQ_ID.ToString(),model.MST_ID.ToString(),
                                     item.REQUISITION_NO,item.REQUISITION_DATE,
                                     item.REQUISITION_UNIT_ID.ToString(), item.ISSUE_NO.ToString(), item.ISSUE_DATE.ToString(),
                                     item.ISSUE_UNIT_ID.ToString(),item.DISPATCH_UNIT_ID.ToString(),
                                     item.COMPANY_ID.ToString(),  item.ENTERED_BY,  item.ENTERED_DATE,  item.ENTERED_TERMINAL
                                })));

                                #region Distribution Product Add

                                if (item.requisitionProductDtlList != null && item.requisitionProductDtlList.Count > 0)
                                {
                                    foreach (var product in item.requisitionProductDtlList)
                                    {
                                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQueryProduct(),
                                        _commonServices.AddParameter(new string[] {
                                          DISPATCH_REQ_PRODUCT_ID.ToString(), DISPATCH_REQ_ID.ToString(),
                                            product.REQUISITION_NO.ToString(), product.COMPANY_ID.ToString(),
                                            product.DISPATCH_UNIT_ID.ToString(), product.DISPATCH_DATE,
                                            model.MST_ID.ToString(), product.SKU_ID.ToString(),
                                            product.SKU_CODE.ToString(), product.UNIT_TP.ToString(),
                                            product.ISSUE_QTY.ToString(), product.DISPATCH_QTY.ToString(),
                                            product.DISPATCH_AMOUNT.ToString(), product.SHIPPER_QTY.ToString(),
                                            product.NO_OF_SHIPPER.ToString(), product.LOOSE_QTY.ToString(),
                                            product.SHIPPER_WEIGHT.ToString(), product.TOTAL_SHIPPER_WEIGHT.ToString(),
                                            product.LOOSE_WEIGHT.ToString(), product.TOTAL_WEIGHT.ToString(),
                                            product.SHIPPER_VOLUME.ToString(), product.TOTAL_SHIPPER_VOLUME.ToString(),
                                            product.LOOSE_VOLUME.ToString(), product.TOTAL_VOLUME.ToString(),
                                            product.PER_PACK_VOLUME.ToString(), product.PER_PACK_WEIGHT.ToString(),
                                            product.ENTERED_BY.ToString(), product.ENTERED_DATE,
                                            product.ENTERED_TERMINAL, model.DISPATCH_NO, product.ISSUE_AMOUNT.ToString(),
                                            product.ISSUE_NO.ToString(), model.DISPATCH_TYPE
                                        })));

                                        DISPATCH_REQ_PRODUCT_ID++;
                                    }
                                }

                                #endregion Distribution Product Add

                                dtl_list += "," + DISPATCH_REQ_ID;

                                DISPATCH_REQ_ID++;
                                Pending_Receive_Add add_data = new Pending_Receive_Add();
                                add_data.REQUISITION_UNIT_ID = item.REQUISITION_UNIT_ID;
                                add_data.ISSUE_NO = item.ISSUE_NO;
                                req_receive_pending.Add(add_data);
                            }
                            else
                            {
                                DEPOT_DISPATCH_MST depot_Distribution_issue_Mst = await LoadDistributionDetailData_ByMasterId(db, model.MST_ID);

                                //-------------Edit on detail table--------------------
                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_UpdateQueryIssue(), _commonServices.AddParameter(new string[] { item.DISPATCH_REQ_ID.ToString(),
                                     item.REQUISITION_NO,item.REQUISITION_DATE,
                                     item.REQUISITION_UNIT_ID.ToString(), item.ISSUE_NO.ToString(), item.ISSUE_DATE.ToString(),
                                     item.ISSUE_UNIT_ID.ToString(),item.DISPATCH_UNIT_ID.ToString(),
                                     item.COMPANY_ID.ToString(),item.UPDATED_BY,item.UPDATED_DATE,item.UPDATED_TERMINAL })));

                                #region Distribution Product Update

                                if (item.requisitionProductDtlList != null && item.requisitionProductDtlList.Count > 0)
                                {
                                    foreach (var product in item.requisitionProductDtlList)
                                    {
                                        //product.DISTRIBUTION_DATE = Convert.ToDateTime(product.DISTRIBUTION_DATE).ToString("dd/MM/yyyy hh:mm:ss");

                                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_UpdateQueryProduct(),
                                        _commonServices.AddParameter(new string[] {
                                            product.DISPATCH_PRODUCT_ID.ToString(),product.DISPATCH_REQ_ID.ToString(),product.REQUISITION_NO.ToString(),
                                             product.COMPANY_ID.ToString(),product.DISPATCH_UNIT_ID.ToString(),product.DISPATCH_DATE,model.MST_ID.ToString(),product.SKU_ID.ToString(),
                                            product.SKU_CODE.ToString(),product.UNIT_TP.ToString(),product.ISSUE_QTY.ToString(),product.DISPATCH_QTY.ToString(),product.DISPATCH_AMOUNT.ToString(),product.SHIPPER_QTY.ToString(),
                                            product.NO_OF_SHIPPER.ToString(),product.LOOSE_QTY.ToString(),product.SHIPPER_WEIGHT.ToString(),product.TOTAL_SHIPPER_WEIGHT.ToString(),product.LOOSE_WEIGHT.ToString(),product.TOTAL_WEIGHT.ToString(),
                                            product.SHIPPER_VOLUME.ToString(),product.TOTAL_SHIPPER_VOLUME.ToString(),product.LOOSE_VOLUME.ToString(),product.TOTAL_VOLUME.ToString(),product.PER_PACK_VOLUME.ToString(),product.PER_PACK_WEIGHT.ToString(),
                                            product.UPDATED_BY.ToString(),  product.UPDATED_DATE,  product.UPDATED_TERMINAL,model.DISPATCH_NO,product.ISSUE_AMOUNT.ToString(),product.ISSUE_NO.ToString()
                                        })));
                                    }
                                }

                                #endregion Distribution Product Update

                                #region delete Requisition

                                foreach (var distribution in depot_Distribution_issue_Mst.requisitionIssueDtlList)
                                {
                                    bool status = true;

                                    foreach (var updateditem in model.requisitionIssueDtlList)
                                    {
                                        if (distribution.DISPATCH_REQ_ID == updateditem.DISPATCH_REQ_ID)
                                        {
                                            #region Delete Product

                                            foreach (var productList in distribution.requisitionProductDtlList)
                                            {
                                                bool productStatus = true;
                                                foreach (var updatedProduct in updateditem.requisitionProductDtlList)
                                                {
                                                    if (productList.DISPATCH_PRODUCT_ID == updatedProduct.DISPATCH_PRODUCT_ID)
                                                    {
                                                        productStatus = false;
                                                        break;
                                                    }
                                                }
                                                if (productStatus)
                                                {
                                                    listOfQuery.Add(_commonServices.AddQuery(Delete_Product_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { productList.DISPATCH_PRODUCT_ID.ToString() })));
                                                }
                                            }

                                            #endregion Delete Product

                                            status = false;
                                        }
                                    }
                                    if (status)
                                    {
                                        //-------------Delete row from detail table--------------------
                                        foreach (var product in distribution.requisitionProductDtlList)
                                        {
                                            listOfQuery.Add(_commonServices.AddQuery(Delete_Product_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { product.DISPATCH_PRODUCT_ID.ToString() })));
                                        }
                                        listOfQuery.Add(_commonServices.AddQuery(Delete_Request_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { distribution.DISPATCH_REQ_ID.ToString() })));
                                    }
                                }

                                #endregion delete Requisition
                            }
                        }
                    }

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    string st = status_action == false ? "Add" : "Edit";
                    await _logManager.AddOrUpdate(model.db_security, st, "DEPOT_REQUISITION_RAISE_MST", model.COMPANY_ID, model.UNIT_ID, Convert.ToInt32(model.ENTERED_BY), model.ENTERED_TERMINAL, "/Inventory/Requisition/Requisition", model.MST_ID, dtl_list);

                    foreach (var item in req_receive_pending)
                    {
                        int distribution_Pending = await this.LoadPendingReceiveCount(db, model.COMPANY_ID, Convert.ToInt32(item.REQUISITION_UNIT_ID));

                        //Notification Add---------------

                        await _NotificationManager.AddOrderNotification(model.db_security, model.db_sales, 5, "New Dispatch Issued. " + model.DISPATCH_NO + "(Issue No: " + item.ISSUE_NO + ") is ready to Receive. Pending Requisition to Receive: " + distribution_Pending, model.COMPANY_ID, Convert.ToInt32(item.REQUISITION_UNIT_ID));
                    }
                    await AddShipperDetails(db, model.MST_ID);
                    return model.MST_ID.ToString();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            // throw new NotImplementedException();
        }

        public async Task<string> AddShipperDetails(string db, int Mst_Id)
        {
            List<QueryPattern> listOfQuery = new List<QueryPattern>();

            List<string> _result;
            Result result_final = new Result();
            string result;
            result_final.Key = "1";
            result = await _commonServices.ProcedureCallAsyn<string>(_configuration.GetConnectionString(db), Insert_Shipper_Dtl_Query(), _commonServices.AddParameter(new string[] { Mst_Id.ToString() }));

            result = JsonSerializer.Serialize(result_final);
            return result;
        }

        public async Task<string> GetPendingRequisition(string db, int companyId, int unitId)
        {
            var query = String.Format(@"begin :param1 := fn_issue_pending_dispatch({0},'{1}');
                          end;", companyId, unitId);

            var dt = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] { "RefCursor" }));
            //var dt = await _commonServices.GetDataSetAsyn(_configuration.GetConnectionString(db), PendingInvoice_Query(),
            //    _commonServices.AddParameter(new string[] { companyId.ToString(), unitId.ToString() }));
            var ds = new DataSet();
            ds.Tables.Add(dt);

            return _commonServices.DataSetToJSON(ds);
        }

        public async Task<string> GetPendingStock(string db, int companyId, int unitId)
        {
            var query = String.Format(@"begin :param1 := fn_trans_pending_dispatch({0},'{1}');
                          end;", companyId, unitId);

            var dt = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] { "RefCursor" }));
            //var dt = await _commonServices.GetDataSetAsyn(_configuration.GetConnectionString(db), PendingInvoice_Query(),
            //    _commonServices.AddParameter(new string[] { companyId.ToString(), unitId.ToString() }));
            var ds = new DataSet();
            ds.Tables.Add(dt);

            return _commonServices.DataSetToJSON(ds);
        }

        public async Task<string> GetProductsByRequisition(string db, int companyId, int unitId, string RequisitionNo)
        {
            var query = String.Format(@"begin :param1 := FN_ISSUE_PROD_PENDING_DISPATCH({0},'{1}', '{2}');
                          end;", companyId, unitId, RequisitionNo);

            var dt = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] { "RefCursor" }));
            //var dt = await _commonServices.GetDataSetAsyn(_configuration.GetConnectionString(db), GetProductsByInvoice_Query(),
            //    _commonServices.AddParameter(new string[] { companyId.ToString(), invoiceNo }));
            var ds = new DataSet();
            ds.Tables.Add(dt);

            return _commonServices.DataSetToJSON(ds);
        }

        public async Task<string> GetProductsByStock(string db, int companyId, int unitId, string RequisitionNo)
        {
            var query = String.Format(@"begin :param1 := FN_TRANS_PROD_PENDING_DISPATCH({0},'{1}', '{2}');
                          end;", companyId, unitId, RequisitionNo);

            var dt = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] { "RefCursor" }));
            //var dt = await _commonServices.GetDataSetAsyn(_configuration.GetConnectionString(db), GetProductsByInvoice_Query(),
            //    _commonServices.AddParameter(new string[] { companyId.ToString(), invoiceNo }));
            var ds = new DataSet();
            ds.Tables.Add(dt);

            return _commonServices.DataSetToJSON(ds);
        }

        public async Task<string> LoadData(string db, int Company_Id)
        {
            List<DEPOT_DISPATCH_MST> requisition_Mst_list = new List<DEPOT_DISPATCH_MST>();
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadMasterData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
            for (int i = 0; i < data.Rows.Count; i++)
            {
                DEPOT_DISPATCH_MST requisition_Mst = new DEPOT_DISPATCH_MST();
                requisition_Mst.ROW_NO = Convert.ToInt32(data.Rows[i]["ROW_NO"]);
                requisition_Mst.MST_ID = Convert.ToInt32(data.Rows[i]["MST_ID"]);
                requisition_Mst.DISPATCH_NO = data.Rows[i]["DISPATCH_NO"].ToString();
                requisition_Mst.DISPATCH_TYPE = data.Rows[i]["DISPATCH_TYPE"].ToString();
                requisition_Mst.DISPATCH_DATE = data.Rows[i]["DISPATCH_DATE"].ToString();
                requisition_Mst.VEHICLE_NO = data.Rows[i]["VEHICLE_NO"].ToString();
                requisition_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[i]["MST_ID"].ToString());
                requisition_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[i]["COMPANY_ID"]);
                requisition_Mst.REMARKS = data.Rows[i]["REMARKS"].ToString();
                requisition_Mst.VECHILE_VOLUME = Convert.ToDouble(data.Rows[i]["VECHILE_VOLUME"]);
                requisition_Mst.VECHILE_WEIGHT = Convert.ToDouble(data.Rows[i]["VECHILE_WEIGHT"]);

                requisition_Mst.DISPATCH_VOLUME = Convert.ToDouble(data.Rows[i]["DISPATCH_VOLUME"]);
                requisition_Mst.DISPATCH_WEIGHT = Convert.ToDouble(data.Rows[i]["DISPATCH_WEIGHT"]);
                requisition_Mst.DISPATCH_UNIT_ID = Convert.ToInt32(data.Rows[i]["DISPATCH_UNIT_ID"]);
                requisition_Mst.DRIVER_ID = data.Rows[i]["DRIVER_ID"].ToString();

                requisition_Mst.VEHICLE_DESCRIPTION = data.Rows[i]["VECHILE_DESCRIPTION"].ToString();
                requisition_Mst.DISPATCH_BY = data.Rows[i]["DISPATCH_BY"].ToString();

                requisition_Mst_list.Add(requisition_Mst);
            }
            return JsonSerializer.Serialize(requisition_Mst_list);
        }

        public async Task<string> LoadDistributionReqData(string db, int Company_Id, int unit_id)
        {
            List<DEPOT_DISPATCH_REQUISITION> requisition_Mst_list = new List<DEPOT_DISPATCH_REQUISITION>();
            DataTable dataTable_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_All_DistributionReq_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), unit_id.ToString() }));
            for (int i = 0; i < dataTable_detail.Rows.Count; i++)
            {
                DEPOT_DISPATCH_REQUISITION _distribution_Req = new DEPOT_DISPATCH_REQUISITION();

                _distribution_Req.COMPANY_ID = Convert.ToInt32(dataTable_detail.Rows[i]["COMPANY_ID"]);
                _distribution_Req.MST_ID = Convert.ToInt32(dataTable_detail.Rows[i]["MST_ID"]);
                _distribution_Req.DISPATCH_REQ_ID = Convert.ToInt32(dataTable_detail.Rows[i]["DISPATCH_REQ_ID"]);

                _distribution_Req.REQUISITION_NO = dataTable_detail.Rows[i]["REQUISITION_NO"].ToString();
                _distribution_Req.REQUISITION_DATE = dataTable_detail.Rows[i]["REQUISITION_DATE_FORMATED"].ToString();
                _distribution_Req.REQUISITION_UNIT_ID = Convert.ToInt32(dataTable_detail.Rows[i]["REQUISITION_UNIT_ID"]);
                _distribution_Req.REQUISITION_UNIT_NAME = dataTable_detail.Rows[i]["REQUISITION_UNIT_NAME"].ToString();
                _distribution_Req.ISSUE_NO = dataTable_detail.Rows[i]["ISSUE_NO"].ToString();
                _distribution_Req.ISSUE_DATE = dataTable_detail.Rows[i]["ISSUE_DATE_FORMATED"].ToString();

                _distribution_Req.ISSUE_UNIT_ID = Convert.ToInt32(dataTable_detail.Rows[i]["ISSUE_UNIT_ID"]);
                _distribution_Req.ISSUE_UNIT_NAME = dataTable_detail.Rows[i]["ISSUE_UNIT_NAME"].ToString();
                _distribution_Req.REQUISITION_NO = dataTable_detail.Rows[i]["REQUISITION_NO"].ToString();
                _distribution_Req.DISPATCH_NO = dataTable_detail.Rows[i]["DISPATCH_NO"].ToString();
                _distribution_Req.ENTERED_BY = dataTable_detail.Rows[i]["ENTERED_BY"].ToString();
                _distribution_Req.ENTERED_DATE = dataTable_detail.Rows[i]["ENTERED_DATE"].ToString();
                _distribution_Req.ROW_NO = Convert.ToInt32(dataTable_detail.Rows[i]["ROW_NO"]);

                requisition_Mst_list.Add(_distribution_Req);
            }
            return JsonSerializer.Serialize(requisition_Mst_list);
        }

        public async Task<string> LoadDistributionProductDataByReqId(string db, int Company_Id, int ReqId)
        {
            List<DEPOT_DISPATCH_PRODUCT> requisition_Mst_list = new List<DEPOT_DISPATCH_PRODUCT>();
            DataTable dataTable_product_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_All_DistributionProduct_Query(), _commonServices.AddParameter(new string[] { ReqId.ToString() }));
            for (int j = 0; j < dataTable_product_detail.Rows.Count; j++)
            {
                DEPOT_DISPATCH_PRODUCT _distribution_product = new DEPOT_DISPATCH_PRODUCT();
                _distribution_product.MST_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["MST_ID"]);
                _distribution_product.DISPATCH_REQ_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["DISPATCH_REQ_ID"]);
                _distribution_product.DISPATCH_PRODUCT_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["DISPATCH_PRODUCT_ID"]);

                _distribution_product.DISPATCH_UNIT_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["DISPATCH_UNIT_ID"]);
                _distribution_product.ISSUE_QTY = Convert.ToInt32(dataTable_product_detail.Rows[j]["ISSUE_QTY"]);
                _distribution_product.DISPATCH_QTY = Convert.ToInt32(dataTable_product_detail.Rows[j]["DISPATCH_QTY"]);

                _distribution_product.SKU_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["SKU_ID"]);

                _distribution_product.SKU_CODE = dataTable_product_detail.Rows[j]["SKU_CODE"].ToString();

                _distribution_product.SKU_NAME = dataTable_product_detail.Rows[j]["SKU_NAME"].ToString();
                _distribution_product.ENTERED_DATE = dataTable_product_detail.Rows[j]["ENTERED_DATE"].ToString();
                _distribution_product.COMPANY_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["COMPANY_ID"]);
                _distribution_product.ROW_NO = Convert.ToInt32(dataTable_product_detail.Rows[j]["ROW_NO"]);

                _distribution_product.UNIT_TP = Convert.ToDouble(dataTable_product_detail.Rows[j]["UNIT_TP"]);
                _distribution_product.SHIPPER_QTY = Convert.ToInt32(dataTable_product_detail.Rows[j]["SHIPPER_QTY"]);
                _distribution_product.SHIPPER_WEIGHT = Convert.ToDouble(dataTable_product_detail.Rows[j]["SHIPPER_WEIGHT"]);
                _distribution_product.SHIPPER_VOLUME = Convert.ToDouble(dataTable_product_detail.Rows[j]["SHIPPER_VOLUME"]);
                _distribution_product.PER_PACK_VOLUME = Convert.ToDouble(dataTable_product_detail.Rows[j]["PER_PACK_VOLUME"]);
                _distribution_product.PER_PACK_WEIGHT = Convert.ToDouble(dataTable_product_detail.Rows[j]["PER_PACK_WEIGHT"]);

                _distribution_product.DISPATCH_AMOUNT = Convert.ToDouble(dataTable_product_detail.Rows[j]["DISPATCH_AMOUNT"]);
                _distribution_product.ISSUE_AMOUNT = Convert.ToDouble(dataTable_product_detail.Rows[j]["ISSUE_AMOUNT"]);
                _distribution_product.DISPATCH_DATE = dataTable_product_detail.Rows[j]["DISPATCH_DATE_FORMATED"].ToString();
                _distribution_product.DISPATCH_NO = dataTable_product_detail.Rows[j]["DISPATCH_NO"].ToString();

                requisition_Mst_list.Add(_distribution_product);
            }
            return JsonSerializer.Serialize(requisition_Mst_list);
        }

        public async Task<string> LoadShipperDtlData(string db, int Company_Id, int Mst_Id)
        {
            List<DEPOT_DISPATCH_SHIPPER_DTL> requisition_Mst_list = new List<DEPOT_DISPATCH_SHIPPER_DTL>();
            DataTable dataTable_product_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Shipper_Dtl_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), Mst_Id.ToString() }));
            for (int j = 0; j < dataTable_product_detail.Rows.Count; j++)
            {
                DEPOT_DISPATCH_SHIPPER_DTL _distribution_product = new DEPOT_DISPATCH_SHIPPER_DTL();
                _distribution_product.MST_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["MST_ID"]);
                _distribution_product.DISPATCH_SHIPPER_DTL_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["DISPATCH_SHIPPER_DTL_ID"]);

                _distribution_product.DISPATCH_UNIT_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["DISPATCH_UNIT_ID"]);

                _distribution_product.DISPATCH_QTY = Convert.ToInt32(dataTable_product_detail.Rows[j]["DISPATCH_QTY"]);

                _distribution_product.SKU_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["SKU_ID"]);

                _distribution_product.SKU_CODE = dataTable_product_detail.Rows[j]["SKU_CODE"].ToString();

                _distribution_product.SKU_NAME = dataTable_product_detail.Rows[j]["SKU_NAME"].ToString();
                _distribution_product.ENTERED_DATE = dataTable_product_detail.Rows[j]["ENTERED_DATE"].ToString();
                _distribution_product.COMPANY_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["COMPANY_ID"]);
                _distribution_product.ROW_NO = Convert.ToInt32(dataTable_product_detail.Rows[j]["ROW_NO"]);
                _distribution_product.NO_OF_SHIPPER = Convert.ToInt32(dataTable_product_detail.Rows[j]["NO_OF_SHIPPER"]);
                _distribution_product.LOOSE_QTY = Convert.ToInt32(dataTable_product_detail.Rows[j]["LOOSE_QTY"]);
                _distribution_product.LOOSE_VOLUME = Convert.ToDouble(dataTable_product_detail.Rows[j]["LOOSE_VOLUME"]);
                _distribution_product.LOOSE_WEIGHT = Convert.ToDouble(dataTable_product_detail.Rows[j]["LOOSE_WEIGHT"]);
                _distribution_product.SHIPPER_WEIGHT = Convert.ToDouble(dataTable_product_detail.Rows[j]["SHIPPER_WEIGHT"]);
                _distribution_product.TOTAL_VOLUME = Convert.ToDouble(dataTable_product_detail.Rows[j]["TOTAL_VOLUME"]);
                _distribution_product.TOTAL_WEIGHT = Convert.ToDouble(dataTable_product_detail.Rows[j]["TOTAL_WEIGHT"]);

                _distribution_product.SHIPPER_QTY = Convert.ToInt32(dataTable_product_detail.Rows[j]["SHIPPER_QTY"]);
                _distribution_product.SHIPPER_WEIGHT = Convert.ToDouble(dataTable_product_detail.Rows[j]["SHIPPER_WEIGHT"]);
                _distribution_product.SHIPPER_VOLUME = Convert.ToDouble(dataTable_product_detail.Rows[j]["SHIPPER_VOLUME"]);

                requisition_Mst_list.Add(_distribution_product);
            }
            return JsonSerializer.Serialize(requisition_Mst_list);
        }

        public async Task<string> LoadDispatchBatchData(string db, int Company_Id, int Mst_Id)
        {
            List<DEPOT_DISPATCH_PRODUCT_BATCH> requisition_Mst_list = new List<DEPOT_DISPATCH_PRODUCT_BATCH>();
            DataTable dataTable_product_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Batch_Dtl_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), Mst_Id.ToString() }));
            for (int j = 0; j < dataTable_product_detail.Rows.Count; j++)
            {
                DEPOT_DISPATCH_PRODUCT_BATCH _distribution_product = new DEPOT_DISPATCH_PRODUCT_BATCH();
                _distribution_product.MST_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["MST_ID"]);
                _distribution_product.DISPATCH_PROD_BATCH_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["DISPATCH_PROD_BATCH_ID"]);
                _distribution_product.BATCH_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["BATCH_ID"]);
                _distribution_product.BATCH_NO = dataTable_product_detail.Rows[j]["BATCH_NO"].ToString();
                _distribution_product.DISPATCH_AMOUNT = Convert.ToDouble(dataTable_product_detail.Rows[j]["DISPATCH_AMOUNT"]);
                _distribution_product.REQUISITION_NO = dataTable_product_detail.Rows[j]["REQUISITION_NO"].ToString();
                _distribution_product.DISPATCH_UNIT_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["DISPATCH_UNIT_ID"]);

                _distribution_product.DISPATCH_QTY = Convert.ToInt32(dataTable_product_detail.Rows[j]["DISPATCH_QTY"]);

                _distribution_product.SKU_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["SKU_ID"]);

                _distribution_product.SKU_CODE = dataTable_product_detail.Rows[j]["SKU_CODE"].ToString();

                _distribution_product.SKU_NAME = dataTable_product_detail.Rows[j]["SKU_NAME"].ToString();

                _distribution_product.COMPANY_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["COMPANY_ID"]);
                _distribution_product.ROW_NO = Convert.ToInt32(dataTable_product_detail.Rows[j]["ROW_NO"]);

                requisition_Mst_list.Add(_distribution_product);
            }
            return JsonSerializer.Serialize(requisition_Mst_list);
        }

        public async Task<string> GenerateDistributionCode(string db, string Company_Id)
        {
            try
            {
                string code;
                DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Get_LastDistribution_no(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
                if (dataTable.Rows.Count > 0)
                {
                    string serial = (Convert.ToInt32(dataTable.Rows[0]["DISPATCH_NO"].ToString().Substring(4, 4)) + 1).ToString();
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

        public async Task<DEPOT_DISPATCH_MST> LoadDistributionDetailData_ByMasterId(string db, int _Id)
        {
            DEPOT_DISPATCH_MST _distribution_Mst = new DEPOT_DISPATCH_MST();

            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_MasterById_Query(), _commonServices.AddParameter(new string[] { _Id.ToString() }));
            if (data.Rows.Count > 0)
            {
                _distribution_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[0]["COMPANY_ID"]);

                _distribution_Mst.ROW_NO = Convert.ToInt32(data.Rows[0]["ROW_NO"]);
                _distribution_Mst.MST_ID = Convert.ToInt32(data.Rows[0]["MST_ID"]);
                _distribution_Mst.DISPATCH_NO = data.Rows[0]["DISPATCH_NO"].ToString();
                _distribution_Mst.DISPATCH_TYPE = data.Rows[0]["DISPATCH_TYPE"].ToString();
                _distribution_Mst.DISPATCH_DATE = data.Rows[0]["DISPATCH_DATE"].ToString();
                _distribution_Mst.VEHICLE_NO = data.Rows[0]["VECHILE_NO"].ToString();
                _distribution_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[0]["MST_ID"].ToString());
                _distribution_Mst.VEHICLE_DESCRIPTION = data.Rows[0]["VECHILE_DESCRIPTION"].ToString();
                _distribution_Mst.REMARKS = data.Rows[0]["REMARKS"].ToString();
                _distribution_Mst.VECHILE_VOLUME = Convert.ToDouble(data.Rows[0]["VECHILE_VOLUME"]);
                _distribution_Mst.VECHILE_WEIGHT = Convert.ToDouble(data.Rows[0]["VECHILE_WEIGHT"]);
                _distribution_Mst.DISPATCH_VOLUME = Convert.ToDouble(data.Rows[0]["DISPATCH_VOLUME"]);
                _distribution_Mst.DISPATCH_WEIGHT = Convert.ToDouble(data.Rows[0]["DISPATCH_WEIGHT"]);
                _distribution_Mst.DRIVER_ID = data.Rows[0]["DRIVER_ID"].ToString();

                _distribution_Mst.VEHICLE_DESCRIPTION = data.Rows[0]["VECHILE_DESCRIPTION"].ToString();
                _distribution_Mst.DISPATCH_BY = data.Rows[0]["DISPATCH_BY"].ToString();

                DataTable dataTable_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DistributionReq_Query(), _commonServices.AddParameter(new string[] { _Id.ToString() }));
                _distribution_Mst.requisitionIssueDtlList = new List<DEPOT_DISPATCH_REQUISITION>();
                for (int i = 0; i < dataTable_detail.Rows.Count; i++)
                {
                    DEPOT_DISPATCH_REQUISITION _distribution_Req = new DEPOT_DISPATCH_REQUISITION();

                    _distribution_Req.COMPANY_ID = Convert.ToInt32(dataTable_detail.Rows[i]["COMPANY_ID"]);
                    _distribution_Req.MST_ID = Convert.ToInt32(dataTable_detail.Rows[i]["MST_ID"]);
                    _distribution_Req.DISPATCH_REQ_ID = Convert.ToInt32(dataTable_detail.Rows[i]["DISPATCH_REQ_ID"]);

                    _distribution_Req.REQUISITION_NO = dataTable_detail.Rows[i]["REQUISITION_NO"].ToString();
                    _distribution_Req.REQUISITION_DATE = dataTable_detail.Rows[i]["REQUISITION_DATE_FORMATED"].ToString();
                    _distribution_Req.REQUISITION_UNIT_ID = Convert.ToInt32(dataTable_detail.Rows[i]["REQUISITION_UNIT_ID"]);
                    _distribution_Req.REQUISITION_UNIT_NAME = dataTable_detail.Rows[i]["REQUISITION_UNIT_NAME"].ToString();
                    _distribution_Req.ISSUE_NO = dataTable_detail.Rows[i]["ISSUE_NO"].ToString();
                    _distribution_Req.ISSUE_DATE = dataTable_detail.Rows[i]["ISSUE_DATE_FORMATED"].ToString();
                    _distribution_Req.ISSUE_UNIT_NAME = dataTable_detail.Rows[i]["ISSUE_UNIT_NAME"].ToString();

                    _distribution_Req.ISSUE_UNIT_ID = Convert.ToInt32(dataTable_detail.Rows[i]["ISSUE_UNIT_ID"]);
                    _distribution_Req.DISPATCH_UNIT_ID = Convert.ToInt32(dataTable_detail.Rows[i]["DISPATCH_UNIT_ID"]);
                    _distribution_Req.ENTERED_BY = dataTable_detail.Rows[i]["ENTERED_BY"].ToString();
                    _distribution_Req.ENTERED_DATE = dataTable_detail.Rows[i]["ENTERED_DATE"].ToString();
                    _distribution_Req.ROW_NO = Convert.ToInt32(dataTable_detail.Rows[i]["ROW_NO"]);
                    _distribution_Mst.requisitionIssueDtlList.Add(_distribution_Req);

                    DataTable dataTable_product_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DistributionProduct_Query(), _commonServices.AddParameter(new string[] { _distribution_Req.DISPATCH_REQ_ID.ToString() }));

                    _distribution_Req.requisitionProductDtlList = new List<DEPOT_DISPATCH_PRODUCT>();
                    for (int j = 0; j < dataTable_product_detail.Rows.Count; j++)
                    {
                        DEPOT_DISPATCH_PRODUCT _distribution_product = new DEPOT_DISPATCH_PRODUCT();
                        _distribution_product.MST_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["MST_ID"]);
                        _distribution_product.DISPATCH_REQ_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["DISPATCH_REQ_ID"]);
                        _distribution_product.DISPATCH_PRODUCT_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["DISPATCH_PRODUCT_ID"]);

                        _distribution_product.ISSUE_QTY = Convert.ToInt32(dataTable_product_detail.Rows[j]["ISSUE_QTY"]);
                        _distribution_product.DISPATCH_QTY = Convert.ToInt32(dataTable_product_detail.Rows[j]["DISPATCH_QTY"]);
                        _distribution_product.TOTALDISQTY = Convert.ToInt32(dataTable_product_detail.Rows[j]["TOTALDISQTY"]);
                        _distribution_product.SKU_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["SKU_ID"]);
                        _distribution_product.DISPATCH_AMOUNT = Convert.ToDouble(dataTable_product_detail.Rows[j]["DISPATCH_AMOUNT"]);

                        _distribution_product.UNIT_TP = Convert.ToDouble(dataTable_product_detail.Rows[j]["UNIT_TP"]);
                        _distribution_product.SHIPPER_QTY = Convert.ToInt32(dataTable_product_detail.Rows[j]["SHIPPER_QTY"]);
                        _distribution_product.SHIPPER_WEIGHT = Convert.ToDouble(dataTable_product_detail.Rows[j]["SHIPPER_WEIGHT"]);
                        _distribution_product.SHIPPER_VOLUME = Convert.ToDouble(dataTable_product_detail.Rows[j]["SHIPPER_VOLUME"]);
                        _distribution_product.PER_PACK_VOLUME = Convert.ToDouble(dataTable_product_detail.Rows[j]["PER_PACK_VOLUME"]);
                        _distribution_product.PER_PACK_WEIGHT = Convert.ToDouble(dataTable_product_detail.Rows[j]["PER_PACK_WEIGHT"]);
                        _distribution_product.SKU_CODE = dataTable_product_detail.Rows[j]["SKU_CODE"].ToString();
                        _distribution_product.ISSUE_NO = dataTable_product_detail.Rows[j]["ISSUE_NO"].ToString();
                        _distribution_product.REQUISITION_NO = dataTable_product_detail.Rows[j]["REQUISITION_NO"].ToString();
                        _distribution_product.SKU_NAME = dataTable_product_detail.Rows[j]["SKU_NAME"].ToString();
                        _distribution_product.DISPATCH_NO = dataTable_product_detail.Rows[j]["DISPATCH_NO"].ToString();
                        _distribution_product.ENTERED_DATE = dataTable_product_detail.Rows[j]["ENTERED_DATE"].ToString();
                        _distribution_product.DISPATCH_DATE = dataTable_product_detail.Rows[j]["DISPATCH_DATE_FORMATED"].ToString();
                        _distribution_product.ROW_NO = Convert.ToInt32(dataTable_product_detail.Rows[j]["ROW_NO"]);
                        _distribution_Req.requisitionProductDtlList.Add(_distribution_product);
                    }
                }
            }

            return _distribution_Mst;
        }



        private async Task<(bool flag, string message)> CheckDelivaryQty(string db, DEPOT_DISPATCH_MST model)
        {
            try
            {
                string invoiceNos = string.Join(",", model.requisitionIssueDtlList.Select(i => $"'{i.REQUISITION_NO}'"));
                string sku_pending = @$"SELECT
           REQUISITION_NO,
           SKU_ID,
           SKU_CODE,
           SKU_NAME,
           ISSUE_QTY,
           DISPATCH_QTY,
           PENDING_DISPATCH_QTY
     FROM
        ( SELECT A.COMPANY_ID,
               FN_COMPANY_NAME(A.COMPANY_ID) COMPANY_NAME,
               A.REQUISITION_UNIT_ID,
               FN_UNIT_NAME(A.COMPANY_ID,A.REQUISITION_UNIT_ID)REQUISITION_UNIT_NAME,
               A.ISSUE_UNIT_ID,
               FN_UNIT_NAME(A.COMPANY_ID,A.ISSUE_UNIT_ID)ISSUE_UNIT_NAME,
               A.REQUISITION_NO,
               TO_CHAR(B.TRANSFER_DATE, 'DD/MM/YYYY') REQUISITION_DATE,
               A.SKU_ID,
               A.SKU_CODE,
               FN_SKU_NAME(A.COMPANY_ID,A.SKU_ID) SKU_NAME,
               FN_PACK_SIZE(A.COMPANY_ID,A.SKU_ID) PACK_SIZE,
               C.SHIPPER_QTY,
               A.UNIT_TP,
               A.ISSUE_QTY,
               A.ISSUE_AMOUNT,
               A.DISPATCH_QTY,
               A.DISPATCH_AMOUNT,
               NVL(A.ISSUE_QTY,0) - NVL(A.DISPATCH_QTY,0) PENDING_DISPATCH_QTY,
               FN_NO_OF_SHIPPER(A.COMPANY_ID,A.SKU_ID,NVL(A.ISSUE_QTY,0) - NVL(A.DISPATCH_QTY,0)) NO_OF_SHIPPER,
               C.SHIPPER_WEIGHT,
               C.SHIPPER_WEIGHT_UNIT WEIGHT_UNIT,

               C.SHIPPER_VOLUME,
               C.SHIPPER_VOLUME_UNIT VOLUME_UNIT,
               FN_LOOSE_QTY(A.COMPANY_ID,A.SKU_ID,NVL(A.ISSUE_QTY,0) - NVL(A.DISPATCH_QTY,0))LOOSE_QTY,
               ROUND(NVL(C.SHIPPER_WEIGHT,0)/NVL(C.SHIPPER_QTY,1),3) PER_PACK_WEIGHT,
               ROUND(NVL(C.SHIPPER_VOLUME,0)/NVL(C.SHIPPER_QTY,1),3) PER_PACK_VOLUME,


               B.ENTERED_BY ISSUE_USER_ID,
               FN_EMPLOYEE_ID(B.ENTERED_BY) ISSUE_EMPLOYEE_ID,
               FN_EMPLOYEE_NAME(B.ENTERED_BY) ISSUE_EMPLOYEE_NAME
         FROM
            (
                SELECT COMPANY_ID,
                       REQUISITION_UNIT_ID,
                       ISSUE_UNIT_ID,
                       REQUISITION_NO,
                       SKU_ID,
                       SKU_CODE,
                       UNIT_TP,
                       SUM(NVL(ISSUE_QTY,0))ISSUE_QTY,
                       SUM(NVL(ISSUE_AMOUNT,0))ISSUE_AMOUNT,
                       SUM(NVL(DISPATCH_QTY,0))DISPATCH_QTY,
                       SUM(NVL(DISPATCH_AMOUNT,0))DISPATCH_AMOUNT
                 FROM
                     (
                               SELECT A.COMPANY_ID,
                                       A.TRANS_RCV_UNIT_ID REQUISITION_UNIT_ID,
                                       A.TRANSFER_UNIT_ID ISSUE_UNIT_ID,
                                       A.TRANSFER_NO REQUISITION_NO,
                                       B.SKU_ID,
                                       B.SKU_CODE,
                                       B.UNIT_TP,
                                       B.TRANSFER_QTY ISSUE_QTY,
                                       B.TRANSFER_AMOUNT ISSUE_AMOUNT,
                                       0 DISPATCH_QTY,
                                       0 DISPATCH_AMOUNT
                                FROM   DEPOT_STOCK_TRANSFER_MST A, DEPOT_STOCK_TRANSFER_DTL B
                                WHERE  A.MST_ID = B.MST_ID
                                AND    A.COMPANY_ID=:param1
                                AND    A.TRANSFER_UNIT_ID=:param2
                                AND    A.TRANSFER_NO IN ( {invoiceNos} )


                        UNION ALL

                        SELECT A.COMPANY_ID,
                               B.REQUISITION_UNIT_ID,
                               B.ISSUE_UNIT_ID,
                               B.REQUISITION_NO,
                               C.SKU_ID,
                               C.SKU_CODE,
                               C.UNIT_TP,
                               0 ISSUE_QTY,
                               0 ISSUE_AMOUNT,
                               C.DISPATCH_QTY,
                               C.DISPATCH_AMOUNT
                        FROM DEPOT_DISPATCH_MST A, DEPOT_DISPATCH_REQUISITION B, DEPOT_DISPATCH_PRODUCT C
                        WHERE A.MST_ID=B.MST_ID
                        AND   B.DISPATCH_REQ_ID=C.DISPATCH_REQ_ID
                        AND   B.COMPANY_ID=:param1
                        AND   B.ISSUE_UNIT_ID=:param2
                        AND   B.REQUISITION_NO IN ( {invoiceNos} )

                     )
                GROUP BY COMPANY_ID,REQUISITION_UNIT_ID,ISSUE_UNIT_ID,REQUISITION_NO,SKU_ID,SKU_CODE,UNIT_TP
           )A, DEPOT_STOCK_TRANSFER_MST B,PRODUCT_INFO C
    WHERE A.REQUISITION_NO=B.TRANSFER_NO
    AND   A.COMPANY_ID=C.COMPANY_ID
    AND   A.SKU_ID=C.SKU_ID
    AND    NVL(A.ISSUE_QTY,0)-NVL(A.DISPATCH_QTY,0) > 0)";
                var parameters = _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), model.DISPATCH_UNIT_ID.ToString() });
                DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), sku_pending, parameters);
                foreach (var invoice in model.requisitionIssueDtlList)
                {
                    foreach (var product in invoice.requisitionProductDtlList)
                    {
                        var matchingRows = dataTable.AsEnumerable().Where(row =>
                        row.Field<string>("REQUISITION_NO") == Convert.ToString(invoice.REQUISITION_NO) &&
                        row.Field<decimal>("SKU_ID") == Convert.ToDecimal(product.SKU_ID)).ToList();

                        if (matchingRows.Count > 1)
                        {
                            return (false, $"Multiple matching rows found for the product {product.SKU_NAME} of Trans./Req {invoice.REQUISITION_NO}");
                        }

                        if (matchingRows.Count == 1)
                        {
                            DataRow row = matchingRows.First();
                            decimal invoiceQty = row.Field<decimal>("PENDING_DISPATCH_QTY");

                            if (invoiceQty < product.PENDING_DISPATCH_QTY)
                            {
                                return (false, $"Qty: {product.PENDING_DISPATCH_QTY} is greater than Pending Qty: {invoiceQty} for the Product {product.SKU_NAME} of Rer./Tras. {invoice.REQUISITION_NO}");
                            }


                        }
                        else
                        {
                            return (false, $"No pending quantity found for the product {product.SKU_NAME} of invoice {invoice.REQUISITION_NO}");
                        }
                    }
                }
                return (true, "Everything is good!");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }


    }
}