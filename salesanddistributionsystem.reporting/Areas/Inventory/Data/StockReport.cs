using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using SalesAndDistributionSystem.Domain.Models.ReportModels.Common.ProductReports;
using SalesAndDistributionSystem.Reporting.ReportConst;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace SalesAndDistributionSystem.Reporting.Areas.Inventory.Data
{
    public class StockReport
    {
        private readonly CommonServices commonServices = new CommonServices();

        //---------------Query Part--------------------------------

        //  public string GetConsumptionReport_Query() => 
        //@"          
        //Select 
        //STOCK_DATE,
        //SKU_CODE,
        //SKU_NAME,
        //STOCK_STY,
        //UNIT_NAME from STL_ERP_SDS.VW_REPORT_CONSUMTION Where COMPANY_ID = :param1";
        //        public string GetDailyStock_Query() => @"SELECT ROW_NUMBER() OVER (
        //      PARTITION BY M.COMPANY_ID,C.UNIT_ID
        //      ORDER BY M.COMPANY_ID,C.UNIT_ID, M.SKU_CODE
        //   )  AS SL, M.STOCK_DATE, M.SKU_CODE, P.SKU_NAME, P.PACK_SIZE , M.CLOSING_PASSED_QTY*I.UNIT_TP as VALUE_IN_TP , M.CLOSING_PASSED_QTY as STOCK_QTY, I.UNIT_TP, C.COMPANY_NAME, C.UNIT_NAME, CONCAT(C.UNIT_NAME,C.COMPANY_NAME) COMPANY_UNIT_NAME
        //FROM DATE_WISE_STOCK M 
        //Left outer join Product_Info P on P.SKU_ID= M.SKU_ID
        //LEFT outer join PRODUCT_PRICE_INFO I ON I.SKU_ID = M.SKU_ID 
        //LEFT OUTER JOIN Company_Info C ON C.COMPANY_ID= M.COMPANY_ID AND C.UNIT_ID= M.UNIT_ID
        //where M.COMPANY_ID = :param1
        //AND TRUNC(M.STOCK_DATE) = ( SELECT MAX(TRUNC(STOCK_DATE)) FROM DATE_WISE_STOCK WHERE SKU_ID=M.SKU_ID AND COMPANY_ID = M.COMPANY_ID AND UNIT_ID = M.UNIT_ID )
        //AND TRUNC(I.PRICE_EFFECT_DATE) = ( SELECT MAX(TRUNC(PRICE_EFFECT_DATE)) FROM PRODUCT_PRICE_INFO WHERE SKU_ID=I.SKU_ID AND COMPANY_ID = I.COMPANY_ID AND UNIT_ID = I.UNIT_ID )";
        public string GetDailyStock_Query() => @"SELECT S.COMPANY_ID,
       S.UNIT_ID,
       S.STOCK_DATE,
       S.SKU_CODE,
       B.SKU_NAME,
       B.PACK_SIZE,
       S.UNIT_TP,
       S.STOCK_QTY,
       FN_PENDIG_DIST_QTY(S.UNIT_ID, S.SKU_ID ) BOOKING_QTY,
       S.FREEZING_STOCK_QTY,
       NVL (S.STOCK_QTY, 0) * NVL (S.UNIT_TP, 0) VALUE_IN_TP,
       S.COMPANY_NAME,
       S.UNIT_NAME,
       CONCAT (S.UNIT_NAME, S.COMPANY_NAME) COMPANY_UNIT_NAME
  FROM (SELECT A.COMPANY_ID,
               A.UNIT_ID,
               A.STOCK_DATE,
               A.SKU_CODE,
               A.SKU_ID,
               FN_UNIT_WISE_SKU_PRICE (A.SKU_ID,
                                       A.SKU_CODE,
                                       A.COMPANY_ID,
                                       A.UNIT_ID)
                  UNIT_TP,
               A.CLOSING_PASSED_QTY AS STOCK_QTY,
               A.CLOSING_BOOKING_QTY AS BOOKING_QTY,
               A.CLOSING_QTN_QTY FREEZING_STOCK_QTY,
               FN_COMPANY_NAME (A.COMPANY_ID) COMPANY_NAME,
               FN_UNIT_NAME (A.COMPANY_ID, A.UNIT_ID) UNIT_NAME
          FROM DATE_WISE_STOCK A
         WHERE     A.COMPANY_ID = :param1
               AND TRUNC (A.STOCK_DATE) =
                      (SELECT MAX (TRUNC (STOCK_DATE))
                         FROM DATE_WISE_STOCK
                        WHERE     SKU_ID = A.SKU_ID
                              AND COMPANY_ID = :param1
                              AND UNIT_ID = A.UNIT_ID --AND TRUNC (STOCK_DATE) <= TRUNC (TO_DATE (:param2, 'dd/mm/rrrr'))
                                                     )) S,
       PRODUCT_INFO B
 WHERE S.COMPANY_ID = B.COMPANY_ID AND S.SKU_ID = B.SKU_ID";
        public string GetDateWiseStockReport_Query() => @"SELECT S.COMPANY_ID,
       S.UNIT_ID,
       S.STOCK_DATE,
       S.SKU_CODE,
       B.SKU_NAME,
       B.PACK_SIZE,
       S.UNIT_TP,
       S.STOCK_QTY,
       FN_PENDIG_DIST_QTY(S.UNIT_ID, S.SKU_ID ) BOOKING_QTY,
       S.FREEZING_STOCK_QTY,
       NVL (S.STOCK_QTY, 0) * NVL (S.UNIT_TP, 0) VALUE_IN_TP,
       S.COMPANY_NAME,
       S.UNIT_NAME,
       CONCAT (S.UNIT_NAME, S.COMPANY_NAME) COMPANY_UNIT_NAME
  FROM (SELECT A.COMPANY_ID,
               A.UNIT_ID,
               A.STOCK_DATE,
               A.SKU_CODE,
               A.SKU_ID,
               FN_UNIT_WISE_SKU_PRICE (A.SKU_ID,
                                       A.SKU_CODE,
                                       A.COMPANY_ID,
                                       A.UNIT_ID)
                  UNIT_TP,
               A.CLOSING_PASSED_QTY AS STOCK_QTY,
               A.CLOSING_BOOKING_QTY AS BOOKING_QTY,
               A.CLOSING_QTN_QTY FREEZING_STOCK_QTY,
               FN_COMPANY_NAME (A.COMPANY_ID) COMPANY_NAME,
               FN_UNIT_NAME (A.COMPANY_ID, A.UNIT_ID) UNIT_NAME
          FROM DATE_WISE_STOCK A
         WHERE     A.COMPANY_ID = :param1
               AND TRUNC (A.STOCK_DATE) =
                      (SELECT MAX (TRUNC (STOCK_DATE))
                         FROM DATE_WISE_STOCK
                        WHERE     SKU_ID = A.SKU_ID
                              AND UNIT_ID = A.UNIT_ID AND COMPANY_ID =  A.COMPANY_ID AND TRUNC (STOCK_DATE) <= TRUNC (TO_DATE (:param2, 'dd/mm/rrrr'))
                                                     )) S,
       PRODUCT_INFO B
 WHERE S.COMPANY_ID = B.COMPANY_ID AND S.SKU_ID = B.SKU_ID";
        public string GetRequsitionRaiseReport_Query() => @"select ROW_NUMBER() OVER (
      PARTITION BY M.REQUISITION_NO
      ORDER BY D.DTL_ID
   ) SL,CR.UNIT_NAME as REQUISITION_UNIT_ID,M.REMARKS MASTER_REMARKS, CI.UNIT_NAME as ISSUE_UNIT_ID, M.REQUISITION_NO,TO_CHAR(M.REQUISITION_DATE,'DD/MM/YYYY HH:MI AM') REQUISITION_DATE, M.TOTAL_VOLUME, M.TOTAL_WEIGHT ,I.SKU_CODE,I.PACK_SIZE, I.SKU_NAME,  D.SKU_ID, D.UNIT_TP, D.REQUISITION_QTY, D.REQUISITION_AMOUNT, D.REMARKS 
From DEPOT_REQUISITION_RAISE_MST M
left outer join DEPOT_REQUISITION_RAISE_DTL D ON D.MST_ID= M.MST_ID
left outer join PRODUCT_INFO I on I.SKU_ID= D.SKU_ID
LEFT OUTER JOIN Company_Info CI on CI.UNIT_ID= M.ISSUE_UNIT_ID AND CI.COMPANY_ID=M.COMPANY_ID
LEFT OUTER JOIN Company_Info CR on CR.UNIT_ID= M.REQUISITION_UNIT_ID AND CR.COMPANY_ID=M.COMPANY_ID
where M.COMPANY_ID = :param1 AND trunc(M.REQUISITION_DATE) BETWEEN TO_DATE(:param2, 'DD/MM/YYYY') AND TO_DATE(:param3, 'DD/MM/YYYY') AND M.REQUISITION_UNIT_ID= NVL(:param4,M.REQUISITION_UNIT_ID)";
        public string GetRequsitionIssueReport_Query() => @"select ROW_NUMBER() OVER (
      PARTITION BY M.ISSUE_NO
      ORDER BY D.DTL_ID
   ) SL,CR.UNIT_NAME as REQUISITION_UNIT_ID,M.REMARKS MASTER_REMARKS, CI.UNIT_NAME as ISSUE_UNIT_ID, M.REQUISITION_NO,TO_CHAR(M.REQUISITION_DATE,'DD/MM/YYYY HH:MI AM') REQUISITION_DATE,I.SKU_CODE,I.PACK_SIZE, I.SKU_NAME,  D.SKU_ID, D.UNIT_TP, M.ISSUE_NO,TO_CHAR(M.ISSUE_DATE,'DD/MM/YYYY HH:MI AM') ISSUE_DATE,D.REQUISITION_QTY, D.REQUISITIO_AMOUNT,D.ISSUE_QTY, D.ISSUE_AMOUNT, D.REMARKS 
