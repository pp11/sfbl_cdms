using Microsoft.Ajax.Utilities;
using Oracle.ManagedDataAccess.Client;
using SalesAndDistributionSystem.Domain.Models.ReportModels.Common.ProductReports;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.DynamicData;

namespace SalesAndDistributionSystem.Reporting.Areas.Inventory.Data
{
    public class FGReceive
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

        public DataTable ReceiveRegisterData(ReportParams reportParameters)
        {
            var query = @"SELECT ROWNUM as SL, R.*, P.SKU_NAME, U.UNIT_NAME FROM 
                (
                SELECT SKU_CODE, SKU_ID, BATCH_NO, PACK_SIZE, TO_CHAR(MFG_DATE, 'DD/MM/YYYY') AS MFG_DATE, TO_CHAR(EXPIRY_DATE, 'DD/MM/YYYY') AS EXP_DATE, RECEIVED_BY_ID, RECEIVE_QTY, 'Production' AS RECEIVE_TYPE, TO_CHAR(RECEIVE_DATE, 'DD/MM/YYYY') AS RECEIVE_DATE, UNIT_ID, UNIT_TP, COMPANY_ID
                FROM FG_RECEIVING_FROM_PRODUCTION
                WHERE COMPANY_ID = :param1 AND trunc(RECEIVE_DATE) BETWEEN TO_DATE(:param2, 'DD/MM/YYYY') AND TO_DATE(:param3, 'DD/MM/YYYY')

                UNION

                SELECT SKU_CODE, SKU_ID, BATCH_NO, PACK_SIZE, TO_CHAR(MFG_DATE, 'DD/MM/YYYY') AS MFG_DATE, TO_CHAR(EXPIRY_DATE, 'DD/MM/YYYY') AS EXP_DATE, RECEIVED_BY_ID, RECEIVE_QTY, 'Others' AS RECEIVE_TYPE, TO_CHAR(RECEIVE_DATE, 'DD/MM/YYYY') AS RECEIVE_DATE, UNIT_ID, UNIT_TP, COMPANY_ID
                FROM FG_RECEIVING_FROM_OTHERS
                WHERE COMPANY_ID = :param1 AND trunc(RECEIVE_DATE) BETWEEN TO_DATE(:param2, 'DD/MM/YYYY') AND TO_DATE(:param3, 'DD/MM/YYYY')
                ) R
                LEFT JOIN PRODUCT_INFO P ON R.SKU_ID = P.SKU_ID
                LEFT JOIN COMPANY_INFO U ON R.UNIT_ID = U.UNIT_ID AND R.COMPANY_ID = U.COMPANY_ID WHERE 1 = 1";

            var parameters = new List<string> { reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO };

            if (reportParameters.UNIT_ID != "ALL")
            {
                query += " And R.UNIT_ID = " + reportParameters.UNIT_ID;
                parameters.Add(reportParameters.UNIT_ID);
            }

            if (!string.IsNullOrWhiteSpace(reportParameters.BASE_PRODUCT_ID)
                && reportParameters.BASE_PRODUCT_ID != "undefined")
            {
                query += " AND R.SKU_ID IN (";
                var arr = reportParameters.BASE_PRODUCT_ID.Split(',');
                var ids = "";
                foreach (var no in arr)
                {
                    ids += "'" + no + "',";
                }
                query += ids.Substring(0, ids.Length - 1) + ") ";
            }
            if (!string.IsNullOrWhiteSpace(reportParameters.BATCH_NO)
                 && reportParameters.BATCH_NO != "undefined")
            {
                var arr = reportParameters.BATCH_NO.Split(',');
                query += " AND R.BATCH_NO IN (";
                var ids = "";
                foreach (var no in arr)
                {
                    ids += "'" + no + "',";
                }

                query += ids.Substring(0, ids.Length - 1) + ") ";
            }

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }

