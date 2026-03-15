using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.ReportModels.Common.ProductReports;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace SalesAndDistributionSystem.Reporting.Areas.Inventory.Data
{
    public class StockTransferReport
    {
        private readonly CommonServices commonServices = new CommonServices();

        public string GetPageHeaderReport_Query() => @"Select COMPANY_NAME,COMPANY_ADDRESS, :param1 as PREVIEW from VW_REPORT_HEADER_DATA where COMPANY_ID = :param2 and ROWNUM =1";

        public DataTable GetPageHeaderReport(ReportParams reportParameters)
        {
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, GetPageHeaderReport_Query(), commonServices.AddParameter(new string[] { reportParameters.PREVIEW, reportParameters.COMPANY_ID.ToString() }));
        }

        public DataTable StockTransferReportData(ReportParams reportParameters)
        {
            var query = @"SELECT  B.DTL_ID,
       A.REF_NO,
       A.REF_DATE,
       A.TRANSFER_TYPE,
       A.TRANSFER_NO,
       TO_CHAR (A.TRANSFER_DATE, 'DD/MM/RRRR') TRANSFER_DATE,
       A.TRANSFER_UNIT_ID,
       FN_UNIT_NAME (A.COMPANY_ID, A.TRANSFER_UNIT_ID) TRANSFER_UNIT_NAME,
       A.TRANS_RCV_UNIT_ID,
       FN_UNIT_NAME (A.COMPANY_ID, A.TRANS_RCV_UNIT_ID) RECEIVE_UNIT_NAME,
       B.SKU_ID,
       B.SKU_CODE,
       FN_SKU_NAME (B.COMPANY_ID, B.SKU_ID) SKU_NAME,
       FN_PACK_SIZE (B.COMPANY_ID, B.SKU_ID) PACK_SIZE,
       FN_SKU_PRICE (B.SKU_ID, B.SKU_CODE,  B.COMPANY_ID,A.TRANSFER_UNIT_ID) UNIT_TP,
       B.TRANSFER_QTY,
       FLOOR ( (NVL (B.TRANSFER_QTY, 0)) / I.SHIPPER_QTY)|| ' Box'|| ',' || MOD ( (NVL (B.TRANSFER_QTY, 0)), I.SHIPPER_QTY) || ' Pcs' QTY_BOX_PCS,
       (  B.TRANSFER_QTY * FN_SKU_PRICE (B.SKU_ID, B.SKU_CODE, B.COMPANY_ID, A.TRANSFER_UNIT_ID)) TRANSFER_AMOUNT,
       C.BATCH_ID,
       C.BATCH_NO,
       FN_UNIT_WISE_SKU_MRP (B.SKU_ID, B.SKU_CODE, A.COMPANY_ID,  A.TRANSFER_UNIT_ID, C.BATCH_ID) MRP,
       C.TRANSFER_QTY BATCH_TRANSFER_QTY,
       A.REMARKS MASTER_REMARKS,
       FLOOR ( (NVL (C.TRANSFER_QTY, 0)) / I.SHIPPER_QTY) || ' Box' || ',' || MOD ( (NVL (C.TRANSFER_QTY, 0)), I.SHIPPER_QTY) || ' Pcs'  BATCH_QTY_BOX_PCS,
       FN_DELIVERY_SLIP_WEIGHT (B.SKU_ID, B.SKU_CODE,  I.PACK_SIZE, NVL (B.TRANSFER_QTY,0),I.SHIPPER_QTY) GOODS_WEIGHT,
       FN_DELIVERY_SLIP_VOLUME (B.SKU_ID,B.SKU_CODE,I.PACK_SIZE,NVL (B.TRANSFER_QTY, 0),I.SHIPPER_QTY) GOODS_VOLUME
  FROM DEPOT_STOCK_TRANSFER_MST A,
       DEPOT_STOCK_TRANSFER_DTL B,
       DEPOT_STOCK_TRANSFER_BATCH C,
       PRODUCT_INFO I
 WHERE A.MST_ID = B.MST_ID AND B.DTL_ID = C.DTL_ID AND I.SKU_ID = B.SKU_ID
       AND A.COMPANY_ID = :param1
       AND TRUNC (A.TRANSFER_DATE) BETWEEN TRUNC ( TO_DATE ( :param2, 'DD/MM/YYYY')) AND TRUNC ( TO_DATE ( :param3, 'DD/MM/YYYY'))";

            var parameters = new List<string> { reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO };
            if (reportParameters.UNIT_ID != "ALL")
            {
                query += " AND A.TRANSFER_UNIT_ID = :param4";
                parameters.Add(reportParameters.UNIT_ID);
            }

            if (!string.IsNullOrWhiteSpace(reportParameters.TRANSFER_NOTE_NO)
            && reportParameters.TRANSFER_NOTE_NO != "undefined")
            {
                query += " AND A.TRANSFER_NO IN (";
                var arr = reportParameters.TRANSFER_NOTE_NO.Split(',');
                var ids = "";
                foreach (var no in arr)
                {
                    ids += "'" + no + "',";
                }
                query += ids.Substring(0, ids.Length - 1) + ") ";
            }

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }
        public DataTable StockTransferReportDataV2(ReportParams reportParameters)
        {
            List<QueryPattern> listOfQuery = new List<QueryPattern>();
            var query = @"SELECT A.MST_ID,
       B.DTL_ID,
       A.REF_NO,
       A.PRINT_COUNT,
       A.PRINT_PREVIEW_COUNT,
       A.REF_DATE,
       A.TRANSFER_TYPE,
       A.TRANSFER_NO,
       TO_CHAR (A.TRANSFER_DATE, 'DD/MM/RRRR') TRANSFER_DATE,
       A.TRANSFER_UNIT_ID,
       FN_UNIT_NAME (A.COMPANY_ID, A.TRANSFER_UNIT_ID) TRANSFER_UNIT_NAME,
       A.TRANS_RCV_UNIT_ID,
       FN_UNIT_NAME (A.COMPANY_ID, A.TRANS_RCV_UNIT_ID) RECEIVE_UNIT_NAME,
       B.SKU_ID,
       B.SKU_CODE,
       FN_SKU_NAME (A.COMPANY_ID, B.SKU_ID) SKU_NAME,
       FN_PACK_SIZE (A.COMPANY_ID, B.SKU_ID) PACK_SIZE,
       FN_SKU_PRICE (B.SKU_ID, B.SKU_CODE,  A.COMPANY_ID,A.TRANSFER_UNIT_ID) UNIT_TP,
       B.TRANSFER_QTY,
       FLOOR ( (NVL (B.TRANSFER_QTY, 0)) / I.SHIPPER_QTY)|| ' Box'|| ',' || MOD ( (NVL (B.TRANSFER_QTY, 0)), I.SHIPPER_QTY) || ' Pcs' QTY_BOX_PCS,
       --(B.TRANSFER_QTY * FN_SKU_PRICE (B.SKU_ID, B.SKU_CODE, A.COMPANY_ID, A.TRANSFER_UNIT_ID)) TRANSFER_AMOUNT,
       (SELECT SUM (TRANSFER_AMOUNT) TRANSFER_AMOUNT FROM  DEPOT_STOCK_TRANSFER_BATCH  WHERE   DTL_ID =B.DTL_ID )  TRANSFER_AMOUNT, 
       A.REMARKS MASTER_REMARKS,
       FN_DELIVERY_SLIP_WEIGHT (B.SKU_ID, B.SKU_CODE,  I.PACK_SIZE, NVL (B.TRANSFER_QTY,0),I.SHIPPER_QTY) GOODS_WEIGHT,
       FN_DELIVERY_SLIP_VOLUME (B.SKU_ID,B.SKU_CODE,I.PACK_SIZE,NVL (B.TRANSFER_QTY, 0),I.SHIPPER_QTY) GOODS_VOLUME,
       FN_USER_NAME(A.COMPANY_ID , COALESCE (A.UPDATED_BY ,A.ENTERED_BY)) ENTERED_BY
  FROM DEPOT_STOCK_TRANSFER_MST A,
       DEPOT_STOCK_TRANSFER_DTL B,
       PRODUCT_INFO I
 WHERE A.MST_ID = B.MST_ID 
 AND I.SKU_ID = B.SKU_ID
       AND A.COMPANY_ID = :param1
       AND TRUNC (A.TRANSFER_DATE) BETWEEN TRUNC ( TO_DATE ( :param2, 'DD/MM/YYYY')) AND TRUNC ( TO_DATE ( :param3, 'DD/MM/YYYY'))";
            var query2 = @"UPDATE DEPOT_STOCK_TRANSFER_MST A
   SET A.PRINT_COUNT =
          CASE
             WHEN :param1 = 'PRINT' THEN NVL (A.PRINT_COUNT, 0) + 1 ELSE NVL (A.PRINT_COUNT, 0)
          END,
       A.PRINT_PREVIEW_COUNT =
          CASE
             WHEN :param2 = 'PREVIEW' THEN  NVL (A.PRINT_PREVIEW_COUNT, 0) + 1 ELSE NVL (A.PRINT_PREVIEW_COUNT, 0)
          END
 WHERE     A.COMPANY_ID = :param3
       AND TRUNC (A.TRANSFER_DATE) BETWEEN TRUNC ( TO_DATE ( :param4,'DD/MM/YYYY')) AND TRUNC ( TO_DATE ( :param5, 'DD/MM/YYYY'))";
            var parameters = new List<string> { reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO };
            if (reportParameters.UNIT_ID != "ALL")
            {
                query += " AND A.TRANSFER_UNIT_ID = :param4";
                parameters.Add(reportParameters.UNIT_ID);
            }
            if (!string.IsNullOrWhiteSpace(reportParameters.TRANSFER_NOTE_NO) && reportParameters.TRANSFER_NOTE_NO != "undefined")
            {
                query += " AND A.TRANSFER_NO IN (";
                var arr = reportParameters.TRANSFER_NOTE_NO.Split(',');
                var ids = "";
                foreach (var no in arr)
                {
                    ids += "'" + no + "',";
                }
                query += ids.Substring(0, ids.Length - 1) + ") ";

            }
            if (!string.IsNullOrWhiteSpace(reportParameters.TRANSFER_NOTE_NO) && reportParameters.TRANSFER_NOTE_NO != "undefined")
            {
                query2 += " AND A.TRANSFER_NO IN (";
                var arr = reportParameters.TRANSFER_NOTE_NO.Split(',');
                var ids = "";
                foreach (var no in arr)
                {
                    ids += "'" + no + "',";
                }
                query2 += ids.Substring(0, ids.Length - 1) + ") ";

            }
            if (reportParameters.UNIT_ID != "ALL")
            {
                query2 += " AND A.TRANSFER_UNIT_ID =" + reportParameters.UNIT_ID;
            }
            query += " ORDER BY A.MST_ID, B.DTL_ID";
            listOfQuery.Add(commonServices.AddQuery(query2, commonServices.AddParameter(new string[] { reportParameters.PREVIEW == "NO" ? "PRINT" : "PREVIEW", reportParameters.PREVIEW == "NO" ? "PRINT" : "PREVIEW", reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO })));
            commonServices.SaveChanges(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, listOfQuery);
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }
        public DataTable StockTransferSubReportData(ReportParams reportParameters)
        {
            try
            {
                var query = @"SELECT C.BATCH_ID,
       C.BATCH_NO,
       FN_UNIT_WISE_SKU_MRP (B.SKU_ID,B.SKU_CODE,  A.COMPANY_ID, A.TRANSFER_UNIT_ID, C.BATCH_ID)MRP,
       C.TRANSFER_QTY BATCH_TRANSFER_QTY,
       C.UNIT_TP,
       FLOOR ( (NVL (C.TRANSFER_QTY, 0)) / I.SHIPPER_QTY)|| ' Box'|| ','|| MOD ( (NVL (C.TRANSFER_QTY, 0)), I.SHIPPER_QTY)|| ' Pcs' BATCH_QTY_BOX_PCS
 FROM DEPOT_STOCK_TRANSFER_MST A,
       DEPOT_STOCK_TRANSFER_DTL B,
       DEPOT_STOCK_TRANSFER_BATCH C,
       PRODUCT_INFO I
 WHERE     A.MST_ID = B.MST_ID
       AND B.DTL_ID = C.DTL_ID
       AND I.SKU_ID = B.SKU_ID
       AND C.MST_ID = :param1
       AND C.DTL_ID = :param2"
                ;
                return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(new string[] { reportParameters.MST_ID, reportParameters.DTL_ID }));
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataTable StockTransferReceiveReportData(ReportParams reportParameters)
        {

            var query = @"SELECT  A.DISPATCH_NO,
                   A.MST_ID,A.TRANS_RCV_NO, 
                   TO_CHAR(A.TRANS_RCV_DATE, 'DD/MM/RRRR')TRANS_RCV_DATE, 
                   A.TRANSFER_NO, 
                   TO_CHAR(A.TRANSFER_DATE, 'DD/MM/YYYY')TRANSFER_DATE, 
                   A.TRANSFER_UNIT_ID, 
                   FN_UNIT_NAME(A.COMPANY_ID,A.TRANSFER_UNIT_ID)TRANSFER_UNIT_NAME,
                   A.TRANS_RCV_UNIT_ID, 
                   FN_UNIT_NAME(A.COMPANY_ID,A.TRANS_RCV_UNIT_ID)RECEIVE_UNIT_NAME,
                   A.TRANS_RCV_BY, 
                   FN_EMPLOYEE_ID(A.TRANS_RCV_BY)EMPLOYEE_ID,
                   FN_EMPLOYEE_NAME(A.TRANS_RCV_BY) EMPLOYEE_NAME,
                   B.DTL_ID,
                   B.SKU_ID, 
                   B.SKU_CODE, 
                   FN_SKU_NAME(A.COMPANY_ID,B.SKU_ID) SKU_NAME,
                   FN_PACK_SIZE(A.COMPANY_ID,B.SKU_ID) PACK_SIZE,
                   B.UNIT_TP, 
                   B.TRANS_RCV_QTY,
                   B.TRANS_RCV_AMOUNT,
                   C.BATCH_ID, 
                   C.BATCH_NO, 
                   C.TRANS_RCV_QTY BATCH_TRANS_RCV_QTY,
                   A.REMARKS MASTER_REMARKS
            FROM DEPOT_STOCK_TRANS_RCV_MST A, DEPOT_STOCK_TRANS_RCV_DTL B, DEPOT_STOCK_TRANS_RCV_BATCH C
            WHERE A.MST_ID=B.MST_ID
            AND   B.DTL_ID=C.DTL_ID
            AND   A.COMPANY_ID = :param1
            AND   TRUNC(A.TRANS_RCV_DATE) BETWEEN TRUNC(TO_DATE(:param2, 'DD/MM/YYYY')) AND TRUNC(TO_DATE(:param3, 'DD/MM/YYYY'))";

            var parameters = new List<string> { reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO };
            if (reportParameters.UNIT_ID != "ALL")
            {
                query += " AND A.TRANS_RCV_UNIT_ID = :param4";
                parameters.Add(reportParameters.UNIT_ID);
            }

            if (!string.IsNullOrWhiteSpace(reportParameters.TRANSFER_NOTE_NO)
            && reportParameters.TRANSFER_NOTE_NO != "undefined")
            {
                query += " AND A.TRANSFER_NO IN (";
                var arr = reportParameters.TRANSFER_NOTE_NO.Split(',');
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