,FLOOR((NVL(D.ISSUE_QTY,0))/I.SHIPPER_QTY)||' Box'||','||MOD((NVL(D.ISSUE_QTY,0)),I.SHIPPER_QTY)||' Pcs' BATCH_QTY_BOX_PCS
   ,FN_DELIVERY_SLIP_WEIGHT (D.SKU_ID,D.SKU_CODE,I.PACK_SIZE, (D.ISSUE_QTY) ,I.SHIPPER_QTY) GOODS_WEIGHT
   ,FN_DELIVERY_SLIP_VOLUME(D.SKU_ID,D.SKU_CODE,I.PACK_SIZE, NVL(D.ISSUE_QTY,0) ,I.SHIPPER_QTY) GOODS_VOLUME
From DEPOT_REQUISITION_ISSUE_MST M
left outer join DEPOT_REQUISITION_ISSUE_DTL D ON D.MST_ID= M.MST_ID
left outer join PRODUCT_INFO I on I.SKU_ID= D.SKU_ID
LEFT OUTER JOIN Company_Info CI ON CI.UNIT_ID= M.ISSUE_UNIT_ID AND CI.COMPANY_ID=M.COMPANY_ID
LEFT OUTER JOIN Company_Info CR on CR.UNIT_ID= M.REQUISITION_UNIT_ID AND CR.COMPANY_ID=M.COMPANY_ID
where M.COMPANY_ID = :param1 AND trunc(M.ISSUE_DATE) BETWEEN TO_DATE(:param2, 'DD/MM/YYYY') AND TO_DATE(:param3, 'DD/MM/YYYY') AND M.ISSUE_UNIT_ID= NVL(:param4,M.ISSUE_UNIT_ID)";
        public string GetRequsitionReceivedReport_Query() => @"select  ROWNUM as SL,CR.UNIT_NAME as REQUISITION_UNIT_ID,M.REMARKS MASTER_REMARKS, CI.UNIT_NAME as ISSUE_UNIT_ID
, M.REQUISITION_NO,TO_CHAR(MR.REQUISITION_DATE,'DD/MM/YYYY HH:MI AM') REQUISITION_DATE ,M.RECEIVE_NO
, TO_CHAR(M.RECEIVE_DATE,'DD/MM/YYYY HH:MI AM') RECEIVE_DATE , M.ISSUE_NO, TO_CHAR(M.ISSUE_DATE,'DD/MM/YYYY HH:MI AM') ISSUE_DATE, I.SKU_CODE,I.PACK_SIZE, I.SKU_NAME,  D.SKU_ID, D.UNIT_TP, D.RECEIVE_QTY , D.RECEIVE_AMOUNT, D.ISSUE_AMOUNT, D.ISSUE_QTY , D.REMARKS 
From DEPOT_REQUISITION_RCV_MST M
left outer join DEPOT_REQUISITION_RCV_DTL D ON D.MST_ID= M.MST_ID
LEFT OUTER JOIN DEPOT_REQUISITION_RAISE_MST MR ON MR.REQUISITION_NO= M.REQUISITION_NO
left outer join PRODUCT_INFO I on I.SKU_ID= D.SKU_ID
LEFT OUTER JOIN Company_Info CI on CI.UNIT_ID= M.ISSUE_UNIT_ID AND M.COMPANY_ID= CI.COMPANY_ID
LEFT OUTER JOIN Company_Info CR on CR.UNIT_ID= M.REQUISITION_UNIT_ID AND M.COMPANY_ID= CR.COMPANY_ID
where M.COMPANY_ID = :param1 AND trunc(M.RECEIVE_DATE ) BETWEEN TO_DATE(:param2, 'DD/MM/YYYY') AND TO_DATE(:param3, 'DD/MM/YYYY') AND M.REQUISITION_UNIT_ID= NVL(:param4,M.REQUISITION_UNIT_ID)";
        public string GetRequsitionReturnReport_Query() => @"select  ROWNUM as SL,CR.UNIT_NAME as RETURN_UNIT_ID,M.REMARKS MASTER_REMARKS, CRR.UNIT_NAME as RETURN_RCV_UNIT_ID
, M.REQUISITION_NO, TO_CHAR(MR.REQUISITION_DATE,'DD/MM/YYYY') REQUISITION_DATE ,M.RECEIVE_NO
, TO_CHAR(M.RECEIVE_DATE,'DD/MM/YYYY HH:MI AM') RECEIVE_DATE, M.RETURN_NO ,TO_CHAR(M.RETURN_DATE ,'DD/MM/YYYY HH:MI AM') RETURN_DATE , I.SKU_CODE,I.PACK_SIZE, I.SKU_NAME,  D.SKU_ID, D.UNIT_TP, D.RECEIVE_QTY , D.RECEIVE_AMOUNT, D.RETURN_AMOUNT , D.RETURN_QTY , D.REMARKS 
From DEPOT_REQUISITION_RETURN_MST M
left outer join DEPOT_REQUISITION_RETURN_DTL D ON D.MST_ID= M.MST_ID
LEFT OUTER JOIN DEPOT_REQUISITION_RAISE_MST MR ON MR.REQUISITION_NO= M.REQUISITION_NO
left outer join PRODUCT_INFO I on I.SKU_ID= D.SKU_ID
LEFT OUTER JOIN Company_Info CR on CR.UNIT_ID= M.RETURN_UNIT_ID AND CR.COMPANY_ID=M.COMPANY_ID
LEFT OUTER JOIN Company_Info CRR on CRR.UNIT_ID= M.RETURN_RCV_UNIT_ID AND CRR.COMPANY_ID=M.COMPANY_ID
where M.COMPANY_ID = :param1 AND trunc(M.RETURN_DATE  ) BETWEEN TO_DATE(:param2, 'DD/MM/YYYY') AND TO_DATE(:param3, 'DD/MM/YYYY') AND M.RETURN_UNIT_ID= NVL(:param4,M.RETURN_UNIT_ID)";
        public string GetRequsitionReturnReceivedReport_Query() => @"select  ROWNUM as SL,CR.UNIT_NAME as RETURN_UNIT_ID,M.REMARKS MASTER_REMARKS, CRR.UNIT_NAME as RETURN_RCV_UNIT_ID