        public DataTable ReceiveRegisterProductWiseData(ReportParams reportParameters)
        {
            var query = @"SELECT ROWNUM as SL, S.* FROM (SELECT  R.SKU_CODE, R.SKU_ID, R.PACK_SIZE, P.SKU_NAME, C.UNIT_NAME, R.UNIT_ID, SUM(R.RECEIVE_QTY) AS RECEIVE_QTY FROM 
                (
                SELECT SKU_CODE, SKU_ID, PACK_SIZE, RECEIVE_QTY, UNIT_ID, COMPANY_ID, BATCH_NO
                FROM FG_RECEIVING_FROM_PRODUCTION
                WHERE COMPANY_ID = :param1 AND trunc(RECEIVE_DATE) BETWEEN TO_DATE(:param2, 'DD/MM/YYYY') AND TO_DATE(:param3, 'DD/MM/YYYY') 

                UNION

                SELECT SKU_CODE, SKU_ID, PACK_SIZE, RECEIVE_QTY, UNIT_ID, COMPANY_ID, BATCH_NO
                FROM FG_RECEIVING_FROM_OTHERS
                WHERE COMPANY_ID = :param1 AND trunc(RECEIVE_DATE) BETWEEN TO_DATE(:param2, 'DD/MM/YYYY') AND TO_DATE(:param3, 'DD/MM/YYYY')
                ) R
                LEFT JOIN PRODUCT_INFO P ON R.SKU_ID = P.SKU_ID
                LEFT JOIN COMPANY_INFO C ON R.UNIT_ID = C.UNIT_ID AND R.COMPANY_ID = C.COMPANY_ID
                WHERE 1 = 1";

            var parameters = new List<string> { reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO };

            if (reportParameters.UNIT_ID != "ALL")
            {
                query += " And R.UNIT_ID = " + reportParameters.UNIT_ID;
            }

            if (!string.IsNullOrWhiteSpace(reportParameters.BASE_PRODUCT_ID)
                && reportParameters.BASE_PRODUCT_ID != "undefined")
            {
                query += " AND R.SKU_ID IN (";
                var arr = reportParameters.BASE_PRODUCT_ID.Split(',');
                var ids = "";
                foreach (var no in arr)
                {
                    ids += "'" + no + "',";
                }
                query += ids.Substring(0, ids.Length - 1) + ") ";
            }
            if (!string.IsNullOrWhiteSpace(reportParameters.BATCH_NO)
                && reportParameters.BATCH_NO != "undefined")
            {
                var arr = reportParameters.BATCH_NO.Split(',');
                query += " AND R.BATCH_NO IN (";
                var ids = "";
                foreach (var no in arr)
                {
                    ids += "'" + no + "',";
                }

                query += ids.Substring(0, ids.Length - 1) + ") ";
            }
            query += @" GROUP BY R.SKU_CODE, R.SKU_ID, R.PACK_SIZE, SKU_NAME, C.UNIT_NAME, R.UNIT_ID
                ORDER BY SKU_NAME
                ) S";

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }
        public DataTable DailyBatchRcvRegister(ReportParams reportParameters)
        {
            string Query = @"begin  :param1 := FN_DATE_WISE_RCV_REG_PROD(TO_DATE(:param2, 'DD/MM/YYYY'), TO_DATE(:param3, 'DD/MM/YYYY'), :param4, :param5); end;";
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.COMPANY_ID.ToString(), reportParameters.UNIT_ID }));
        }
        public DataTable DateWiseFgRcvFromOtherRegister(ReportParams reportParameters)
        {
            string Query = @"begin  :param1 := FN_DATE_WISE_RCV_REG_OTH(TO_DATE(:param2, 'DD/MM/YYYY'), TO_DATE(:param3, 'DD/MM/YYYY'), :param4, :param5); end;";
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.COMPANY_ID.ToString(), reportParameters.UNIT_ID }));
        }
        public DataTable DateWiseMiscellaneousRegister(ReportParams reportParameters)
        {
            string Query = @"begin  :param1 := FN_DATE_WISE_RCV_REG_MISC(TO_DATE(:param2, 'DD/MM/YYYY'), TO_DATE(:param3, 'DD/MM/YYYY'), :param4, :param5); end;";
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.COMPANY_ID.ToString(), reportParameters.UNIT_ID }));
        }
        public DataTable DateWiseReceivingRegister(ReportParams reportParameters)
        {
            string Query = @"begin  :param1 := FN_DATE_WISE_RECEIVING_REG(TO_DATE(:param2, 'DD/MM/YYYY'), TO_DATE(:param3, 'DD/MM/YYYY'), :param4, :param5,:param6); end;";
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.COMPANY_ID.ToString(), reportParameters.UNIT_ID, reportParameters.BASE_PRODUCT_ID == null || reportParameters.BASE_PRODUCT_ID == "null" ? "ALL" : reportParameters.BASE_PRODUCT_ID }));
        }
        public DataTable DateWiseConsumptionRegi(ReportParams reportParameters)
        {
            string Query = @"begin  :param1 := FN_DATE_WISE_CON_REG(TO_DATE(:param2, 'DD/MM/YYYY'), TO_DATE(:param3, 'DD/MM/YYYY'), :param4, :param5,:param6); end;";

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.COMPANY_ID.ToString(), reportParameters.UNIT_ID, reportParameters.BASE_PRODUCT_ID == null || reportParameters.BASE_PRODUCT_ID == "null" ? "ALL" : reportParameters.BASE_PRODUCT_ID }));
        }
        public DataTable BatchSizeVsReceivingQty(ReportParams reportParameters)
        {
            string Query = @"begin  :param1 := fn_batch_size_vs_rcv_qty(TO_DATE(:param2, 'DD/MM/YYYY'), TO_DATE(:param3, 'DD/MM/YYYY') ,:param4, :param5, :param6); end;";
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.COMPANY_ID.ToString(), reportParameters.UNIT_ID, reportParameters.PRIMARY_PRODUCT_ID == null || reportParameters.PRIMARY_PRODUCT_ID == "null" || reportParameters.PRIMARY_PRODUCT_ID == "0" || reportParameters.PRIMARY_PRODUCT_ID == "" ? "ALL" : reportParameters.PRIMARY_PRODUCT_ID })); ;
        }
        
        public DataTable FGReceiveFromProduction(ReportParams reportParameters)
        {
            string query = @"SELECT RECEIVE_ID,
                   TO_CHAR(RECEIVE_DATE, 'DD/MM/YY') RECEIVE_DATE,
                   TRANSFER_NOTE_NO,
                   TO_CHAR(TRANSFER_DATE, 'DD/MM/YY') TRANSFER_DATE,
                   TRANSFER_TYPE,
                   UNIT_ID,
                   FN_UNIT_NAME(COMPANY_ID, UNIT_ID) UNIT_NAME,
                   BATCH_NO,
                   TO_CHAR(MFG_DATE, 'MON, YY') MFG_DATE,
                   TO_CHAR(EXPIRY_DATE, 'MON, YY') EXPIRY_DATE,
                   SKU_CODE,
                   FN_SKU_NAME(COMPANY_ID, SKU_ID) SKU_NAME,
                   PACK_SIZE,
                   UNIT_TP,
                   TRANSFER_QTY,
                   RECEIVE_QTY,
                   RECEIVE_AMOUNT,
                   FN_EMPLOYEE_ID(RECEIVED_BY_ID) EMPLOYEE_CODE,
                   FN_EMPLOYEE_NAME(RECEIVED_BY_ID) RECEIVED_BY_NAME
            FROM FG_RECEIVING_FROM_PRODUCTION
            WHERE COMPANY_ID =:param1 AND trunc(RECEIVE_DATE) BETWEEN TO_DATE(:param2, 'DD/MM/YYYY') AND TO_DATE(:param3, 'DD/MM/YYYY')";


            var parameters = new List<string> { reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO };

            if (reportParameters.UNIT_ID != "ALL")
            {
                query += " And UNIT_ID = :param4";
                parameters.Add(reportParameters.UNIT_ID);
            }

            if (!string.IsNullOrWhiteSpace(reportParameters.TRANSFER_NOTE_NO)
            && reportParameters.TRANSFER_NOTE_NO != "undefined")
            {
                query += " AND TRANSFER_NOTE_NO IN (";
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

        public DataTable FGReceiveFromOthers(ReportParams reportParameters)
        {
            string query = @"SELECT RECEIVE_ID,
                   TO_CHAR(RECEIVE_DATE, 'DD/MM/YY') RECEIVE_DATE,
                   CHALLAN_NO,
                   TO_CHAR(CHALLAN_DATE, 'DD/MM/YY') CHALLAN_DATE,
                   F.UNIT_ID,
                   FN_UNIT_NAME(F.COMPANY_ID, F.UNIT_ID) UNIT_NAME,
                   BATCH_NO,
                   TO_CHAR(MFG_DATE, 'MON, YY') MFG_DATE,
                   TO_CHAR(EXPIRY_DATE, 'MON, YY') EXPIRY_DATE,
                   SKU_CODE,
                   FN_SKU_NAME(F.COMPANY_ID, SKU_ID) SKU_NAME,
                   PACK_SIZE,
                   UNIT_TP,
                   RECEIVE_QTY,
                   RECEIVE_AMOUNT,
                   FN_EMPLOYEE_ID(RECEIVED_BY_ID) EMPLOYEE_CODE,
                   FN_EMPLOYEE_NAME(RECEIVED_BY_ID) RECEIVED_BY_NAME,
                   S.SUPPLIER_NAME
            FROM FG_RECEIVING_FROM_OTHERS F
            LEFT JOIN SUPPLIER_INFO S ON F.SUPPLIER_ID = S.SUPPLIER_ID
            WHERE F.COMPANY_ID =:param1 AND trunc(RECEIVE_DATE) BETWEEN TO_DATE(:param2, 'DD/MM/YYYY') AND TO_DATE(:param3, 'DD/MM/YYYY')";

            var parameters = new List<string> { reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO };

            if (reportParameters.UNIT_ID != "ALL")
            {
                query += " And F.UNIT_ID = :param4";
                parameters.Add(reportParameters.UNIT_ID);
            }

            if (!string.IsNullOrWhiteSpace(reportParameters.CHALLAN_NO)
            && reportParameters.CHALLAN_NO != "undefined")
            {
                query += " AND CHALLAN_NO IN (";
                var arr = reportParameters.CHALLAN_NO.Split(',');
                var ids = "";
                foreach (var no in arr)
                {
                    ids += "'" + no + "',";
                }
                query += ids.Substring(0, ids.Length - 1) + ") ";
            }

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }

        public DataTable RequisitionPending(ReportParams reportParameters)
        {
            string query = @"SELECT ISSUE_UNIT_ID,
                        FN_UNIT_NAME(COMPANY_ID,ISSUE_UNIT_ID) ISSUE_UNIT_NAME,
                        REQUISITION_NO,
                        TO_CHAR(REQUISITION_DATE, 'DD/MM/RRRR') REQUISITION_DATE,
                        REQUISITION_AMOUNT,
                        REQUISITION_UNIT_ID REQUISITION_RAISE_UNIT_ID,
                        FN_UNIT_NAME(COMPANY_ID,REQUISITION_UNIT_ID)
                        REQUISITION_RASIE_UNIT_NAME,
                        REQUISITION_RAISE_BY,
                        FN_EMPLOYEE_ID(REQUISITION_RAISE_BY) REQUISITION_RAISE_BY_EMP_ID,
                        FN_EMPLOYEE_NAME(REQUISITION_RAISE_BY)REQUISITION_RAISE_BY_NAME 
                FROM DEPOT_REQUISITION_RAISE_MST
                WHERE COMPANY_ID=:param1
                AND TRUNC(REQUISITION_DATE) BETWEEN TRUNC(TO_DATE(:param2,'DD/MM/RRRR')) AND TRUNC(TO_DATE(:param3,'DD/MM/RRRR'))
                AND REQUISITION_NO NOT IN (SELECT NVL(REQUISITION_NO,0)
                                            FROM DEPOT_REQUISITION_ISSUE_MST
                                            WHERE COMPANY_ID=:param1
                                           )";

            var parameters = new List<string> { reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO };

            if (reportParameters.UNIT_ID != "ALL" && !String.IsNullOrWhiteSpace(reportParameters.UNIT_ID))
            {
                query += "  AND ISSUE_UNIT_ID = " + reportParameters.UNIT_ID;
            }

            if (!string.IsNullOrWhiteSpace(reportParameters.MST_ID)
                && reportParameters.MST_ID != "undefined")
            {
                query += " AND MST_ID IN (";
                var arr = reportParameters.MST_ID.Split(',');
                var ids = "";
                foreach (var no in arr)
                {
                    ids += "'" + no + "',";
                }
                query += ids.Substring(0, ids.Length - 1) + ") ";
            }

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }
        public DataTable RequisitionPendingForDispatch(ReportParams reportParameters)
        {
            string query = @"SELECT DISTINCT
                       A.COMPANY_ID,
                       FN_COMPANY_NAME(A.COMPANY_ID) COMPANY_NAME,
                       A.REQUISITION_UNIT_ID,
                       FN_UNIT_NAME(A.COMPANY_ID,A.REQUISITION_UNIT_ID) REQUISITION_RASIE_UNIT_NAME, 
                       A.ISSUE_UNIT_ID,
                       FN_UNIT_NAME(A.COMPANY_ID,A.ISSUE_UNIT_ID)ISSUE_UNIT_NAME,
                       A.REQUISITION_NO,
                       TO_CHAR(B.REQUISITION_DATE, 'DD/MM/YYYY') REQUISITION_DATE,
                       B.ISSUE_NO,
                       TO_CHAR(B.ISSUE_DATE, 'DD/MM/YYYY') ISSUE_DATE,
                       B.ISSUE_BY ISSUE_USER_ID,
                       FN_EMPLOYEE_ID(B.ISSUE_BY) REQUISITION_RAISE_BY_EMP_ID,
                       FN_EMPLOYEE_NAME(B.ISSUE_BY) REQUISITION_RAISE_BY_NAME
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
                                WHERE  A.MST_ID = B.MST_ID";

            if (reportParameters.UNIT_ID != "ALL" && !String.IsNullOrWhiteSpace(reportParameters.UNIT_ID))
            {
                query += " AND A.ISSUE_UNIT_ID = " + reportParameters.UNIT_ID;
            }
            if (!string.IsNullOrWhiteSpace(reportParameters.REQUISITION_NO)
               && reportParameters.REQUISITION_NO != "undefined")
            {
                query += " AND A.REQUISITION_NO IN (";
                var arr = reportParameters.REQUISITION_NO.Split(',');
                var ids = "";
                foreach (var no in arr)
                {
                    ids += "'" + no + "',";
                }
                query += ids.Substring(0, ids.Length - 1) + ") ";
            }

            query += @" UNION ALL
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
                                AND   B.DISPATCH_REQ_ID=C.DISPATCH_REQ_ID";

            if (reportParameters.UNIT_ID != "ALL" && !String.IsNullOrWhiteSpace(reportParameters.UNIT_ID))
            {
                query += " AND B.ISSUE_UNIT_ID = " + reportParameters.UNIT_ID;
            }

            if (!string.IsNullOrWhiteSpace(reportParameters.REQUISITION_NO)
               && reportParameters.REQUISITION_NO != "undefined")
            {
                query += " AND B.REQUISITION_NO IN (";
                var arr = reportParameters.REQUISITION_NO.Split(',');
                var ids = "";
                foreach (var no in arr)
                {
                    ids += "'" + no + "',";
                }
                query += ids.Substring(0, ids.Length - 1) + ") ";
            }

            query += @"
                             )
                        GROUP BY COMPANY_ID,REQUISITION_UNIT_ID,ISSUE_UNIT_ID,REQUISITION_NO,SKU_ID,SKU_CODE,UNIT_TP
                   )A, DEPOT_REQUISITION_ISSUE_MST B
            WHERE A.REQUISITION_NO=B.REQUISITION_NO
            AND NVL(A.ISSUE_QTY,0)-NVL(A.DISPATCH_QTY,0) > 0
            AND A.COMPANY_ID=:param1
            AND TRUNC(B.REQUISITION_DATE) BETWEEN TRUNC(TO_DATE(:param2,'DD/MM/RRRR')) AND TRUNC(TO_DATE(:param3,'DD/MM/RRRR'))";

            var parameters = new List<string> { reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO };


            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }

        public DataTable RequisitionPendingForReceive(ReportParams reportParameters)
        {
            string query = @"SELECT DISTINCT
                       A.COMPANY_ID,
                       FN_COMPANY_NAME(A.COMPANY_ID) COMPANY_NAME,
                       A.REQUISITION_UNIT_ID,
                       FN_UNIT_NAME(A.COMPANY_ID,A.REQUISITION_UNIT_ID) REQUISITION_RASIE_UNIT_NAME, 
                       A.REQUISITION_NO,
                       TO_CHAR(B.REQUISITION_DATE, 'DD/MM/YYYY') REQUISITION_DATE,
                       B.REQUISITION_RAISE_BY,
                       FN_EMPLOYEE_ID(B.REQUISITION_RAISE_BY) REQUISITION_RAISE_BY_EMP_ID,
                       FN_EMPLOYEE_NAME(B.REQUISITION_RAISE_BY) REQUISITION_RAISE_BY_NAME
                 FROM
                    (
                        SELECT COMPANY_ID,
                               REQUISITION_UNIT_ID,
                               REQUISITION_NO,
                               MST_ID,
                               SKU_ID,
                               SKU_CODE,
                               SUM(NVL(REQUISITION_QTY,0))REQUISITION_QTY, 
                               SUM(NVL(RECEIVE_QTY,0))RECEIVE_QTY
                         FROM
                             (
                                SELECT A.COMPANY_ID,
                                       A.REQUISITION_UNIT_ID,
                                       A.REQUISITION_NO,
                                       A.MST_ID,
                                       B.SKU_ID,
                                       B.SKU_CODE,
                                       NVL(B.REQUISITION_QTY,0)REQUISITION_QTY,
                                       0 RECEIVE_QTY
                                FROM DEPOT_REQUISITION_RAISE_MST A, DEPOT_REQUISITION_RAISE_DTL B
                                WHERE A.MST_ID=B.MST_ID
                                AND   A.COMPANY_ID=:param1";

            if (reportParameters.UNIT_ID != "ALL" && !String.IsNullOrWhiteSpace(reportParameters.UNIT_ID))
            {
                query += " AND   A.REQUISITION_UNIT_ID=" + reportParameters.UNIT_ID;
            }

            query += @" AND   TRUNC(A.REQUISITION_DATE) BETWEEN TRUNC(TO_DATE(:param2,'DD/MM/RRRR')) AND TRUNC(TO_DATE(:param3,'DD/MM/RRRR'))

                                UNION ALL

                                SELECT A.COMPANY_ID,
                                       A.REQUISITION_UNIT_ID,
                                       A.REQUISITION_NO,
                                       A.MST_ID,
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

            if (reportParameters.UNIT_ID != "ALL" && !String.IsNullOrWhiteSpace(reportParameters.UNIT_ID))
            {
                query += " AND   A.REQUISITION_UNIT_ID=" + reportParameters.UNIT_ID;
            }

            query += @" AND   TRUNC(C.REQUISITION_DATE) BETWEEN TRUNC(TO_DATE(:param2,'DD/MM/RRRR')) AND TRUNC(TO_DATE(:param3,'DD/MM/RRRR'))
                             )
                        GROUP BY COMPANY_ID,REQUISITION_UNIT_ID,REQUISITION_NO,MST_ID,SKU_ID,SKU_CODE
                   )A, DEPOT_REQUISITION_RAISE_MST B
            WHERE A.REQUISITION_NO=B.REQUISITION_NO";

            //if (!string.IsNullOrWhiteSpace(reportParameters.REQUISITION_NO)
            //   && reportParameters.REQUISITION_NO != "undefined")
            //{
            //    query += " AND A.REQUISITION_NO IN (";
            //    var arr = reportParameters.REQUISITION_NO.Split(',');
            //    var ids = "";
            //    foreach (var no in arr)
            //    {
            //        ids += "'" + no + "',";
            //    }
            //    query += ids.Substring(0, ids.Length - 1) + ") ";
            //}

            if (!string.IsNullOrWhiteSpace(reportParameters.MST_ID)
                && reportParameters.MST_ID != "undefined"
                && reportParameters.MST_ID != "All")
            {
                query += " AND A.MST_ID IN (";
                var arr = reportParameters.MST_ID.Split(',');
                var ids = "";
                foreach (var no in arr)
                {
                    ids += "'" + no + "',";
                }
                query += ids.Substring(0, ids.Length - 1) + ") ";
            }

            var parameters = new List<string> { reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO };

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }
        public DataTable OverallRequisitionStatus(ReportParams reportParameters)
        {
            string query = @"SELECT FN_UNIT_NAME(A.COMPANY_ID,A.REQUISITION_UNIT_ID) REQUISITION_UNIT_NAME,
                   A.REQUISITION_NO,
                   TO_CHAR(A.REQUISITION_DATE, 'DD/MM/YYYY') REQUISITION_DATE,
                   A.SKU_ID,
                   A.SKU_CODE,
                   FN_SKU_NAME(A.COMPANY_ID,A.SKU_ID) SKU_NAME,
                   FN_PACK_SIZE(A.COMPANY_ID,A.SKU_ID) PACK_SIZE,
                   A.REQUISITION_QTY,
                   B.ISSUE_QTY,
                   C.DISPATCH_QTY,
                   D.RECEIVE_QTY
            FROM 
               (
                --Requisition Raise
                SELECT A.COMPANY_ID,
                       A.REQUISITION_UNIT_ID,
                       A.REQUISITION_NO,
                       A.REQUISITION_DATE,
                       B.SKU_ID,
                       B.SKU_CODE,
                       NVL(B.REQUISITION_QTY,0)REQUISITION_QTY
                FROM DEPOT_REQUISITION_RAISE_MST A, DEPOT_REQUISITION_RAISE_DTL B
                WHERE A.MST_ID=B.MST_ID
                AND   A.COMPANY_ID=:param1";

            if (reportParameters.UNIT_ID != "ALL" && !String.IsNullOrWhiteSpace(reportParameters.UNIT_ID))
            {
                query += " AND   A.REQUISITION_UNIT_ID=" + reportParameters.UNIT_ID;
            }

            if (!string.IsNullOrWhiteSpace(reportParameters.MST_ID)
                && reportParameters.MST_ID != "undefined"
                && reportParameters.MST_ID != "All")
            {
                query += " AND A.MST_ID IN (";
                var arr = reportParameters.MST_ID.Split(',');
                var ids = "";
                foreach (var no in arr)
                {
                    ids += "'" + no + "',";
                }
                query += ids.Substring(0, ids.Length - 1) + ") ";
            }

            query += @" AND   TRUNC(A.REQUISITION_DATE) BETWEEN TRUNC(TO_DATE(:param2,'DD/MM/RRRR')) AND TRUNC(TO_DATE(:param3,'DD/MM/RRRR'))
              )A,
              (
                   --Requisition Issue
                SELECT A.REQUISITION_NO,
                       B.SKU_ID,
                       B.SKU_CODE,
                       SUM(NVL(B.ISSUE_QTY,0))ISSUE_QTY
                FROM   DEPOT_REQUISITION_ISSUE_MST A, DEPOT_REQUISITION_ISSUE_DTL B
                WHERE  A.MST_ID = B.MST_ID
                AND    A.COMPANY_ID=:param1
                AND    TRUNC(A.REQUISITION_DATE) BETWEEN TRUNC(TO_DATE(:param2,'DD/MM/RRRR')) AND TRUNC(TO_DATE(:param3,'DD/MM/RRRR'))
                GROUP BY A.REQUISITION_NO,B.SKU_ID,B.SKU_CODE 
                ) B,
                (
                ---Requisition Dispatch
                SELECT B.REQUISITION_NO,
                       C.SKU_ID,
                       C.SKU_CODE,
                       SUM(NVL(C.DISPATCH_QTY,0))DISPATCH_QTY
                FROM DEPOT_DISPATCH_MST A, DEPOT_DISPATCH_REQUISITION B, DEPOT_DISPATCH_PRODUCT C
                WHERE   A.COMPANY_ID=:param1
                AND     A.MST_ID=B.MST_ID
                AND     B.DISPATCH_REQ_ID=C.DISPATCH_REQ_ID
                AND     TRUNC(B.REQUISITION_DATE) BETWEEN TRUNC(TO_DATE(:param2,'DD/MM/RRRR')) AND TRUNC(TO_DATE(:param3,'DD/MM/RRRR')) 
                GROUP BY B.REQUISITION_NO,C.SKU_ID,C.SKU_CODE
                )C,
                (
                 -- Requisition Dispatch
                SELECT A.REQUISITION_NO,
                       B.SKU_ID,
                       B.SKU_CODE,
                       SUM(NVL(B.RECEIVE_QTY,0)) RECEIVE_QTY
                FROM   DEPOT_REQUISITION_RCV_MST A, DEPOT_REQUISITION_RCV_DTL B,DEPOT_REQUISITION_RAISE_MST C
                WHERE  A.MST_ID = B.MST_ID
                AND    A.REQUISITION_NO=C.REQUISITION_NO 
                AND   A.COMPANY_ID=:param1
                AND   TRUNC(C.REQUISITION_DATE) BETWEEN TRUNC(TO_DATE(:param2,'DD/MM/RRRR')) AND TRUNC(TO_DATE(:param3,'DD/MM/RRRR')) 
                GROUP BY A.REQUISITION_NO,B.SKU_ID,B.SKU_CODE
                )D
            WHERE A.REQUISITION_NO=B.REQUISITION_NO(+)
            AND   A.SKU_ID=B.SKU_ID(+) 

            AND   A.REQUISITION_NO=C.REQUISITION_NO(+)
            AND   A.SKU_ID=C.SKU_ID(+)  

            AND   A.REQUISITION_NO=D.REQUISITION_NO(+)
            AND   A.SKU_ID=D.SKU_ID(+)
            ";

            var parameters = new List<string> { reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO };

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }

        public DataTable OverallRequisitionStatusDetails(ReportParams reportParameters)
        {
            string query = @"SELECT A.REQUISITION_UNIT_ID,
                   FN_UNIT_NAME(A.COMPANY_ID, A.REQUISITION_UNIT_ID) REQUISITION_UNIT_NAME,
                   A.REQUISITION_NO,
                   TO_CHAR(A.REQUISITION_DATE, 'DD/MM/RR') REQUISITION_DATE,
                   A.SKU_ID,
                   A.SKU_CODE,
                   FN_SKU_NAME(A.COMPANY_ID, A.SKU_ID) SKU_NAME,
                   FN_PACK_SIZE(A.COMPANY_ID, A.SKU_ID) PACK_SIZE,
                   A.REQUISITION_QTY,
       
                   B.ISSUE_NO,
                   FN_UNIT_NAME(A.COMPANY_ID, B.ISSUE_UNIT_ID) ISSUE_UNIT,
                   TO_CHAR(B.ISSUE_DATE, 'DD/MM/RR') ISSUE_DATE,
                   B.ISSUE_QTY,
       
                   C.DISPATCH_NO,
                   C.DISPATCH_UNIT_ID,
                   TO_CHAR(C.DISPATCH_DATE, 'DD/MM/RR') DISPATCH_DATE,
                   C.DISPATCH_QTY,
       
                   D.RECEIVE_NO,
                   D.REQUISITION_RCV_UNIT_ID,
                   TO_CHAR(D.RECEIVE_DATE, 'DD/MM/RR') RECEIVE_DATE,
                   D.RECEIVE_QTY
            FROM 
               (
                --Requisition Raise
                SELECT A.REQUISITION_UNIT_ID,
                       A.REQUISITION_NO,
                       A.REQUISITION_DATE,
                       A.COMPANY_ID,
                       B.SKU_ID,
                       B.SKU_CODE,
                       NVL(B.REQUISITION_QTY,0)REQUISITION_QTY
                FROM DEPOT_REQUISITION_RAISE_MST A, DEPOT_REQUISITION_RAISE_DTL B
                WHERE A.MST_ID=B.MST_ID
                AND   A.COMPANY_ID=:param1";

            if (reportParameters.UNIT_ID != "ALL" && !String.IsNullOrWhiteSpace(reportParameters.UNIT_ID))
            {
                query += " AND   A.REQUISITION_UNIT_ID=" + reportParameters.UNIT_ID;
            }

            if (!string.IsNullOrWhiteSpace(reportParameters.MST_ID)
                && reportParameters.MST_ID != "undefined"
                && reportParameters.MST_ID != "All")
            {
                query += " AND A.MST_ID IN (";
                var arr = reportParameters.MST_ID.Split(',');
                var ids = "";
                foreach (var no in arr)
                {
                    ids += "'" + no + "',";
                }
                query += ids.Substring(0, ids.Length - 1) + ") ";
            }

            query += @" AND   TRUNC(A.REQUISITION_DATE) BETWEEN TRUNC(TO_DATE(:param2,'DD/MM/RRRR')) AND TRUNC(TO_DATE(:param3,'DD/MM/RRRR'))
              )A,
              (
                   --Requisition Issue
                SELECT A.REQUISITION_NO,
                      A.ISSUE_NO,
                       A.ISSUE_UNIT_ID,
                       A.ISSUE_DATE,
                       B.SKU_ID,
                       B.SKU_CODE,
                       NVL(B.ISSUE_QTY,0)ISSUE_QTY
                FROM   DEPOT_REQUISITION_ISSUE_MST A, DEPOT_REQUISITION_ISSUE_DTL B
                WHERE  A.MST_ID = B.MST_ID
                AND    A.COMPANY_ID=:param1 
                ) B,
                (
                ---Requisition Dispatch
                SELECT B.REQUISITION_NO,
                       A.DISPATCH_NO,
                       B.DISPATCH_UNIT_ID,
                       A.DISPATCH_DATE,
                       C.SKU_ID,
                       C.SKU_CODE,
                       C.DISPATCH_QTY
                FROM DEPOT_DISPATCH_MST A, DEPOT_DISPATCH_REQUISITION B, DEPOT_DISPATCH_PRODUCT C
                WHERE   A.COMPANY_ID=:param1
                AND     A.MST_ID=B.MST_ID
                AND     B.DISPATCH_REQ_ID=C.DISPATCH_REQ_ID
                )C,
                (
                 -- Requisition Dispatch
                SELECT A.REQUISITION_NO,
                       A.RECEIVE_NO,
                       A.RECEIVE_DATE,
                       A.REQUISITION_UNIT_ID REQUISITION_RCV_UNIT_ID ,
                       B.SKU_ID,
                       B.SKU_CODE,
                       NVL(B.RECEIVE_QTY,0)RECEIVE_QTY
                FROM   DEPOT_REQUISITION_RCV_MST A, DEPOT_REQUISITION_RCV_DTL B,DEPOT_REQUISITION_RAISE_MST C
                WHERE  A.MST_ID = B.MST_ID
                AND    A.REQUISITION_NO=C.REQUISITION_NO 
                AND   A.COMPANY_ID=:param1
                )D
            WHERE A.REQUISITION_NO=B.REQUISITION_NO(+)
            AND   A.SKU_ID=B.SKU_ID(+) 

            AND   A.REQUISITION_NO=C.REQUISITION_NO(+)
            AND   A.SKU_ID=C.SKU_ID(+)  

            AND   A.REQUISITION_NO=D.REQUISITION_NO(+)
            AND   A.SKU_ID=D.SKU_ID(+)
            ";

            var parameters = new List<string> { reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO };

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }
        public DataTable TransferVsReceive(ReportParams reportParameters)
        {
            string Query = @"begin  :param1 := FN_TRANSFER_VS_RECEIVE_REPORT(TO_DATE(:param2, 'DD/MM/YYYY'), TO_DATE(:param3, 'DD/MM/YYYY') ,:param4, :param5, :param6); end;";
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.UNIT_ID, reportParameters.COMPANY_ID.ToString(), reportParameters.PRIMARY_PRODUCT_ID == null || reportParameters.PRIMARY_PRODUCT_ID == "null" || reportParameters.PRIMARY_PRODUCT_ID == "0" || reportParameters.PRIMARY_PRODUCT_ID == "" ? "ALL" : reportParameters.PRIMARY_PRODUCT_ID })); ;
        }

    }
}