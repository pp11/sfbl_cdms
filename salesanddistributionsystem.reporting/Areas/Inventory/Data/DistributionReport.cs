using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Oracle.ManagedDataAccess.Client;
using SalesAndDistributionSystem.Domain.Models.ReportModels.Common.ProductReports;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI.WebControls;

namespace SalesAndDistributionSystem.Reporting.Areas.Inventory.Data
{
    public class DistributionReport
    {
        private readonly CommonServices commonServices = new CommonServices();
        public string GetPageHeaderReport_Query() => @"Select  
          COMPANY_NAME 
         ,COMPANY_ADDRESS
         from VW_REPORT_HEADER_DATA where COMPANY_ID = :param1 and ROWNUM =1";
        public DataTable GetPageHeaderReport(ReportParams reportParameters)
        {
            var dt = commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, GetPageHeaderReport_Query(), commonServices.AddParameter(new string[] { reportParameters.COMPANY_ID.ToString() }));
            dt.Columns.Add("UNIT_NAME");
            dt.Columns.Add("DATE_FROM");
            dt.Columns.Add("DATE_TO");
            dt.Rows[0]["UNIT_NAME"] = reportParameters.UNIT_NAME;
            dt.Rows[0]["DATE_FROM"] = reportParameters.DATE_FROM;
            dt.Rows[0]["DATE_TO"] = reportParameters.DATE_TO;
            return dt;
        }
        //Dispatch Challan (Depot)
        public DataTable DispatchChallan(ReportParams reportParameters)
        {
            var query = @"SELECT 
                R.REQUISITION_NO,
                TO_CHAR(R.REQUISITION_DATE, 'DD/MM/YYYY') REQUISITION_DATE,
                FN_UNIT_NAME(M.COMPANY_ID, R.REQUISITION_UNIT_ID) REQUISITION_UNIT,
                R.ISSUE_NO,
                TO_CHAR(R.ISSUE_DATE, 'DD/MM/YYYY') ISSUE_DATE,
                R.ISSUE_UNIT_ID,
                FN_UNIT_NAME(M.COMPANY_ID, R.ISSUE_UNIT_ID) ISSUE_UNIT,
                RR.REQUISITION_AMOUNT,
                RI.ISSUE_AMOUNT
                FROM DEPOT_DISPATCH_REQUISITION R, DEPOT_DISPATCH_MST M, 
                   DEPOT_REQUISITION_RAISE_MST RR, DEPOT_REQUISITION_ISSUE_MST RI
                WHERE R.MST_ID = M.MST_ID
                AND RR.REQUISITION_NO = R.REQUISITION_NO
                AND RR.REQUISITION_UNIT_ID = R.REQUISITION_UNIT_ID
                AND RI.ISSUE_NO = R.ISSUE_NO
                AND RI.ISSUE_UNIT_ID = R.ISSUE_UNIT_ID

                AND M.COMPANY_ID = :param1
                AND M.DISPATCH_NO = :param2
UNION ALL 

SELECT 
    R.REQUISITION_NO,
    TO_CHAR(R.REQUISITION_DATE, 'DD/MM/YYYY') REQUISITION_DATE,
    FN_UNIT_NAME(M.COMPANY_ID, R.REQUISITION_UNIT_ID) REQUISITION_UNIT,
    R.ISSUE_NO,
    TO_CHAR(R.ISSUE_DATE, 'DD/MM/YYYY') ISSUE_DATE,
    R.ISSUE_UNIT_ID,
    FN_UNIT_NAME(M.COMPANY_ID, R.ISSUE_UNIT_ID) ISSUE_UNIT,
    RR.TRANSFER_AMOUNT REQUISITION_AMOUNT,
    RR.TRANSFER_AMOUNT ISSUE_AMOUNT
FROM DEPOT_DISPATCH_MST M, DEPOT_DISPATCH_REQUISITION R,
     DEPOT_STOCK_TRANSFER_MST RR
WHERE M.DISPATCH_TYPE ='Stock' AND R.MST_ID = M.MST_ID
AND R.REQUISITION_NO = RR.TRANSFER_NO
AND M.COMPANY_ID = :param1
AND M.DISPATCH_NO = :param2";

            var parameters = new List<string> { reportParameters.COMPANY_ID.ToString(), reportParameters.DISPATCH_NO.ToString() };

            //if (reportParameters.UNIT_ID != "ALL")
            //{
            //    query += " And R.REQUISITION_UNIT_ID = " + reportParameters.UNIT_ID;
            //}

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }
        public DataTable DispatchChallanSubReportData(ReportParams reportParameters)
        {
            try
            {
                var query = @"SELECT C.BATCH_ID,
       C.BATCH_NO,
       FN_UNIT_WISE_SKU_MRP (C.SKU_ID,C.SKU_CODE,  C.COMPANY_ID, C.DISPATCH_UNIT_ID, C.BATCH_ID)MRP,
       SUM(C.DISPATCH_QTY) BATCH_TRANSFER_QTY,
       FLOOR ( (NVL (SUM(C.DISPATCH_QTY), 0)) / I.SHIPPER_QTY)|| ' Box'|| ','|| MOD ( (NVL (SUM(C.DISPATCH_QTY), 0)), I.SHIPPER_QTY)|| ' Pcs' BATCH_QTY_BOX_PCS
 FROM DEPOT_DISPATCH_PRODUCT_BATCH C,
       PRODUCT_INFO I
 WHERE    
       I.SKU_ID = C.SKU_ID
       AND C.MST_ID = :param1
       AND C.SKU_CODE = :param2
       GROUP BY  C.BATCH_ID, C.BATCH_NO,C.SKU_ID,C.SKU_CODE,  C.COMPANY_ID, C.DISPATCH_UNIT_ID,I.SHIPPER_QTY
       ORDER BY C.BATCH_ID";
                return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(new string[] { reportParameters.MST_ID, reportParameters.DTL_ID }));
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataTable DispatchChallanV2(ReportParams reportParameters)
        {
            var query = @"SELECT REPLACE(B.REQUISITION_UNIT,'Square Toiletries Ltd.','')REQUISITION_UNIT,
       B.REQUISITION_NO,
       B.REQUISITION_DATE,
       B.ENTERED_BY,
       A.MST_ID,
       A.DISPATCH_NO,
       A.DISPATCH_DATE,
       A.VECHILE_NO || ' (' || A.VECHILE_DESCRIPTION || ')' VECHILE_DESCRIPTION,
       A.DRIVER_NAME,
       A.DRIVER_MOBILE_NO,
       A.ISSUE_UNIT,
       A.SKU_CODE,
       A.SKU_NAME,
       A.PACK_SIZE,
       A.UNIT_TP,
       SUM(A.DISPATCH_QTY) DISPATCH_QTY,
       SUM(A.DISPATCH_AMT) DISPATCH_AMT,
       FLOOR ( (NVL (SUM(A.DISPATCH_QTY), 0)) / A.SHIPPER_QTY)|| ' Box'|| ',' || MOD ( (NVL (SUM(A.DISPATCH_QTY), 0)), A.SHIPPER_QTY) || ' Pcs' QTY_BOX_PCS
  FROM (SELECT M.COMPANY_ID,
               M.MST_ID,
               M.DISPATCH_NO,
               TO_CHAR (M.DISPATCH_DATE, 'DD/MM/YYYY') DISPATCH_DATE,
               M.VECHILE_DESCRIPTION,
               M.VECHILE_NO,
               I.DRIVER_NAME,
               I.CONTACT_NO DRIVER_MOBILE_NO,
               FN_UNIT_NAME (M.COMPANY_ID, M.DISPATCH_UNIT_ID) ISSUE_UNIT,
               D.DISPATCH_PRODUCT_ID,
               D.SKU_CODE,
               FN_SKU_NAME (D.COMPANY_ID, D.SKU_ID) SKU_NAME,
               FN_PACK_SIZE (D.COMPANY_ID, D.SKU_ID) PACK_SIZE,
               FN_SKU_PRICE (D.SKU_ID, D.SKU_CODE,   D.COMPANY_ID, D.DISPATCH_UNIT_ID) UNIT_TP,
               D.DISPATCH_QTY,
               D.DISPATCH_QTY* FN_SKU_PRICE (D.SKU_ID,D.SKU_CODE,D.COMPANY_ID,D.DISPATCH_UNIT_ID) DISPATCH_AMT,
               P.SHIPPER_QTY
          FROM DEPOT_DISPATCH_MST M
               INNER JOIN DEPOT_DISPATCH_PRODUCT D ON M.MST_ID = D.MST_ID
               INNER JOIN PRODUCT_INFO P ON  P.SKU_ID = D.SKU_ID
               LEFT OUTER JOIN DRIVER_INFO I ON M.DRIVER_ID = I.DRIVER_ID
         WHERE M.COMPANY_ID=:param1 AND M.MST_ID = :param2) A,
       (SELECT X.COMPANY_ID,
               X.MST_ID,
               X.REQUISITION_NO,
               X.REQUISITION_UNIT,
               X.REQUISITION_DATE,
               X.ENTERED_BY
          FROM (    SELECT R.MST_ID,LISTAGG (R.REQUISITION_NO, ' / ')WITHIN GROUP (ORDER BY R.REQUISITION_NO) AS REQUISITION_NO
                    ,LISTAGG (FN_UNIT_NAME (COMPANY_ID, REQUISITION_UNIT_ID), ' / ') WITHIN GROUP (ORDER BY R.REQUISITION_NO) AS REQUISITION_UNIT,
                    LISTAGG (R.REQUISITION_DATE, ' / ')WITHIN GROUP (ORDER BY R.REQUISITION_DATE) AS REQUISITION_DATE,
                    FN_USER_NAME(MIN(R.COMPANY_ID) , COALESCE (MIN(R.UPDATED_BY) ,MIN(R.ENTERED_BY))) ENTERED_BY,
                    MIN(R.COMPANY_ID) COMPANY_ID
                    FROM DEPOT_DISPATCH_REQUISITION R
                    GROUP BY R.MST_ID) X
                   WHERE X.COMPANY_ID=:param1 AND X.MST_ID = :param2
                ) B
 WHERE A.COMPANY_ID = B.COMPANY_ID AND A.MST_ID = B.MST_ID
 GROUP BY  B.REQUISITION_UNIT,B.REQUISITION_NO,B.REQUISITION_DATE,B.ENTERED_BY,A.MST_ID,A.DISPATCH_NO,A.DISPATCH_DATE,A.VECHILE_NO,A.VECHILE_DESCRIPTION,A.DRIVER_NAME,A.DRIVER_MOBILE_NO,A.ISSUE_UNIT,A.SKU_CODE,A.SKU_NAME,A.PACK_SIZE,A.UNIT_TP,A.SHIPPER_QTY
 ORDER BY A.MST_ID,A.SKU_CODE ";
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(new string[] {reportParameters.COMPANY_ID.ToString(),reportParameters.MST_ID }));
        }
        //Dispatch Challan(Depot)
        public DataTable DispatchMasterData(ReportParams reportParameters)
        {
            var query = @"SELECT M.DISPATCH_NO,
       M.VECHILE_NO || ' (' || M.VECHILE_DESCRIPTION || ')'
          VECHILE_DESCRIPTION,
       M.DRIVER_ID,
       D.DRIVER_NAME,
       D.CONTACT_NO DRIVER_MOBILE_NO,
       TO_CHAR (M.DISPATCH_DATE, 'DD/MM/YYYY') DISPATCH_DATE,
       FN_UNIT_NAME (M.COMPANY_ID, M.DISPATCH_UNIT_ID) ISSUE_UNIT,
       ( SELECT LISTAGG(FN_UNIT_NAME, ', ') WITHIN GROUP (ORDER BY REQUISITION_UNIT_ID) AS REQUISITION_UNIT_ID
FROM (
    SELECT DISTINCT G.COMPANY_ID, G.REQUISITION_UNIT_ID, FN_UNIT_NAME (G.COMPANY_ID, G.REQUISITION_UNIT_ID) AS FN_UNIT_NAME
    FROM DEPOT_DISPATCH_REQUISITION G, DEPOT_DISPATCH_MST K 
    WHERE K.MST_ID = G.MST_ID AND K.DISPATCH_NO = :param1
)
GROUP BY COMPANY_ID)
          RCV_UNIT
  FROM DEPOT_DISPATCH_MST M, DRIVER_INFO D
 WHERE     D.DRIVER_ID = M.DRIVER_ID(+)
       AND M.COMPANY_ID = :param2
       AND M.DISPATCH_NO = :param3 ";

            var parameters = new List<string> { reportParameters.DISPATCH_NO.ToString(),reportParameters.COMPANY_ID.ToString(), reportParameters.DISPATCH_NO.ToString() };

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }
        //Dispatch Challan(Depot)
        public DataTable DispatchReqProducts(ReportParams reportParameters)
        {
            var query = @"SELECT
                R.REQUISITION_NO,
                TO_CHAR(R.REQUISITION_DATE, 'DD/MM/YYYY') REQUISITION_DATE,
                R.ISSUE_NO,
                TO_CHAR(R.ISSUE_DATE, 'DD/MM/YYYY') ISSUE_DATE,
                FN_UNIT_NAME(R.COMPANY_ID, R.REQUISITION_UNIT_ID) REQUISITION_UNIT,
                FN_UNIT_NAME(R.COMPANY_ID, R.ISSUE_UNIT_ID) ISSUE_UNIT,
                P.SKU_ID,
                P.SKU_CODE,
                FN_SKU_NAME(P.COMPANY_ID, P.SKU_ID) SKU_NAME,
                FN_PACK_SIZE(P.COMPANY_ID, P.SKU_ID) PACK_SIZE,
                P.SHIPPER_QTY,
                P.NO_OF_SHIPPER,
                P.LOOSE_QTY,
                B.BATCH_NO,
                B.DISPATCH_QTY,
                FN_NO_OF_SHIPPER(P.COMPANY_ID, P.SKU_ID, P.ISSUE_QTY - P.DISPATCH_QTY) REMAINING_SHIPPER,
                FN_LOOSE_QTY(P.COMPANY_ID, P.SKU_ID, P.ISSUE_QTY - P.DISPATCH_QTY) REMAINING_LOOSE
                FROM DEPOT_DISPATCH_MST M, DEPOT_DISPATCH_REQUISITION R, DEPOT_DISPATCH_PRODUCT P, DEPOT_DISPATCH_PRODUCT_BATCH B 
                WHERE M.MST_ID = R.MST_ID
                AND R.DISPATCH_REQ_ID = P.DISPATCH_REQ_ID
                AND P.DISPATCH_PRODUCT_ID = B.DISPATCH_PRODUCT_ID

                AND M.COMPANY_ID = :PARAM1
                AND M.DISPATCH_NO = :PARAM2";

            var parameters = new List<string> { reportParameters.COMPANY_ID.ToString(), reportParameters.DISPATCH_NO.ToString() };

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }
        //Dispatch Challan(Depot)
        public DataTable DispatchProductSummery(ReportParams reportParameters)
        {
            var query = @"SELECT
                A.*,
                FN_SKU_NAME(COMPANY_ID, SKU_ID) SKU_NAME,
                FN_PACK_SIZE(COMPANY_ID, SKU_ID) PACK_SIZE
                FROM (
                SELECT
                SKU_ID,
                SKU_CODE,
                SUM(SHIPPER_QTY)SHIPPER_QTY,
                SUM(NO_OF_SHIPPER)NO_OF_SHIPPER,
                SUM(LOOSE_QTY)LOOSE_QTY,
                M.COMPANY_ID
                FROM DEPOT_DISPATCH_SHIPPER_DTL D, DEPOT_DISPATCH_MST M
                WHERE D.MST_ID = M.MST_ID
                AND M.COMPANY_ID = :PARAM1
                AND M.DISPATCH_NO = :PARAM2
                GROUP BY SKU_ID, SKU_CODE, M.COMPANY_ID
                ) A";

            var parameters = new List<string> { reportParameters.COMPANY_ID.ToString(), reportParameters.DISPATCH_NO.ToString() };

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }
        //Delivery Challan (Customer)
        public DataTable CustomerInvoiceByDistNo(ReportParams reportParameters)
        {
            var query = @"SELECT DISTINCT
                M.DISTRIBUTION_NO,
                TO_CHAR(M.DISTRIBUTION_DATE, 'DD/MM/YYYY') DISTRIBUTION_DATE,
                M.VEHICLE_NO,
                INVOICE_NO,
                TO_CHAR(INVOICE_DATE, 'DD/MM/YYYY') INVOICE_DATE,
                CUSTOMER_CODE,
                FN_CUSTOMER_NAME(I.COMPANY_ID, CUSTOMER_ID) CUSTOMER_NAME
                FROM DEPOT_CUSTOMER_DIST_MST M, DEPOT_CUSTOMER_DIST_INVOICE I
                WHERE M.MST_ID = I.MST_ID
                AND DISTRIBUTION_NO = :param1";

            var parameters = new List<string> { reportParameters.DISPATCH_NO.ToString() };

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }
        //Delivery Challan (Customer)
        public DataTable PickerProductsByDistNo(ReportParams reportParameters)
        {
            var query = @"SELECT DISTRIBUTION_NO
                       COMPANY_ID,
                       SKU_ID,
                       SKU_CODE,
                       FN_SKU_NAME(COMPANY_ID, SKU_ID) SKU_NAME,
                       FN_PACK_SIZE(COMPANY_ID, SKU_ID) PACK_SIZE,
                       FN_DIST_BOX_PCS(DISTRIBUTION_NO,SKU_ID)BOX_PCS,
                       BATCH_NO,
                       FN_NO_OF_SHIPPER(COMPANY_ID,SKU_ID, TOTAL_DISTRUBUTION_QTY) CARTON,
                       FN_LOOSE_QTY(COMPANY_ID,SKU_ID,TOTAL_DISTRUBUTION_QTY) PIECE
                FROM
                   (
                      SELECT A.DISTRIBUTION_NO,
                             A.COMPANY_ID,
                             D.SKU_ID,
                             D.SKU_CODE,  
                             D.BATCH_NO,
                             SUM(NVL(D.TOTAL_DISTRUBUTION_QTY,0))TOTAL_DISTRUBUTION_QTY
                       FROM DEPOT_CUSTOMER_DIST_MST A,DEPOT_CUSTOMER_DIST_INVOICE B, DEPOT_CUSTOMER_DIST_PRODUCT C,DEPOT_CUSTOMER_DIST_PROD_BATCH  D
                       WHERE A.MST_ID = B.MST_ID
                       AND   B.DEPOT_INV_ID=C.DEPOT_INV_ID   
                       AND   C.DEPOT_PRODUCT_ID=D.DEPOT_PRODUCT_ID
                       AND   A.DISTRIBUTION_NO = :param1
                       GROUP BY A.DISTRIBUTION_NO,A.COMPANY_ID,D.SKU_ID,D.SKU_CODE,D.BATCH_NO
                   )";

            var parameters = new List<string> { reportParameters.DISPATCH_NO.ToString() };

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }
        //Delivery Challan (Customer)
        public DataTable HelperProductsByDistNo(ReportParams reportParameters)
        {
            var query = @"SELECT
                            DISTRIBUTION_NO,
                            DISTRIBUTION_DATE,
                            VEHICLE_NO,
                            CUSTOMER_CODE,
                            CUSTOMER_NAME,
                            SKU_ID,
                            SKU_NAME,
                            PACK_SIZE,
                            SKU_CODE,
                            FN_NO_OF_SHIPPER(COMPANY_ID,SKU_ID,TOTAL_DISTRIBUTION_QTY) CARTON,
                            FN_LOOSE_QTY(COMPANY_ID,SKU_ID,TOTAL_DISTRIBUTION_QTY) PIECE
                    FROM
                        (
                        SELECT
                            I.COMPANY_ID,
                            M.DISTRIBUTION_NO,
                            TO_CHAR(M.DISTRIBUTION_DATE, 'DD/MM/YYY') DISTRIBUTION_DATE,
                            M.VEHICLE_NO,
                            CUSTOMER_CODE,
                            FN_CUSTOMER_NAME(I.COMPANY_ID, CUSTOMER_ID) CUSTOMER_NAME,
                            P.SKU_ID,
                            FN_SKU_NAME(I.COMPANY_ID, SKU_ID) SKU_NAME,
                            FN_PACK_SIZE(I.COMPANY_ID, SKU_ID) PACK_SIZE,
                            P.SKU_CODE,
                            SUM(NVL(P.TOTAL_DISTRIBUTION_QTY,0))TOTAL_DISTRIBUTION_QTY
                        FROM DEPOT_CUSTOMER_DIST_MST M, DEPOT_CUSTOMER_DIST_INVOICE I, DEPOT_CUSTOMER_DIST_PRODUCT P
                        WHERE M.MST_ID = I.MST_ID
                        AND I.DEPOT_INV_ID = P.DEPOT_INV_ID
                        AND DISTRIBUTION_NO = :param1
                        GROUP BY SKU_ID, I.COMPANY_ID, SKU_CODE, M.DISTRIBUTION_NO, M.DISTRIBUTION_DATE, M.VEHICLE_NO, CUSTOMER_CODE, CUSTOMER_ID
                        )";

            var parameters = new List<string> { reportParameters.DISPATCH_NO.ToString() };

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }
        //Delivery Challan (Customer)
        public DataTable CustomerInvoice(ReportParams reportParameters)
        {
            var query = @"SELECT DISTINCT
                M.DISTRIBUTION_NO,
                TO_CHAR(M.DISTRIBUTION_DATE, 'DD/MM/YYY') DISTRIBUTION_DATE,
                M.VEHICLE_NO,
                I.CUSTOMER_ID,
                I.CUSTOMER_CODE,
                C.CUSTOMER_NAME,
                I.INVOICE_NO,
                TO_CHAR(I.INVOICE_DATE, 'DD/MM/YYY') INVOICE_DATE,
                INV.NET_INVOICE_AMOUNT INVOICE_AMOUNT,
                --I.CHALLAN_AMOUNT,
                C.CUSTOMER_CONTACT CUSTOMER_PHONE,
                C.CUSTOMER_ADDRESS,
                D.DRIVER_NAME,
                D.CONTACT_NO DRIVER_PHONE,
                I.CUSTOMER_CHALLAN_NO,
                '" + reportParameters.DB + @"' DB
                FROM DEPOT_CUSTOMER_DIST_MST M, DEPOT_CUSTOMER_DIST_INVOICE I, CUSTOMER_INFO C, VEHICLE_INFO V, DRIVER_INFO D, INVOICE_MST INV
                WHERE M.MST_ID = I.MST_ID
                AND I.INVOICE_NO = INV.INVOICE_NO
                AND I.CUSTOMER_ID = C.CUSTOMER_ID
                AND M.VEHICLE_NO = V.VEHICLE_NO
                AND V.DRIVER_ID = D.DRIVER_ID
                AND DISTRIBUTION_NO = :param1";

            var parameters = new List<string> { reportParameters.DISPATCH_NO.ToString() };

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }
        //Delivery Challan(Customer) - Sub Report
        public DataTable DeliveryProductsByCustAndDist(ReportParams reportParameters)
        {
            var query = @"SELECT DISTRIBUTION_NO
                       COMPANY_ID,
                       SKU_ID,
                       SKU_CODE,
                       FN_SKU_NAME(COMPANY_ID, SKU_ID) SKU_NAME,
                       FN_PACK_SIZE(COMPANY_ID, SKU_ID) PACK_SIZE,
                       FN_DIST_BOX_PCS(DISTRIBUTION_NO,SKU_ID)BOX_PCS,
                       BATCH_NO,
                       FN_NO_OF_SHIPPER(COMPANY_ID,SKU_ID, TOTAL_DISTRUBUTION_QTY) CARTON,
                       FN_LOOSE_QTY(COMPANY_ID,SKU_ID,TOTAL_DISTRUBUTION_QTY) PIECE
                FROM
                   (
                      SELECT A.DISTRIBUTION_NO,
                             A.COMPANY_ID,
                             D.SKU_ID,
                             D.SKU_CODE,  
                             D.BATCH_NO,
                             SUM(NVL(D.TOTAL_DISTRUBUTION_QTY,0))TOTAL_DISTRUBUTION_QTY
                       FROM DEPOT_CUSTOMER_DIST_MST A,DEPOT_CUSTOMER_DIST_INVOICE B, DEPOT_CUSTOMER_DIST_PRODUCT C,DEPOT_CUSTOMER_DIST_PROD_BATCH  D
                       WHERE A.MST_ID = B.MST_ID
                       AND   B.DEPOT_INV_ID=C.DEPOT_INV_ID   
                       AND   C.DEPOT_PRODUCT_ID=D.DEPOT_PRODUCT_ID
                       AND   A.DISTRIBUTION_NO = :param1
                       AND   B.CUSTOMER_ID = :param2
                       GROUP BY A.DISTRIBUTION_NO,A.COMPANY_ID,D.SKU_ID,D.SKU_CODE,D.BATCH_NO)";

            var parameters = new List<string> { reportParameters.DISPATCH_NO.ToString(), reportParameters.CUSTOMER_ID };

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }
        //Delivery Challan(Customer)- Sub Report
        public DataTable DeliveryGiftByCustAndDist(ReportParams reportParameters)
        {
            var query = @"SELECT 
                GIFT_ITEM_ID,
                FN_GIFT_NAME(A.COMPANY_ID, GIFT_ITEM_ID) GIFT_NAME,
                GIFT_QTY,
                UNIT_TP,
                (GIFT_QTY * UNIT_TP) AMOUNT
                FROM DEPOT_CUSTOMER_DIST_MST A,
                    DEPOT_CUSTOMER_DIST_INVOICE B,
                    DEPOT_CUSTOMER_DIST_GIFT_BATCH C
                WHERE B.DEPOT_INV_ID = C.DEPOT_INVOICE_ID
                AND A.MST_ID = B.MST_ID
                AND A.DISTRIBUTION_NO = :param1
                AND B.CUSTOMER_ID = :param2";
            var parameters = new List<string> { reportParameters.DISPATCH_NO.ToString(), reportParameters.CUSTOMER_ID };
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }
        //Pending Invoices Prod
        public DataTable GetPendingInvoices(ReportParams reportParameters)
        {
            var query = String.Format(@"begin :param1 := FN_INV_DIS_PEN_FOR_DIST_RPT({0},'{1}'); end;", reportParameters.COMPANY_ID, reportParameters.UNIT_ID);
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(new string[] { "RefCursor" }));
        }
        //Pending Invoices Prod
        public DataTable GetSkuWisePending(ReportParams reportParameters)
        {
            var query = String.Format(@"begin :param1 := FN_SKU_WISE_PENDING_REPORT({0},'{1}'); end;"
            , reportParameters.COMPANY_ID, reportParameters.UNIT_ID);

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(new string[] { "RefCursor" }));
        }
        //Pending Invoices
        public DataTable GetAllPendingInvoices(ReportParams reportParameters)
        {
            var query = String.Format(@"begin :param1 := FN_INVOICE_PENDING_FOR_DIST({0},'{1}'); 
                          end;"
            , reportParameters.COMPANY_ID, reportParameters.UNIT_ID);

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(new string[] { "RefCursor" }));
        }
        //Date Wise Delivery Pending Report
        public DataTable GetDateWisePendingInvoices(ReportParams reportParameters)
        {
            string query = @"begin  :param1 := fn_inv_pending_for_dist_rpt(:param2, :param3, TO_DATE(:param4, 'DD/MM/YYYY'), TO_DATE(:param5, 'DD/MM/YYYY')); end;";
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(new string[] { "RefCursor", reportParameters.COMPANY_ID.ToString(), reportParameters.UNIT_ID, reportParameters.DATE_FROM, reportParameters.DATE_TO }));
        }
        public DataTable GetPendingGift(ReportParams reportParameters)
        {
            var query = String.Format(@"begin :param1 := fn_gift_pending_for_dist({0},'{1}', '{2}'); 
                          end;"
            , reportParameters.COMPANY_ID, reportParameters.UNIT_ID, reportParameters.INVOICE_NO_FROM);

            var dt = commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(new string[] { "RefCursor" }));
            return dt;
        }
        //Date Wise Delivery List 
        public DataTable GetDateWiseDeliveryList(ReportParams reportParameters)
        {
            var query = @"SELECT M.DISTRIBUTION_NO,
         TO_CHAR(M.DISTRIBUTION_DATE,'DD/MM/YYYY') AS DISTRIBUTION_DATE,
         M.DIST_ROUTE_ID,
         M.VEHICLE_NO,
         M.VEHICLE_DESCRIPTION,
         M.DRIVER_ID,
         COALESCE(M.DRIVER_PHONE,D.CONTACT_NO ) DRIVER_PHONE,
         D.DRIVER_NAME,
         I.INVOICE_NO,
         TO_CHAR(I.INVOICE_DATE,'DD/MM/YYYY') AS INVOICE_DATE,
         I.INVOICE_UNIT_ID,
         SUM (P.TOTAL_DISTRIBUTION_QTY) AS TOTAL_QTY,
         TO_CHAR(IM.ORDER_DATE,'DD/MM/YYYY') ORDER_DATE,
         IM.ORDER_NO,
         C.CUSTOMER_NAME
 FROM DEPOT_CUSTOMER_DIST_MST M
         LEFT OUTER JOIN DEPOT_CUSTOMER_DIST_INVOICE I ON M.MST_ID = I.MST_ID
         LEFT OUTER JOIN DEPOT_CUSTOMER_DIST_PRODUCT P ON P.DEPOT_INV_ID = I.DEPOT_INV_ID
         LEFT OUTER JOIN DRIVER_INFO D ON D.DRIVER_ID = M.DRIVER_ID
         LEFT OUTER JOIN INVOICE_MST IM ON IM.INVOICE_NO = I.INVOICE_NO
         LEFT OUTER JOIN CUSTOMER_INFO C ON C.CUSTOMER_ID = IM.CUSTOMER_ID
 WHERE M.COMPANY_ID=:param1 AND M.INVOICE_UNIT_ID=:param2 AND TRUNC( M.DISTRIBUTION_DATE ) BETWEEN TO_DATE(:param3, 'DD/MM/YYYY') AND TO_DATE(:param4, 'DD/MM/YYYY')         
 GROUP BY I.INVOICE_NO,
         I.INVOICE_DATE,
         I.INVOICE_UNIT_ID,
         M.DISTRIBUTION_NO,
         M.DISTRIBUTION_DATE,
         M.DIST_ROUTE_ID,
         M.VEHICLE_NO,
         M.DRIVER_ID,
         COALESCE(M.DRIVER_PHONE,D.CONTACT_NO ) ,
         D.DRIVER_NAME,
         M.VEHICLE_DESCRIPTION,
         IM.ORDER_DATE,
         IM.ORDER_NO,
         C.CUSTOMER_NAME
         ORDER BY M.DISTRIBUTION_DATE";

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(new string[] { reportParameters.COMPANY_ID.ToString(), reportParameters.UNIT_ID, reportParameters.DATE_FROM, reportParameters.DATE_TO }));
        }
        //Date Wise Delivery List 
        public DataTable InvoiceStatus(ReportParams reportParameters)
        {
            var query = @"begin  :param1 := fn_invoice_status(:param2, :param3, :param4); end;";
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.UNIT_ID, reportParameters.CUSTOMER_CODE, reportParameters.INVOICE_STATUS }));
        }
        public DataTable RefurbishmentReceiveData(ReportParams reportParameters)
        {
            var query = @"SELECT M.MST_SLNO,
       M.CLAIM_NO,
       M.RECEIVE_DATE,
       M.RECEIVE_SHIFT,
       M.RECEIVE_CATEGORY,
       M.COMPANY_ID,
       M.UNIT_ID,
       M.CUSTOMER_CODE,
       C.CUSTOMER_NAME,
       C.CUSTOMER_ADDRESS,
       M.CUSTOMER_TYPE,
       M.CHALLAN_NUMBER,
       M.CHALLAN_DATE,
       M.SENDING_CARTON_QTY,
       M.SENDING_BAG_QTY,
       M.SENDING_TOTAL_AMOUNT,
       M.RECEIVE_CARTON_QTY,
       M.RECEIVE_BAG_QTY,
       M.RECEIVE_TOTAL_AMOUNT,
       M.TSO_CODE,
       M.TSO_NAME,
       M.TSO_CONTACT_NO,
       M.DRIVER_CODE,
       M.DRIVER_NAME,
       M.DRIVER_CONTACT_NO,
       M.VEHICLE_NO,
       M.RECEIVED_BY,
       M.REMARKS,
       FN_USER_NAME (M.COMPANY_ID , M.ENTERED_BY) ENTERED_BY,
       M.ENTERED_DATE,
       M.ENTERED_TERMINAL,
       M.UPDATED_BY,
       M.UPDATED_DATE,
       M.UPDATED_TERMINAL,
       CASE WHEN M.APPROVED_STATUS = 'A' THEN 'Approved' ELSE 'Pending' END APPROVED_STATUS,
       FN_USER_NAME (M.COMPANY_ID , M.APPROVED_BY)  APPROVED_BY,
       M.APPROVED_DATE,
       D.DTL_SLNO,
       D.REFURBISHMENT_PRODUCT_STATUS,
       D.PRODUCT_CODE,
       FN_PACK_SIZE_BY_CODE (M.COMPANY_ID, D.PRODUCT_CODE) PACK_SIZE,
       FN_SKU_NAME_BY_CODE (M.COMPANY_ID, D.PRODUCT_CODE) PRODUCT_NAME,
       D.BATCH_NO,
       D.LOT_NO,
       D.CLAIM_QTY,
       D.RECEIVED_QTY,
       D.DISPUTE_QTY,
       D.TRADE_PRICE,
       D.REVISED_PRICE,
       D.EXPIRY_DATE,
       D.REMARKS DTL_REMARKS
  FROM REFURBISHMENT_RECEIVING_MST M,
       REFURBISHMENT_RECEIVING_DTL D,
       CUSTOMER_INFO C
 WHERE     M.MST_SLNO = D.MST_SLNO
       AND M.CUSTOMER_CODE = C.CUSTOMER_CODE
       AND M.MST_SLNO = :param1";
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(new string[] {reportParameters.MST_ID }));
        }
        public DataTable RefurbishmentReceiveFinalizeMstData(ReportParams reportParameters)
        {
            var query = @"SELECT M.MST_SLNO,
       M.CLAIM_NO,
       M.RECEIVE_DATE,
       M.RECEIVE_SHIFT,
       M.RECEIVE_CATEGORY,
       M.COMPANY_ID,
       M.UNIT_ID,
       M.CUSTOMER_CODE,
       C.CUSTOMER_NAME,
       C.CUSTOMER_ADDRESS,
       M.CUSTOMER_TYPE,
       M.CHALLAN_NUMBER,
       M.CHALLAN_DATE,
       M.SENDING_CARTON_QTY,
       M.SENDING_BAG_QTY,
       M.SENDING_TOTAL_AMOUNT,
       M.RECEIVE_CARTON_QTY,
       M.RECEIVE_BAG_QTY,
       M.RECEIVE_TOTAL_AMOUNT,
       M.TSO_CODE,
       M.TSO_NAME,
       M.TSO_CONTACT_NO,
       M.DRIVER_CODE,
       M.DRIVER_NAME,
       M.DRIVER_CONTACT_NO,
       M.VEHICLE_NO,
       M.RECEIVED_BY,
       M.REMARKS,
       FN_USER_NAME (M.COMPANY_ID , M.ENTERED_BY) ENTERED_BY,
       M.ENTERED_DATE,
       M.ENTERED_TERMINAL,
       M.UPDATED_BY,
       M.UPDATED_DATE,
       M.UPDATED_TERMINAL,
       CASE WHEN M.APPROVED_STATUS = 'A' THEN 'Approved' ELSE 'Pending' END APPROVED_STATUS,
       FN_USER_NAME (M.COMPANY_ID , M.APPROVED_BY)  APPROVED_BY,
       M.APPROVED_DATE,
       D.DTL_SLNO,
       D.REFURBISHMENT_PRODUCT_STATUS,
       D.PRODUCT_CODE,
       FN_PACK_SIZE_BY_CODE (M.COMPANY_ID, D.PRODUCT_CODE) PACK_SIZE,
       FN_SKU_NAME_BY_CODE (M.COMPANY_ID, D.PRODUCT_CODE) PRODUCT_NAME,
       D.BATCH_NO,
       D.LOT_NO,
       D.CLAIM_QTY,
       D.RECEIVED_QTY,
       D.DISPUTE_QTY,
       D.TRADE_PRICE,
       D.REVISED_PRICE,
       D.EXPIRY_DATE,
       D.REMARKS DTL_REMARKS
  FROM REFURBISHMENT_RECEIVING_MST M,
  REFURBISHMENT_FINALIZE_MST  MF,
       REFURBISHMENT_RECEIVING_DTL D,
       CUSTOMER_INFO C
 WHERE   M.CLAIM_NO= MF.CLAIM_NO
         AND  M.MST_SLNO = D.MST_SLNO
         AND M.CUSTOMER_CODE = C.CUSTOMER_CODE
         AND MF.MST_SLNO = :param1";
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(new string[] { reportParameters.MST_ID }));
        }
        public DataTable RefurbishmentFinalizeData(ReportParams reportParameters)
        {
            var query = @"SELECT M.MST_SLNO,
       M.CLAIM_NO,
       M.RECEIVE_DATE,
       MF.FINALIZE_DATE,
       M.RECEIVE_SHIFT,
       M.RECEIVE_CATEGORY,
       M.COMPANY_ID,
       M.UNIT_ID,
       M.CUSTOMER_CODE,
       C.CUSTOMER_NAME,
       C.CUSTOMER_ADDRESS,
       M.CUSTOMER_TYPE,
       M.CHALLAN_NUMBER,
       M.CHALLAN_DATE,
       M.SENDING_CARTON_QTY,
       M.SENDING_BAG_QTY,
       M.SENDING_TOTAL_AMOUNT,
       M.RECEIVE_CARTON_QTY,
       M.RECEIVE_BAG_QTY,
       M.RECEIVE_TOTAL_AMOUNT,
       M.TSO_CODE,
       M.TSO_NAME,
       M.TSO_CONTACT_NO,
       M.DRIVER_CODE,
       M.DRIVER_NAME,
       M.DRIVER_CONTACT_NO,
       M.VEHICLE_NO,
       M.RECEIVED_BY,
       M.REMARKS,
       FN_USER_NAME (M.COMPANY_ID , M.ENTERED_BY) ENTERED_BY,
       M.ENTERED_DATE,
       M.ENTERED_TERMINAL,
       M.UPDATED_BY,
       M.UPDATED_DATE,
       M.UPDATED_TERMINAL,
       CASE WHEN MF.APPROVED_STATUS = 'A' THEN 'Approved' ELSE 'Pending' END APPROVED_STATUS,
       FN_USER_NAME (MF.COMPANY_ID , MF.APPROVED_BY)  APPROVED_BY,
       MF.APPROVED_DATE,
       D.DTL_SLNO,
       D.PRODUCT_CODE,
       FN_PACK_SIZE_BY_CODE (M.COMPANY_ID, D.PRODUCT_CODE) PACK_SIZE,
       FN_SKU_NAME_BY_CODE (M.COMPANY_ID, D.PRODUCT_CODE) PRODUCT_NAME,
       D.AMOUNT,
       D.PROD_QTY,
       D.REMARKS DTL_REMARKS,
       D.TRADE_PRICE
  FROM REFURBISHMENT_RECEIVING_MST M,
       REFURBISHMENT_FINALIZE_MST  MF,
       REFURBISHMENT_FINALIZE_DTL D,
       CUSTOMER_INFO C
 WHERE     M.CLAIM_NO= MF.CLAIM_NO
       AND MF.MST_SLNO = D.MST_SLNO
       AND C.CUSTOMER_CODE= M.CUSTOMER_CODE
       AND MF.MST_SLNO = :param1";
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(new string[] { reportParameters.MST_ID }));
        }
    }
}