, M.REQUISITION_NO,MR.REQUISITION_DATE ,M.RET_RCV_NO
,TO_CHAR(M.RET_RCV_DATE,'DD/MM/YYYY HH:MI AM') RET_RCV_DATE , M.RETURN_NO , TO_CHAR(M.RETURN_DATE,'DD/MM/YYYY HH:MI AM') RETURN_DATE , I.SKU_CODE,I.PACK_SIZE, I.SKU_NAME,  D.SKU_ID, D.UNIT_TP, 
D.RET_RCV_QTY , D.RET_RCV_AMOUNT , D.RETURN_AMOUNT , D.RETURN_QTY , D.REMARKS 
From DEPOT_REQUISITION_RET_RCV_MST M
left outer join DEPOT_REQUISITION_RET_RCV_DTL D ON D.MST_ID= M.MST_ID
LEFT OUTER JOIN DEPOT_REQUISITION_RAISE_MST MR ON MR.REQUISITION_NO= M.REQUISITION_NO
left outer join PRODUCT_INFO I on I.SKU_ID= D.SKU_ID
LEFT OUTER JOIN Company_Info CR on CR.UNIT_ID= M.RETURN_UNIT_ID AND CR.COMPANY_ID=M.COMPANY_ID
LEFT OUTER JOIN Company_Info CRR on CRR.UNIT_ID= M.RETURN_RCV_UNIT_ID AND CRR.COMPANY_ID=M.COMPANY_ID
where M.COMPANY_ID = :param1 AND trunc(M.RET_RCV_DATE ) BETWEEN TO_DATE(:param2, 'DD/MM/YYYY') AND TO_DATE(:param3, 'DD/MM/YYYY') AND M.RETURN_RCV_UNIT_ID= NVL(:param4,M.RETURN_RCV_UNIT_ID)";
        private string GetStockConsumptionReport_Query() => @"begin  :param1 := FN_STOCK_CONSUPTION( TO_DATE(:param2, 'DD/MM/YYYY'), TO_DATE(:param3, 'DD/MM/YYYY '), :param4, :param5 , :param6); end;";
        //        public string GetInvoiceReport_Query() => @"select '' SL, M.ORDER_NO, MI.MARKET_NAME, U.USER_NAME EMPLOYEE_NAME, M.DELIVERY_DATE, M.ORDER_DATE,M.REMARKS MASTER_REMARKS, 
        //C.CUSTOMER_CODE, C.CUSTOMER_NAME, C.CUSTOMER_ADDRESS, CI.CUSTOMER_TYPE_NAME ,
        //I.PACK_SIZE, I.SKU_NAME, D.UNIT_TP, D.ORDER_QTY, D.SPA_DISCOUNT_AMOUNT, D.REMARKS, D.SPA_TOTAL_AMOUNT
        //FROM ORDER_MST M
        //LEFT OUTER JOIN ORDER_DTL D ON M.ORDER_MST_ID= D.ORDER_MST_ID
        //LEFT OUTER JOIN CUSTOMER_INFO C ON C.CUSTOMER_ID= M.CUSTOMER_ID
        //LEFT OUTER JOIN PRODUCT_INFO I on I.SKU_ID= D.SKU_ID
        //LEFT OUTER JOIN User_Info U ON U.USER_ID=M.ENTERED_BY
        //LEFT OUTER JOIN MARKET_INFO MI ON MI.MARKET_ID= M.MARKET_ID
        //LEFT OUTER JOIN CUSTOMER_TYPE_INFO CI ON CI.CUSTOMER_TYPE_ID= C.CUSTOMER_TYPE_ID";
        public string GetPageHeaderReport_Query() => @"Select  
  COMPANY_NAME 
 ,COMPANY_ADDRESS
 from VW_REPORT_HEADER_DATA where COMPANY_ID = :param1 and ROWNUM =1";
        //------Execution Part------------------------

        public DataTable GetPageHeaderReport(ReportParams reportParameters)
        {
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, GetPageHeaderReport_Query(), commonServices.AddParameter(new string[] { reportParameters.COMPANY_ID.ToString() }));

        }
        public DataTable GetDailyStockReport(ReportParams reportParameters) //For Daily Stock Report && Stock Report (Date Wise) Report
        {

            string Query = GetDailyStock_Query();
            if (reportParameters.BRAND_ID != null && reportParameters.BRAND_ID != "" && reportParameters.BRAND_ID != "undefined")
            {
                Query = Query + " AND B.BRAND_ID in (" + reportParameters.BRAND_ID + ")";
            }
            if (reportParameters.CATEGORY_ID != null && reportParameters.CATEGORY_ID != "" && reportParameters.CATEGORY_ID != "undefined")
            {
                Query = Query + " AND B.CATEGORY_ID in (" + reportParameters.CATEGORY_ID + ")";
            }
            if (reportParameters.BASE_PRODUCT_ID != null && reportParameters.BASE_PRODUCT_ID != "" && reportParameters.BASE_PRODUCT_ID != "undefined")
            {
                Query = Query + " AND B.BASE_PRODUCT_ID in (" + reportParameters.BASE_PRODUCT_ID + ")";
            }
            if (reportParameters.GROUP_ID != null && reportParameters.GROUP_ID != "" && reportParameters.GROUP_ID != "undefined")
            {
                Query = Query + " AND B.GROUP_ID in (" + reportParameters.GROUP_ID + ")";
            }
            if (reportParameters.PRODUCT_SEASON_ID != null && reportParameters.PRODUCT_SEASON_ID != "" && reportParameters.PRODUCT_SEASON_ID != "undefined")
            {
                Query = Query + " AND B.PRODUCT_SEASON_ID in (" + reportParameters.PRODUCT_SEASON_ID + ")";
            }
            if (reportParameters.PRIMARY_PRODUCT_ID != null && reportParameters.PRIMARY_PRODUCT_ID != "" && reportParameters.PRIMARY_PRODUCT_ID != "undefined")
            {
                Query = Query + " AND B.PRIMARY_PRODUCT_ID in (" + reportParameters.PRIMARY_PRODUCT_ID + ")";
            }
            if (reportParameters.PRODUCT_TYPE_ID != null && reportParameters.PRIMARY_PRODUCT_ID != "" && reportParameters.PRIMARY_PRODUCT_ID != "undefined")
            {

                Query = Query + " AND B.PRODUCT_TYPE_ID in (" + reportParameters.PRODUCT_TYPE_ID + ")";
            }

            if (reportParameters.UNIT_ID != "ALL" && reportParameters.UNIT_ID != null && reportParameters.UNIT_ID != "" && reportParameters.UNIT_ID != "undefined")
            {
                Query = Query + " AND S.UNIT_ID in (" + reportParameters.UNIT_ID + ")";
            }
            if (reportParameters.SKU_ID != "ALL" && reportParameters.SKU_ID != null && reportParameters.SKU_ID != "" && reportParameters.SKU_ID != "undefined")
            {
                Query = Query + " AND B.SKU_ID in (" + reportParameters.SKU_ID + ")";
            }




            Query = Query + " ORDER BY S.COMPANY_ID, S.UNIT_ID,B.SKU_NAME, B.SKU_ID";

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { reportParameters.COMPANY_ID.ToString() }));


        }
        public DataTable GetDateWiseStockReport(ReportParams reportParameters) //For Stock Report (Date Wise)
        {

            string Query = GetDateWiseStockReport_Query();
            if (reportParameters.BRAND_ID != null && reportParameters.BRAND_ID != "" && reportParameters.BRAND_ID != "undefined")
            {
                Query = Query + " AND B.BRAND_ID in (" + reportParameters.BRAND_ID + ")";
            }
            if (reportParameters.CATEGORY_ID != null && reportParameters.CATEGORY_ID != "" && reportParameters.CATEGORY_ID != "undefined")
            {
                Query = Query + " AND B.CATEGORY_ID in (" + reportParameters.CATEGORY_ID + ")";
            }
            if (reportParameters.BASE_PRODUCT_ID != null && reportParameters.BASE_PRODUCT_ID != "" && reportParameters.BASE_PRODUCT_ID != "undefined")
            {
                Query = Query + " AND B.BASE_PRODUCT_ID in (" + reportParameters.BASE_PRODUCT_ID + ")";
            }
            if (reportParameters.GROUP_ID != null && reportParameters.GROUP_ID != "" && reportParameters.GROUP_ID != "undefined")
            {
                Query = Query + " AND B.GROUP_ID in (" + reportParameters.GROUP_ID + ")";
            }
            if (reportParameters.PRODUCT_SEASON_ID != null && reportParameters.PRODUCT_SEASON_ID != "" && reportParameters.PRODUCT_SEASON_ID != "undefined")
            {
                Query = Query + " AND B.PRODUCT_SEASON_ID in (" + reportParameters.PRODUCT_SEASON_ID + ")";
            }
            if (reportParameters.PRIMARY_PRODUCT_ID != null && reportParameters.PRIMARY_PRODUCT_ID != "" && reportParameters.PRIMARY_PRODUCT_ID != "undefined")
            {
                Query = Query + " AND B.PRIMARY_PRODUCT_ID in (" + reportParameters.PRIMARY_PRODUCT_ID + ")";
            }
            if (reportParameters.PRODUCT_TYPE_ID != null && reportParameters.PRIMARY_PRODUCT_ID != "" && reportParameters.PRIMARY_PRODUCT_ID != "undefined")
            {

                Query = Query + " AND B.PRODUCT_TYPE_ID in (" + reportParameters.PRODUCT_TYPE_ID + ")";
            }

            if (reportParameters.UNIT_ID != "ALL" && reportParameters.UNIT_ID != null && reportParameters.UNIT_ID != "" && reportParameters.UNIT_ID != "undefined")
            {
                Query = Query + " AND S.UNIT_ID in (" + reportParameters.UNIT_ID + ")";
            }
            if (reportParameters.SKU_ID != "ALL" && reportParameters.SKU_ID != null && reportParameters.SKU_ID != "" && reportParameters.SKU_ID != "undefined")
            {
                Query = Query + " AND B.SKU_ID in (" + reportParameters.SKU_ID + ")";
            }
            Query = Query + " ORDER BY S.COMPANY_ID, S.UNIT_ID,B.SKU_NAME, B.SKU_ID";

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_TO }));


        }

        public DataTable GetRequsitionRaiseReport(ReportParams reportParameters)
        {
            string Query = GetRequsitionRaiseReport_Query();
            if (reportParameters.MST_ID != "undefined" && !String.IsNullOrWhiteSpace(reportParameters.MST_ID))
            {
                Query = Query + " and M.MST_ID IN(" + reportParameters.MST_ID + ")";
            }

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.UNIT_ID }));



        }
        public DataTable GetRequsitionIssueReport(ReportParams reportParameters)
        {
            string Query = GetRequsitionIssueReport_Query();
            if (reportParameters.MST_ID != "undefined" && !String.IsNullOrWhiteSpace(reportParameters.MST_ID))
            {
                Query = Query + " and M.MST_ID IN(" + reportParameters.MST_ID + ")";
            }

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.UNIT_ID }));
        }
        public DataTable GetRequsitionReceivedReport(ReportParams reportParameters)
        {
            string Query = GetRequsitionReceivedReport_Query();
            if (reportParameters.MST_ID != "undefined" && !String.IsNullOrWhiteSpace(reportParameters.MST_ID))
            {
                Query = Query + " and M.MST_ID IN(" + reportParameters.MST_ID + ")";
            }

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.UNIT_ID }));
        }
        public DataTable GetRequsitionReturnReport(ReportParams reportParameters)
        {
            string Query = GetRequsitionReturnReport_Query();
            if (reportParameters.MST_ID != "undefined" && !String.IsNullOrWhiteSpace(reportParameters.MST_ID))
            {
                Query = Query + " and M.MST_ID IN(" + reportParameters.MST_ID + ")";
            }

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.UNIT_ID }));
        }
        public DataTable GetRequsitionReturnReveivedReport(ReportParams reportParameters)
        {
            string Query = GetRequsitionReturnReceivedReport_Query();
            if (reportParameters.MST_ID != "undefined" && !String.IsNullOrWhiteSpace(reportParameters.MST_ID))
            {
                Query = Query + " and M.MST_ID IN(" + reportParameters.MST_ID + ")";
            }

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.UNIT_ID }));
        }
        public DataTable GetStockConsumptionReport(ReportParams reportParameters)
        {
            string Query = GetStockConsumptionReport_Query();
            if (reportParameters.MST_ID != "undefined" && !String.IsNullOrWhiteSpace(reportParameters.MST_ID))
            {
                Query = Query + " and M.MST_ID IN(" + reportParameters.MST_ID + ")";
            }

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.COMPANY_ID.ToString(), reportParameters.UNIT_ID, reportParameters.SKU_ID }));
        }
        public DataTable GetBatchWiseStockReport(ReportParams reportParameters)
        {
            string Query = @"SELECT B.COMPANY_ID,

       B.UNIT_ID,

       B.SKU_CODE,

       FN_SKU_NAME (B.COMPANY_ID, B.SKU_ID) SKU_NAME,

       FN_UNIT_WISE_SKU_MRP (B.SKU_ID,  B.SKU_CODE, B.COMPANY_ID,  B.UNIT_ID,    B.BATCH_ID) MRP,

       FN_PACK_SIZE (B.COMPANY_ID, B.SKU_ID) PACK_SIZE,

       B.UNIT_TP,

       B.BATCH_NO,

       NVL(B.PASSED_QTY,0) STOCK_QTY,

       NVL(B.QTN_QTY,0) FREEZING_STOCK_QTY,

       NVL(C.BOOKING_STOCK_QTY,0) BOOKING_QTY,      

       NVL(B.PASSED_QTY,0)+NVL(B.QTN_QTY,0)+ NVL(C.BOOKING_STOCK_QTY,0) TOTAL_PHYSICAL_STOCK,

       (NVL(B.PASSED_QTY,0)+NVL(B.QTN_QTY,0)+ NVL(C.BOOKING_STOCK_QTY,0)) * NVL (B.UNIT_TP, 0) VALUE_IN_TP,

       FN_COMPANY_NAME (B.COMPANY_ID) COMPANY_NAME,

       FN_UNIT_NAME (B.COMPANY_ID, B.UNIT_ID) UNIT_NAME,                 

       CONCAT (FN_COMPANY_NAME(B.COMPANY_ID), FN_UNIT_NAME(B.COMPANY_ID, B.UNIT_ID)) COMPANY_UNIT_NAME

FROM BATCH_WISE_STOCK B, VW_BATCH_WISE_BOOKING_STOCK C

WHERE B.BATCH_ID=C.BATCH_ID(+)

AND   B.SKU_ID=C.SKU_ID(+)

AND   NVL(B.PASSED_QTY,0)+NVL(B.QTN_QTY,0)+NVL(C.BOOKING_STOCK_QTY,0)>0

AND   B.COMPANY_ID= :param1 ";

            if (reportParameters.SKU_ID != "ALL" && !string.IsNullOrEmpty(reportParameters.SKU_ID) && reportParameters.SKU_ID != "undefined")
            {
                Query += " AND B.SKU_ID IN (" + reportParameters.SKU_ID + ")";
            }

            if (reportParameters.BATCH_NO != "ALL" && !string.IsNullOrEmpty(reportParameters.BATCH_NO) && reportParameters.BATCH_NO != "undefined")
            {
                Query += " AND B.BATCH_NO IN ('" + string.Join("','", reportParameters.BATCH_NO.Split(',')) + "')";
            }

            if (reportParameters.UNIT_ID != "ALL" && !string.IsNullOrEmpty(reportParameters.UNIT_ID))
            {
                Query = Query + " AND B.UNIT_ID =" + reportParameters.UNIT_ID + "";
            }

            if (reportParameters.BRAND_ID != null && reportParameters.BRAND_ID != "" && reportParameters.BRAND_ID != "undefined")
            {
                Query = Query + " AND p.BRAND_ID in (" + reportParameters.BRAND_ID + ")";
            }
            if (reportParameters.CATEGORY_ID != null && reportParameters.CATEGORY_ID != "" && reportParameters.CATEGORY_ID != "undefined")
            {
                Query = Query + " AND p.CATEGORY_ID in (" + reportParameters.CATEGORY_ID + ")";
            }
            if (reportParameters.BASE_PRODUCT_ID != null && reportParameters.BASE_PRODUCT_ID != "" && reportParameters.BASE_PRODUCT_ID != "undefined")
            {
                Query = Query + " AND p.BASE_PRODUCT_ID in (" + reportParameters.BASE_PRODUCT_ID + ")";
            }
            if (reportParameters.GROUP_ID != null && reportParameters.GROUP_ID != "" && reportParameters.GROUP_ID != "undefined")
            {
                Query = Query + " AND p.GROUP_ID in (" + reportParameters.GROUP_ID + ")";
            }
            if (reportParameters.PRODUCT_SEASON_ID != null && reportParameters.PRODUCT_SEASON_ID != "" && reportParameters.PRODUCT_SEASON_ID != "undefined")
            {
                Query = Query + " AND p.PRODUCT_SEASON_ID in (" + reportParameters.PRODUCT_SEASON_ID + ")";
            }
            if (reportParameters.PRIMARY_PRODUCT_ID != null && reportParameters.PRIMARY_PRODUCT_ID != "" && reportParameters.PRIMARY_PRODUCT_ID != "undefined")
            {
                Query = Query + " AND p.PRIMARY_PRODUCT_ID in (" + reportParameters.PRIMARY_PRODUCT_ID + ")";
            }
            if (reportParameters.PRODUCT_TYPE_ID != null && reportParameters.PRIMARY_PRODUCT_ID != "" && reportParameters.PRIMARY_PRODUCT_ID != "undefined")
            {

                Query = Query + " AND p.PRODUCT_TYPE_ID in (" + reportParameters.PRODUCT_TYPE_ID + ")";
            }
            Query = Query + " ORDER BY B.COMPANY_ID, B.UNIT_ID,FN_SKU_NAME (B.COMPANY_ID, B.SKU_ID), B.SKU_ID, B.BATCH_NO";
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { reportParameters.COMPANY_ID.ToString() }));
        }
        public DataTable GetCurrentBatchFreeezReport(ReportParams reportParameters)
        {
            string Query = @"SELECT B.COMPANY_ID,

       B.UNIT_ID,

       B.SKU_CODE,

       FN_SKU_NAME (B.COMPANY_ID, B.SKU_ID) SKU_NAME,

       FN_UNIT_WISE_SKU_MRP (B.SKU_ID,  B.SKU_CODE, B.COMPANY_ID,  B.UNIT_ID,    B.BATCH_ID) MRP,

       FN_PACK_SIZE (B.COMPANY_ID, B.SKU_ID) PACK_SIZE,

       B.UNIT_TP,

       B.BATCH_NO,

       NVL(B.PASSED_QTY,0) STOCK_QTY,

       NVL(B.QTN_QTY,0) FREEZING_STOCK_QTY,

       NVL(C.BOOKING_STOCK_QTY,0) BOOKING_QTY,      

       NVL(B.PASSED_QTY,0)+NVL(B.QTN_QTY,0)+ NVL(C.BOOKING_STOCK_QTY,0) TOTAL_PHYSICAL_STOCK,

       (NVL(B.PASSED_QTY,0)+NVL(B.QTN_QTY,0)+ NVL(C.BOOKING_STOCK_QTY,0)) * NVL (B.UNIT_TP, 0) VALUE_IN_TP,

       FN_COMPANY_NAME (B.COMPANY_ID) COMPANY_NAME,

       FN_UNIT_NAME (B.COMPANY_ID, B.UNIT_ID) UNIT_NAME,                 

       CONCAT (FN_COMPANY_NAME(B.COMPANY_ID), FN_UNIT_NAME(B.COMPANY_ID, B.UNIT_ID)) COMPANY_UNIT_NAME

FROM BATCH_WISE_STOCK B, VW_BATCH_WISE_BOOKING_STOCK C

WHERE B.BATCH_ID=C.BATCH_ID(+)

AND   B.SKU_ID=C.SKU_ID(+)

AND   NVL(B.PASSED_QTY,0)+NVL(B.QTN_QTY,0)+NVL(C.BOOKING_STOCK_QTY,0)>0

AND   NVL(B.QTN_QTY,0)>0

AND   B.COMPANY_ID= :param1 ";

            if (reportParameters.SKU_ID != "ALL" && !string.IsNullOrEmpty(reportParameters.SKU_ID) && reportParameters.SKU_ID != "undefined")
            {
                Query += " AND B.SKU_ID IN (" + reportParameters.SKU_ID + ")";
            }

            if (reportParameters.BATCH_NO != "ALL" && !string.IsNullOrEmpty(reportParameters.BATCH_NO) && reportParameters.BATCH_NO != "undefined")
            {
                Query += " AND B.BATCH_NO IN ('" + string.Join("','", reportParameters.BATCH_NO.Split(',')) + "')";
            }

            if (reportParameters.UNIT_ID != "ALL" && !string.IsNullOrEmpty(reportParameters.UNIT_ID))
            {
                Query = Query + " AND B.UNIT_ID =" + reportParameters.UNIT_ID + "";
            }

            if (reportParameters.BRAND_ID != null && reportParameters.BRAND_ID != "" && reportParameters.BRAND_ID != "undefined")
            {
                Query = Query + " AND p.BRAND_ID in (" + reportParameters.BRAND_ID + ")";
            }
            if (reportParameters.CATEGORY_ID != null && reportParameters.CATEGORY_ID != "" && reportParameters.CATEGORY_ID != "undefined")
            {
                Query = Query + " AND p.CATEGORY_ID in (" + reportParameters.CATEGORY_ID + ")";
            }
            if (reportParameters.BASE_PRODUCT_ID != null && reportParameters.BASE_PRODUCT_ID != "" && reportParameters.BASE_PRODUCT_ID != "undefined")
            {
                Query = Query + " AND p.BASE_PRODUCT_ID in (" + reportParameters.BASE_PRODUCT_ID + ")";
            }
            if (reportParameters.GROUP_ID != null && reportParameters.GROUP_ID != "" && reportParameters.GROUP_ID != "undefined")
            {
                Query = Query + " AND p.GROUP_ID in (" + reportParameters.GROUP_ID + ")";
            }
            if (reportParameters.PRODUCT_SEASON_ID != null && reportParameters.PRODUCT_SEASON_ID != "" && reportParameters.PRODUCT_SEASON_ID != "undefined")
            {
                Query = Query + " AND p.PRODUCT_SEASON_ID in (" + reportParameters.PRODUCT_SEASON_ID + ")";
            }
            if (reportParameters.PRIMARY_PRODUCT_ID != null && reportParameters.PRIMARY_PRODUCT_ID != "" && reportParameters.PRIMARY_PRODUCT_ID != "undefined")
            {
                Query = Query + " AND p.PRIMARY_PRODUCT_ID in (" + reportParameters.PRIMARY_PRODUCT_ID + ")";
            }
            if (reportParameters.PRODUCT_TYPE_ID != null && reportParameters.PRIMARY_PRODUCT_ID != "" && reportParameters.PRIMARY_PRODUCT_ID != "undefined")
            {

                Query = Query + " AND p.PRODUCT_TYPE_ID in (" + reportParameters.PRODUCT_TYPE_ID + ")";
            }
            Query = Query + " ORDER BY B.COMPANY_ID, B.UNIT_ID,FN_SKU_NAME (B.COMPANY_ID, B.SKU_ID), B.SKU_ID, B.BATCH_NO";
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { reportParameters.COMPANY_ID.ToString() }));
        }
        public DataTable GetBatchWiseStockV2Report(ReportParams reportParameters)
        {
            string Query = @"SELECT A.COMPANY_ID,
       A.UNIT_ID,
       A.SKU_ID,
       A.SKU_CODE,
       FN_SKU_NAME (A.COMPANY_ID, A.SKU_ID) SKU_NAME,
       A.UNIT_TP,
       FN_PACK_SIZE (A.COMPANY_ID, A.SKU_ID) PACK_SIZE,
       A.MRP,
       A.BATCH_ID,
       A.BATCH_NO,
       NVL(A.PASSED_QTY,0) SALEABLE_STOCK_QTY,
       NVL(A.QTN_QTY,0) FREEZE_STOCK_QTY,
       NVL(B.BOOKED_STOCK_QTY,0) BOOKED_STOCK_QTY,
       NVL(A.PASSED_QTY,0)+NVL(A.QTN_QTY,0)+NVL(B.BOOKED_STOCK_QTY,0) TOTAL_PHYSICAL_STOCK_QTY,
       NVL(A.UNIT_TP,0)*(NVL(A.PASSED_QTY,0)+NVL(A.QTN_QTY,0)+NVL(B.BOOKED_STOCK_QTY,0)) VALUE_IN_TP,
       FN_COMPANY_NAME (A.COMPANY_ID) COMPANY_NAME,
       FN_UNIT_NAME (A.COMPANY_ID, A.UNIT_ID) UNIT_NAME,           
       CONCAT (FN_COMPANY_NAME(A.COMPANY_ID), FN_UNIT_NAME(A.COMPANY_ID, A.UNIT_ID)) COMPANY_UNIT_NAME
FROM   BATCH_WISE_STOCK A,
   (
    SELECT A.COMPANY_ID,
          A.UNIT_ID,
          A.SKU_ID,
          A.BATCH_ID,
          NVL (A.INVOICE_QTY, 0) - NVL (B.DISTRIBUTION_QTY, 0) BOOKED_STOCK_QTY
    FROM 
        (       
         SELECT COMPANY_ID,
                UNIT_ID,
                SKU_ID,
                BATCH_ID,
                NVL(INVOICE_QTY,0)+NVL(BONUS_QTY,0) INVOICE_QTY
         FROM
             (                              
             SELECT COMPANY_ID,
                    UNIT_ID,
                    SKU_ID,
                    BATCH_ID,
                    SUM(NVL (INVOICE_QTY,0)) INVOICE_QTY,
                    SUM(NVL (BONUS_QTY, 0)) BONUS_QTY
             FROM 
                 ( 
                 SELECT A.COMPANY_ID,
                        A.INVOICE_UNIT_ID UNIT_ID,
                        C.SKU_ID,
                        C.BATCH_ID,
                        NVL (C.ISSUE_QTY, 0) INVOICE_QTY,
                        0 BONUS_QTY
                 FROM   INVOICE_MST A, INVOICE_DTL B, INVOICE_ISSUE C
                 WHERE  A.MST_ID = B.MST_ID 
                 AND    B.DTL_ID = C.DTL_ID                         
                 UNION ALL                             
                 SELECT A.COMPANY_ID,
                        A.INVOICE_UNIT_ID UNIT_ID,
                        C.SKU_ID,
                        C.BATCH_ID,
                        0 INVOICE_QTY,
                        NVL (C.BONUS_QTY, 0) BONUS_QTY
                 FROM   INVOICE_MST A, INVOICE_DTL B, INVOICE_BONUS C
                 WHERE  A.MST_ID = B.MST_ID 
                 AND    B.DTL_ID = C.DTL_ID                               
                 UNION ALL                             
                 SELECT A.COMPANY_ID,
                        A.INVOICE_UNIT_ID UNIT_ID,
                        B.SKU_ID,
                        B.BATCH_ID,
                        0 INVOICE_QTY,
                        NVL (B.BONUS_QTY, 0) BONUS_QTY
                 FROM   INVOICE_MST A, INVOICE_COMBO_BONUS B
                 WHERE  A.MST_ID = B.MST_ID                               
                 )
             GROUP BY COMPANY_ID,UNIT_ID,SKU_ID,BATCH_ID
            )                                                         
        ) A,                      
        (            
            SELECT  COMPANY_ID,
                    UNIT_ID,
                    SKU_ID,
                    BATCH_ID,
                    NVL(DISTRIBUTION_INVOICE_QTY,0)+ NVL(DISTRIBUTION_BONUS_QTY,0)DISTRIBUTION_QTY
            FROM 
                (                
                SELECT COMPANY_ID,
                        UNIT_ID,
                        SKU_ID,
                        BATCH_ID,
                        SUM (NVL (DISTRIBUTION_INVOICE_QTY, 0))DISTRIBUTION_INVOICE_QTY,
                        SUM(NVL (DISTRIBUTION_BONUS_QTY, 0))DISTRIBUTION_BONUS_QTY
                FROM 
                   (
                     SELECT A.COMPANY_ID,
                            A.INVOICE_UNIT_ID UNIT_ID,
                            D.SKU_ID,
                            D.BATCH_ID,
                            D.DISTRIBUTION_INVOICE_QTY,
                            0 DISTRIBUTION_BONUS_QTY
                     FROM   DEPOT_CUSTOMER_DIST_MST A,DEPOT_CUSTOMER_DIST_INVOICE B,DEPOT_CUSTOMER_DIST_PRODUCT C,DEPOT_CUSTOMER_DIST_PROD_BATCH D
                     WHERE  A.MST_ID = B.MST_ID
                     AND    B.DEPOT_INV_ID = C.DEPOT_INV_ID
                     AND    C.DEPOT_PRODUCT_ID = D.DEPOT_PRODUCT_ID 
                     AND    NVL (D.DISTRIBUTION_INVOICE_QTY, 0) > 0
                     UNION ALL
                     SELECT A.COMPANY_ID,
                            A.INVOICE_UNIT_ID UNIT_ID,
                            D.SKU_ID,
                            D.BATCH_ID,
                            0 DISTRIBUTION_INVOICE_QTY,
                            D.DISTRIBUTION_BONUS_QTY
                     FROM   DEPOT_CUSTOMER_DIST_MST A,DEPOT_CUSTOMER_DIST_INVOICE B,DEPOT_CUSTOMER_DIST_PRODUCT C,DEPOT_CUSTOMER_DIST_PROD_BATCH D
                     WHERE  A.MST_ID = B.MST_ID
                     AND    B.DEPOT_INV_ID = C.DEPOT_INV_ID
                     AND    C.DEPOT_PRODUCT_ID = D.DEPOT_PRODUCT_ID 
                     AND    NVL (D.DISTRIBUTION_BONUS_QTY, 0) > 0
                )
                GROUP BY COMPANY_ID,UNIT_ID,SKU_ID,BATCH_ID                      
           )           
      ) B
    WHERE A.COMPANY_ID = B.COMPANY_ID(+)
    AND   A.UNIT_ID = B.UNIT_ID(+)
    AND   A.SKU_ID = B.SKU_ID(+)
    AND   A.BATCH_ID = B.BATCH_ID(+)
    AND   NVL (A.INVOICE_QTY, 0) - NVL (B.DISTRIBUTION_QTY, 0) > 0

)B
WHERE A.COMPANY_ID=B.COMPANY_ID(+)
AND   A.UNIT_ID=B.UNIT_ID(+)
AND   A.SKU_ID=B.SKU_ID(+)
AND   A.BATCH_ID=B.BATCH_ID(+)
AND   A.COMPANY_ID= :param1
AND   A.UNIT_ID = NVL(:param2, A.UNIT_ID)
AND   NVL(A.PASSED_QTY,0)+NVL(A.QTN_QTY,0)+NVL(B.BOOKED_STOCK_QTY,0)>0";

            if (reportParameters.SKU_ID != "ALL" && !string.IsNullOrEmpty(reportParameters.SKU_ID) && reportParameters.SKU_ID != "undefined")
            {
                Query += " AND A.SKU_ID IN (" + reportParameters.SKU_ID + ")";
            }


            Query = Query + " ORDER BY A.COMPANY_ID,A.UNIT_ID,A.SKU_CODE";
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { reportParameters.COMPANY_ID.ToString(),reportParameters.UNIT_ID=="ALL"?null: reportParameters.UNIT_ID }));
        }
        public DataTable GetFinishedGoodsHoldingTimeReport(ReportParams reportParameters)
        {
            string Query = @"SELECT A.COMPANY_ID,
       FN_COMPANY_NAME(A.COMPANY_ID) COMPANY_NAME,
       A.UNIT_ID,
       FN_UNIT_NAME(A.COMPANY_ID,A.UNIT_ID) UNIT_NAME,
       A.SKU_ID,
       A.SKU_CODE,
       B.SKU_NAME,
       B.PACK_SIZE,
       A.UNIT_TP,
       A.MRP,
       A.BATCH_NO,
       A.PASSED_QTY STOCK_QTY,
       C.RECEIVE_DATE,
       C.MFG_DATE,
       C.EXPIRY_DATE,      
       CEIL(MONTHS_BETWEEN(TO_DATE(SYSDATE,'DD/MM/RRRR'),TO_DATE(C.RECEIVE_DATE,'DD/MM/RRRR'))) STOCK_HOLDING_MONTHS,
       CEIL(MONTHS_BETWEEN(TO_DATE(C.EXPIRY_DATE,'DD/MM/RRRR'),TO_DATE(SYSDATE,'DD/MM/RRRR'))) MONTHS_DUE_TO_EXPIRE      
FROM   BATCH_WISE_STOCK A,PRODUCT_INFO B,VW_BATCH_WISE_RECEVING_TP C
WHERE  A.COMPANY_ID=B.COMPANY_ID
AND    A.SKU_ID=B.SKU_ID
AND    A.COMPANY_ID=C.COMPANY_ID
AND    A.SKU_ID=C.SKU_ID
AND    A.BATCH_ID=C.BATCH_ID
AND    A.COMPANY_ID=:param1
AND    A.UNIT_ID=NVL(:param2, A.UNIT_ID)
AND    A.PASSED_QTY>0 ";

            if (reportParameters.SKU_ID != "ALL" && !string.IsNullOrEmpty(reportParameters.SKU_ID) && reportParameters.SKU_ID != "undefined")
            {
                Query += " AND A.SKU_ID IN (" + reportParameters.SKU_ID + ")";
            }


            Query = Query + " ORDER BY A.COMPANY_ID,A.UNIT_ID,B.SKU_NAME,C.RECEIVE_DATE DESC ";
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { reportParameters.COMPANY_ID.ToString(), reportParameters.UNIT_ID == "ALL" ? null : reportParameters.UNIT_ID }));
        }
        //public DataTable GetInvoiceReport(ReportParams reportParameters)
        //{
        //    string Query = GetInvoiceReport_Query();
        //    if (reportParameters.MST_ID != "undefined" && !String.IsNullOrWhiteSpace(reportParameters.MST_ID))
        //    {
        //        Query = Query + " and M.MST_ID IN(" + reportParameters.MST_ID + ")";
        //    }

        //    return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO }));



        //}
        public DataTable BatchFreezing(ReportParams reportParameters)
        {
            var query = @"Select * from (SELECT 
         0 DTL_ID,0 MST_ID,
         (Select F.FREEZE_DATE from BATCH_FREEZING_DTL F WHERE F.BATCH_NO = U.BATCH_NO AND ROWNUM =1) FREEZE_DATE,
         FN_COMPANY_NAME (U.COMPANY_ID) COMPANY_NAME,
         FN_UNIT_NAME (U.COMPANY_ID, U.UNIT_ID) UNIT_NAME,
         FN_SKU_NAME (U.COMPANY_ID, U.SKU_ID) SKU_NAME,
         FN_PACK_SIZE(U.COMPANY_ID, U.SKU_ID) PACK_SIZE,
         U.COMPANY_ID,
         U.UNIT_ID,
         U.SKU_ID,
         U.SKU_CODE,
         U.UNIT_TP,
         U.BATCH_ID,
         U.BATCH_NO,
         SUM(NVL(U.BATCH_QTY,0)) BATCH_QTY,
         SUM(NVL(U.FREEZE_QTY,0)) FREEZE_QTY,
         SUM(NVL (U.UN_FREEZE_QTY, 0)) UN_FREEZE_QTY,
         SUM(NVL (U.FREEZE_QTY, 0)) - SUM( NVL (U.UN_FREEZE_QTY, 0)) CURRENT_STOCK,
         0 ENTERED_BY,
         '' ENTERED_DATE
    FROM  BATCH_UN_FREEZING_DTL U     
   WHERE NVL (U.FREEZE_QTY, 0) - NVL (U.UN_FREEZE_QTY, 0) >= 0 AND U.COMPANY_ID =:param1 ";
            var query2 = @"UNION ALL
 SELECT  0 DTL_ID,
         0 MST_ID,
         U.FREEZE_DATE,
         FN_COMPANY_NAME (U.COMPANY_ID) COMPANY_NAME,
         FN_UNIT_NAME (U.COMPANY_ID, U.UNIT_ID) UNIT_NAME,
         FN_SKU_NAME (U.COMPANY_ID, U.SKU_ID) SKU_NAME,
         FN_PACK_SIZE(U.COMPANY_ID, U.SKU_ID) PACK_SIZE,
         U.COMPANY_ID,
         U.UNIT_ID,
         U.SKU_ID,
         U.SKU_CODE,
         U.UNIT_TP,
         U.BATCH_ID,
         U.BATCH_NO,
         U.BATCH_QTY,
         U.FREEZE_QTY FREEZE_QTY,
         0 UN_FREEZE_QTY,
         U.FREEZE_QTY CURRENT_STOCK,
         0 ENTERED_BY,
         '' ENTERED_DATE
    FROM  BATCH_FREEZING_DTL U     
   WHERE U.BATCH_NO not in (Select F.BATCH_NO FROM BATCH_UN_FREEZING_DTL F ) AND U.COMPANY_ID =:param1 ";

            var parameters = new List<string> { reportParameters.COMPANY_ID.ToString() };

            if (reportParameters.UNIT_ID != "ALL")
            {
                //query += " And F.UNIT_ID = " + reportParameters.UNIT_ID;
                query += " And U.UNIT_ID = " + reportParameters.UNIT_ID;
                query2 += " And U.UNIT_ID = " + reportParameters.UNIT_ID;

            }



            if (!string.IsNullOrWhiteSpace(reportParameters.SKU_ID)
                && reportParameters.SKU_ID != "undefined" && reportParameters.SKU_ID != "ALL")
            {
                // query += " AND F.SKU_ID IN (";
                query += " AND U.SKU_ID IN (";
                query2 += " AND U.SKU_ID IN (";

                var arr = reportParameters.SKU_ID.Split(',');
                var ids = "";
                foreach (var no in arr)
                {
                    ids += "'" + no + "',";
                }
                query += ids.Substring(0, ids.Length - 1) + ") ";
                query2 += ids.Substring(0, ids.Length - 1) + ") ";

            }

            query = query + @"    GROUP BY 
         U.COMPANY_ID,
         U.UNIT_ID,
         U.SKU_ID,
         U.SKU_CODE,
         U.UNIT_TP,
         U.BATCH_ID,
         U.BATCH_NO ";

            //            query = query + @" GROUP BY F.DTL_ID,
            //         F.MST_ID,
            //         F.FREEZE_DATE,
            //         F.COMPANY_ID,
            //         F.UNIT_ID,
            //         F.SKU_ID,
            //         F.SKU_CODE,
            //         F.UNIT_TP,
            //         F.BATCH_ID,
            //         F.BATCH_NO,
            //         F.BATCH_QTY,
            //         F.FREEZE_QTY,
            //         F.ENTERED_BY,
            //         F.ENTERED_DATE
            //ORDER BY FN_SKU_NAME (F.COMPANY_ID, F.SKU_ID), F.SKU_CODE, F.BATCH_ID";
            query = query + query2 + @") ORDER BY FN_SKU_NAME (COMPANY_ID, SKU_ID), SKU_CODE, BATCH_ID, ENTERED_DATE";
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }

        public DataTable BatchFreezingTansection(ReportParams reportParameters)
        {
            var query = @"SELECT COMPANY_ID,

         FN_COMPANY_NAME (COMPANY_ID) COMPANY_NAME,

         UNIT_ID,

         FN_UNIT_NAME (COMPANY_ID, UNIT_ID) UNIT_NAME,

         SKU_ID,

         SKU_CODE,

         FN_SKU_NAME (COMPANY_ID, SKU_ID) SKU_NAME,

         FN_PACK_SIZE (COMPANY_ID, SKU_ID) PACK_SIZE,        

         UNIT_TP,

         BATCH_ID,

         BATCH_NO,

         TRANSACTION_QTY,

         TRANSACTION_TYPE,

         TRANSACTION_BY,

         TRANSACTION_DATE,                 

         REMARKS

  FROM

         (

          SELECT FREEZE_DATE TRANSACTION_DATE,

                 'Freeze' TRANSACTION_TYPE,

                 COMPANY_ID,

                 UNIT_ID,

                 SKU_ID,

                 SKU_CODE,

                 UNIT_TP,

                 BATCH_ID,

                 BATCH_NO,

                 FREEZE_QTY TRANSACTION_QTY,

                 FN_USER_NAME (1, ENTERED_BY) TRANSACTION_BY,

                 (SELECT T.REMARKS FROM BATCH_FREEZING_MST T WHERE T.MST_ID = DD.MST_ID) REMARKS

          FROM BATCH_FREEZING_DTL DD

          UNION ALL

          SELECT UN_FREEZE_DATE TRANSACTION_DATE,

                 'Unfreeze' TRANSACTION_TYPE,

                 COMPANY_ID,

                 UNIT_ID,

                 SKU_ID,

                 SKU_CODE,

                 UNIT_TP,

                 BATCH_ID,

                 BATCH_NO,

                 UN_FREEZE_QTY TRANSACTION_QTY,

                 FN_USER_NAME (1, ENTERED_BY) TRANSACTION_BY,

                 (SELECT T.REMARKS FROM BATCH_UN_FREEZING_MST T WHERE T.MST_ID = D.MST_ID) REMARKS

          FROM BATCH_UN_FREEZING_DTL D

         )

  WHERE COMPANY_ID = :param1
  AND TRUNC(TRANSACTION_DATE) BETWEEN TO_DATE(:param2, 'DD/MM/YYYY') AND TO_DATE(:param3, 'DD/MM/YYYY') ";


            var parameters = new List<string> { reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO };

            if (reportParameters.UNIT_ID != "ALL")
            {
                query += " And UNIT_ID = " + reportParameters.UNIT_ID;


            }
            if (!string.IsNullOrWhiteSpace(reportParameters.SKU_ID)
                && reportParameters.SKU_ID != "undefined" && reportParameters.SKU_ID != "ALL")
            {
                query += " AND SKU_ID IN (";


                var arr = reportParameters.SKU_ID.Split(',');
                var ids = "";
                foreach (var no in arr)
                {
                    ids += "'" + no + "',";
                }
                query += ids.Substring(0, ids.Length - 1) + ") ";

            }

            query = query + @"   ORDER BY UNIT_ID, SKU_ID, BATCH_ID, TRANSACTION_DATE";
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }
        public DataTable StockAdjustment(ReportParams reportParameters)
        {
            var query = @"SELECT COMPANY_ID,
                   FN_COMPANY_NAME(COMPANY_ID) COMPANY_NAME,
                   UNIT_ID,
                   FN_UNIT_NAME(COMPANY_ID,UNIT_ID) UNIT_NAME,
                   ADJUSTMENT_ID, 
                   TO_CHAR(ADJUSTMENT_DATE, 'DD/MM/YYYY') ADJUSTMENT_DATE,
                   SKU_ID,
                   SKU_CODE,
                   FN_SKU_NAME(COMPANY_ID,SKU_ID)SKU_NAME,
                   FN_PACK_SIZE(COMPANY_ID,SKU_ID) PACK_SIZE,
                   UNIT_TP,
                   BATCH_ID,
                   BATCH_NO,
                   DECODE(ADJUSTMENT_TYPE,'G','Gain','L','Lose')ADJUSTMENT_TYPE,
                   ADJUSTMENT_QTY,
                   FN_EMPLOYEE_ID(ENTERED_BY) ADJUSTMENT_BY_ID,
                   FN_EMPLOYEE_NAME(ENTERED_BY) ADJUSTMENT_BY_NAME
            FROM SKU_STOCK_ADJUSTMENT
            WHERE COMPANY_ID=:param1";

            var parameters = new List<string> { reportParameters.COMPANY_ID.ToString() };

            if (reportParameters.UNIT_ID != "ALL")
            {
                query += " And UNIT_ID = " + reportParameters.UNIT_ID;
            }

            if (!string.IsNullOrWhiteSpace(reportParameters.SKU_ID) && reportParameters.SKU_ID != "undefined" && reportParameters.SKU_ID != "ALL")
            {
                query += " AND SKU_ID IN (";
                var arr = reportParameters.SKU_ID.Split(',');
                var ids = "";
                foreach (var no in arr)
                {
                    ids += "'" + no + "',";
                }
                query += ids.Substring(0, ids.Length - 1) + ") ";
            }

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }
    }
}