using Microsoft.Extensions.Configuration;
using NUnit.Framework.Internal;
using Oracle.ManagedDataAccess.Client;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.TableModels.Company;
using SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.Company;
using SalesAndDistributionSystem.Services.Business.Security;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public class SalesOrderManager : ISalesOrderManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        private readonly IUserLogManager _logManager;
        private readonly INotificationManager _NotificationManager;

        public SalesOrderManager(ICommonServices commonServices, IConfiguration configuration, IUserLogManager logManager, INotificationManager NotificationManager)
        {
            _commonServices = commonServices;
            _configuration = configuration;
            _logManager = logManager;
            _NotificationManager = NotificationManager;
        }

        //-------------------Query Part ---------------------------------------------------
        string LoadCustomerOrderData_Query() => @"SELECT 
   ROWNUM ROW_NO, 
   O.ORDER_MST_ID, 
   O.ORDER_NO,
   N.INVOICE_NO,
   TO_CHAR(N.INVOICE_DATE, 'DD/MM/YYYY') INVOICE_DATE, 
   I.INVOICE_TYPE_NAME, 
   O.ORDER_UNIT_ID,
   TO_CHAR(O.ORDER_DATE, 'DD/MM/YYYY') ORDER_DATE, 
   O.ORDER_TYPE, 
   TO_CHAR(O.DELIVERY_DATE, 'DD/MM/YYYY') DELIVERY_DATE, 
   O.CUSTOMER_ID, 
   O.CUSTOMER_CODE, 
   O.MARKET_ID, 
   M.MARKET_NAME, 
   C.CUSTOMER_NAME,
   O.ORDER_ENTRY_TYPE, 
   O.ORDER_AMOUNT, 
   O.ORDER_STATUS, 
   O.FINAL_SUBMISSION_STATUS
FROM 
   ORDER_MST O
LEFT OUTER JOIN 
   CUSTOMER_INFO C ON C.CUSTOMER_ID = O.CUSTOMER_ID
LEFT OUTER JOIN 
   MARKET_INFO M ON M.MARKET_ID = O.MARKET_ID
LEFT OUTER JOIN 
   INVOICE_TYPE_INFO I ON I.INVOICE_TYPE_CODE = O.ORDER_TYPE
LEFT OUTER JOIN 
   INVOICE_MST N ON N.ORDER_MST_ID = O.ORDER_MST_ID
