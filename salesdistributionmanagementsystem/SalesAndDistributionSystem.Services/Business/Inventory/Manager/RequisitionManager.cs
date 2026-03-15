using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.Company;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager;
using SalesAndDistributionSystem.Services.Business.Security;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution.Manager
{
    public class RequisitionManager : IRequisitionManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        private readonly INotificationManager _NotificationManager;
        private readonly IUserLogManager _logManager;

        public RequisitionManager(ICommonServices commonServices, IConfiguration configuration, INotificationManager notificationManager, IUserLogManager userLogManager)
        {
            _commonServices = commonServices;
            _configuration = configuration;
            _NotificationManager = notificationManager;
            _logManager = userLogManager;
        }

        private string AddOrUpdate_AddQuery() => @"INSERT INTO DEPOT_REQUISITION_RAISE_MST
                                        (
                                            MST_ID
                                            ,REQUISITION_UNIT_ID
                                            ,ISSUE_UNIT_ID
                                            ,REQUISITION_NO
                                            ,REQUISITION_DATE
                                            ,REQUISITION_AMOUNT
                                            ,REQUISITION_RAISE_BY
                                            ,STATUS
                                            ,COMPANY_ID
                                            ,REMARKS
                                            ,ENTERED_BY
                                            ,ENTERED_DATE
                                            ,ENTERED_TERMINAL
                                            ,TOTAL_WEIGHT
                                            ,TOTAL_VOLUME
                                         )
                                          VALUES ( :param1, :param2, :param3, :param4,TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM'),:param6, :param7, :param8, :param9, :param10, :param11,TO_DATE(:param12, 'DD/MM/YYYY HH:MI:SS AM'), :param13, :param14, :param15)";

        private string AddOrUpdate_AddQueryDTL() => @"INSERT INTO DEPOT_REQUISITION_RAISE_DTL
                    (MST_ID
                        ,DTL_ID
                        ,SKU_ID
                        ,SKU_CODE
                        ,UNIT_TP
                        ,REQUISITION_QTY
                        ,REQUISITION_AMOUNT
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
                    VALUES ( :param1, :param2, :param3, :param4, :param5, :param6, :param7, :param8, :param9, :param10, :param11,TO_DATE(:param12, 'DD/MM/YYYY HH:MI:SS AM'), :param13
                , :param14, :param15, :param16, :param17, :param18, :param19, :param20, :param21, :param22, :param23, :param24, :param25, :param26)";

        private string AddOrUpdate_UpdateQuery() => @"UPDATE DEPOT_REQUISITION_RAISE_MST SET
                                                 REQUISITION_UNIT_ID= :param2
                                                ,ISSUE_UNIT_ID= :param3
                                                ,REQUISITION_NO= :param4
                                                ,REQUISITION_DATE= TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM')
                                                ,REQUISITION_AMOUNT= :param6
                                                ,REQUISITION_RAISE_BY= :param7
                                                ,STATUS= :param8
                                                ,COMPANY_ID= :param9
                                                ,REMARKS= :param10
                                                ,UPDATED_BY= :param11
                                                ,UPDATED_DATE= TO_DATE(:param12, 'DD/MM/YYYY HH:MI:SS AM')
                                                ,UPDATED_TERMINAL= :param13
                                                ,TOTAL_WEIGHT= :param14
                                                ,TOTAL_VOLUME= :param15
                                                WHERE MST_ID = :param1";

        private string AddOrUpdate_UpdateQueryDTL() => @"UPDATE DEPOT_REQUISITION_RAISE_DTL SET
                                                 SKU_ID= :param2
                                                ,SKU_CODE= :param3
                                                ,UNIT_TP= :param4
                                                ,REQUISITION_QTY= :param5
                                                ,REQUISITION_AMOUNT= :param6
                                                ,STATUS= :param7
                                                ,COMPANY_ID= :param8
                                                ,REMARKS= :param9
                                                ,UPDATED_BY= :param10
                                                ,UPDATED_DATE=  TO_DATE(:param11, 'DD/MM/YYYY HH:MI:SS AM')
                                                ,UPDATED_TERMINAL= :param12
                                                ,SHIPPER_QTY= :param13
                                                ,NO_OF_SHIPPER= :param14
                                                ,LOOSE_QTY= :param15
                                                ,SHIPPER_WEIGHT= :param16
                                                ,TOTAL_SHIPPER_WEIGHT= :param17
                                                ,LOOSE_WEIGHT= :param18
                                                ,TOTAL_WEIGHT= :param19

                                                ,SHIPPER_VOLUME= :param20
                                                ,TOTAL_SHIPPER_VOLUME= :param21
                                                ,LOOSE_VOLUME= :param22
                                                ,TOTAL_VOLUME= :param23
                                                ,PER_PACK_VOLUME= :param24
                                                ,PER_PACK_WEIGHT= :param25
                                                WHERE DTL_ID = :param1";

        private string LoadMasterData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO, M.MST_ID
                                            ,M.REQUISITION_UNIT_ID
                                            ,M.ISSUE_UNIT_ID
                                            ,M.REQUISITION_NO
                                            ,TO_CHAR(M.REQUISITION_DATE, 'DD/MM/YYYY') REQUISITION_DATE
                                            ,M.REQUISITION_AMOUNT
                                            ,M.REQUISITION_RAISE_BY
                                            ,M.STATUS
                                            ,M.COMPANY_ID
                                            ,M.REMARKS
                                            ,M.ENTERED_BY
                                            ,M.ENTERED_DATE
                                            ,M.ENTERED_TERMINAL
                                            ,M.TOTAL_WEIGHT
                                            ,M.TOTAL_VOLUME
                                            ,(SELECT  UNIT_NAME FROM COMPANY_INFO CI WHERE M.ISSUE_UNIT_ID = CI.UNIT_ID AND ROWNUM = 1) ISSUE_UNIT_NAME
                                            ,(SELECT  UNIT_NAME FROM COMPANY_INFO CI WHERE M.REQUISITION_UNIT_ID = CI.UNIT_ID AND ROWNUM = 1) REQUISITION_UNIT_NAME
            FROM DEPOT_REQUISITION_RAISE_MST  M

            where  M.COMPANY_ID = :param1 and M.REQUISITION_UNIT_ID = :param2 and  M.REQUISITION_NO NOT IN (SELECT REQUISITION_NO FROM DEPOT_REQUISITION_ISSUE_MST) ORDER BY M.MST_ID DESC";

        private string LoadMasterData_issue_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO, M.MST_ID
                                            ,M.REQUISITION_UNIT_ID
                                            ,M.ISSUE_UNIT_ID
                                            ,M.REQUISITION_NO
                                            ,TO_CHAR(M.REQUISITION_DATE, 'DD/MM/YYYY') REQUISITION_DATE
                                            ,M.REQUISITION_AMOUNT
                                            ,M.REQUISITION_RAISE_BY
                                            ,M.STATUS
                                            ,M.COMPANY_ID
                                            ,M.REMARKS
                                            ,M.ENTERED_BY
                                            ,M.ENTERED_DATE
                                            ,M.ENTERED_TERMINAL
                                            ,M.TOTAL_WEIGHT
                                            ,M.TOTAL_VOLUME
                                            ,(SELECT  UNIT_NAME FROM COMPANY_INFO CI WHERE M.ISSUE_UNIT_ID = CI.UNIT_ID AND ROWNUM = 1) ISSUE_UNIT_NAME
                                            ,(SELECT  UNIT_NAME FROM COMPANY_INFO CI WHERE M.REQUISITION_UNIT_ID = CI.UNIT_ID AND ROWNUM = 1) REQUISITION_UNIT_NAME
            FROM DEPOT_REQUISITION_RAISE_MST  M

            where  M.COMPANY_ID = :param1 and M.ISSUE_UNIT_ID = :param2 and  M.REQUISITION_NO NOT IN (SELECT REQUISITION_NO FROM DEPOT_REQUISITION_ISSUE_MST) ORDER BY M.MST_ID DESC";

        private string LoadRequisitionRaiseBetweenDate_Query => @"SELECT  M.MST_ID
        ,M.REQUISITION_NO
        FROM DEPOT_REQUISITION_RAISE_MST  M
        where M.COMPANY_ID = :paramISSUE_UNIT_ID1
        and trunc(M.REQUISITION_DATE) BETWEEN TO_DATE(:param2, 'DD/MM/YYYY') AND TO_DATE(:param3, 'DD/MM/YYYY') AND M.REQUISITION_UNIT_ID= NVL(:param4,M.REQUISITION_UNIT_ID)
        ORDER BY M.MST_ID DESC";

        private string LoadRequisitionIssueBetweenDate_Query => @"SELECT  M.MST_ID
        ,M.ISSUE_NO REQUISITION_NO
        FROM DEPOT_REQUISITION_ISSUE_MST  M
        where M.COMPANY_ID = :param1
        and trunc(M.ISSUE_DATE) BETWEEN TO_DATE(:param2, 'DD/MM/YYYY') AND TO_DATE(:param3, 'DD/MM/YYYY') AND M.ISSUE_UNIT_ID= NVL(:param4,M.ISSUE_UNIT_ID)
        ORDER BY M.MST_ID DESC";

        private string LoadRequisitionReceivedBetweenDate_Query => @"SELECT  M.MST_ID
        ,M.RECEIVE_NO REQUISITION_NO
        FROM DEPOT_REQUISITION_RCV_MST  M
        where M.COMPANY_ID = :param1
        and trunc(M.RECEIVE_DATE) BETWEEN TO_DATE(:param2, 'DD/MM/YYYY') AND TO_DATE(:param3, 'DD/MM/YYYY') AND M.REQUISITION_UNIT_ID= NVL(:param4,M.REQUISITION_UNIT_ID)
        ORDER BY M.MST_ID DESC";

        private string LoadRequisitionReturnBetweenDate_Query => @"SELECT  M.MST_ID
        ,M.RETURN_NO REQUISITION_NO
        FROM DEPOT_REQUISITION_RETURN_MST  M
        where M.COMPANY_ID = :param1
        and trunc(M.RETURN_DATE) BETWEEN TO_DATE(:param2, 'DD/MM/YYYY') AND TO_DATE(:param3, 'DD/MM/YYYY') AND M.RETURN_UNIT_ID= NVL(:param4,M.RETURN_UNIT_ID)
        ORDER BY M.MST_ID DESC";

        private string LoadRequisitionReceivedReturnBetweenDate_Query => @"SELECT  M.MST_ID
        ,M.RET_RCV_NO REQUISITION_NO
        FROM DEPOT_REQUISITION_RET_RCV_MST  M
        where M.COMPANY_ID = :param1
        and trunc(M.RET_RCV_DATE) BETWEEN TO_DATE(:param2, 'DD/MM/YYYY') AND TO_DATE(:param3, 'DD/MM/YYYY')  AND M.RETURN_RCV_UNIT_ID= NVL(:param4,M.RETURN_RCV_UNIT_ID)
        ORDER BY M.MST_ID DESC";

        private string LoadRequisitionPendingForIssueBetweenDate_Query => @"SELECT ISSUE_UNIT_ID,
               MST_ID,
               REQUISITION_NO
        FROM DEPOT_REQUISITION_RAISE_MST
        WHERE COMPANY_ID=:param1
        AND TRUNC(REQUISITION_DATE) BETWEEN TRUNC(TO_DATE(:param2,'DD/MM/RRRR')) AND TRUNC(TO_DATE(:param3,'DD/MM/RRRR'))
        AND ISSUE_UNIT_ID = NVL(:param4,ISSUE_UNIT_ID)
        AND REQUISITION_NO NOT IN (SELECT NVL(REQUISITION_NO,0)
                                    FROM DEPOT_REQUISITION_ISSUE_MST
                                    WHERE COMPANY_ID=:param1
                                   )
        ORDER BY MST_ID DESC";

        private string LoadRequisitionPendingForDispatchBetweenDate_Query => @"SELECT ROWNUM as MST_ID, REQUISITION_NO FROM
            ( SELECT DISTINCT
                       A.REQUISITION_NO
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
                                       A.REQUISITION_UNIT_ID,
                                       A.ISSUE_UNIT_ID,
                                       A.REQUISITION_NO,
                                       B.SKU_ID,
                                       B.SKU_CODE,
                                       B.UNIT_TP,
                                       B.ISSUE_QTY,
                                       B.ISSUE_AMOUNT,
                                       0 DISPATCH_QTY,
                                       0 DISPATCH_AMOUNT
                                FROM   DEPOT_REQUISITION_ISSUE_MST A, DEPOT_REQUISITION_ISSUE_DTL B
                                WHERE  A.MST_ID = B.MST_ID

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
                             )
                        GROUP BY COMPANY_ID,REQUISITION_UNIT_ID,ISSUE_UNIT_ID,REQUISITION_NO,SKU_ID,SKU_CODE,UNIT_TP
                   )A, DEPOT_REQUISITION_ISSUE_MST B
            WHERE A.REQUISITION_NO=B.REQUISITION_NO
            AND NVL(A.ISSUE_QTY,0)-NVL(A.DISPATCH_QTY,0) > 0
            AND A.COMPANY_ID=:param1
            AND TRUNC(B.REQUISITION_DATE) BETWEEN TRUNC(TO_DATE(:param2,'DD/MM/RRRR')) AND TRUNC(TO_DATE(:param3,'DD/MM/RRRR'))
            AND A.ISSUE_UNIT_ID = NVL(:param4, A.ISSUE_UNIT_ID )
            )";

        private string LoadRequisitionPendingForReceiveBetweenDate_Query(string unitId)
        {
            var query = @"SELECT ROWNUM AS MST_ID, REQUISITION_NO FROM ( SELECT DISTINCT
                       A.REQUISITION_NO
                 FROM
                    (
                        SELECT COMPANY_ID,
                               REQUISITION_UNIT_ID,
                               REQUISITION_NO,
                               SKU_ID,
                               SKU_CODE,
                               SUM(NVL(REQUISITION_QTY,0))REQUISITION_QTY,
                               SUM(NVL(RECEIVE_QTY,0))RECEIVE_QTY
                         FROM
                             (
                                SELECT A.COMPANY_ID,
                                       A.REQUISITION_UNIT_ID,
                                       A.REQUISITION_NO,
                                       B.SKU_ID,
                                       B.SKU_CODE,
                                       NVL(B.REQUISITION_QTY,0)REQUISITION_QTY,
                                       0 RECEIVE_QTY
                                FROM DEPOT_REQUISITION_RAISE_MST A, DEPOT_REQUISITION_RAISE_DTL B
                                WHERE A.MST_ID=B.MST_ID
                                AND   A.COMPANY_ID=:param1";

            if (unitId != "ALL" && !String.IsNullOrWhiteSpace(unitId))
            {
                query += " AND   A.REQUISITION_UNIT_ID=" + unitId;
            }

            query += @" AND   TRUNC(A.REQUISITION_DATE) BETWEEN TRUNC(TO_DATE(:param2,'DD/MM/RRRR')) AND TRUNC(TO_DATE(:param3,'DD/MM/RRRR'))

                                UNION ALL

                                SELECT A.COMPANY_ID,
                                       A.REQUISITION_UNIT_ID,
                                       A.REQUISITION_NO,
                                       B.SKU_ID,
                                       B.SKU_CODE,
                                       0 REQUISITION_QTY,
                                       NVL(B.RECEIVE_QTY,0)RECEIVE_QTY
                                FROM   DEPOT_REQUISITION_RCV_MST A,
                                       DEPOT_REQUISITION_RCV_DTL B,
                                       DEPOT_REQUISITION_RAISE_MST C
                                WHERE  A.MST_ID = B.MST_ID
                                AND    A.REQUISITION_NO=C.REQUISITION_NO
                                AND   A.COMPANY_ID=:param1";

            if (unitId != "ALL" && !String.IsNullOrWhiteSpace(unitId))
            {
                query += " AND   A.REQUISITION_UNIT_ID=" + unitId;
            }

            query += @" AND   TRUNC(C.REQUISITION_DATE) BETWEEN TRUNC(TO_DATE(:param2,'DD/MM/RRRR')) AND TRUNC(TO_DATE(:param3,'DD/MM/RRRR'))
                             )
                        GROUP BY COMPANY_ID,REQUISITION_UNIT_ID,REQUISITION_NO,SKU_ID,SKU_CODE
                   )A, DEPOT_REQUISITION_RAISE_MST B
            WHERE A.REQUISITION_NO=B.REQUISITION_NO)";
            return query;
        }

        private string LoadData_MasterById_Query() => @"
            SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO,M.MST_ID
                                            ,M.REQUISITION_UNIT_ID
                                            ,M.ISSUE_UNIT_ID
                                            ,M.REQUISITION_NO
                                            ,TO_CHAR(M.REQUISITION_DATE, 'DD/MM/YYYY') REQUISITION_DATE
                                            ,M.REQUISITION_AMOUNT
                                            ,M.REQUISITION_RAISE_BY
                                            ,M.STATUS
                                            ,M.COMPANY_ID
                                            ,M.REMARKS
                                            ,M.ENTERED_BY
                                            ,M.ENTERED_DATE
                                            ,M.ENTERED_TERMINAL
                                            ,M.TOTAL_WEIGHT
                                            ,M.TOTAL_VOLUME
            FROM DEPOT_REQUISITION_RAISE_MST  M
            where MST_ID = :param1";

        private string LoadData_DetailByMasterId_Query() => @"SELECT ROW_NUMBER () OVER (ORDER BY B.DTL_ID ASC) AS ROW_NO,
                                                           B.*,
                                                           C.SKU_NAME,
                                                           C.PACK_SIZE,
                                                           NVL(FN_CURRENT_STOCK_QTY (A.COMPANY_ID, A.REQUISITION_UNIT_ID, B.SKU_ID),0) STOCK_QTY,
                                                           NVL(FN_DEPOT_SKU_SUGGESTED_QTY (A.COMPANY_ID, A.REQUISITION_UNIT_ID, B.SKU_ID),0) SUGGESTED_QTY,
                                                           NVL((SELECT STOCK_QTY FROM VW_SKU_WISE_STOCK WHERE COMPANY_ID=A.COMPANY_ID AND UNIT_ID=A.ISSUE_UNIT_ID AND SKU_ID=B.SKU_ID),0) ISSUE_STOCK_QTY,
                                                           B.REQUISITION_QTY ISSUE_QTY
                                                    FROM DEPOT_REQUISITION_RAISE_MST A, DEPOT_REQUISITION_RAISE_DTL B,PRODUCT_INFO C
                                                    WHERE A.MST_ID        = B.MST_ID
                                                    AND   B.SKU_ID        = C.SKU_ID
                                                    AND   A.MST_ID        = :param1";

        private string LoadProductWeightData_Query() => @"select  FN_NO_OF_SHIPPER(:param1,:param2,:param3) NO_OF_SHIPPER,
                                                            FN_LOOSE_QTY(:param1,:param2,:param3) LOOSE_QTY,
                                                            P.SHIPPER_QTY,
                                                            P.SHIPPER_WEIGHT,
                                                             P.SHIPPER_VOLUME,
                                                            ROUND(NVL(P.SHIPPER_WEIGHT,0)/NVL(P.SHIPPER_QTY,1),3) PER_PACK_WEIGHT,
                                                            ROUND(NVL(P.SHIPPER_VOLUME,0)/NVL(P.SHIPPER_QTY,1),3) PER_PACK_VOLUME

                                                            from PRODUCT_INFO P WHERE P.SKU_ID = :param2";

        private string DeleteArea_Requisition_Dtl_IdQuery() => "DELETE  FROM DEPOT_REQUISITION_RAISE_DTL WHere DTL_ID = :param1";

        private string GetRequisition_Mst_IdQuery() => "SELECT NVL(MAX(MST_ID),0) + 1 MST_ID  FROM DEPOT_REQUISITION_RAISE_MST";

        private string GetRequisition_Dtl_IdQuery() => "SELECT NVL(MAX(DTL_ID),0) + 1 DTL_ID  FROM DEPOT_REQUISITION_RAISE_DTL";

        private string Get_LastRequisition_no() => "SELECT  REQUISITION_NO  FROM DEPOT_REQUISITION_RAISE_MST Where  MST_ID = (SELECT   NVL(MAX(MST_ID),0) MST_ID From DEPOT_REQUISITION_RAISE_MST where COMPANY_ID = :param1 )";

        private string Load_IssuePendingRequisition_Count_Query() => @"SELECT
   Count( R.MST_ID) as ROWCOUNT