WHERE 
   O.ORDER_PROCESS_STATUS = 'Complete' 
   AND O.COMPANY_ID = :param1 
   AND O.CUSTOMER_CODE = :param2 
   AND TRUNC(O.ENTERED_DATE) BETWEEN ADD_MONTHS(TRUNC(SYSDATE), -1) AND TRUNC(SYSDATE) ORDER BY O.ORDER_DATE DESC";
        string LoadOsmOrderData_Query() => @"SELECT 
   ROWNUM ROW_NO, O.ORDER_MST_ID, O.ORDER_NO, I.INVOICE_TYPE_NAME,O.ORDER_UNIT_ID,
   TO_CHAR(O.ORDER_DATE, 'DD/MM/YYYY') ORDER_DATE, 
   O.ORDER_TYPE, TO_CHAR(O.DELIVERY_DATE, 'DD/MM/YYYY') DELIVERY_DATE, O.CUSTOMER_ID, 
   O.CUSTOMER_CODE, O.MARKET_ID, M.MARKET_NAME, C.CUSTOMER_NAME ,
   O.ORDER_ENTRY_TYPE, O.ORDER_AMOUNT, O.ORDER_STATUS, O.FINAL_SUBMISSION_STATUS
   FROM ORDER_MST O
   LEFT OUTER JOIN CUSTOMER_INFO C on C.CUSTOMER_ID = O.CUSTOMER_ID
   LEFT OUTER JOIN MARKET_INFO M on M.MARKET_ID = O.MARKET_ID
   LEFT OUTER JOIN INVOICE_TYPE_INFO I on I.INVOICE_TYPE_CODE= O.ORDER_TYPE
   Where O.ORDER_PROCESS_STATUS = 'Complete' AND O.COMPANY_ID = :param1 and O.CUSTOMER_CODE = :param2 AND TO_DATE(SYSDATE,'DD/MM/YYYY') = TO_DATE(O.ENTERED_DATE,'DD/MM/YYYY') ";
        string LoadCustomerOrderMonitoringData_Query() => @"SELECT O.ORDER_NO,
       TO_CHAR (O.ORDER_DATE, 'DD/MM/YYYY') ORDER_DATE,
       T.INVOICE_TYPE_NAME,
       O.CUSTOMER_CODE,
       C.CUSTOMER_NAME,
       M.MARKET_CODE,
       M.MARKET_NAME,
       O.ORDER_STATUS,
       O.SM_CONFIRM_STATUS,
       O.DSM_CONFIRM_STATUS,
       O.HOS_CONFIRM_STATUS,
       O.FINAL_SUBMIT_CONFIRM_STATUS,
       O.ORDER_AMOUNT,
       O.DISCOUNT_AMOUNT,
       O.TOTAL_LOADING_CHARGE,
       O.COMBO_DISCOUNT,
       O.ADJUSTMENT_AMOUNT,
       O.NET_ORDER_AMOUNT,
       O.REMARKS,
       K.UNIT_NAME
  FROM ORDER_MST O
       LEFT OUTER JOIN INVOICE_TYPE_INFO T
          ON T.INVOICE_TYPE_CODE = O.ORDER_TYPE
       LEFT OUTER JOIN CUSTOMER_INFO C ON C.CUSTOMER_ID = O.CUSTOMER_ID
       LEFT OUTER JOIN MARKET_INFO M ON M.MARKET_ID = O.MARKET_ID
       LEFT OUTER JOIN STL_ERP_SCS.COMPANY_INFO K ON K.COMPANY_ID=O.COMPANY_ID AND K.UNIT_ID=O.INVOICE_UNIT_ID
 WHERE O.ORDER_PROCESS_STATUS = 'Complete' AND O.COMPANY_ID = :param1 AND TO_DATE(SYSDATE,'DD/MM/YYYY') = TO_DATE(O.ENTERED_DATE,'DD/MM/YYYY')";
        string LoadDistributorOrderMonitoringData_Query() => @"SELECT O.ORDER_NO,
       TO_CHAR (O.ORDER_DATE, 'DD/MM/YYYY') ORDER_DATE,
       T.INVOICE_TYPE_NAME,
       O.CUSTOMER_CODE,
       C.CUSTOMER_NAME,
       M.MARKET_CODE,
       M.MARKET_NAME,
       O.ORDER_STATUS,
       O.SM_CONFIRM_STATUS,
       O.DSM_CONFIRM_STATUS,
       O.HOS_CONFIRM_STATUS,
       O.FINAL_SUBMIT_CONFIRM_STATUS,
       O.ORDER_AMOUNT,
       O.DISCOUNT_AMOUNT,
       O.TOTAL_LOADING_CHARGE,
       O.COMBO_DISCOUNT,
       O.ADJUSTMENT_AMOUNT,
       O.NET_ORDER_AMOUNT,
       O.REMARKS,
       K.UNIT_NAME
  FROM ORDER_MST O
       LEFT OUTER JOIN INVOICE_TYPE_INFO T
          ON T.INVOICE_TYPE_CODE = O.ORDER_TYPE
       LEFT OUTER JOIN CUSTOMER_INFO C ON C.CUSTOMER_ID = O.CUSTOMER_ID
       LEFT OUTER JOIN MARKET_INFO M ON M.MARKET_ID = O.MARKET_ID
       LEFT OUTER JOIN STL_ERP_SCS.COMPANY_INFO K ON K.COMPANY_ID=O.COMPANY_ID AND K.UNIT_ID=O.INVOICE_UNIT_ID
 WHERE O.ORDER_PROCESS_STATUS = 'Complete' AND O.COMPANY_ID = :param1 AND TO_DATE(SYSDATE,'DD/MM/YYYY') = TO_DATE(O.ENTERED_DATE,'DD/MM/YYYY')
       AND O.CUSTOMER_CODE=:param2";
        string LoadData_Query() => @"SELECT 
   ROWNUM ROW_NO, O.ORDER_MST_ID, O.ORDER_NO,I.INVOICE_TYPE_NAME, O.ORDER_UNIT_ID,
   TO_CHAR(O.ORDER_DATE, 'DD/MM/YYYY') ORDER_DATE,
   O.ORDER_TYPE, TO_CHAR(O.DELIVERY_DATE, 'DD/MM/YYYY') DELIVERY_DATE, O.CUSTOMER_ID, 
   O.CUSTOMER_CODE, O.MARKET_ID, M.MARKET_NAME, C.CUSTOMER_NAME ,
   O.ORDER_ENTRY_TYPE, O.ORDER_AMOUNT, O.ORDER_STATUS, O.FINAL_SUBMISSION_STATUS
   FROM ORDER_MST O
   LEFT OUTER JOIN CUSTOMER_INFO C on C.CUSTOMER_ID = O.CUSTOMER_ID
   LEFT OUTER JOIN MARKET_INFO M on M.MARKET_ID = O.MARKET_ID
   LEFT OUTER JOIN INVOICE_TYPE_INFO I on I.INVOICE_TYPE_CODE= O.ORDER_TYPE
   Where O.ORDER_PROCESS_STATUS = 'Complete' AND O.COMPANY_ID = :param1 AND  TO_DATE(SYSDATE,'DD/MM/YYYY') = TO_DATE(O.ENTERED_DATE,'DD/MM/YYYY')";
        string LoadDataProcessed_Query() => @"SELECT 
   ROWNUM ROW_NO, O.ORDER_MST_ID, O.ORDER_NO,
   TO_CHAR(O.ORDER_DATE, 'DD/MM/YYYY') ORDER_DATE, 
   O.ORDER_TYPE, TO_CHAR(O.DELIVERY_DATE, 'DD/MM/YYYY') DELIVERY_DATE, O.CUSTOMER_ID, 
   O.CUSTOMER_CODE, O.MARKET_ID, M.MARKET_NAME, C.CUSTOMER_NAME ,
   O.ORDER_ENTRY_TYPE, O.ORDER_AMOUNT, O.ORDER_STATUS
   FROM ORDER_MST O
   LEFT OUTER JOIN CUSTOMER_INFO C on C.CUSTOMER_ID = O.CUSTOMER_ID
   LEFT OUTER JOIN MARKET_INFO M on M.MARKET_ID = O.MARKET_ID
   Where O.ORDER_PROCESS_STATUS != 'Complete' AND O.COMPANY_ID = :param1 and O.ORDER_UNIT_ID = :param2 AND TO_DATE(SYSDATE,'DD/MM/YYYY') = TO_DATE(O.ENTERED_DATE,'DD/MM/YYYY') ";
        string LoadDataProcessedCustomer_Query() => @"SELECT 
   ROWNUM ROW_NO, O.ORDER_MST_ID, O.ORDER_NO,
   TO_CHAR(O.ORDER_DATE, 'DD/MM/YYYY') ORDER_DATE, 
   O.ORDER_TYPE, TO_CHAR(O.DELIVERY_DATE, 'DD/MM/YYYY') DELIVERY_DATE, O.CUSTOMER_ID, 
   O.CUSTOMER_CODE, O.MARKET_ID, M.MARKET_NAME, C.CUSTOMER_NAME ,
   O.ORDER_ENTRY_TYPE, O.ORDER_AMOUNT, O.ORDER_STATUS
   FROM ORDER_MST O
   LEFT OUTER JOIN CUSTOMER_INFO C on C.CUSTOMER_ID = O.CUSTOMER_ID
   LEFT OUTER JOIN MARKET_INFO M on M.MARKET_ID = O.MARKET_ID
   Where O.ORDER_PROCESS_STATUS != 'Complete' AND O.COMPANY_ID = :param1 and O.CUSTOMER_CODE = :param2 AND TO_DATE(SYSDATE,'DD/MM/YYYY') = TO_DATE(O.ENTERED_DATE,'DD/MM/YYYY') ";

        string LoadFilteredData_Query() => @" SELECT 
   ROWNUM ROW_NO, O.ORDER_MST_ID, O.ORDER_NO, O.ORDER_UNIT_ID,
   TO_CHAR(O.ORDER_DATE, 'DD/MM/YYYY') ORDER_DATE, 
   O.ORDER_TYPE,I.INVOICE_TYPE_NAME, TO_CHAR(O.DELIVERY_DATE, 'DD/MM/YYYY') DELIVERY_DATE, O.CUSTOMER_ID, 
   O.CUSTOMER_CODE, O.MARKET_ID, M.MARKET_NAME, C.CUSTOMER_NAME , O.FINAL_SUBMISSION_STATUS,
   O.ORDER_ENTRY_TYPE, O.ORDER_AMOUNT, O.ORDER_STATUS
   FROM ORDER_MST O
   LEFT OUTER JOIN CUSTOMER_INFO C on C.CUSTOMER_ID = O.CUSTOMER_ID
   LEFT OUTER JOIN MARKET_INFO M on M.MARKET_ID = O.MARKET_ID
   LEFT OUTER JOIN INVOICE_TYPE_INFO I on I.INVOICE_TYPE_CODE= O.ORDER_TYPE
   Where O.ORDER_PROCESS_STATUS = 'Complete' AND  O.COMPANY_ID = :param1  ";
        string LoadFilteredMonitoringData_Query() => @"SELECT O.ORDER_NO,
       TO_CHAR (O.ORDER_DATE, 'DD/MM/YYYY') ORDER_DATE,
       T.INVOICE_TYPE_NAME,
       O.CUSTOMER_CODE,
       C.CUSTOMER_NAME,
       M.MARKET_CODE,
       M.MARKET_NAME,
       O.ORDER_STATUS,
       O.SM_CONFIRM_STATUS,
       O.DSM_CONFIRM_STATUS,
       O.HOS_CONFIRM_STATUS,
       O.FINAL_SUBMIT_CONFIRM_STATUS,
       O.ORDER_AMOUNT,
       O.DISCOUNT_AMOUNT,
       O.TOTAL_LOADING_CHARGE,
       O.COMBO_DISCOUNT,
       O.ADJUSTMENT_AMOUNT,
       O.NET_ORDER_AMOUNT,
       O.REMARKS,
       K.UNIT_NAME
  FROM ORDER_MST O
       LEFT OUTER JOIN INVOICE_TYPE_INFO T
          ON T.INVOICE_TYPE_CODE = O.ORDER_TYPE
       LEFT OUTER JOIN CUSTOMER_INFO C ON C.CUSTOMER_ID = O.CUSTOMER_ID
       LEFT OUTER JOIN MARKET_INFO M ON M.MARKET_ID = O.MARKET_ID
       LEFT OUTER JOIN STL_ERP_SCS.COMPANY_INFO K ON K.COMPANY_ID=O.COMPANY_ID AND K.UNIT_ID=O.INVOICE_UNIT_ID
 WHERE O.ORDER_PROCESS_STATUS = 'Complete' AND  O.COMPANY_ID = :param1 and O.ORDER_UNIT_ID = NVL(:param2,O.ORDER_UNIT_ID) ";
        string LoadFilteredDistMonitoringData_Query() => @"SELECT O.ORDER_NO,
       TO_CHAR (O.ORDER_DATE, 'DD/MM/YYYY') ORDER_DATE,
       T.INVOICE_TYPE_NAME,
       O.CUSTOMER_CODE,
       C.CUSTOMER_NAME,
       M.MARKET_CODE,
       M.MARKET_NAME,
       O.ORDER_STATUS,
       O.SM_CONFIRM_STATUS,
       O.DSM_CONFIRM_STATUS,
       O.HOS_CONFIRM_STATUS,
       O.FINAL_SUBMIT_CONFIRM_STATUS,
       O.ORDER_AMOUNT,
       O.DISCOUNT_AMOUNT,
       O.TOTAL_LOADING_CHARGE,
       O.COMBO_DISCOUNT,
       O.ADJUSTMENT_AMOUNT,
       O.NET_ORDER_AMOUNT,
       O.REMARKS,
       K.UNIT_NAME
  FROM ORDER_MST O
       LEFT OUTER JOIN INVOICE_TYPE_INFO T
          ON T.INVOICE_TYPE_CODE = O.ORDER_TYPE
       LEFT OUTER JOIN CUSTOMER_INFO C ON C.CUSTOMER_ID = O.CUSTOMER_ID
       LEFT OUTER JOIN MARKET_INFO M ON M.MARKET_ID = O.MARKET_ID
       LEFT OUTER JOIN STL_ERP_SCS.COMPANY_INFO K ON K.COMPANY_ID=O.COMPANY_ID AND K.UNIT_ID=O.INVOICE_UNIT_ID
 WHERE O.ORDER_PROCESS_STATUS = 'Complete' AND  O.COMPANY_ID = :param1 and O.ORDER_UNIT_ID = NVL(:param2,O.ORDER_UNIT_ID) AND O.CUSTOMER_CODE=:param3 ";
        string LoadFilteredProcessedData_Query() => @"SELECT 
   ROWNUM ROW_NO, O.ORDER_MST_ID, O.ORDER_NO,I.INVOICE_TYPE_NAME,
   TO_CHAR(O.ORDER_DATE, 'DD/MM/YYYY') ORDER_DATE, 
   O.ORDER_TYPE, TO_CHAR(O.DELIVERY_DATE, 'DD/MM/YYYY') DELIVERY_DATE, O.CUSTOMER_ID, 
   O.CUSTOMER_CODE, O.MARKET_ID, M.MARKET_NAME, C.CUSTOMER_NAME ,
   O.ORDER_ENTRY_TYPE, O.ORDER_AMOUNT, O.ORDER_STATUS
   FROM ORDER_MST O
   LEFT OUTER JOIN CUSTOMER_INFO C on C.CUSTOMER_ID = O.CUSTOMER_ID
   LEFT OUTER JOIN MARKET_INFO M on M.MARKET_ID = O.MARKET_ID
   LEFT OUTER JOIN INVOICE_TYPE_INFO I on I.INVOICE_TYPE_CODE= O.ORDER_TYPE
   Where O.ORDER_PROCESS_STATUS != 'Complete' AND  O.COMPANY_ID = :param1 ";
        //string LoadProducts_Query() => @"Select p.SKU_ID, p.SKU_CODE, p.SKU_NAME, p.PACK_SIZE,  p.COMPANY_ID, p.UNIT_ID ,0 UNIT_TP, 
        //               0 CURRENT_STOCK, 0 SUGGESTED_QTY, BRAND_ID, CATEGORY_ID,GROUP_ID,BASE_PRODUCT_ID
        //               from Product_Info p
        //               Left outer join Customer_Info C on C.DISTRIBUTOR_PRODUCT_TYPE = P.DISTRIBUTOR_PRODUCT_TYPE
        //               Where  p.PRODUCT_STATUS = 'Active' and    p.company_ID = :param1 and C.CUSTOMER_ID = :param2";

        string LoadProductsSpecific_Query() => @"Select p.SKU_ID, p.SKU_CODE, p.SKU_NAME, p.PACK_SIZE,  p.COMPANY_ID, p.UNIT_ID ,0 UNIT_TP, 
                       0 CURRENT_STOCK, 0 SUGGESTED_QTY, BRAND_ID, CATEGORY_ID,GROUP_ID,BASE_PRODUCT_ID
                       from Customer_info C 
                       Left outer join distributor_product_mst m on m.DISTRIBUTOR_PRODUCT_TYPE = C.DISTRIBUTOR_PRODUCT_TYPE
                       left outer join distributor_product_dtl d on D.Mst_ID = M.Mst_ID
                       left outer join Product_Info p on P.SKU_ID = d.SKU_ID
                       Where  p.PRODUCT_STATUS = 'Active' and  C.DISTRIBUTOR_PRODUCT_TYPE = M.DISTRIBUTOR_PRODUCT_TYPE and   p.company_ID = :param1 and  C.CUSTOMER_ID = :param2
                       ";  
        string LoadProducts_Query() => @"Select p.SKU_ID, p.SKU_CODE, p.SKU_NAME, p.PACK_SIZE,  p.COMPANY_ID, p.UNIT_ID ,0 UNIT_TP, 
                       0 CURRENT_STOCK, 0 SUGGESTED_QTY, BRAND_ID, CATEGORY_ID,GROUP_ID,BASE_PRODUCT_ID,M.DISTRIBUTOR_PRODUCT_TYPE DISTRIBUTOR_PRODUCT_TYPE_MST,
                       C.DISTRIBUTOR_PRODUCT_TYPE PRODUCT_TYPE_CUSTOMER
                       from Product_Info p
                       left outer join Customer_Info C on c.CUSTOMER_ID = :param2
                       left outer join distributor_product_dtl d on D.SKU_ID = P.SKU_ID
                       Left outer join distributor_product_mst m on m.MST_ID = D.MST_ID
                       Where  p.PRODUCT_STATUS = 'Active' and  M.DISTRIBUTOR_PRODUCT_TYPE = 'ALL' and   p.company_ID = :param1 and  C.CUSTOMER_ID = :param2
                       ";
        string LoadCustomerData_Query() => @"Select COMPANY_ID, DIVISION_ID, DIVISION_CODE, DIVISION_NAME, REGION_ID, REGION_CODE,
 REGION_NAME, AREA_ID, AREA_CODE, AREA_NAME, TERRITORY_ID, TERRITORY_CODE, TERRITORY_NAME,
  MARKET_ID, MARKET_CODE, MARKET_NAME, INVOICE_FLAG, CUSTOMER_ID, CUSTOMER_CODE, CUSTOMER_NAME, CUSTOMER_ADDRESS
   from VW_LOCATION_FOR_ORDER Where COMPANY_ID = :param1 ";
        string LoadProductPrice_Query() => @"select  FN_ORDER_SKU_PRICE(:param1, :param2, :param3)  from DUAL";
        string AddOrUpdate_Master_AddQuery() => @"INSERT INTO ORDER_MST 
                                         (ORDER_MST_ID,ORDER_NO, ORDER_DATE, ORDER_TYPE,DELIVERY_DATE,
                                         CUSTOMER_ID,CUSTOMER_CODE,MARKET_ID , TERRITORY_ID, AREA_ID,REGION_ID,DIVISION_ID,PAYMENT_MODE,REPLACE_CLAIM_NO,
                                         BONUS_PROCESS_NO,BONUS_CLAIM_NO,INVOICE_STATUS,ORDER_AMOUNT,ORDER_STATUS,COMPANY_ID,ORDER_UNIT_ID,INVOICE_UNIT_ID,REMARKS
                                         ,SPA_TOTAL_AMOUNT,SPA_COMMISSION_PCT,SPA_COMMISSION_AMOUNT,SPA_NET_AMOUNT,ENTERED_BY,ENTERED_DATE,ENTERED_TERMINAL,ORDER_ENTRY_TYPE, REFURBISHMENT_CLAIM_NO) 
                                         VALUES ( :param1, :param2,  TO_DATE(:param3, 'DD/MM/YYYY HH:MI:SS AM'), :param4,TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM'), :param6,
                                        :param7,:param8, :param9, :param10, :param11, :param12, :param13, :param14
                                         , :param15, :param16, :param17, :param18, :param19 , :param20
                                          , :param21, :param22, :param23, :param24, :param25 , :param26
                                           , :param27, :param28, TO_DATE(:param29, 'DD/MM/YYYY HH:MI:SS AM'), :param30, :param31,:param32
                                          )";
        string AddOrUpdate_Detail_AddQuery() => @"INSERT INTO ORDER_DTL
                                         (ORDER_DTL_ID, ORDER_MST_ID, SKU_ID, SKU_CODE, ORDER_QTY, UNIT_TP, ORDER_AMOUNT, STATUS, 
                                         COMPANY_ID, UNIT_ID, REMARKS, SPA_UNIT_TP, SPA_AMOUNT, SPA_REQ_TIME_STOCK, SPA_DISCOUNT_TYPE, SPA_DISCOUNT_VAL_PCT,
                                         SPA_CUST_COM, SPA_DISCOUNT_AMOUNT, SPA_TOTAL_AMOUNT,
                                         ENTERED_BY,ENTERED_DATE,ENTERED_TERMINAL) 
                                         VALUES ( :param1, :param2,  :param3, :param4,:param5, :param6,
                                        :param7,:param8, :param9, :param10, :param11, :param12, :param13, :param14
                                         , :param15, :param16, :param17, :param18, :param19 , :param20
                                          , TO_DATE(:param21, 'DD/MM/YYYY HH:MI:SS AM'), :param22
                                          )";
        string AddOrUpdate_Master_UpdateQuery() => @"UPDATE ORDER_MST SET  ORDER_TYPE = :param2,
                      DELIVERY_DATE = TO_DATE(:param3, 'DD/MM/YYYY HH:MI:SS AM'),
                      CUSTOMER_ID = :param4, CUSTOMER_CODE = :param5,
                      MARKET_ID = :param6 , TERRITORY_ID = :param7 , AREA_ID= :param8 ,REGION_ID = :param9 ,DIVISION_ID = :param10, 
                      PAYMENT_MODE = :param11,
                      REPLACE_CLAIM_NO =  :param12,                
                      BONUS_PROCESS_NO =  :param13,                   
                      BONUS_CLAIM_NO =   :param14,                   
                      INVOICE_STATUS =  :param15,                   
                      ORDER_AMOUNT =  :param16,
                      ORDER_STATUS =  :param17,
                      ORDER_UNIT_ID   = :param18,                  
                      INVOICE_UNIT_ID =   :param19,               
                      REMARKS =    :param20,
                      UPDATED_BY = :param21, UPDATED_DATE = TO_DATE(:param22, 'DD/MM/YYYY HH:MI:SS AM'), 
                      UPDATED_TERMINAL = :param23 WHERE ORDER_MST_ID = :param1";
        string Save_Final_Order_Query() => @"UPDATE ORDER_MST SET  ORDER_STATUS = 'Active',
                      ORDER_PROCESS_STATUS = 'Complete'
                     WHERE ORDER_MST_ID = :param1";
        string Save_Final_Post_Order_Query() => @"UPDATE ORDER_MST SET  ORDER_STATUS = 'Active', DSM_CONFIRM_STATUS = 'Complete',SM_CONFIRM_STATUS = 'Complete',HOS_CONFIRM_STATUS = 'Complete',
                      ORDER_PROCESS_STATUS = 'Complete', FINAL_SUBMISSION_STATUS = 'Complete', FINAL_SUBMIT_CONFIRM_STATUS = 'Complete'
                     WHERE ORDER_MST_ID = :param1";

        string Save_On_DSM_Pending_Order_Query() => @"UPDATE ORDER_MST SET FINAL_SUBMISSION_STATUS = 'Complete',
                     DSM_CONFIRM_STATUS = 'Pending', NOTIFY_TEXT = :param2
                     WHERE ORDER_MST_ID = :param1";
        string Cancel_On_DSM_Pending_Order_Query() => @"UPDATE ORDER_MST SET FINAL_SUBMISSION_STATUS = 'Complete',
                     DSM_CONFIRM_STATUS = 'Cancel', FINAL_SUBMIT_CONFIRM_STATUS = 'Cancel', NOTIFY_TEXT = :param2
                     WHERE ORDER_MST_ID = :param1";
        string Save_On_SM_Pending_Order_Query() => @"UPDATE ORDER_MST SET  DSM_CONFIRM_STATUS = 'Complete',
                     SM_CONFIRM_STATUS = 'Pending', NOTIFY_TEXT = :param2
                     WHERE ORDER_MST_ID = :param1";
        string Cancel_On_SM_Pending_Order_Query() => @"UPDATE ORDER_MST SET  DSM_CONFIRM_STATUS = 'Complete',
                     SM_CONFIRM_STATUS = 'Cancel', FINAL_SUBMIT_CONFIRM_STATUS = 'Cancel', NOTIFY_TEXT = :param2
                     WHERE ORDER_MST_ID = :param1";
        string Save_On_HOS_Pending_Order_Query() => @"UPDATE ORDER_MST SET  DSM_CONFIRM_STATUS = 'Complete',
                     HOS_CONFIRM_STATUS = 'Pending',  NOTIFY_TEXT = :param2
                     WHERE ORDER_MST_ID = :param1";
        string Cancel_On_HOS_Pending_Order_Query() => @"UPDATE ORDER_MST SET  DSM_CONFIRM_STATUS = 'Complete',
                     HOS_CONFIRM_STATUS = 'Cancel', FINAL_SUBMIT_CONFIRM_STATUS = 'Cancel', NOTIFY_TEXT = :param2
                     WHERE ORDER_MST_ID = :param1";

        string AddOrUpdate_Detail_UpdateQuery() => @"UPDATE ORDER_DTL SET   SKU_ID = :param2
                       ,SKU_CODE =:param3
                       ,ORDER_QTY = :param4
                       ,UNIT_TP = :param5
                       ,ORDER_AMOUNT = :param6
                       ,STATUS =  :param7
                       ,REMARKS = :param8          
                      
                       
                       ,UPDATED_BY = :param9, UPDATED_DATE = TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'), 
                       UPDATED_TERMINAL = :param11 WHERE ORDER_DTL_ID = :param1";

        string GetNewOrder_MST_IdQuery() => "SELECT NVL(MAX(ORDER_MST_ID),0) + 1 ORDER_MST_ID  FROM ORDER_MST";

        string LoadData_Detail_Query() => @"Select  ROWNUM ROW_NO, ORDER_DTL_ID, ORDER_MST_ID, SKU_ID, SKU_CODE, ORDER_QTY, REVISED_ORDER_QTY, UNIT_TP,
 ORDER_AMOUNT, STATUS, COMPANY_ID, UNIT_ID, REMARKS, SPA_UNIT_TP, SPA_AMOUNT, SPA_REQ_TIME_STOCK,
  SPA_DISCOUNT_TYPE, SPA_DISCOUNT_VAL_PCT, SPA_CUST_COM, SPA_DISCOUNT_AMOUNT, SPA_TOTAL_AMOUNT from ORDER_DTL WHERE ORDER_MST_ID = :param1";
        string LoadData_DetailByMasterId_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.REGION_AREA_DTL_ID ASC) AS ROW_NO,M.REGION_AREA_DTL_ID,
                                          M.REGION_AREA_MST_ID, N.AREA_NAME,  M.AREA_ID,M.AREA_CODE, M.COMPANY_ID, M.REMARKS,
                                          M.REGION_AREA_DTL_STATUS,TO_CHAR(M.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE, TO_CHAR(M.EFFECT_START_DATE, 'DD/MM/YYYY') EFFECT_START_DATE  
                                          FROM REGION_AREA_DTL  M
                                          INNER JOIN AREA_INFO N ON N.AREA_ID = M.AREA_ID
                                          Where M.REGION_AREA_MST_ID = :param1";

             
        string GetNewOrder_Dtl_IdQuery() => "SELECT NVL(MAX(ORDER_DTL_ID),0) + 1 ORDER_DTL_ID  FROM ORDER_DTL";
        string GetNewOrder_MST_NoQuery() => "SELECT ORDER_NO +1  ORDER_NO FROM ORDER_MST where ORDER_MST_ID = (SELECT NVL(MAX(ORDER_MST_ID),0)  ORDER_MST_ID  FROM ORDER_MST WHERE COMPANY_ID = :param1)";

        string GetOrder_MST_BY_Id_Query() => @"SELECT ROWNUM ROW_NO, ORDER_MST_ID, ORDER_NO,TO_CHAR(ORDER_DATE, 'DD/MM/YYYY')  ORDER_DATE, 
ORDER_TYPE,TO_CHAR(DELIVERY_DATE, 'DD/MM/YYYY') DELIVERY_DATE, CUSTOMER_ID, CUSTOMER_CODE, MARKET_ID,
 TERRITORY_ID, AREA_ID, REGION_ID, DIVISION_ID, PAYMENT_MODE, REPLACE_CLAIM_NO, BONUS_PROCESS_NO, 
 BONUS_CLAIM_NO, INVOICE_STATUS, ORDER_AMOUNT, ORDER_STATUS, COMPANY_ID, ORDER_UNIT_ID, INVOICE_UNIT_ID, 
 REMARKS, SPA_TOTAL_AMOUNT, SPA_COMMISSION_PCT, SPA_COMMISSION_AMOUNT, SPA_NET_AMOUNT, ORDER_ENTRY_TYPE ,
 DISCOUNT_AMOUNT, ADJUSTMENT_AMOUNT, NET_ORDER_AMOUNT, ORDER_PROCESS_STATUS ,FN_ORDER_SLIP_VOLUME(:param1) TOTAL_VOLUME, FN_ORDER_SLIP_WEIGHT(:param1) TOTAL_WEIGHT, FINAL_SUBMISSION_STATUS,FINAL_SUBMIT_CONFIRM_STATUS,NVL(COMBO_DISCOUNT,0) COMBO_DISCOUNT,NVL(COMBO_LOADING_CHARGE,0) COMBO_LOADING_CHARGE, REFURBISHMENT_CLAIM_NO
 FROM ORDER_MST where ORDER_MST_ID = :param1";
        string GetOrder_DTL_BY_MstId_Query() => @"Select ROWNUM ROW_NO, ORDER_DTL_ID, ORDER_MST_ID, SKU_ID, SKU_CODE, ORDER_QTY,
 REVISED_ORDER_QTY, UNIT_TP, ORDER_AMOUNT, STATUS, COMPANY_ID, UNIT_ID, REMARKS,
  SPA_UNIT_TP, SPA_AMOUNT, SPA_REQ_TIME_STOCK, SPA_DISCOUNT_TYPE, SPA_DISCOUNT_VAL_PCT,
   SPA_CUST_COM, SPA_DISCOUNT_AMOUNT, SPA_TOTAL_AMOUNT
   ,CUSTOMER_DISC_AMOUNT,CUSTOMER_ADD1_DISC_AMOUNT,CUSTOMER_ADD2_DISC_AMOUNT,
   CUSTOMER_PRODUCT_DISC_AMOUNT,PROD_BONUS_PRICE_DISC_AMOUNT,LOADING_CHARGE_AMOUNT,
   INVOICE_ADJUSTMENT_AMOUNT,BONUS_PRICE_DISC_AMOUNT,NET_ORDER_AMOUNT,
   (Select NVL(SUM(PASSED_QTY),0) from VW_BATCH_WISE_STOCK WHERE COMPANY_ID= DT.COMPANY_ID and UNIT_ID = DT.UNIT_ID and  SKU_ID= DT.SKU_ID) CURRENT_STOCK, 0 SUGGESTED_QTY,
    FN_SKU_SHIPPER_DETAILS(SKU_ID,ORDER_QTY) SKU_SHIPPER_DETAILS

    from ORDER_DTL DT where DT.ORDER_MST_ID= :param1";
        string GetOrder_DTL_BY_Mst_Id_Query() => @"begin  :param1 := FN_ORDER_DTL(:param2); end;";
        string Delete_Order_Dtl_by_Id() => @"Delete From ORDER_DTL where ORDER_DTL_ID = :param1";
        string Order_Ready_For_Invoice_Count_Query() => @"Select Count(ORDER_MST_ID) Order_Count from ORDER_MST Where ORDER_STATUS = 'Active' and COMPANY_ID = :param1 and ORDER_UNIT_ID  = :param2";

        string  Get_Customer_Balance_Query() => @" Select  CUSTOMER_ID, CUSTOMER_CODE, sum(INVOICE_BALANCE) INVOICE_BALANCE, Sum(TDS_BALANCE) TDS_BALANCE from CUSTOMER_BALANCE_CENTRAL WHERE CUSTOMER_ID = :param1 and COMPANY_ID = :param2
 group by  CUSTOMER_ID, CUSTOMER_CODE";

        string Get_Customer_Credit_Limit_Query() => @"Select * from Credit_INFO Where STATUS = 'Active' AND CUSTOMER_ID = :param1 and 
COMPANY_ID = :param2  and 
TO_DATE(SYSDATE,'DD/MM/YYYY') between TO_DATE(EFFECT_START_DATE,'DD/MM/YYYY') and TO_DATE(EFFECT_END_DATE,'DD/MM/YYYY')";

        string Process_Order_Query() => @"
                      begin
                            PRC_PRE_ORDER_GENERATION(:param1 , TO_DATE(:param2,'DD/MM/YYYY HH:MI:SS AM') , :param3  ,:param4, :param5, :param6,:param7 );
                      end;";
        string Delete_Order_dtl_Query() => @"
                      begin

                            PRC_ORDER_DELETE(:param1);
                      end;";
        string Delete_Order_full_Query() => @" begin

                            PRC_ORDER_DELETE_FULL(:param1);
                      end;";
        string Update_Order_Process_Status() => @"
                     UPDATE ORDER_MST SET ORDER_PROCESS_STATUS = 'Processed' Where ORDER_MST_ID = :param1";
        string LoadProductData_Query() => @"Select
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
                              ,:param2 UNIT_ID
                              ,p.WEIGHT_PER_PACK
                              ,p.WEIGHT_UNIT
                              ,BP.BASE_PRODUCT_NAME
                              ,b.BRAND_NAME
                              ,g.GROUP_NAME
                              ,C.CATEGORY_NAME
                              ,0 UNIT_TP,
                               (Select NVL(SUM(PASSED_QTY),0) from VW_BATCH_WISE_STOCK WHERE COMPANY_ID= :param1 and UNIT_ID = :param2 and  SKU_ID= p.SKU_ID) CURRENT_STOCK, 0 SUGGESTED_QTY

                               from Product_Info p
                               left outer join Base_Product_Info bp on BP.BASE_PRODUCT_ID = P.BASE_PRODUCT_ID
                               left outer join Category_info c on C.CATEGORY_ID = P.CATEGORY_ID
                               left outer join Group_Info g on g.GROUP_ID = P.GROUP_ID
                               left outer join BRAND_INFO b on b.BRAND_ID = P.BRAND_ID
                                Where p.PRODUCT_STATUS = 'Active' and p.COMPANY_ID = :param1";

        string Invoice_Details_InvoiceComboGift_Query() => @"
          SELECT 
    ROWNUM ROW_NO, B.ORDER_MST_ID,
   B.GIFT_ITEM_ID, P.GIFT_ITEM_NAME,
   B.GIFT_QTY, 
   B.COMPANY_ID
FROM ORDER_COMBO_GIFT B
Left OUTER JOIN GIFT_ITEM_INFO P on P.GIFT_ITEM_ID = B.GIFT_ITEM_ID
WHERE B.ORDER_MST_ID = :param1";

        string Invoice_Details_InvoiceComboBonus_Query() => @"
      
    SELECT 
   ROWNUM ROW_NO, B.ORDER_MST_ID, B.SKU_ID, B.SKU_CODE, P.SKU_NAME,
   B.UNIT_TP,
   B.BONUS_QTY, B.BONUS_AMOUNT, B.COMPANY_ID
  
FROM ORDER_COMBO_BONUS B
LEFT OUTER JOIN PRODUCT_INFO P on P.SKU_ID = B.SKU_ID WHERE B.ORDER_MST_ID = :param1";
        string Invoice_Details_InvoiceBonus_Query() => @"
        
  SELECT ROWNUM ROW_NO, B.ORDER_DTL_ID,
    B.SKU_ID,  B.SKU_CODE, P.SKU_NAME,
    B.UNIT_TP, 
    B.BONUS_QTY,  B.BONUS_AMOUNT,  B.COMPANY_ID
  
FROM ORDER_BONUS B
LEFT OUTER JOIN PRODUCT_INFO P on P.SKU_ID = B.SKU_ID";




        string Invoice_Details_InvoiceGift_Query() => @"
 SELECT 
   ROWNUM ROW_NO,  B.ORDER_DTL_ID, P.GIFT_ITEM_NAME
  , B.GIFT_ITEM_ID, 
   B.GIFT_QTY, 
  B.COMPANY_ID

FROM ORDER_GIFT B
Left OUTER JOIN GIFT_ITEM_INFO P on P.GIFT_ITEM_ID = B.GIFT_ITEM_ID";
         string Replacement_Master_Query() => @" SELECT CLAIM_NO REPLACE_CLAIM_NO,
       CHALLAN_NUMBER,
       CUSTOMER_ID,
       CUSTOMER_CODE,
       MARKET_ID,
       MARKET_CODE,
       CHALLAN_DATE,
       RECEIVE_CATEGORY,
       UNIT_ID,
       REPLACE_PRODUCT_STATUS REFURBISHMENT_PRODUCT_STATUS
  FROM VIEW_REPLACEMENT_CLAIM
 WHERE     CUSTOMER_ID = :param1 
       AND NVL (CLAIM_NO, '') NOT IN (SELECT O.REPLACE_CLAIM_NO
                                        FROM ORDER_MST O
                                             LEFT OUTER JOIN INVOICE_MST I
                                                ON I.ORDER_NO = O.ORDER_NO
                                             LEFT OUTER JOIN RETURN_MST R
                                                ON R.INVOICE_NO =
                                                      I.INVOICE_NO
                                       WHERE     NVL (O.REPLACE_CLAIM_NO,
                                                      'V') != 'V'
                                             AND NVL (R.MST_ID, 0) = 0)
UNION
SELECT C.CLAIM_NO REPLACE_CLAIM_NO,
       C.CHALLAN_NUMBER,
       C.CUSTOMER_ID,
       C.CUSTOMER_CODE,
       C.MARKET_ID,
       C.MARKET_CODE,
       C.CHALLAN_DATE,
       C.RECEIVE_CATEGORY,
       C.UNIT_ID,
       C.REPLACE_PRODUCT_STATUS REFURBISHMENT_PRODUCT_STATUS
  FROM VIEW_REPLACEMENT_CLAIM C, ORDER_MST O
 WHERE O.REPLACE_CLAIM_NO = C.CLAIM_NO AND O.ORDER_MST_ID = :param2 ";
        string Replacement_Master_Query_Rupshi() => @" SELECT CLAIM_NO REPLACE_CLAIM_NO,
       CHALLAN_NUMBER,
       CUSTOMER_ID,
       CUSTOMER_CODE,
       MARKET_ID,
       MARKET_CODE,
       CHALLAN_DATE,
       RECEIVE_CATEGORY,
       UNIT_ID,
       REPLACE_PRODUCT_STATUS REFURBISHMENT_PRODUCT_STATUS
  FROM VIEW_REPLACEMENT_CLAIM_RUPSHI
WHERE     CUSTOMER_ID = :param1 AND UNIT_ID= :param2
       AND NVL (CLAIM_NO, '') NOT IN (SELECT O.REPLACE_CLAIM_NO
                                        FROM ORDER_MST O
                                             LEFT OUTER JOIN INVOICE_MST I
                                                ON I.ORDER_NO = O.ORDER_NO
                                             LEFT OUTER JOIN RETURN_MST R
                                                ON R.INVOICE_NO =
                                                      I.INVOICE_NO
                                       WHERE     NVL (O.REPLACE_CLAIM_NO,
                                                      'V') != 'V'
                                             AND NVL (R.MST_ID, 0) = 0)
UNION
SELECT C.CLAIM_NO REPLACE_CLAIM_NO,
       C.CHALLAN_NUMBER,
       C.CUSTOMER_ID,
       C.CUSTOMER_CODE,
       C.MARKET_ID,
       C.MARKET_CODE,
       C.CHALLAN_DATE,
       C.RECEIVE_CATEGORY,
       C.UNIT_ID,
       C.REPLACE_PRODUCT_STATUS REFURBISHMENT_PRODUCT_STATUS
  FROM VIEW_REPLACEMENT_CLAIM_RUPSHI C, ORDER_MST O
WHERE O.REPLACE_CLAIM_NO = C.CLAIM_NO AND O.ORDER_MST_ID = :param3";

        string Replacement_Dtl_Query() => @"
                            Select
                               ROW_NUMBER() OVER(ORDER BY p.SKU_ID ASC) AS ROW_NO,
                               CLAIM_NO REPLACEMENT_CLAIM_NO
                              ,0 PACK_SIZE
                              ,0 PACK_UNIT
                              ,0 PACK_VALUE
                              ,SKU_STATUS PRODUCT_STATUS
                              ,0 QTY_PER_PACK
                              ,0 SHIPPER_QTY
                              ,0 SHIPPER_VOLUME
                              ,'' SHIPPER_VOLUME_UNIT
                              ,0 SHIPPER_WEIGHT
                              ,'' SHIPPER_WEIGHT_UNIT
                              ,SKU_CODE
                              ,SKU_ID
                              ,SKU_NAME
                              ,'' SKU_NAME_BANGLA
                              ,0 UNIT_ID
                              ,0 WEIGHT_PER_PACK
                              ,''  WEIGHT_UNIT
                              ,REPLACE_QTY ORDER_QTY
                              ,0 UNIT_TP
                              ,0 CURRENT_STOCK
                              ,0 SUGGESTED_QTY
                              ,0 MAXIMUM_QTY
                              ,0 MINIMUM_QTY
                              ,0 REMAINING_QTY
                              ,0 TARGET_QTY
                              ,0 IMS_QTY
                              ,0 DISTRIBUTOR_STOCK
                              ,0 ORDER_AMOUNT
                               from VIEW_REPLACEMENT_CLAIM_SKU p
                              Where CLAIM_NO = :param1";

        string Replacement_Dtl_Query_rupshi() => @"
                            Select
                               ROW_NUMBER() OVER(ORDER BY p.SKU_ID ASC) AS ROW_NO,
                               CLAIM_NO REPLACEMENT_CLAIM_NO
                              ,0 PACK_SIZE
                              ,0 PACK_UNIT
                              ,0 PACK_VALUE
                              ,SKU_STATUS PRODUCT_STATUS
                              ,0 QTY_PER_PACK
                              ,0 SHIPPER_QTY
                              ,0 SHIPPER_VOLUME
                              ,'' SHIPPER_VOLUME_UNIT
                              ,0 SHIPPER_WEIGHT
                              ,'' SHIPPER_WEIGHT_UNIT
                              ,SKU_CODE
                              ,SKU_ID
                              ,SKU_NAME
                              ,'' SKU_NAME_BANGLA
                              ,0 UNIT_ID
                              ,0 WEIGHT_PER_PACK
                              ,''  WEIGHT_UNIT
                              ,REPLACE_QTY ORDER_QTY
                              ,0 UNIT_TP
                              ,0 CURRENT_STOCK
                              ,0 SUGGESTED_QTY
                              ,0 MAXIMUM_QTY
                              ,0 MINIMUM_QTY
                              ,0 REMAINING_QTY
                              ,0 TARGET_QTY
                              ,0 IMS_QTY
                              ,0 DISTRIBUTOR_STOCK
                              ,0 ORDER_AMOUNT
                               from VIEW_REPLACE_CLAIM_SKU_RUPSHI p
                              Where CLAIM_NO = :param1";


        string Refurbishment_Master_Query() => @"Select Distinct CLAIM_NO REFURBISHMENT_CLAIM_NO,'' CHALLAN_NUMBER, CUSTOMER_ID, CUSTOMER_CODE,
      '' REFURBISHMENT_PRODUCT_STATUS from VW_PENDING_FOR_REFURBISHMENT Where  CUSTOMER_ID = :param1 AND :param2  = :param2  ";
        string Refurbishment_Dtl_Query() => @"
                            Select
                               ROW_NUMBER() OVER(ORDER BY p.SKU_ID ASC) AS ROW_NO,
                               CLAIM_NO REFURBISHMENT_CLAIM_NO
                              ,0 PACK_SIZE
                              ,0 PACK_UNIT
                              ,0 PACK_VALUE
                              ,I.PRODUCT_STATUS PRODUCT_STATUS
                              ,0 QTY_PER_PACK
                              ,0 SHIPPER_QTY
                              ,0 SHIPPER_VOLUME
                              ,'' SHIPPER_VOLUME_UNIT
                              ,0 SHIPPER_WEIGHT
                              ,'' SHIPPER_WEIGHT_UNIT
                              ,SKU_CODE
                              ,P.SKU_ID
                              ,SKU_NAME
                              ,'' SKU_NAME_BANGLA
                              ,0 UNIT_ID
                              ,0 WEIGHT_PER_PACK
                              ,''  WEIGHT_UNIT
                              ,PROD_QTY ORDER_QTY
                              ,0 UNIT_TP
                              ,0 CURRENT_STOCK
                              ,0 SUGGESTED_QTY
                              ,0 MAXIMUM_QTY
                              ,0 MINIMUM_QTY
                              ,0 REMAINING_QTY
                              ,0 TARGET_QTY
                              ,0 IMS_QTY
                              ,0 DISTRIBUTOR_STOCK
                              ,0 ORDER_AMOUNT
                               from VW_PENDING_FOR_REFURBISHMENT p, PRODUCT_INFO I
                              Where I.SKU_ID = p.SKU_ID AND CLAIM_NO = :param1";

        //---------- Method Execution Part ------------------------------------------------
        public async Task<string> GetProductDataFiltered(string db, int Company_Id, ProductFilterParameters parameters,int Unit_Id)
        {
            string Query =string.Format(@"Select
                               ROW_NUMBER() OVER(ORDER BY p.SKU_ID ASC) AS ROW_NO
                              ,p.COMPANY_ID
                              ,p.PACK_SIZE
                              ,p.PACK_UNIT
                              ,p.PACK_VALUE
                              ,p.PRODUCT_STATUS
                              ,p.QTY_PER_PACK
                              ,p.SHIPPER_QTY
                              ,p.SHIPPER_VOLUME
                              ,p.SHIPPER_VOLUME_UNIT
                              ,p.SHIPPER_WEIGHT
                              ,p.SHIPPER_WEIGHT_UNIT
                              ,p.SKU_CODE
                              ,p.SKU_ID
                              ,p.SKU_NAME
                              ,p.SKU_NAME_BANGLA
                              ,0 UNIT_ID
                              ,p.WEIGHT_PER_PACK
                              ,p.WEIGHT_UNIT 
                              ,FN_SKU_SELECT_PRICE('"+parameters.ORDER_TYPE+"',"+parameters.CUSTOMER_ID+","+@"p.SKU_ID,P.SKU_CODE,"+Company_Id.ToString()+","+Unit_Id.ToString()+@") UNIT_TP
                              ,(Select NVL(SUM(PASSED_QTY),0) from VW_BATCH_WISE_STOCK WHERE COMPANY_ID= :param1 and UNIT_ID = " + Unit_Id.ToString() + @" and  SKU_ID= p.SKU_ID) CURRENT_STOCK
                              ,0 SUGGESTED_QTY
                              ,0 MAXIMUM_QTY
                              ,0 MINIMUM_QTY
                              ,0 REMAINING_QTY
                              ,FN_CUSTOMER_SKU_TARGET(1,"+parameters.CUSTOMER_ID.ToString() + @",p.SKU_ID) TARGET_QTY
                              ,FN_CUSTOMER_L15D_SKU_IMS(1,"+parameters.CUSTOMER_ID.ToString() + @",p.SKU_ID) IMS_QTY
                              ,NVL(FN_CUSTOMER_STOCK("+parameters.CUSTOMER_CODE + @",p.SKU_CODE),0) DISTRIBUTOR_STOCK
                              ,NVL(FN_CURRENT_LIFTING_QTY(TRUNC(SYSDATE), p.SKU_ID, {0}, {1},{2}),0) CURRENT_LIFTING
                              ,NVL(FN_CUSTOMER_TRANSIT_STOCK("+parameters.CUSTOMER_CODE + @",p.SKU_CODE),0) TRANSIT_STOCK
                               from Product_Info p
                               left outer join Base_Product_Info bp on BP.BASE_PRODUCT_ID = P.BASE_PRODUCT_ID
                               left outer join Category_info c on C.CATEGORY_ID = P.CATEGORY_ID
                               left outer join Group_Info g on g.GROUP_ID = P.GROUP_ID
                               left outer join BRAND_INFO b on b.BRAND_ID = P.BRAND_ID
                               Where p.PRODUCT_STATUS = 'Active' and p.COMPANY_ID = :param1", Company_Id,Unit_Id, parameters.CUSTOMER_ID);

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
                            if(_SKU_ID == "")
                            {
                                _SKU_ID = parameters.SKU_ID[i];

                            }
                            else
                            {
                                _SKU_ID = _SKU_ID + "," + parameters.SKU_ID[i];

                            }

                        }
                    }

                }
                if (_SKU_ID != "")
                {
                    Query = Query + " AND  p.SKU_ID in (" + _SKU_ID + ")";

                }
            }

            DataTable dt = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Query, _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
            string suggested =  string.Format(@"Select NVL(SUGGEST_PERCENT, 70) SUGGEST_PERCENT,NVL(MAXIMUM_PERCENT,100) MAXIMUM_PERCENT,NVL(MINIMUM_PERCENT,50) MINIMUM_PERCENT  from CUSTOMER_INFO where CUSTOMER_ID = {0}", parameters.CUSTOMER_ID);

            DataTable dt_percent = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), suggested, _commonServices.AddParameter(new string[] { Company_Id.ToString() }));


            decimal sgst_qty = 0, min_qty = 0, max_qty = 0, remaining_qty = 0, dist_stock = 0, target_qty = 0, ims_qty = 0, crnt_stock = 0, currrent_lifting = 0;
            decimal suggest_percent = Convert.ToDecimal(dt_percent.Rows[0]["SUGGEST_PERCENT"]), max_percent = Convert.ToDecimal(dt_percent.Rows[0]["MAXIMUM_PERCENT"]), min_percent = Convert.ToDecimal(dt_percent.Rows[0]["MINIMUM_PERCENT"]);

            for (int i = 0; i < dt.Rows.Count; i++)
            {

                target_qty = Convert.ToDecimal(dt.Rows[i]["TARGET_QTY"]);
                ims_qty = Convert.ToDecimal(dt.Rows[i]["IMS_QTY"]);
                crnt_stock = Convert.ToDecimal(dt.Rows[i]["CURRENT_STOCK"]);
                dist_stock = Convert.ToDecimal(dt.Rows[i]["DISTRIBUTOR_STOCK"]);
                currrent_lifting = Convert.ToDecimal(dt.Rows[i]["CURRENT_LIFTING"]);

                sgst_qty = (target_qty * suggest_percent) / 100 - dist_stock;
                min_qty = (target_qty * min_percent) / 100 - dist_stock;
                max_qty = (target_qty * max_percent ) / 100 - dist_stock;
                remaining_qty = target_qty - currrent_lifting;
                dt.Rows[i]["SUGGESTED_QTY"] = sgst_qty;
                dt.Rows[i]["MAXIMUM_QTY"] = max_qty;
                dt.Rows[i]["MINIMUM_QTY"] = min_qty;
                dt.Rows[i]["REMAINING_QTY"] = remaining_qty;
            }

            return _commonServices.DataTableToJSON(dt);
        }

        public async Task<string> AddOrUpdate(string db, Order_Mst model)
        {
            if (model == null)
            {
                return "No data provided to insert!!!!";
            }
            else
            {
                Result result = new Result();

                if ((model.IsDistributor == "True" || model.IsOSM == "True") && model.FINAL_SUBMISSION_STATUS == "Complete")
                {
                        result.Key = "0";
                        result.Status = "Can not update Order after final posting";
                        return JsonSerializer.Serialize(result);
                }
                //if (model.IsDistributor != "True" && model.FINAL_SUBMIT_CONFIRM_STATUS == "Complete")
                //{
                //        result.Key = "0";
                //        result.Status = "Can not update Order after final posting";
                //        return JsonSerializer.Serialize(result);

                //}
                bool status_action = true;
                string dtl_list = "";

                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                try
                {
                    int detail_id = 0;
                    int detail_id_log = 0;


                    if (model.ORDER_MST_ID == 0)
                    {
                        //-------------Add to master table--------------------
                        model.ORDER_MST_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewOrder_MST_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        model.ORDER_NO = _commonServices.GenerateRequisitionCode(_configuration.GetConnectionString(db), "FN_GENERATE_SALES_ORDER_NO", model.COMPANY_ID.ToString(), model.UNIT_ID.ToString());

                        //_commonServices.GetMaximumNumber<string>(_configuration.GetConnectionString(db), GetNewOrder_MST_NoQuery(), _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString() })).ToString();

                        detail_id_log = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewOrder_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        
                        if (model.ORDER_NO == "")
                        {
                            model.ORDER_NO = "1";

                        }

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Master_AddQuery(),
                            _commonServices.AddParameter(new string[] {
                                model.ORDER_MST_ID.ToString(), model.ORDER_NO.ToString(), model.ORDER_DATE, model.ORDER_TYPE,
                                model.DELIVERY_DATE, model.CUSTOMER_ID.ToString(), model.CUSTOMER_CODE, model.MARKET_ID.ToString(), 
                                model.TERRITORY_ID.ToString(), model.AREA_ID.ToString(), model.REGION_ID.ToString(),
                                model.DIVISION_ID.ToString(),model.PAYMENT_MODE,model.REPLACE_CLAIM_NO,model.BONUS_PROCESS_NO,
                                model.BONUS_CLAIM_NO,model.INVOICE_STATUS, model.ORDER_AMOUNT.ToString(), model.ORDER_STATUS
                            , model.COMPANY_ID.ToString(), model.ORDER_UNIT_ID.ToString(), model.ORDER_UNIT_ID.ToString(), model.REMARKS, model.SPA_TOTAL_AMOUNT.ToString()
                            , model.SPA_COMMISSION_PCT.ToString(), model.SPA_COMMISSION_AMOUNT.ToString(), model.SPA_NET_AMOUNT.ToString(), model.ENTERED_BY,
                             model.ENTERED_DATE, model.ENTERED_TERMINAL, model.ORDER_ENTRY_TYPE,model.REFURBISHMENT_CLAIM_NO
                            })));
                        
                        //-------------Add to detail table--------------------

                        foreach (var item in model.Order_Dtls)
                        {
                            detail_id = 0;
                            listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Detail_AddQuery(),
                                _commonServices.AddParameter(new string[] {
                                    detail_id.ToString(), model.ORDER_MST_ID.ToString(), item.SKU_ID.ToString(), item.SKU_CODE, item.ORDER_QTY.ToString(),
                                     item.UNIT_TP.ToString(), item.ORDER_AMOUNT.ToString(), item.STATUS, model.COMPANY_ID.ToString(),
                                    item.UNIT_ID.ToString(), item.REMARKS,item.SPA_UNIT_TP.ToString(),item.SPA_AMOUNT.ToString() ,
                                    item.SPA_REQ_TIME_STOCK.ToString(),item.SPA_DISCOUNT_TYPE.ToString(),item.SPA_DISCOUNT_VAL_PCT.ToString()
                                    ,item.SPA_CUST_COM.ToString(),item.SPA_DISCOUNT_AMOUNT.ToString(), item.SPA_TOTAL_AMOUNT.ToString(),
                                    model.ENTERED_BY,model.ENTERED_DATE,model.ENTERED_TERMINAL
                                })));
                            dtl_list += "," + ++detail_id_log;

                        }
                        status_action = false;

                    }
                    else
                    {
                        Order_Mst _Mst = JsonSerializer.Deserialize<Order_Mst>(await this.GetEditDatabyIdProcessCheck(db, model.ORDER_MST_ID));
                       
                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_Master_UpdateQuery(),
                            _commonServices.AddParameter(new string[] { model.ORDER_MST_ID.ToString(),
                                model.ORDER_TYPE.ToString(), model.DELIVERY_DATE, model.CUSTOMER_ID.ToString(), model.CUSTOMER_CODE
                               , model.MARKET_ID.ToString(), model.TERRITORY_ID.ToString(), model.AREA_ID.ToString(),
                                model.REGION_ID.ToString(), model.DIVISION_ID.ToString(),
                               model.PAYMENT_MODE, model.REPLACE_CLAIM_NO,
                               model.BONUS_PROCESS_NO, model.BONUS_CLAIM_NO,
                               model.INVOICE_STATUS, model.ORDER_AMOUNT.ToString(),
                               model.ORDER_STATUS, model.ORDER_UNIT_ID.ToString(),
                               model.INVOICE_UNIT_ID.ToString(), model.REMARKS,
                               model.UPDATED_BY, model.UPDATED_DATE,
                               model.UPDATED_TERMINAL.ToString(),

                               })));


                        foreach (var item in model.Order_Dtls)
                        {
                            if (item.ORDER_DTL_ID == 0)
                            {
                                //-------------Add new row on detail table--------------------

                                detail_id = 0;
                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Detail_AddQuery(),
                                                               _commonServices.AddParameter(new string[] {
                                    detail_id.ToString(), model.ORDER_MST_ID.ToString(), item.SKU_ID.ToString(), item.SKU_CODE, item.ORDER_QTY.ToString(),
                                    item.UNIT_TP.ToString(), item.ORDER_AMOUNT.ToString(), item.STATUS, model.COMPANY_ID.ToString(),
                                    item.UNIT_ID.ToString(), item.REMARKS,item.SPA_UNIT_TP.ToString(),item.SPA_AMOUNT.ToString() ,
                                    item.SPA_REQ_TIME_STOCK.ToString(),item.SPA_DISCOUNT_TYPE.ToString(),item.SPA_DISCOUNT_VAL_PCT.ToString()
                                    ,item.SPA_CUST_COM.ToString(),item.SPA_DISCOUNT_AMOUNT.ToString(), item.SPA_TOTAL_AMOUNT.ToString(),
                                    model.ENTERED_BY,model.ENTERED_DATE,model.ENTERED_TERMINAL
                                  })));

                                dtl_list += "," + ++detail_id_log;

                            }
                            else
                            {
                                //-------------Edit on detail table--------------------
                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Detail_UpdateQuery(),
                                    _commonServices.AddParameter(new string[] { 
                                        item.ORDER_DTL_ID.ToString(), item.SKU_ID.ToString(),
                                        item.SKU_CODE, item.ORDER_QTY.ToString(),
                                        item.UNIT_TP.ToString(), item.ORDER_AMOUNT.ToString(), 
                                        item.STATUS,item.REMARKS,
                                        model.UPDATED_BY.ToString(), model.UPDATED_DATE, model.UPDATED_TERMINAL })));
                                dtl_list += "," + item.ORDER_DTL_ID;

                            }

                        }
                        foreach (var item in _Mst.Order_Dtls)
                        {
                            bool status = true;

                            foreach (var updateditem in model.Order_Dtls)
                            {
                                if (item.ORDER_DTL_ID == updateditem.ORDER_DTL_ID)
                                {
                                    status = false;
                                }
                            }
                            if (status)
                            {
                                //-------------Delete row from detail table--------------------

                                listOfQuery.Add(_commonServices.AddQuery(Delete_Order_Dtl_by_Id(), _commonServices.AddParameter(new string[] { item.ORDER_DTL_ID.ToString() })));
                                dtl_list += ",Deleted: " + item.ORDER_DTL_ID;

                            }

                        }


                    }
                    
                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);

                    string st = status_action == false ? "Add" : "Edit";
                    await _logManager.AddOrUpdate(model.db_security, st, "ORDER_MST", model.COMPANY_ID, model.UNIT_ID, Convert.ToInt32(model.ENTERED_BY), model.ENTERED_TERMINAL, "/SalesAndDistribution/SalesOrder/InsertOrEdit", model.ORDER_MST_ID, dtl_list);

                    ////Notification Add---------------
                    //var Pending_Invoice_Count = await _commonServices.GetMaximumNumberAsyn<int>(_configuration.GetConnectionString(model.db_sales), Order_Ready_For_Invoice_Count_Query(), _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), model.UNIT_ID.ToString() }));

                    //await _NotificationManager.AddOrderNotification(model.db_security,model.db_sales, 1, "New Order " + model.ORDER_NO + " Has been added/Edited and ready for Invoice. Order Ready for Invoice Count: " + Pending_Invoice_Count, model.COMPANY_ID, model.UNIT_ID);
                    result.Key = model.ORDER_MST_ID.ToString();
                    result.Status = _commonServices.Encrypt(model.ORDER_MST_ID.ToString());
                    return JsonSerializer.Serialize(result);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
        public async Task<string> LoadData( Order_Mst model)
        {
            DataTable dataTable = new DataTable();
            if (model.IsDistributor == "True")
            {
                
                dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(model.db_sales), LoadCustomerOrderData_Query(), _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), model.CUSTOMER_CODE.ToString() }));

            }
            else if (model.IsOSM == "True")
            {
                string Query = LoadOsmOrderData_Query();
                Query += string.Format(" AND O.CUSTOMER_ID in (Select ACCOUNT_ID from USER_ACCOUNT_RELATION_MST UM LEFT OUTER JOIN USER_ACCOUNT_RELATION_DTL UD on UD.USER_ACCOUNT_MST_ID = UM.USER_ACCOUNT_MST_ID WHERE UM.USER_TYPE = 'OSM' AND  UM.USER_ID = {0})", model.ENTERED_BY);
               dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(model.db_sales), Query, _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), model.CUSTOMER_CODE.ToString() }));

            }
            else
            {
                string Query = LoadData_Query();
                if(model.USER_TYPE != null)
                {
                    if(model.USER_TYPE == "DSM")
                    {
                        Query += string.Format(" AND DSM_CONFIRM_STATUS = 'Pending' and  :param2 = :param2 AND O.CUSTOMER_ID in (Select ACCOUNT_ID from USER_ACCOUNT_RELATION_MST UM LEFT OUTER JOIN USER_ACCOUNT_RELATION_DTL UD on UD.USER_ACCOUNT_MST_ID = UM.USER_ACCOUNT_MST_ID WHERE UM.USER_TYPE = 'DSM' AND  UM.USER_ID = {0})", model.ENTERED_BY);
                    }
                    else if (model.USER_TYPE == "SM")
                    {
                        Query += " AND SM_CONFIRM_STATUS = 'Pending' and :param2 = :param2 ";
                    }
                    else if (model.USER_TYPE == "HOS")
                    {
                        Query += " AND HOS_CONFIRM_STATUS = 'Pending' and :param2 = :param2";
                    }
                    else
                    {
                        Query += " and O.ORDER_UNIT_ID = :param2";
                    }

                }
                 dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(model.db_sales), Query, _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), model.UNIT_ID.ToString() }));

            }
            List<Order_Mst> bonus_Msts = new List<Order_Mst>();
            if (dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Order_Mst _bonus_Mst = new Order_Mst()   ; 
                    _bonus_Mst.ROW_NO = Convert.ToInt32(dataTable.Rows[i]["ROW_NO"]);
                    _bonus_Mst.CUSTOMER_ID = Convert.ToInt32(dataTable.Rows[i]["CUSTOMER_ID"]);
                    _bonus_Mst.MARKET_ID = Convert.ToInt32(dataTable.Rows[i]["MARKET_ID"]);

                    _bonus_Mst.ORDER_MST_ID = Convert.ToInt32(dataTable.Rows[i]["ORDER_MST_ID"]);
                    _bonus_Mst.ORDER_NO = dataTable.Rows[i]["ORDER_NO"].ToString();
                    if (model.IsDistributor == "True")
                    {
                        _bonus_Mst.INVOICE_NO = dataTable.Rows[i]["INVOICE_NO"].ToString();
                        _bonus_Mst.INVOICE_DATE = dataTable.Rows[i]["INVOICE_DATE"].ToString();

                    }
                    _bonus_Mst.INVOICE_TYPE_NAME = dataTable.Rows[i]["INVOICE_TYPE_NAME"].ToString();
                    _bonus_Mst.ORDER_STATUS = dataTable.Rows[i]["ORDER_STATUS"].ToString();
                    _bonus_Mst.ORDER_DATE = dataTable.Rows[i]["ORDER_DATE"].ToString();
                    _bonus_Mst.ORDER_TYPE = dataTable.Rows[i]["ORDER_TYPE"].ToString();
                    _bonus_Mst.DELIVERY_DATE = dataTable.Rows[i]["DELIVERY_DATE"].ToString();
                    _bonus_Mst.CUSTOMER_CODE = dataTable.Rows[i]["CUSTOMER_CODE"].ToString();
                    _bonus_Mst.MARKET_NAME = dataTable.Rows[i]["MARKET_NAME"].ToString();
                    _bonus_Mst.ORDER_ENTRY_TYPE = dataTable.Rows[i]["ORDER_ENTRY_TYPE"].ToString();
                    _bonus_Mst.CUSTOMER_NAME = dataTable.Rows[i]["CUSTOMER_NAME"].ToString();
                    _bonus_Mst.ORDER_AMOUNT = Convert.ToInt32(dataTable.Rows[i]["ORDER_AMOUNT"]);
                    _bonus_Mst.ORDER_AMOUNT = Convert.ToInt32(dataTable.Rows[i]["ORDER_AMOUNT"]);
                    _bonus_Mst.FINAL_SUBMISSION_STATUS =dataTable.Rows[i]["FINAL_SUBMISSION_STATUS"].ToString();
                    _bonus_Mst.ORDER_UNIT_ID = Convert.ToInt32(dataTable.Rows[i]["ORDER_UNIT_ID"].ToString());


                    _bonus_Mst.ORDER_MST_ID_ENCRYPTED = _commonServices.Encrypt(_bonus_Mst.ORDER_MST_ID.ToString());
                    bonus_Msts.Add(_bonus_Mst);
                }
            }
           
            await _logManager.AddOrUpdate(model.db_security, "View", "ORDER_MST", model.COMPANY_ID, model.UNIT_ID, Convert.ToInt32(model.ENTERED_BY), model.ENTERED_TERMINAL, "/SalesAndDistribution/Division/DivisionInfo", 0, "");
          
            
         
            return JsonSerializer.Serialize(bonus_Msts);
        }

        public async Task<string> LoadMonitoringData(Order_Mst model)
        {   if(model.IsDistributor== "True")
            {
                var dt = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(model.db_sales), LoadDistributorOrderMonitoringData_Query(), _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), model.CUSTOMER_CODE }));
                return _commonServices.DataTableToJSON(dt);
            }
            else
            {
                var dt = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(model.db_sales), LoadCustomerOrderMonitoringData_Query(), _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString() }));
                return _commonServices.DataTableToJSON(dt);
            }

        }

        public async Task<string> LoadDataProcessed( Order_Mst model)
        {
            DataTable dataTable = new DataTable();
            if (model.IsDistributor == "True")
            {
                 dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(model.db_sales), LoadDataProcessedCustomer_Query(), _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), model.CUSTOMER_CODE.ToString() }));
            }
            else if (model.IsOSM == "True")
            {
                string Query = LoadDataProcessedCustomer_Query();
                Query += string.Format(" AND O.CUSTOMER_ID in (Select ACCOUNT_ID from USER_ACCOUNT_RELATION_MST UM LEFT OUTER JOIN USER_ACCOUNT_RELATION_DTL UD on UD.USER_ACCOUNT_MST_ID = UM.USER_ACCOUNT_MST_ID WHERE UM.USER_TYPE = 'OSM' AND  UM.USER_ID = {0})", model.ENTERED_BY);
                dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(model.db_sales), Query, _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), model.CUSTOMER_CODE.ToString() }));

            }
            else
            {
                dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(model.db_sales), LoadDataProcessed_Query(), _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), model.UNIT_ID.ToString() }));
            }
            List<Order_Mst> bonus_Msts = new List<Order_Mst>();
            if (dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Order_Mst _bonus_Mst = new Order_Mst()   ; 
                    _bonus_Mst.ROW_NO = Convert.ToInt32(dataTable.Rows[i]["ROW_NO"]);
                    _bonus_Mst.CUSTOMER_ID = Convert.ToInt32(dataTable.Rows[i]["CUSTOMER_ID"]);
                    _bonus_Mst.MARKET_ID = Convert.ToInt32(dataTable.Rows[i]["MARKET_ID"]);

                    _bonus_Mst.ORDER_MST_ID = Convert.ToInt32(dataTable.Rows[i]["ORDER_MST_ID"]);
                    _bonus_Mst.ORDER_NO = dataTable.Rows[i]["ORDER_NO"].ToString();
                    _bonus_Mst.ORDER_STATUS = dataTable.Rows[i]["ORDER_STATUS"].ToString();

                    _bonus_Mst.ORDER_DATE = dataTable.Rows[i]["ORDER_DATE"].ToString();
                    _bonus_Mst.ORDER_TYPE = dataTable.Rows[i]["ORDER_TYPE"].ToString();
                    _bonus_Mst.DELIVERY_DATE = dataTable.Rows[i]["DELIVERY_DATE"].ToString();
                    _bonus_Mst.CUSTOMER_CODE = dataTable.Rows[i]["CUSTOMER_CODE"].ToString();
                    _bonus_Mst.MARKET_NAME = dataTable.Rows[i]["MARKET_NAME"].ToString();
                    _bonus_Mst.ORDER_ENTRY_TYPE = dataTable.Rows[i]["ORDER_ENTRY_TYPE"].ToString();
                    _bonus_Mst.CUSTOMER_NAME = dataTable.Rows[i]["CUSTOMER_NAME"].ToString();
                    _bonus_Mst.ORDER_AMOUNT = Convert.ToInt32(dataTable.Rows[i]["ORDER_AMOUNT"]);

                    _bonus_Mst.ORDER_MST_ID_ENCRYPTED = _commonServices.Encrypt(_bonus_Mst.ORDER_MST_ID.ToString());
                    bonus_Msts.Add(_bonus_Mst);
                }
            }
           
            //await _logManager.AddOrUpdate(model.db_security, "View", "ORDER_MST", model.COMPANY_ID, model.UNIT_ID, Convert.ToInt32(model.ENTERED_BY), model.ENTERED_TERMINAL, "/SalesAndDistribution/Division/DivisionInfo", 0, "");
          
            
         
            return JsonSerializer.Serialize(bonus_Msts);
        }
        public async Task<string> LoadFilteredData(OrderSKUFilterParameters model)
        {
            string Query = LoadFilteredData_Query();
            if(model.DIVISION_ID != null  && model.DIVISION_ID!="")
            {
                Query += string.Format("  AND O.DIVISION_ID = {0}", model.DIVISION_ID);
            } 
            if(model.REGION_ID != null && model.REGION_ID!="")
            {
                Query += string.Format("  AND O.REGION_ID = {0}", model.REGION_ID);

            }
            if (model.AREA_ID != null &&  model.AREA_ID != "")
            {
                Query += string.Format("  AND O.AREA_ID = {0}", model.AREA_ID);

            }
            if (model.TERRITORY_ID != null && model.TERRITORY_ID != "")
            {
                Query += string.Format("  AND O.TERRITORY_ID = {0}", model.TERRITORY_ID);

            }
            if (model.CUSTOMER_ID != null && model.CUSTOMER_ID != "")
            {
                Query += string.Format("  AND O.CUSTOMER_ID = {0}", model.CUSTOMER_ID);

            }
            if (model.ORDER_TYPE != null && model.ORDER_TYPE != "")
            {
                Query += string.Format("  AND O.ORDER_TYPE = '{0}'", model.ORDER_TYPE);

            }
            if (model.ORDER_STATUS != null && model.ORDER_STATUS != "")
            {
                Query += string.Format("  AND O.ORDER_STATUS = '{0}'", model.ORDER_STATUS);

            }
            if (model.ORDER_ENTRY_TYPE != null && model.ORDER_ENTRY_TYPE != "")
            {
                Query += string.Format("  AND O.ORDER_ENTRY_TYPE = '{0}'", model.ORDER_ENTRY_TYPE);

            }
            if ((model.DATE_FROM != null && model.DATE_FROM != "") && (model.DATE_TO != null || model.DATE_TO != ""))
            {
                Query += string.Format("  AND  O.ORDER_DATE between TO_DATE('{0}','DD/MM/YYYY') AND TO_DATE('{1}','DD/MM/YYYY')+1", model.DATE_FROM,model.DATE_TO);

            }
            if (model.USER_TYPE != null)
            {
                if (model.USER_TYPE == "DSM")
                {
                    Query += string.Format(" AND DSM_CONFIRM_STATUS = 'Pending' and  :param2 = :param2 AND O.CUSTOMER_ID in (Select ACCOUNT_ID from USER_ACCOUNT_RELATION_MST UM LEFT OUTER JOIN USER_ACCOUNT_RELATION_DTL UD on UD.USER_ACCOUNT_MST_ID = UM.USER_ACCOUNT_MST_ID WHERE UM.USER_TYPE = 'DSM' AND  UM.USER_ID = {0})", model.ENTERED_BY);
                }
                else if (model.USER_TYPE == "SM")
                {
                    Query += " AND SM_CONFIRM_STATUS = 'Pending' and :param2 = :param2 ";
                }
                else if (model.USER_TYPE == "HOS")
                {
                    Query += " AND HOS_CONFIRM_STATUS = 'Pending' and :param2 = :param2";
                }
                else if(model.USER_TYPE == "Distributor")
                {
                    Query += string.Format(" AND O.CUSTOMER_CODE = '{0}' and :param2 = :param2",model.CUSTOMER_CODE);

                }
                else if (model.USER_TYPE == "OSM")
                {
                    Query += string.Format(" AND :param2 = :param2 AND O.CUSTOMER_ID in (Select ACCOUNT_ID from USER_ACCOUNT_RELATION_MST UM LEFT OUTER JOIN USER_ACCOUNT_RELATION_DTL UD on UD.USER_ACCOUNT_MST_ID = UM.USER_ACCOUNT_MST_ID WHERE UM.USER_TYPE = 'OSM' AND  UM.USER_ID = {0})", model.ENTERED_BY);

                }
                else
                {
                    Query += " and O.ORDER_UNIT_ID = :param2";
                }
               
            }
            Query += string.Format(" ORDER BY O.ORDER_DATE DESC");
            DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(model.db_sales), Query, _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), model.UNIT_ID.ToString() }));
            List<Order_Mst> bonus_Msts = new List<Order_Mst>();
            if (dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Order_Mst _bonus_Mst = new Order_Mst();
                    _bonus_Mst.ROW_NO = Convert.ToInt32(dataTable.Rows[i]["ROW_NO"]);
                    _bonus_Mst.CUSTOMER_ID = Convert.ToInt32(dataTable.Rows[i]["CUSTOMER_ID"]);
                    _bonus_Mst.MARKET_ID = Convert.ToInt32(dataTable.Rows[i]["MARKET_ID"]);

                    _bonus_Mst.ORDER_MST_ID = Convert.ToInt32(dataTable.Rows[i]["ORDER_MST_ID"]);
                    _bonus_Mst.ORDER_NO = dataTable.Rows[i]["ORDER_NO"].ToString();
                    _bonus_Mst.INVOICE_TYPE_NAME = dataTable.Rows[i]["INVOICE_TYPE_NAME"].ToString();
                    _bonus_Mst.ORDER_STATUS = dataTable.Rows[i]["ORDER_STATUS"].ToString();

                    _bonus_Mst.ORDER_DATE = dataTable.Rows[i]["ORDER_DATE"].ToString();
                    _bonus_Mst.ORDER_TYPE = dataTable.Rows[i]["ORDER_TYPE"].ToString();
                    _bonus_Mst.DELIVERY_DATE = dataTable.Rows[i]["DELIVERY_DATE"].ToString();
                    _bonus_Mst.CUSTOMER_CODE = dataTable.Rows[i]["CUSTOMER_CODE"].ToString();
                    _bonus_Mst.MARKET_NAME = dataTable.Rows[i]["MARKET_NAME"].ToString();
                    _bonus_Mst.ORDER_ENTRY_TYPE = dataTable.Rows[i]["ORDER_ENTRY_TYPE"].ToString();
                    _bonus_Mst.CUSTOMER_NAME = dataTable.Rows[i]["CUSTOMER_NAME"].ToString();
                    _bonus_Mst.ORDER_AMOUNT = Convert.ToDecimal(dataTable.Rows[i]["ORDER_AMOUNT"]);
                    _bonus_Mst.FINAL_SUBMISSION_STATUS = dataTable.Rows[i]["FINAL_SUBMISSION_STATUS"].ToString();
                    _bonus_Mst.ORDER_UNIT_ID = Convert.ToInt32(dataTable.Rows[i]["ORDER_UNIT_ID"].ToString());

                    _bonus_Mst.ORDER_MST_ID_ENCRYPTED = _commonServices.Encrypt(_bonus_Mst.ORDER_MST_ID.ToString());
                    bonus_Msts.Add(_bonus_Mst);
                }
            }

            await _logManager.AddOrUpdate(model.db_security, "View", "ORDER_MST", model.COMPANY_ID, model.UNIT_ID, Convert.ToInt32(model.ENTERED_BY), model.ENTERED_TERMINAL, "/SalesAndDistribution/Division/DivisionInfo", 0, "");



            return JsonSerializer.Serialize(bonus_Msts);
        }        
        public async Task<string> LoadFilteredMonitoringData(OrderSKUFilterParameters model)
        {
            string Query = "";

            if(model.IsDistributor =="True")
            {
                Query = LoadFilteredDistMonitoringData_Query();
            }
            else
            {
                Query = LoadFilteredMonitoringData_Query();
            }

            if (model.DIVISION_ID != null  && model.DIVISION_ID!="")
            {
                Query += string.Format("  AND O.DIVISION_ID = {0}", model.DIVISION_ID);
            } 
            if(model.REGION_ID != null && model.REGION_ID!="")
            {
                Query += string.Format("  AND O.REGION_ID = {0}", model.REGION_ID);

            }
            if (model.AREA_ID != null &&  model.AREA_ID != "")
            {
                Query += string.Format("  AND O.AREA_ID = {0}", model.AREA_ID);

            }
            if (model.TERRITORY_ID != null && model.TERRITORY_ID != "")
            {
                Query += string.Format("  AND O.TERRITORY_ID = {0}", model.TERRITORY_ID);

            }
            if (model.CUSTOMER_ID != null && model.CUSTOMER_ID != "")
            {
                Query += string.Format("  AND O.CUSTOMER_ID = {0}", model.CUSTOMER_ID);

            }
            if (model.ORDER_TYPE != null && model.ORDER_TYPE != "")
            {
                Query += string.Format("  AND O.ORDER_TYPE = '{0}'", model.ORDER_TYPE);

            }
            if (model.ORDER_STATUS != null && model.ORDER_STATUS != "")
            {
                Query += string.Format("  AND O.ORDER_STATUS = '{0}'", model.ORDER_STATUS);

            }
            if (model.ORDER_ENTRY_TYPE != null && model.ORDER_ENTRY_TYPE != "")
            {
                Query += string.Format("  AND O.ORDER_ENTRY_TYPE = '{0}'", model.ORDER_ENTRY_TYPE);

            }
            if ((model.DATE_FROM != null && model.DATE_FROM != "") && (model.DATE_TO != null || model.DATE_TO != ""))
            {
                Query += string.Format("  AND  O.ORDER_DATE between TO_DATE('{0}','DD/MM/YYYY') AND TO_DATE('{1}','DD/MM/YYYY')+1", model.DATE_FROM,model.DATE_TO);

            }
            if (model.USER_TYPE != null)
            {
                if (model.USER_TYPE == "DSM")
                {
                    Query += " AND O.DSM_CONFIRM_STATUS = 'Pending'";
                }
                if (model.USER_TYPE == "SM")
                {
                    Query += " AND O.SM_CONFIRM_STATUS = 'Pending'";
                }
                if (model.USER_TYPE == "HOS")
                {
                    Query += " AND O.HOS_CONFIRM_STATUS = 'Pending'";
                }
            }

            if (model.IsDistributor == "True")
            {
                DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(model.db_sales), Query, _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), model.UNIT_ID == 0 ? "" : model.UNIT_ID.ToString(), model.CUSTOMER_CODE }));
                return _commonServices.DataTableToJSON(dataTable);
            }
            else
            {
                DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(model.db_sales), Query, _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), model.UNIT_ID == 0 ? "" : model.UNIT_ID.ToString() }));
                return _commonServices.DataTableToJSON(dataTable);
            }


        }
        public async Task<string> LoadFilteredProcessedData(OrderSKUFilterParameters model)
        {
            string Query = LoadFilteredProcessedData_Query();
            if(model.DIVISION_ID != null  && model.DIVISION_ID!="")
            {
                Query += string.Format("  AND O.DIVISION_ID = {0}", model.DIVISION_ID);
            } 
            if(model.REGION_ID != null && model.REGION_ID!="")
            {
                Query += string.Format("  AND O.REGION_ID = {0}", model.REGION_ID);

            }
            if (model.AREA_ID != null &&  model.AREA_ID != "")
            {
                Query += string.Format("  AND O.AREA_ID = {0}", model.AREA_ID);

            }
            if (model.TERRITORY_ID != null && model.TERRITORY_ID != "")
            {
                Query += string.Format("  AND O.TERRITORY_ID = {0}", model.TERRITORY_ID);

            }
            if (model.CUSTOMER_ID != null && model.CUSTOMER_ID != "")
            {
                Query += string.Format("  AND O.CUSTOMER_ID = {0}", model.CUSTOMER_ID);

            }
            if (model.ORDER_TYPE != null && model.ORDER_TYPE != "")
            {
                Query += string.Format("  AND O.ORDER_TYPE = '{0}'", model.ORDER_TYPE);

            }
            if (model.ORDER_STATUS != null && model.ORDER_STATUS != "")
            {
                Query += string.Format("  AND O.ORDER_STATUS = '{0}'", model.ORDER_STATUS);

            }
            if (model.ORDER_ENTRY_TYPE != null && model.ORDER_ENTRY_TYPE != "")
            {
                Query += string.Format("  AND O.ORDER_ENTRY_TYPE = '{0}'", model.ORDER_ENTRY_TYPE);

            }
            if ((model.DATE_FROM != null && model.DATE_FROM != "") && (model.DATE_TO != null || model.DATE_TO != ""))
            {
                Query += string.Format("  AND  O.ORDER_DATE between TO_DATE('{0}','DD/MM/YYYY') AND TO_DATE('{1}','DD/MM/YYYY')+1", model.DATE_FROM,model.DATE_TO);

            }
            if (model.USER_TYPE != null)
            {
                if (model.USER_TYPE == "DSM")
                {
                    Query += " AND O.DSM_CONFIRM_STATUS = 'Pending'";
                }
                if (model.USER_TYPE == "SM")
                {
                    Query += " AND O.SM_CONFIRM_STATUS = 'Pending'";
                }
                if (model.USER_TYPE == "HOS")
                {
                    Query += " AND O.HOS_CONFIRM_STATUS = 'Pending'";
                }
                else if (model.USER_TYPE == "Distributor")
                {
                    Query += string.Format(" AND O.CUSTOMER_CODE = '{0}' and :param2 = :param2", model.CUSTOMER_CODE);

                }
                else if (model.USER_TYPE == "OSM")
                {
                    Query += string.Format(" AND :param2 = :param2 AND O.CUSTOMER_ID in (Select ACCOUNT_ID from USER_ACCOUNT_RELATION_MST UM LEFT OUTER JOIN USER_ACCOUNT_RELATION_DTL UD on UD.USER_ACCOUNT_MST_ID = UM.USER_ACCOUNT_MST_ID WHERE UM.USER_TYPE = 'OSM' AND  UM.USER_ID = {0})", model.ENTERED_BY);

                }
                else
                {
                    Query += " and O.ORDER_UNIT_ID = :param2";
                }
            }

            DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(model.db_sales), Query, _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), model.UNIT_ID.ToString() }));
            List<Order_Mst> bonus_Msts = new List<Order_Mst>();
            if (dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Order_Mst _bonus_Mst = new Order_Mst();
                    _bonus_Mst.ROW_NO = Convert.ToInt32(dataTable.Rows[i]["ROW_NO"]);
                    _bonus_Mst.CUSTOMER_ID = Convert.ToInt32(dataTable.Rows[i]["CUSTOMER_ID"]);
                    _bonus_Mst.MARKET_ID = Convert.ToInt32(dataTable.Rows[i]["MARKET_ID"]);

                    _bonus_Mst.ORDER_MST_ID = Convert.ToInt32(dataTable.Rows[i]["ORDER_MST_ID"]);
                    _bonus_Mst.ORDER_NO = dataTable.Rows[i]["ORDER_NO"].ToString();
                    _bonus_Mst.INVOICE_TYPE_NAME = dataTable.Rows[i]["INVOICE_TYPE_NAME"].ToString();
                    _bonus_Mst.ORDER_STATUS = dataTable.Rows[i]["ORDER_STATUS"].ToString();

                    _bonus_Mst.ORDER_DATE = dataTable.Rows[i]["ORDER_DATE"].ToString();
                    _bonus_Mst.ORDER_TYPE = dataTable.Rows[i]["ORDER_TYPE"].ToString();
                    _bonus_Mst.DELIVERY_DATE = dataTable.Rows[i]["DELIVERY_DATE"].ToString();
                    _bonus_Mst.CUSTOMER_CODE = dataTable.Rows[i]["CUSTOMER_CODE"].ToString();
                    _bonus_Mst.MARKET_NAME = dataTable.Rows[i]["MARKET_NAME"].ToString();
                    _bonus_Mst.ORDER_ENTRY_TYPE = dataTable.Rows[i]["ORDER_ENTRY_TYPE"].ToString();
                    _bonus_Mst.CUSTOMER_NAME = dataTable.Rows[i]["CUSTOMER_NAME"].ToString();
                    _bonus_Mst.ORDER_AMOUNT = Convert.ToDecimal(dataTable.Rows[i]["ORDER_AMOUNT"]);

                    _bonus_Mst.ORDER_MST_ID_ENCRYPTED = _commonServices.Encrypt(_bonus_Mst.ORDER_MST_ID.ToString());
                    bonus_Msts.Add(_bonus_Mst);
                }
            }

            await _logManager.AddOrUpdate(model.db_security, "View", "ORDER_MST", model.COMPANY_ID, model.UNIT_ID, Convert.ToInt32(model.ENTERED_BY), model.ENTERED_TERMINAL, "/SalesAndDistribution/Division/DivisionInfo", 0, "");



            return JsonSerializer.Serialize(bonus_Msts);
        }

        public async Task<decimal> LoadProductPrice(string db, string Company_Id, int SKU_ID, string Order_Type,int Customer_Id) 
            =>await _commonServices.GetMaximumNumberAsyn<decimal>(_configuration.GetConnectionString(db), LoadProductPrice_Query(), _commonServices.AddParameter(new string[] { Order_Type, Customer_Id.ToString(), SKU_ID.ToString() })); 

        public async Task<string> LoadCustomer(string db, Order_Mst model)
        {
            string Query =  LoadCustomerData_Query();
            if (model.USER_TYPE != null)
            {
                
                if (model.USER_TYPE == "Distributor")
                {
                    Query += string.Format(" AND CUSTOMER_CODE = '{0}'", model.CUSTOMER_CODE);

                }
                else if (model.USER_TYPE == "OSM")
                {
                    Query += string.Format(" AND CUSTOMER_ID in (Select ACCOUNT_ID from USER_ACCOUNT_RELATION_MST UM LEFT OUTER JOIN USER_ACCOUNT_RELATION_DTL UD on UD.USER_ACCOUNT_MST_ID = UM.USER_ACCOUNT_MST_ID WHERE UM.USER_TYPE = 'OSM' AND  UM.USER_ID = {0})", model.ENTERED_BY);

                }
                

            }
            return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Query, _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString() })));

        }

        public async Task<string> LoadProductsSpecificType(string db, string Company_Id, string type)
        {
            var query = string.Format(@"Select p.SKU_ID, p.SKU_CODE, p.SKU_NAME, p.PACK_SIZE,  p.COMPANY_ID, p.UNIT_ID ,0 UNIT_TP, 
                       0 CURRENT_STOCK, 0 SUGGESTED_QTY, BRAND_ID, CATEGORY_ID,GROUP_ID,BASE_PRODUCT_ID,d.UNIT_ID PRODUCT_UNIT_ID
                       from Product_Info p 
                       left outer join distributor_product_dtl d on D.SKU_ID = P.SKU_ID
                       Left outer join distributor_product_mst m on M.MST_ID = d.MST_ID
                       Where  p.PRODUCT_STATUS = 'Active' and p.company_ID = :param1  and M.DISTRIBUTOR_PRODUCT_TYPE  = '{0}' ", type);

            DataTable dt = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] { Company_Id }));
            return _commonServices.DataTableToJSON(dt);
        }
        public string LoadProductType(string db,  string Customer_Id)
        {
            string dist_prod_type = _commonServices.GetMaximumNumber<string>(_configuration.GetConnectionString(db), "Select DISTRIBUTOR_PRODUCT_TYPE from CUSTOMER_INFO WHERE CUSTOMER_ID = :param1", _commonServices.AddParameter(new string[] { Customer_Id })).ToString();
            string[] dist_prod_type_ = dist_prod_type.Split(',');
           
            return JsonSerializer.Serialize(dist_prod_type_);
        }

        public async Task<string> LoadProductsSpecific(string db, string Company_Id, string Customer_Id)
        {
            string query = "";
            string dist_prod_type = _commonServices.GetMaximumNumber<string>(_configuration.GetConnectionString(db), "Select DISTRIBUTOR_PRODUCT_TYPE from CUSTOMER_INFO WHERE CUSTOMER_ID = :param1", _commonServices.AddParameter(new string[] { Customer_Id })).ToString();
            string[] dist_prod_type_ = dist_prod_type.Split(',');
            for (var i = 0; i<dist_prod_type_.Length; i++)
            {
                if(i==0)
                {
                    query = query + string.Format(@"Select p.SKU_ID, p.SKU_CODE, p.SKU_NAME, p.PACK_SIZE,  p.COMPANY_ID, p.UNIT_ID ,0 UNIT_TP, 
                       0 CURRENT_STOCK, 0 SUGGESTED_QTY, BRAND_ID, CATEGORY_ID,GROUP_ID,BASE_PRODUCT_ID, d.UNIT_ID PRODUCT_UNIT_ID
                       from Product_Info p 
                       left outer join distributor_product_dtl d on D.SKU_ID = P.SKU_ID
                       Left outer join distributor_product_mst m on M.MST_ID = d.MST_ID
                       Where  p.PRODUCT_STATUS = 'Active' and p.company_ID = :param1  and M.DISTRIBUTOR_PRODUCT_TYPE  = '{0}' ", dist_prod_type_[i]);
                }
                else
                {
                    query = query + string.Format(@"Union ALL 
                       Select p.SKU_ID, p.SKU_CODE, p.SKU_NAME, p.PACK_SIZE,  p.COMPANY_ID, p.UNIT_ID ,0 UNIT_TP, 
                       0 CURRENT_STOCK, 0 SUGGESTED_QTY, BRAND_ID, CATEGORY_ID,GROUP_ID,BASE_PRODUCT_ID, d.UNIT_ID PRODUCT_UNIT_ID
                       from Product_Info p 
                       left outer join distributor_product_dtl d on D.SKU_ID = P.SKU_ID
                       Left outer join distributor_product_mst m on M.MST_ID = d.MST_ID
                       Where  p.PRODUCT_STATUS = 'Active' and p.company_ID = :param1  and M.DISTRIBUTOR_PRODUCT_TYPE  = '{0}' ", dist_prod_type_[i]);
                }
            }
            DataTable dt = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] { Company_Id }));
            return _commonServices.DataTableToJSON(dt);
        }

        public async Task<string> LoadProducts(string db, string Company_Id, string Customer_Id)
        {   
            var query = string.Format(@"Select p.SKU_ID, p.SKU_CODE, p.SKU_NAME, p.PACK_SIZE,  p.COMPANY_ID, p.UNIT_ID ,0 UNIT_TP, 
                       0 CURRENT_STOCK, 0 SUGGESTED_QTY, BRAND_ID, CATEGORY_ID,GROUP_ID,BASE_PRODUCT_ID,M.DISTRIBUTOR_PRODUCT_TYPE DISTRIBUTOR_PRODUCT_TYPE_MST,
                       C.DISTRIBUTOR_PRODUCT_TYPE PRODUCT_TYPE_CUSTOMER
                       from Product_Info p
                       left outer join Customer_Info C on c.CUSTOMER_ID = {0}
                       left outer join distributor_product_dtl d on D.SKU_ID = P.SKU_ID
                       Left outer join distributor_product_mst m on m.MST_ID = D.MST_ID
                       Where  p.PRODUCT_STATUS = 'Active' and  M.DISTRIBUTOR_PRODUCT_TYPE = 'ALL' and   p.company_ID = :param1 and  C.CUSTOMER_ID = :param2", Customer_Id);
            
            return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] { Company_Id, Customer_Id })));

        }

        public async Task<string> Replacement_Master(string db, string customerId, string unitId, string orderMstId)
        {
            var connectionString = _configuration.GetConnectionString(db);
            string query;
            Dictionary<string,string> parameters;
            //if (unitId=="2")
            //{
            //    query = Replacement_Master_Query_Rupshi();
            //     parameters = _commonServices.AddParameter(new string[] { customerId, unitId, orderMstId });

            //}
            //else
            //{
            //    query = Replacement_Master_Query();
            //    parameters = _commonServices.AddParameter(new string[] { customerId, orderMstId });

            //}


            query = @"SELECT CLAIM_NO REPLACE_CLAIM_NO,
       CHALLAN_NUMBER,
       CUSTOMER_ID,
       CUSTOMER_CODE,
       MARKET_ID,
       MARKET_CODE,
       CHALLAN_DATE,
       RECEIVE_CATEGORY,
       UNIT_ID,
       REPLACE_PRODUCT_STATUS REFURBISHMENT_PRODUCT_STATUS
  FROM VIEW_REPLACEMENT_CLAIM_RUPSHI
WHERE     CUSTOMER_ID = :param1 
       AND NVL (CLAIM_NO, '') NOT IN (SELECT O.REPLACE_CLAIM_NO
                                        FROM ORDER_MST O
                                             LEFT OUTER JOIN INVOICE_MST I
                                                ON I.ORDER_NO = O.ORDER_NO
                                             LEFT OUTER JOIN RETURN_MST R
                                                ON R.INVOICE_NO =
                                                      I.INVOICE_NO
                                       WHERE     NVL (O.REPLACE_CLAIM_NO,
                                                      'V') != 'V'
                                             AND NVL (R.MST_ID, 0) = 0)
UNION
SELECT C.CLAIM_NO REPLACE_CLAIM_NO,
       C.CHALLAN_NUMBER,
       C.CUSTOMER_ID,
       C.CUSTOMER_CODE,
       C.MARKET_ID,
       C.MARKET_CODE,
       C.CHALLAN_DATE,
       C.RECEIVE_CATEGORY,
       C.UNIT_ID,
       C.REPLACE_PRODUCT_STATUS REFURBISHMENT_PRODUCT_STATUS
  FROM VIEW_REPLACEMENT_CLAIM_RUPSHI C, ORDER_MST O
WHERE O.REPLACE_CLAIM_NO = C.CLAIM_NO AND O.ORDER_MST_ID = :param2

UNION 

SELECT CLAIM_NO REPLACE_CLAIM_NO,
       CHALLAN_NUMBER,
       CUSTOMER_ID,
       CUSTOMER_CODE,
       MARKET_ID,
       MARKET_CODE,
       CHALLAN_DATE,
       RECEIVE_CATEGORY,
       UNIT_ID,
       REPLACE_PRODUCT_STATUS REFURBISHMENT_PRODUCT_STATUS
  FROM VIEW_REPLACEMENT_CLAIM
 WHERE     CUSTOMER_ID = :param1 
       AND NVL (CLAIM_NO, '') NOT IN (SELECT O.REPLACE_CLAIM_NO
                                        FROM ORDER_MST O
                                             LEFT OUTER JOIN INVOICE_MST I
                                                ON I.ORDER_NO = O.ORDER_NO
                                             LEFT OUTER JOIN RETURN_MST R
                                                ON R.INVOICE_NO =
                                                      I.INVOICE_NO
                                       WHERE     NVL (O.REPLACE_CLAIM_NO,
                                                      'V') != 'V'
                                             AND NVL (R.MST_ID, 0) = 0)
UNION
SELECT C.CLAIM_NO REPLACE_CLAIM_NO,
       C.CHALLAN_NUMBER,
       C.CUSTOMER_ID,
       C.CUSTOMER_CODE,
       C.MARKET_ID,
       C.MARKET_CODE,
       C.CHALLAN_DATE,
       C.RECEIVE_CATEGORY,
       C.UNIT_ID,
       C.REPLACE_PRODUCT_STATUS REFURBISHMENT_PRODUCT_STATUS
  FROM VIEW_REPLACEMENT_CLAIM C, ORDER_MST O
 WHERE O.REPLACE_CLAIM_NO = C.CLAIM_NO AND O.ORDER_MST_ID = :param2";

                parameters = _commonServices.AddParameter(new string[] { customerId, orderMstId });

            var dataTable = await _commonServices.GetDataTableAsyn(connectionString, query, parameters);
            var jsonResult = _commonServices.DataTableToJSON(dataTable);
            return jsonResult;
        }
        public async Task<string> Replacement_Dtl(string db, string Claim_NO, int ORDER_UNIT_ID)
        {
            string quary;
            //if (ORDER_UNIT_ID == 2)
            //{
            //    quary = Replacement_Dtl_Query_rupshi();
            //}
            //else
            //{
            //    quary = Replacement_Dtl_Query();

            //}
            quary = @"Select
   ROW_NUMBER() OVER(ORDER BY p.SKU_ID ASC) AS ROW_NO,
   CLAIM_NO REPLACEMENT_CLAIM_NO
  ,0 PACK_SIZE
  ,0 PACK_UNIT
  ,0 PACK_VALUE
  ,SKU_STATUS PRODUCT_STATUS
  ,0 QTY_PER_PACK
  ,0 SHIPPER_QTY
  ,0 SHIPPER_VOLUME
  ,'' SHIPPER_VOLUME_UNIT
  ,0 SHIPPER_WEIGHT
  ,'' SHIPPER_WEIGHT_UNIT
  ,SKU_CODE
  ,SKU_ID
  ,SKU_NAME
  ,'' SKU_NAME_BANGLA
  ,0 UNIT_ID
  ,0 WEIGHT_PER_PACK
  ,''  WEIGHT_UNIT
  ,REPLACE_QTY ORDER_QTY
  ,0 UNIT_TP
  ,0 CURRENT_STOCK
  ,0 SUGGESTED_QTY
  ,0 MAXIMUM_QTY
  ,0 MINIMUM_QTY
  ,0 REMAINING_QTY
  ,0 TARGET_QTY
  ,0 IMS_QTY
  ,0 DISTRIBUTOR_STOCK
  ,0 ORDER_AMOUNT
   from VIEW_REPLACEMENT_CLAIM_SKU p
  Where CLAIM_NO = :param1
  
  UNION ALL 
  
  Select
   ROW_NUMBER() OVER(ORDER BY p.SKU_ID ASC) AS ROW_NO,
   CLAIM_NO REPLACEMENT_CLAIM_NO
  ,0 PACK_SIZE
  ,0 PACK_UNIT
  ,0 PACK_VALUE
  ,SKU_STATUS PRODUCT_STATUS
  ,0 QTY_PER_PACK
  ,0 SHIPPER_QTY
  ,0 SHIPPER_VOLUME
  ,'' SHIPPER_VOLUME_UNIT
  ,0 SHIPPER_WEIGHT
  ,'' SHIPPER_WEIGHT_UNIT
  ,SKU_CODE
  ,SKU_ID
  ,SKU_NAME
  ,'' SKU_NAME_BANGLA
  ,0 UNIT_ID
  ,0 WEIGHT_PER_PACK
  ,''  WEIGHT_UNIT
  ,REPLACE_QTY ORDER_QTY
  ,0 UNIT_TP
  ,0 CURRENT_STOCK
  ,0 SUGGESTED_QTY
  ,0 MAXIMUM_QTY
  ,0 MINIMUM_QTY
  ,0 REMAINING_QTY
  ,0 TARGET_QTY
  ,0 IMS_QTY
  ,0 DISTRIBUTOR_STOCK
  ,0 ORDER_AMOUNT
   from VIEW_REPLACE_CLAIM_SKU_RUPSHI p
  Where CLAIM_NO = :param1";


          return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), quary, _commonServices.AddParameter(new string[] { Claim_NO })));
        }


        public async Task<string> Refurbishment_Master(string db, string Customer_Id, string Unit_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Refurbishment_Master_Query(), _commonServices.AddParameter(new string[] { Customer_Id, Unit_Id })));
        public async Task<string> Refurbishment_Dtl(string db, string Claim_NO) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Refurbishment_Dtl_Query(), _commonServices.AddParameter(new string[] { Claim_NO })));

        public async Task<string> GetEditDatabyId(string db, int id,string dist_status)
        {
            DataTable _InvoiceMst = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), GetOrder_MST_BY_Id_Query(), _commonServices.AddParameter(new string[] { id.ToString() }));
            DataTable _InvoiceDtl = new DataTable();
            if (dist_status == "True")
            {
                _InvoiceDtl = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), GetOrder_DTL_BY_Mst_Id_Query(), _commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), id.ToString() }));
               for(int i = 0;i<_InvoiceDtl.Rows.Count;i++)
                {
                    _InvoiceDtl.Rows[i]["CURRENT_STOCK"] = 0;
                }
            }
            else
            {
                 _InvoiceDtl = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), GetOrder_DTL_BY_Mst_Id_Query(), _commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), id.ToString() }));
            }
            //DataTable _InvoiceDtl = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), GetOrder_DTL_BY_MstId_Query(), _commonServices.AddParameter(new string[] { id.ToString() }));

            DataSet dataSet = new DataSet();

            DataTable _InvoiceComboGifts = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Invoice_Details_InvoiceComboGift_Query(), _commonServices.AddParameter(new string[] { id.ToString() }));
            DataTable _InvoiceComboBonus = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Invoice_Details_InvoiceComboBonus_Query(), _commonServices.AddParameter(new string[] { id.ToString() }));

            List<int> InvDtl_Ids = new List<int>();
            string Dtl_Ids = "";
            bool is_unit_tp_zero = false;

            foreach (DataRow row in _InvoiceDtl.Rows)
            {

                int dtl_id = Convert.ToInt32(row["ORDER_DTL_ID"]);
                int st = InvDtl_Ids.Where(x => x == dtl_id).FirstOrDefault();
                if (st < 1)
                {
                    InvDtl_Ids.Add(dtl_id);
                }
                if (Convert.ToDecimal(row["UNIT_TP"]) == 0)
                {
                    is_unit_tp_zero = true;
                }

            }
           
            for (int i = 0; i < InvDtl_Ids.Count; i++)
            {
                if (i == 0)
                {
                    Dtl_Ids = InvDtl_Ids[i].ToString();
                }
                else
                {
                    Dtl_Ids = Dtl_Ids + "," + InvDtl_Ids[i].ToString();

                }


            }

            string inv_gift = Invoice_Details_InvoiceGift_Query() + " WHERE B.ORDER_DTL_ID in (" + Dtl_Ids + ")";
            string inv_bonus = Invoice_Details_InvoiceBonus_Query() + " WHERE B.ORDER_DTL_ID in (" + Dtl_Ids + ")";

            DataTable _invoiceGifts = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), inv_gift, _commonServices.AddParameter(new string[] { }));

            DataTable _InvoiceBonus = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), inv_bonus, _commonServices.AddParameter(new string[] { }));
            dataSet.Tables.Add(_InvoiceMst);
            dataSet.Tables.Add(_InvoiceDtl);
            dataSet.Tables.Add(_InvoiceComboGifts);
            dataSet.Tables.Add(_InvoiceComboBonus);
            dataSet.Tables.Add(_invoiceGifts);
            dataSet.Tables.Add(_InvoiceBonus);

            if (is_unit_tp_zero == true)
            {
                await Delete_Order_full(db, Convert.ToInt32(_InvoiceMst.Rows[0]["ORDER_MST_ID"]));
            }
            return _commonServices.DataSetToJSON(dataSet);


        }

        public async Task<string> GetEditDatabyIdProcessCheck(string db, int id)
        {
            DataTable data_master = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), GetOrder_MST_BY_Id_Query(), _commonServices.AddParameter(new string[] { id.ToString() }));
            DataTable data_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), GetOrder_DTL_BY_MstId_Query(), _commonServices.AddParameter(new string[] { id.ToString() }));
            Order_Mst order_Mst = new Order_Mst();
            if (data_master.Rows.Count > 0)
            {



                order_Mst.COMPANY_ID = Convert.ToInt32(data_master.Rows[0]["COMPANY_ID"]);
                order_Mst.ROW_NO = Convert.ToInt32(data_master.Rows[0]["ROW_NO"]);
                order_Mst.ORDER_MST_ID = Convert.ToInt32(data_master.Rows[0]["ORDER_MST_ID"]);
                order_Mst.ORDER_NO = data_master.Rows[0]["ORDER_NO"].ToString();
                order_Mst.ORDER_DATE = data_master.Rows[0]["ORDER_DATE"].ToString();
                order_Mst.ORDER_TYPE = data_master.Rows[0]["ORDER_TYPE"].ToString();
                order_Mst.DELIVERY_DATE = data_master.Rows[0]["DELIVERY_DATE"].ToString();
                order_Mst.CUSTOMER_ID = Convert.ToInt32(data_master.Rows[0]["CUSTOMER_ID"]);
                order_Mst.CUSTOMER_CODE = data_master.Rows[0]["CUSTOMER_CODE"].ToString();
                order_Mst.PAYMENT_MODE = data_master.Rows[0]["PAYMENT_MODE"].ToString();
                order_Mst.ORDER_ENTRY_TYPE = data_master.Rows[0]["ORDER_ENTRY_TYPE"] != null ? data_master.Rows[0]["ORDER_ENTRY_TYPE"].ToString() : "Menual";
                order_Mst.REMARKS = data_master.Rows[0]["REMARKS"] != null ? data_master.Rows[0]["REMARKS"].ToString() : "";
                order_Mst.ORDER_STATUS = data_master.Rows[0]["ORDER_STATUS"].ToString();
                order_Mst.AREA_ID = Convert.ToInt32(data_master.Rows[0]["AREA_ID"]);
                order_Mst.MARKET_ID = Convert.ToInt32(data_master.Rows[0]["MARKET_ID"]);
                order_Mst.DIVISION_ID = Convert.ToInt32(data_master.Rows[0]["DIVISION_ID"]);
                order_Mst.TERRITORY_ID = Convert.ToInt32(data_master.Rows[0]["TERRITORY_ID"]);
                order_Mst.REGION_ID = Convert.ToInt32(data_master.Rows[0]["REGION_ID"]);
                order_Mst.ORDER_UNIT_ID = Convert.ToInt32(data_master.Rows[0]["ORDER_UNIT_ID"]);
                order_Mst.INVOICE_UNIT_ID = Convert.ToInt32(data_master.Rows[0]["INVOICE_UNIT_ID"]);
                order_Mst.REPLACE_CLAIM_NO = data_master.Rows[0]["REPLACE_CLAIM_NO"] != null ? data_master.Rows[0]["REPLACE_CLAIM_NO"].ToString() : "";
                order_Mst.BONUS_PROCESS_NO = data_master.Rows[0]["BONUS_PROCESS_NO"] != null ? data_master.Rows[0]["BONUS_PROCESS_NO"].ToString() : "";
                order_Mst.BONUS_CLAIM_NO = data_master.Rows[0]["BONUS_CLAIM_NO"] != null ? data_master.Rows[0]["BONUS_CLAIM_NO"].ToString() : "";
                order_Mst.INVOICE_STATUS = data_master.Rows[0]["INVOICE_STATUS"].ToString();
                order_Mst.ORDER_AMOUNT = Convert.ToDecimal(data_master.Rows[0]["ORDER_AMOUNT"].ToString());
                order_Mst.SPA_TOTAL_AMOUNT = Convert.ToDecimal(data_master.Rows[0]["SPA_TOTAL_AMOUNT"].ToString());
                order_Mst.SPA_COMMISSION_PCT = Convert.ToDecimal(data_master.Rows[0]["SPA_COMMISSION_PCT"].ToString());
                order_Mst.SPA_COMMISSION_AMOUNT = Convert.ToDecimal(data_master.Rows[0]["SPA_COMMISSION_AMOUNT"].ToString());
                order_Mst.SPA_NET_AMOUNT = Convert.ToDecimal(data_master.Rows[0]["SPA_NET_AMOUNT"].ToString());
                order_Mst.ORDER_ENTRY_TYPE = data_master.Rows[0]["ORDER_ENTRY_TYPE"].ToString();
                order_Mst.DISCOUNT_AMOUNT = Convert.ToDecimal(data_master.Rows[0]["DISCOUNT_AMOUNT"].ToString());
                order_Mst.ADJUSTMENT_AMOUNT = Convert.ToDecimal(data_master.Rows[0]["ADJUSTMENT_AMOUNT"].ToString());
                order_Mst.NET_ORDER_AMOUNT = Convert.ToDecimal(data_master.Rows[0]["NET_ORDER_AMOUNT"].ToString());
                order_Mst.ORDER_PROCESS_STATUS = data_master.Rows[0]["ORDER_PROCESS_STATUS"].ToString();

                order_Mst.ORDER_MST_ID = Convert.ToInt32(data_master.Rows[0]["ORDER_MST_ID"]);

                if (data_detail.Rows.Count > 0)
                {
                    order_Mst.Order_Dtls = new List<Order_Dtl>();
                    for (int i = 0; i < data_detail.Rows.Count; i++)
                    {

                        Order_Dtl order_Dtl = new Order_Dtl();
                        order_Dtl.COMPANY_ID = Convert.ToInt32(data_detail.Rows[i]["COMPANY_ID"]);
                        order_Dtl.ROW_NO = Convert.ToInt32(data_detail.Rows[i]["ROW_NO"]);
                        order_Dtl.ORDER_MST_ID = Convert.ToInt32(data_detail.Rows[i]["ORDER_MST_ID"]);
                        order_Dtl.ORDER_DTL_ID = Convert.ToInt32(data_detail.Rows[i]["ORDER_DTL_ID"]);

                        order_Dtl.SKU_ID = Convert.ToInt32(data_detail.Rows[i]["SKU_ID"]);
                        order_Dtl.SKU_CODE = data_detail.Rows[i]["SKU_CODE"] != null ? data_detail.Rows[i]["SKU_CODE"].ToString() : "";
                        order_Dtl.ORDER_QTY = data_detail.Rows[i]["ORDER_QTY"] != null ? Convert.ToInt32(data_detail.Rows[i]["ORDER_QTY"]) : 0;
                        //order_Dtl.REVISED_ORDER_QTY = data_detail.Rows[i]["REVISED_ORDER_QTY"] != null ? Convert.ToInt32(data_detail.Rows[i]["REVISED_ORDER_QTY"]) : 0;
                        order_Dtl.UNIT_TP = data_detail.Rows[i]["UNIT_TP"] != null ? Convert.ToDecimal(data_detail.Rows[i]["UNIT_TP"]) : 0;
                        order_Dtl.ORDER_AMOUNT = data_detail.Rows[i]["ORDER_AMOUNT"] != null ? Convert.ToDecimal(data_detail.Rows[i]["ORDER_AMOUNT"]) : 0;
                        order_Dtl.STATUS = data_detail.Rows[i]["STATUS"] != null ? data_detail.Rows[i]["STATUS"].ToString() : "";
                        order_Dtl.REMARKS = data_detail.Rows[i]["REMARKS"] != null ? data_detail.Rows[i]["REMARKS"].ToString() : "";
                        order_Dtl.SPA_UNIT_TP = data_detail.Rows[i]["SPA_UNIT_TP"] != null ? Convert.ToDecimal(data_detail.Rows[i]["SPA_UNIT_TP"]) : 0;
                        order_Dtl.SPA_AMOUNT = data_detail.Rows[i]["SPA_AMOUNT"] != null ? Convert.ToDecimal(data_detail.Rows[i]["SPA_AMOUNT"]) : 0;
                        order_Dtl.SPA_DISCOUNT_AMOUNT = data_detail.Rows[i]["SPA_DISCOUNT_AMOUNT"] != null ? Convert.ToDecimal(data_detail.Rows[i]["SPA_DISCOUNT_AMOUNT"]) : 0;
                        order_Dtl.SPA_REQ_TIME_STOCK = data_detail.Rows[i]["SPA_REQ_TIME_STOCK"] != null ? Convert.ToDecimal(data_detail.Rows[i]["SPA_REQ_TIME_STOCK"]) : 0;
                        order_Dtl.SPA_CUST_COM = data_detail.Rows[i]["SPA_CUST_COM"] != null ? Convert.ToDecimal(data_detail.Rows[i]["SPA_CUST_COM"]) : 0;
                        order_Dtl.SPA_DISCOUNT_VAL_PCT = data_detail.Rows[i]["SPA_DISCOUNT_VAL_PCT"] != null ? Convert.ToDecimal(data_detail.Rows[i]["SPA_DISCOUNT_VAL_PCT"]) : 0;
                        order_Dtl.SPA_DISCOUNT_TYPE = data_detail.Rows[i]["SPA_DISCOUNT_TYPE"] != null ? data_detail.Rows[i]["SPA_DISCOUNT_TYPE"].ToString() : "";
                        order_Dtl.SPA_TOTAL_AMOUNT = data_detail.Rows[i]["SPA_TOTAL_AMOUNT"] != null ? Convert.ToDecimal(data_detail.Rows[i]["SPA_TOTAL_AMOUNT"]) : 0;
                        order_Dtl.UNIT_ID = Convert.ToInt32(data_detail.Rows[i]["UNIT_ID"]);
                        order_Dtl.CUSTOMER_DISC_AMOUNT = data_detail.Rows[i]["CUSTOMER_DISC_AMOUNT"] != null ? Convert.ToDecimal(data_detail.Rows[i]["CUSTOMER_DISC_AMOUNT"]) : 0;
                        order_Dtl.CUSTOMER_ADD1_DISC_AMOUNT = data_detail.Rows[i]["CUSTOMER_ADD1_DISC_AMOUNT"] != null ? Convert.ToDecimal(data_detail.Rows[i]["SPA_TOTAL_AMOUNT"]) : 0;
                        order_Dtl.CUSTOMER_ADD2_DISC_AMOUNT = data_detail.Rows[i]["CUSTOMER_ADD2_DISC_AMOUNT"] != null ? Convert.ToDecimal(data_detail.Rows[i]["SPA_TOTAL_AMOUNT"]) : 0;
                        order_Dtl.CUSTOMER_PRODUCT_DISC_AMOUNT = data_detail.Rows[i]["CUSTOMER_PRODUCT_DISC_AMOUNT"] != null ? Convert.ToDecimal(data_detail.Rows[i]["SPA_TOTAL_AMOUNT"]) : 0;
                        order_Dtl.PROD_BONUS_PRICE_DISC_AMOUNT = data_detail.Rows[i]["PROD_BONUS_PRICE_DISC_AMOUNT"] != null ? Convert.ToDecimal(data_detail.Rows[i]["SPA_TOTAL_AMOUNT"]) : 0;
                        order_Dtl.LOADING_CHARGE_AMOUNT = data_detail.Rows[i]["LOADING_CHARGE_AMOUNT"] != null ? Convert.ToDecimal(data_detail.Rows[i]["SPA_TOTAL_AMOUNT"]) : 0;
                        order_Dtl.INVOICE_ADJUSTMENT_AMOUNT = data_detail.Rows[i]["INVOICE_ADJUSTMENT_AMOUNT"] != null ? Convert.ToDecimal(data_detail.Rows[i]["SPA_TOTAL_AMOUNT"]) : 0;
                        order_Dtl.BONUS_PRICE_DISC_AMOUNT = data_detail.Rows[i]["BONUS_PRICE_DISC_AMOUNT"] != null ? Convert.ToDecimal(data_detail.Rows[i]["SPA_TOTAL_AMOUNT"]) : 0;
                        order_Dtl.NET_ORDER_AMOUNT = data_detail.Rows[i]["NET_ORDER_AMOUNT"] != null ? Convert.ToDecimal(data_detail.Rows[i]["SPA_TOTAL_AMOUNT"]) : 0;


                        order_Mst.Order_Dtls.Add(order_Dtl);

                     
                    }
                   
                }
            }

            return JsonSerializer.Serialize(order_Mst);


        }
        public async Task<string> Get_Customer_Balance(string db, string Company_Id,string Unit_Id,string Customer_Id)
        {
            DataTable Balance = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Get_Customer_Balance_Query(), _commonServices.AddParameter(new string[] { Customer_Id, Company_Id }));
            DataTable Credit_limit = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Get_Customer_Credit_Limit_Query(), _commonServices.AddParameter(new string[] { Customer_Id, Company_Id, }));
            DataSet Balance_Credit_Limit = new DataSet();
            Balance_Credit_Limit.Tables.Add(Balance);
            Balance_Credit_Limit.Tables.Add(Credit_limit);
            return _commonServices.DataSetToJSON(Balance_Credit_Limit);

        }
        public async Task<string> Save_Final_Post_Order(string db, Order_Mst model)
        {
           
            List<QueryPattern> listOfQuery = new List<QueryPattern>();
          
            if(model.IsDistributor == "True" || model.IsOSM == "True")
            {
                if(model.NOTIFY_TEXT == "Max QTY Exceded" || model.NOTIFY_TEXT ==  "Max Credit Exceded")
                {
                    listOfQuery.Add(_commonServices.AddQuery(Save_On_DSM_Pending_Order_Query(), _commonServices.AddParameter(new string[] { model.ORDER_MST_ID.ToString(), "Order Generated from Distributor End" })));

                }
                else 
                {
                    listOfQuery.Add(_commonServices.AddQuery(Save_Final_Post_Order_Query(), _commonServices.AddParameter(new string[] { model.ORDER_MST_ID.ToString() })));

                }

            }
            else
            {
                listOfQuery.Add(_commonServices.AddQuery(Save_Final_Post_Order_Query(), _commonServices.AddParameter(new string[] { model.ORDER_MST_ID.ToString() })));

            }

            await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
            //Notification Add---------------
            var Pending_Invoice_Count = await _commonServices.GetMaximumNumberAsyn<int>(_configuration.GetConnectionString(model.db_sales), Order_Ready_For_Invoice_Count_Query(), _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), model.UNIT_ID.ToString() }));

            await _NotificationManager.AddOrderNotification(model.db_security, model.db_sales, 1, "New Order " + model.ORDER_NO + " Has been added/Edited and ready for Invoice. Order Ready for Invoice Count: " + Pending_Invoice_Count, model.COMPANY_ID, model.UNIT_ID);

            return _commonServices.Encrypt(model.ORDER_MST_ID.ToString());


        }
        public async Task<string> Save_Post_Confirmation_Order(string db, Order_Mst model)
        {

            List<QueryPattern> listOfQuery = new List<QueryPattern>();

            if (model.USER_TYPE == "DSM")
            {
                if(model.NOTIFY_TEXT == "Max QTY Exceded")
                {
                    listOfQuery.Add(_commonServices.AddQuery(Save_On_SM_Pending_Order_Query(), _commonServices.AddParameter(new string[] { model.ORDER_MST_ID.ToString(), "Order Generated from Distributor End" })));

                }
                else
                {
                    listOfQuery.Add(_commonServices.AddQuery(Save_On_HOS_Pending_Order_Query(), _commonServices.AddParameter(new string[] { model.ORDER_MST_ID.ToString(), "Order Generated from Distributor End" })));

                }

            }

            if (model.USER_TYPE == "SM" || model.USER_TYPE == "HOS")
            {
                listOfQuery.Add(_commonServices.AddQuery(Save_Final_Post_Order_Query(), _commonServices.AddParameter(new string[] { model.ORDER_MST_ID.ToString() })));

            }
           

            await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);

            return "1";


        }
        public async Task<string> Cancel_Post_Confirmation_Order(string db, Order_Mst model)
        {
            List<QueryPattern> listOfQuery = new List<QueryPattern>();

            if (model.USER_TYPE == "DSM")
            {
                listOfQuery.Add(_commonServices.AddQuery(Cancel_On_DSM_Pending_Order_Query(), _commonServices.AddParameter(new string[] { model.ORDER_MST_ID.ToString(), "Order Generated from Distributor End" })));

            }

            if (model.USER_TYPE == "SM")
            {
                listOfQuery.Add(_commonServices.AddQuery(Cancel_On_SM_Pending_Order_Query(), _commonServices.AddParameter(new string[] { model.ORDER_MST_ID.ToString(), "Order Generated from Distributor End" })));

            }
            if (model.USER_TYPE == "HOS")
            {
                listOfQuery.Add(_commonServices.AddQuery(Cancel_On_HOS_Pending_Order_Query(), _commonServices.AddParameter(new string[] { model.ORDER_MST_ID.ToString() })));

            }

            await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);

            return "1";


        }


        public async Task<string> Save_Final_Order(string db, Order_Mst model)
        {
            Result result1 = new Result();
            if ((model.IsDistributor == "True" || model.IsOSM == "True") && model.FINAL_SUBMISSION_STATUS == "Complete")
            {
                result1.Key = "0";
                result1.Status = "Can not update Order after final posting";
                return JsonSerializer.Serialize(result1);
            }
            //if (model.IsDistributor != "True" && model.FINAL_SUBMIT_CONFIRM_STATUS == "Complete")
            //{
            //    result1.Key = "0";
            //    result1.Status = "Can not update Order after final posting";
            //    return JsonSerializer.Serialize(result1);

            //}

            List<QueryPattern> listOfQuery = new List<QueryPattern>();
          
            listOfQuery.Add(_commonServices.AddQuery(Save_Final_Order_Query(), _commonServices.AddParameter(new string[] { model.ORDER_MST_ID.ToString() })));

            await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
            //Notification Add---------------
            //var Pending_Invoice_Count = await _commonServices.GetMaximumNumberAsyn<int>(_configuration.GetConnectionString(model.db_sales), Order_Ready_For_Invoice_Count_Query(), _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), model.UNIT_ID.ToString() }));

            //await _NotificationManager.AddOrderNotification(model.db_security, model.db_sales, 1, "New Order " + model.ORDER_NO + " Has been added/Edited and ready for Invoice. Order Ready for Invoice Count: " + Pending_Invoice_Count, model.COMPANY_ID, model.UNIT_ID);
            result1.Key = _commonServices.Encrypt(model.ORDER_MST_ID.ToString());
            result1.Status = "1";

            return JsonSerializer.Serialize(result1);


        }
        
        public async Task<string> Process_Order(string db, Order_Mst model)
        {
            List<QueryPattern> listOfQuery = new List<QueryPattern>();
            string result = string.Empty;
            List<string> _result = new List<string>();
            Result result_final = new Result();
            string flag = await this.Delete_Processed_Order(db,model);
            if(flag == "1")
            {
                await _commonServices.ProcedureCallAsyn<string>(_configuration.GetConnectionString(db), Process_Order_Query(), _commonServices.AddParameter(new string[] { model.ORDER_MST_ID.ToString(), model.ORDER_DATE, model.CUSTOMER_ID.ToString(), model.CUSTOMER_CODE, model.ORDER_UNIT_ID.ToString(), model.INVOICE_UNIT_ID.ToString(), model.COMPANY_ID.ToString() }));

                listOfQuery.Add(_commonServices.AddQuery(Update_Order_Process_Status(), _commonServices.AddParameter(new string[] { model.ORDER_MST_ID.ToString() })));

                await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                await _logManager.AddOrUpdate(model.db_security, "Process Order", "INVOICE_MST", model.COMPANY_ID, model.UNIT_ID, Convert.ToInt32(model.ENTERED_BY), model.ENTERED_TERMINAL, "/SalesAndDistribution/Invoice/InsertOrUpdate", 0, "Order_id" + model.ORDER_MST_ID);


            }

            return "1";


        }
        
        public async Task<string> Delete_Processed_Order(string db, Order_Mst model)
        {
            List<QueryPattern> listOfQuery = new List<QueryPattern>();
            string result = string.Empty;
            List<string> _result = new List<string>();
            Result result_final = new Result();                                                                                          
            await _commonServices.ProcedureCallAsyn<string>(_configuration.GetConnectionString(db), Delete_Order_dtl_Query(), _commonServices.AddParameter(new string[] { model.ORDER_MST_ID.ToString() }));
           
            await _logManager.AddOrUpdate(model.db_security, "Delete Process Order Dtl", "INVOICE_MST", model.COMPANY_ID, model.UNIT_ID,Convert.ToInt32(model.ENTERED_BY), model.ENTERED_TERMINAL.ToString(), "/SalesAndDistribution/Invoice/InsertOrUpdate", 0, "Order_id" + model.ORDER_MST_ID);

            return "1";

        }

        public async Task<string> Delete_Order_full(string db, int ORDER_MST_ID)
        {
            try
            {
                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                string result = string.Empty;
                List<string> _result = new List<string>();
                Result result_final = new Result();
                await _commonServices.ProcedureCallAsyn<string>(_configuration.GetConnectionString(db), Delete_Order_full_Query(), _commonServices.AddParameter(new string[] { ORDER_MST_ID.ToString() }));


                return "1";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
           

        }
        public async Task<string> LoadInvoiceDetailsData(string db, string Mst_Id)
        {

            DataSet ds = new DataSet();
            DataTable _InvoiceMst = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), GetOrder_MST_BY_Id_Query(), _commonServices.AddParameter(new string[] { Mst_Id }));
            DataTable _InvoiceDtl = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), GetOrder_DTL_BY_MstId_Query(), _commonServices.AddParameter(new string[] { Mst_Id }));
            DataTable _InvoiceComboGifts = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Invoice_Details_InvoiceComboGift_Query(), _commonServices.AddParameter(new string[] { Mst_Id }));
            DataTable _InvoiceComboBonus = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Invoice_Details_InvoiceComboBonus_Query(), _commonServices.AddParameter(new string[] { Mst_Id }));

            List<int> InvDtl_Ids = new List<int>();
            string Dtl_Ids = "";
            foreach (DataRow row in _InvoiceDtl.Rows)
            {

                int dtl_id = Convert.ToInt32(row["DTL_ID"]);
                int st = InvDtl_Ids.Where(x => x == dtl_id).FirstOrDefault();
                if (st < 1)
                {
                    InvDtl_Ids.Add(dtl_id);
                }

            }

            for (int i = 0; i < InvDtl_Ids.Count; i++)
            {
                if (i == 0)
                {
                    Dtl_Ids = InvDtl_Ids[i].ToString();
                }
                else
                {
                    Dtl_Ids = Dtl_Ids + "," + InvDtl_Ids[i].ToString();

                }


            }
            
            string inv_gift = Invoice_Details_InvoiceGift_Query() + " WHERE B.ORDER_DTL_ID in (" + Dtl_Ids + ")";
            string inv_bonus = Invoice_Details_InvoiceBonus_Query() + " WHERE B.ORDER_DTL_ID in (" + Dtl_Ids + ")";

            DataTable _invoiceGifts = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), inv_gift, _commonServices.AddParameter(new string[] { }));

            DataTable _InvoiceBonus = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), inv_bonus, _commonServices.AddParameter(new string[] { }));

            ds.Tables.Add(_InvoiceComboGifts);
            ds.Tables.Add(_InvoiceComboBonus);
            ds.Tables.Add(_invoiceGifts);
            ds.Tables.Add(_InvoiceBonus);

            return _commonServices.DataSetToJSON(ds);

        }

        public async Task<string> LoadProductPerfactOrderQty(string db, Order_Mst model)
        {
            //SELECT* FROM TABLE(FN_FIND_PRODUCT_BONUS(:vCUSTOMER_CODE, :vORDER_TYPE, :vORDER_UNIT_ID, :vSKU_CODE, :vSKU_PRICE, :vORDER_QTY))
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), @"SELECT CUSTOMER_CODE, SKU_ID, SKU_CODE, BONUS_SKU_ID, BONUS_SKU_CODE, SLAB_QTY, DECLARE_BONUS_QTY, BONUS, COMPANY_ID, UNIT_ID FROM TABLE(FN_FIND_PRODUCT_BONUS(:param1, :param2 , :param3 , :param4 , :param5 , :param6))", _commonServices.AddParameter(new string[] { model.CUSTOMER_CODE, model.ORDER_TYPE, model.ORDER_UNIT_ID.ToString(), model.SKU_CODE, model.ORDER_AMOUNT.ToString(), model.REGION_ID.ToString() }));
            return _commonServices.DataTableToJSON(data);
        }

    }
}