FROM DEPOT_REQUISITION_RAISE_MST R WHERE R.REQUISITION_NO NOT IN (Select REQUISITION_NO FROM DEPOT_REQUISITION_ISSUE_MST)
and COMPANY_ID = :param1 and ISSUE_UNIT_ID = :param2";

        private string LoadProductData_Query(int unitid) => string.Format(@"SELECT SKU_ID,
                                           SKU_CODE,
                                           SKU_NAME,
                                           PACK_SIZE,
                                           FN_SKU_PRICE(SKU_ID,SKU_CODE,COMPANY_ID,{0} ) UNIT_TP,
                                           FN_CURRENT_STOCK_QTY(COMPANY_ID ,:param1 ,SKU_ID) STOCK_QTY
                                    FROM Product_Info
                                    WHERE COMPANY_ID=:param2
                                    ",unitid);

        private string LoadProductData_Query_Filtered() => @"Select
                               ROW_NUMBER() OVER(ORDER BY p.SKU_ID ASC) AS ROW_NO
                              ,p.BASE_PRODUCT_ID
                              ,p.BRAND_ID
                              ,p.CATEGORY_ID
                              ,p.COMPANY_ID
                              ,p.FONT_COLOR
                              ,p.GROUP_ID
                              ,p.PACK_SIZE
                              ,p.PACK_UNIT
                              ,p.PACK_VALUE
                              ,p.PRIMARY_PRODUCT_ID
                              ,p.PRODUCT_SEASON_ID
                              ,p.PRODUCT_STATUS
                              ,p.PRODUCT_TYPE_ID
                              ,p.QTY_PER_PACK
                              ,p.REMARKS
                              ,p.SHIPPER_QTY
                              ,p.SHIPPER_VOLUME
                              ,p.SHIPPER_VOLUME_UNIT
                              ,p.SHIPPER_WEIGHT
                              ,p.SHIPPER_WEIGHT_UNIT
                              ,p.SKU_CODE
                              ,p.SKU_ID
                              ,p.SKU_NAME
                              ,p.SKU_NAME_BANGLA
                              ,p.STORAGE_ID
                              ,p.UNIT_ID
                              ,p.WEIGHT_PER_PACK
                              ,p.WEIGHT_UNIT
                              ,BP.BASE_PRODUCT_NAME
                              ,b.BRAND_NAME
                              ,g.GROUP_NAME
                              ,C.CATEGORY_NAME
                               ,FN_SKU_PRICE(p.SKU_ID,p.SKU_CODE,p.COMPANY_ID,:param1) UNIT_TP
                               ,FN_CURRENT_STOCK_QTY(p.COMPANY_ID ,:param1 ,p.SKU_ID) STOCK_QTY
                               ,FN_DEPOT_SKU_SUGGESTED_QTY(p.COMPANY_ID ,:param1 ,p.SKU_ID) SUGGESTED_QTY
                               from Product_Info p
                               left outer join Base_Product_Info bp on BP.BASE_PRODUCT_ID = P.BASE_PRODUCT_ID
                               left outer join Category_info c on C.CATEGORY_ID = P.CATEGORY_ID
                               left outer join Group_Info g on g.GROUP_ID = P.GROUP_ID
                               left outer join BRAND_INFO b on b.BRAND_ID = P.BRAND_ID
                              ";

        public async Task<string> LoadData(string db, int Company_Id, int unit_id)
        {
            List<Depot_Requisition_Raise_Mst> requisition_Mst_list = new List<Depot_Requisition_Raise_Mst>();
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadMasterData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), unit_id.ToString() }));
            for (int i = 0; i < data.Rows.Count; i++)
            {
                Depot_Requisition_Raise_Mst requisition_Mst = new Depot_Requisition_Raise_Mst();
                requisition_Mst.ROW_NO = Convert.ToInt32(data.Rows[i]["ROW_NO"]);
                requisition_Mst.MST_ID = Convert.ToInt32(data.Rows[i]["MST_ID"]);
                requisition_Mst.ISSUE_UNIT_ID = data.Rows[i]["ISSUE_UNIT_ID"].ToString();
                requisition_Mst.REQUISITION_UNIT_NAME = data.Rows[i]["REQUISITION_UNIT_NAME"].ToString();
                requisition_Mst.ISSUE_UNIT_NAME = data.Rows[i]["ISSUE_UNIT_NAME"].ToString();
                requisition_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[i]["MST_ID"].ToString());
                requisition_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[i]["COMPANY_ID"]);
                requisition_Mst.REMARKS = data.Rows[i]["REMARKS"].ToString();
                requisition_Mst.REQUISITION_AMOUNT = Convert.ToInt32(data.Rows[i]["REQUISITION_AMOUNT"]);
                requisition_Mst.STATUS = data.Rows[i]["STATUS"].ToString();
                requisition_Mst.TOTAL_WEIGHT = Convert.ToDouble(data.Rows[i]["TOTAL_WEIGHT"]);
                requisition_Mst.TOTAL_VOLUME = Convert.ToDouble(data.Rows[i]["TOTAL_VOLUME"]);
                requisition_Mst.REQUISITION_DATE = data.Rows[i]["REQUISITION_DATE"].ToString();
                requisition_Mst.REQUISITION_NO = data.Rows[i]["REQUISITION_NO"].ToString();
                requisition_Mst.REQUISITION_UNIT_ID = Convert.ToInt32(data.Rows[i]["REQUISITION_UNIT_ID"]);
                requisition_Mst_list.Add(requisition_Mst);
            }
            return JsonSerializer.Serialize(requisition_Mst_list);
        }

        public async Task<string> LoadDataForIssue(string db, int Company_Id, int unit_id)
        {
            List<Depot_Requisition_Raise_Mst> requisition_Mst_list = new List<Depot_Requisition_Raise_Mst>();
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadMasterData_issue_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), unit_id.ToString() }));
            for (int i = 0; i < data.Rows.Count; i++)
            {
                Depot_Requisition_Raise_Mst requisition_Mst = new Depot_Requisition_Raise_Mst();
                requisition_Mst.ROW_NO = Convert.ToInt32(data.Rows[i]["ROW_NO"]);
                requisition_Mst.MST_ID = Convert.ToInt32(data.Rows[i]["MST_ID"]);
                requisition_Mst.ISSUE_UNIT_ID = data.Rows[i]["ISSUE_UNIT_ID"].ToString();
                requisition_Mst.REQUISITION_UNIT_NAME = data.Rows[i]["REQUISITION_UNIT_NAME"].ToString();
                requisition_Mst.ISSUE_UNIT_NAME = data.Rows[i]["ISSUE_UNIT_NAME"].ToString();
                requisition_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[i]["MST_ID"].ToString());
                requisition_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[i]["COMPANY_ID"]);
                requisition_Mst.REMARKS = data.Rows[i]["REMARKS"].ToString();
                requisition_Mst.REQUISITION_AMOUNT = Convert.ToInt32(data.Rows[i]["REQUISITION_AMOUNT"]);
                requisition_Mst.STATUS = data.Rows[i]["STATUS"].ToString();
                requisition_Mst.TOTAL_WEIGHT = Convert.ToDouble(data.Rows[i]["TOTAL_WEIGHT"]);
                requisition_Mst.TOTAL_VOLUME = Convert.ToDouble(data.Rows[i]["TOTAL_VOLUME"]);
                requisition_Mst.REQUISITION_DATE = data.Rows[i]["REQUISITION_DATE"].ToString();
                requisition_Mst.REQUISITION_NO = data.Rows[i]["REQUISITION_NO"].ToString();
                requisition_Mst.REQUISITION_UNIT_ID = Convert.ToInt32(data.Rows[i]["REQUISITION_UNIT_ID"]);
                requisition_Mst_list.Add(requisition_Mst);
            }
            return JsonSerializer.Serialize(requisition_Mst_list);
        }

        public async Task<string> AddOrUpdate(string db, Depot_Requisition_Raise_Mst model)
        {
            Result result = new Result();

            if (model == null)
            {
                result.Status = "No data provided to insert!!!!";
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

                        model.REQUISITION_NO = _commonServices.GenerateRequisitionCode(_configuration.GetConnectionString(db), "FN_GENERATE_REQ_RAISE_NO", model.COMPANY_ID.ToString(), model.UNIT_ID.ToString());
                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(),
                         _commonServices.AddParameter(new string[] {model.MST_ID.ToString(),
                             model.REQUISITION_UNIT_ID.ToString(), model.ISSUE_UNIT_ID.ToString(),model.REQUISITION_NO,
                             model.REQUISITION_DATE, model.REQUISITION_AMOUNT.ToString(),model.REQUISITION_RAISE_BY,model.STATUS, model.COMPANY_ID.ToString(), model.REMARKS,
                             model.ENTERED_BY, model.ENTERED_DATE,  model.ENTERED_TERMINAL,model.TOTAL_WEIGHT.ToString(),model.TOTAL_VOLUME.ToString()
                            }))); ;

                        if (model.requisitionDtlList != null && model.requisitionDtlList.Count > 0)
                        {
                            dtlId = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetRequisition_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { }));
                            foreach (var item in model.requisitionDtlList)
                            {
                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQueryDTL(),
                               _commonServices.AddParameter(new string[] { model.MST_ID.ToString(),dtlId.ToString(),
                                     item.SKU_ID.ToString(),item.SKU_CODE,
                                     item.UNIT_TP.ToString(), item.REQUISITION_QTY.ToString(), item.REQUISITION_AMOUNT.ToString(), item.STATUS,
                                     item.COMPANY_ID.ToString(), item.REMARKS,  item.ENTERED_BY,  item.ENTERED_DATE,  item.ENTERED_TERMINAL,item.SHIPPER_QTY.ToString(),
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
                        Depot_Requisition_Raise_Mst depot_Requisition_Raise_Mst = await LoadDetailData_ByMasterId_List(db, model.MST_ID);
                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] { model.MST_ID.ToString(),
                             model.REQUISITION_UNIT_ID.ToString(), model.ISSUE_UNIT_ID.ToString(),model.REQUISITION_NO,
                             model.REQUISITION_DATE, model.REQUISITION_AMOUNT.ToString(),model.REQUISITION_RAISE_BY,model.STATUS, model.COMPANY_ID.ToString(), model.REMARKS,
                             model.UPDATED_BY.ToString(), model.UPDATED_DATE.ToString(),
                             model.UPDATED_TERMINAL.ToString(),model.TOTAL_WEIGHT.ToString(),model.TOTAL_VOLUME.ToString()})));
                        foreach (var item in model.requisitionDtlList)
                        {
                            if (item.DTL_ID == 0)
                            {
                                //-------------Add new row on detail table--------------------

                                dtlId = dtlId == 0 ? _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetRequisition_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { })) : (dtlId + 1);

                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQueryDTL(), _commonServices.AddParameter(new string[] {model.MST_ID.ToString(),dtlId.ToString(),
                                     item.SKU_ID.ToString(),item.SKU_CODE,
                                     item.UNIT_TP.ToString(), item.REQUISITION_QTY.ToString(), item.REQUISITION_AMOUNT.ToString(), item.STATUS,
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
                                     item.UNIT_TP.ToString(), item.REQUISITION_QTY.ToString(), item.REQUISITION_AMOUNT.ToString(), item.STATUS,
                                     item.COMPANY_ID.ToString(), item.REMARKS,item.UPDATED_BY,item.UPDATED_DATE,item.UPDATED_TERMINAL,item.SHIPPER_QTY.ToString(),
                                            item.NO_OF_SHIPPER.ToString(),item.LOOSE_QTY.ToString(),item.SHIPPER_WEIGHT.ToString(),item.TOTAL_SHIPPER_WEIGHT.ToString(),item.LOOSE_WEIGHT.ToString(),item.TOTAL_WEIGHT.ToString(),
                                            item.SHIPPER_VOLUME.ToString(),item.TOTAL_SHIPPER_VOLUME.ToString(),item.LOOSE_VOLUME.ToString(),item.TOTAL_VOLUME.ToString(),item.PER_PACK_VOLUME.ToString(),item.PER_PACK_WEIGHT.ToString() })));
                                dtl_list += "," + item.DTL_ID;
                            }
                        }

                        foreach (var item in depot_Requisition_Raise_Mst.requisitionDtlList)
                        {
                            bool status = true;

                            foreach (var updateditem in model.requisitionDtlList)
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
                    result.Key = _commonServices.Encrypt(model.MST_ID.ToString());
                    string st = status_action == false ? "Add" : "Edit";

                    await _logManager.AddOrUpdate(model.db_security, st, "DEPOT_REQUISITION_RAISE_MST", model.COMPANY_ID, model.UNIT_ID, Convert.ToInt32(model.ENTERED_BY), model.ENTERED_TERMINAL, "/Inventory/Requisition/Requisition", model.MST_ID, dtl_list);
                    int issue_Pending = await this.LoadIssuePendingRequisitionCount(db, model.COMPANY_ID, Convert.ToInt32(model.ISSUE_UNIT_ID));

                    //Notification Add---------------

                    await _NotificationManager.AddOrderNotification(model.db_security, model.db_sales, 3, "New Requisition Raise " + model.REQUISITION_NO + " Has been added/Edited and ready for Issue. Pending Requisition for Issue: " + issue_Pending, model.COMPANY_ID, Convert.ToInt32(model.ISSUE_UNIT_ID));

                    result.Status = "1";
                }
                catch (Exception ex)
                {
                    result.Status = ex.Message;
                }
            }
            return JsonSerializer.Serialize(result);
        }

        public Task<string> LoadSKUPriceDtlData(string db, int Company_Id)
        {
            throw new NotImplementedException();
        }

        public Task<string> LoadSKUPriceMstData(string db, int Company_Id)
        {
            throw new NotImplementedException();
        }

        public async Task<int> LoadIssuePendingRequisitionCount(string db, int Company_Id, int Unit_Id)
        {
            return await _commonServices.GetMaximumNumberAsyn<int>(_configuration.GetConnectionString(db), Load_IssuePendingRequisition_Count_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), Unit_Id.ToString() }));
        }

        public Task<string> GetCustomerExistingSKUData(string db, int Company_Id, int Customer_Id)
        {
            throw new NotImplementedException();
        }

        public Task<string> LoadData_Master(string db, int Company_Id)
        {
            throw new NotImplementedException();
        }

        public async Task<Depot_Requisition_Raise_Mst> LoadDetailData_ByMasterId_List(string db, int _Id)
        {
            Depot_Requisition_Raise_Mst _requisition_Mst = new Depot_Requisition_Raise_Mst();

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
                _requisition_Mst.REQUISITION_RAISE_BY = data.Rows[0]["REQUISITION_RAISE_BY"].ToString();
                _requisition_Mst.REQUISITION_AMOUNT = Convert.ToDouble(data.Rows[0]["REQUISITION_AMOUNT"]);
                _requisition_Mst.STATUS = data.Rows[0]["STATUS"].ToString();
                _requisition_Mst.REQUISITION_DATE = data.Rows[0]["REQUISITION_DATE"].ToString();
                _requisition_Mst.REQUISITION_NO = data.Rows[0]["REQUISITION_NO"].ToString();
                _requisition_Mst.REQUISITION_UNIT_ID = Convert.ToInt32(data.Rows[0]["REQUISITION_UNIT_ID"]);
                _requisition_Mst.TOTAL_WEIGHT = Convert.ToDouble(data.Rows[0]["TOTAL_WEIGHT"]);
                _requisition_Mst.TOTAL_VOLUME = Convert.ToDouble(data.Rows[0]["TOTAL_VOLUME"]);
                DataTable dataTable_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DetailByMasterId_Query(), _commonServices.AddParameter(new string[] { _Id.ToString() }));
                _requisition_Mst.requisitionDtlList = new List<Depot_Requisition_Raise_Dtl>();
                for (int i = 0; i < dataTable_detail.Rows.Count; i++)
                {
                    Depot_Requisition_Raise_Dtl _requisition_Dtl = new Depot_Requisition_Raise_Dtl();

                    _requisition_Dtl.COMPANY_ID = Convert.ToInt32(dataTable_detail.Rows[i]["COMPANY_ID"]);
                    _requisition_Dtl.MST_ID = Convert.ToInt32(dataTable_detail.Rows[i]["MST_ID"]);
                    _requisition_Dtl.DTL_ID = Convert.ToInt32(dataTable_detail.Rows[i]["DTL_ID"]);

                    _requisition_Dtl.SKU_ID = dataTable_detail.Rows[i]["SKU_ID"].ToString();
                    _requisition_Dtl.SKU_CODE = dataTable_detail.Rows[i]["SKU_CODE"].ToString();
                    _requisition_Dtl.UNIT_TP = Convert.ToInt32(dataTable_detail.Rows[i]["UNIT_TP"]);
                    _requisition_Dtl.REQUISITION_QTY = Convert.ToInt32(dataTable_detail.Rows[i]["REQUISITION_QTY"]);
                    _requisition_Dtl.STOCK_QTY = Convert.ToInt32(dataTable_detail.Rows[i]["STOCK_QTY"]);
                    _requisition_Dtl.SUGGESTED_QTY = Convert.ToInt32(dataTable_detail.Rows[i]["SUGGESTED_QTY"]);
                    _requisition_Dtl.ISSUE_STOCK_QTY = Convert.ToInt32(dataTable_detail.Rows[i]["ISSUE_STOCK_QTY"]);
                    _requisition_Dtl.REQUISITION_AMOUNT = Convert.ToInt32(dataTable_detail.Rows[i]["REQUISITION_AMOUNT"]);
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
                    _requisition_Mst.requisitionDtlList.Add(_requisition_Dtl);
                }
            }

            return _requisition_Mst;
        }



        public async Task<string> LoadProductData(string db, int Company_Id, int Unit_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadProductData_Query(Unit_Id), _commonServices.AddParameter(new string[] { Unit_Id.ToString(), Company_Id.ToString() })));

        public async Task<string> GetProductDataFiltered(string db, int Company_Id, ProductFilterParameters parameters, int Unit_Id)
        {
            string Query = LoadProductData_Query_Filtered();

            if (parameters.BASE_PRODUCT_ID != null && parameters.BASE_PRODUCT_ID.Count > 0)
            {
                string _BASE_PRODUCT_ID = "";
                for (int i = 0; i < parameters.BASE_PRODUCT_ID.Count; i++)
                {
                    if (parameters.BASE_PRODUCT_ID[i] != "0")
                    {
                        if (i == 0)
                        {
                            _BASE_PRODUCT_ID = parameters.BASE_PRODUCT_ID[i];
                        }
                        else
                        {
                            _BASE_PRODUCT_ID = _BASE_PRODUCT_ID + "," + parameters.BASE_PRODUCT_ID[i];
                        }
                    }
                }
                if (_BASE_PRODUCT_ID != "")
                {
                    Query = Query + " AND p.BASE_PRODUCT_ID in (" + _BASE_PRODUCT_ID + ")";
                }
            }
            if (parameters.GROUP_ID != null && parameters.GROUP_ID.Count > 0)
            {
                string _GROUP_ID = "";
                for (int i = 0; i < parameters.GROUP_ID.Count; i++)
                {
                    if (parameters.GROUP_ID[i] != "0")
                    {
                        if (i == 0)
                        {
                            _GROUP_ID = parameters.GROUP_ID[i];
                        }
                        else
                        {
                            _GROUP_ID = _GROUP_ID + "," + parameters.GROUP_ID[i];
                        }
                    }
                }
                if (_GROUP_ID != "")
                {
                    Query = Query + " AND  p.GROUP_ID in (" + _GROUP_ID + ")";
                }
            }

            if (parameters.BRAND_ID != null && parameters.BRAND_ID.Count > 0)
            {
                string _BRAND_ID = "";
                for (int i = 0; i < parameters.BRAND_ID.Count; i++)
                {
                    if (parameters.BRAND_ID[i] != "0")
                    {
                        if (i == 0)
                        {
                            _BRAND_ID = parameters.BRAND_ID[i];
                        }
                        else
                        {
                            _BRAND_ID = _BRAND_ID + "," + parameters.BRAND_ID[i];
                        }
                    }
                }
                if (_BRAND_ID != "")
                {
                    Query = Query + " AND  p.BRAND_ID in (" + _BRAND_ID + ")";
                }
            }

            if (parameters.CATEGORY_ID != null && parameters.CATEGORY_ID.Count > 0)
            {
                string _CATEGORY_ID = "";
                for (int i = 0; i < parameters.CATEGORY_ID.Count; i++)
                {
                    if (parameters.CATEGORY_ID[i] != "0")
                    {
                        if (i == 0)
                        {
                            _CATEGORY_ID = parameters.CATEGORY_ID[i];
                        }
                        else
                        {
                            _CATEGORY_ID = _CATEGORY_ID + "," + parameters.CATEGORY_ID[i];
                        }
                    }
                }
                if (_CATEGORY_ID != "")
                {
                    Query = Query + " AND  p.CATEGORY_ID in (" + _CATEGORY_ID + ")";
                }
            }

            if (parameters.SKU_ID != null && parameters.SKU_ID.Count > 0)
            {
                string _SKU_ID = "";
                for (int i = 0; i < parameters.SKU_ID.Count; i++)
                {
                    if (parameters.SKU_ID[i] != "0")
                    {
                        if (i == 0)
                        {
                            _SKU_ID = parameters.SKU_ID[i];
                        }
                        else
                        {
                            _SKU_ID = _SKU_ID + "," + parameters.SKU_ID[i];
                        }
                    }
                }
                if (_SKU_ID != "")
                {
                    Query = Query + " AND  p.SKU_ID in (" + _SKU_ID + ")";
                }
            }

            var dd = _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Query, _commonServices.AddParameter(new string[] { Unit_Id.ToString() })));
            return dd;
        }

        public async Task<string> LoadProductWeightData(string db, int Company_Id, int Sku_Id, int Req_Qty) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadProductWeightData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), Sku_Id.ToString(), Req_Qty.ToString() })));

        public async Task<string> LoadDataBetweenDate(string db, int Company_Id, string unit_id, string date_form, string date_to, string MST_ID)
        {
            if (MST_ID == "6" || MST_ID == "37" || MST_ID == "38")
            {
                return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadRequisitionRaiseBetweenDate_Query, _commonServices.AddParameter(new string[] { Company_Id.ToString(), date_form, date_to, unit_id })));
            }
            else if (MST_ID == "7")
            {
                return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadRequisitionIssueBetweenDate_Query, _commonServices.AddParameter(new string[] { Company_Id.ToString(), date_form, date_to, unit_id })));
            }
            else if (MST_ID == "8")
            {
                return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadRequisitionReceivedBetweenDate_Query, _commonServices.AddParameter(new string[] { Company_Id.ToString(), date_form, date_to, unit_id })));
            }
            else if (MST_ID == "9")
            {
                return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadRequisitionReturnBetweenDate_Query, _commonServices.AddParameter(new string[] { Company_Id.ToString(), date_form, date_to, unit_id })));
            }
            else if (MST_ID == "24")
            {
                return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadRequisitionPendingForIssueBetweenDate_Query, _commonServices.AddParameter(new string[] { Company_Id.ToString(), date_form, date_to, unit_id })));
            }
            else if (MST_ID == "28")
            {
                return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadRequisitionPendingForDispatchBetweenDate_Query, _commonServices.AddParameter(new string[] { Company_Id.ToString(), date_form, date_to, unit_id })));
            }
            else if (MST_ID == "33")
            {
                return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadRequisitionPendingForReceiveBetweenDate_Query(unit_id), _commonServices.AddParameter(new string[] { Company_Id.ToString(), date_form, date_to })));
            }
            else
            {
                return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadRequisitionReceivedReturnBetweenDate_Query, _commonServices.AddParameter(new string[] { Company_Id.ToString(), date_form, date_to, unit_id })));
            }
        }
    }
}