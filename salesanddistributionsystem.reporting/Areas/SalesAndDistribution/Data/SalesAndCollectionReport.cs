using Oracle.ManagedDataAccess.Client;
using SalesAndDistributionSystem.Domain.Models.ReportModels.Common.ProductReports;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SalesAndDistributionSystem.Reporting.Areas.SalesAndDistribution.Data
{
    public class SalesAndCollectionReport
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
        string CollectionRegister_query() => @"SELECT M.COLLECTION_MST_ID, BATCH_NO,
                TO_CHAR(BATCH_DATE, 'DD/MM/YYYY') BATCH_DATE,
                BATCH_STATUS,
                M.BATCH_POSTING_STATUS,
                FN_UNIT_NAME(M.COMPANY_ID, M.UNIT_ID) COLLECTION_UNIT,
                D.REMARKS,
                CUSTOMER_CODE,
                FN_CUSTOMER_NAME(D.COMPANY_ID, CUSTOMER_ID) CUSTOMER_NAME,
                VOUCHER_NO,
                TO_CHAR(VOUCHER_DATE, 'DD/MM/YYYY') VOUCHER_DATE,
                FN_BRANCH_NAME(BANK_ID, BRANCH_ID) BRANCH_NAME,
                FN_BANK_NAME(BANK_ID) BANK_NAME,
                INVOICE_NO,
                COLLECTION_MODE,
                COLLECTION_AMT,
                TDS_AMT,
                MEMO_COST,
                NET_COLLECTION_AMT
                FROM COLLECTION_MST M, COLLECTION_DTL D
                WHERE M.COLLECTION_MST_ID = D.COLLECTION_MST_ID
                AND M.COMPANY_ID = :param1
                AND TO_DATE(BATCH_DATE,'DD/MM/rrrr') BETWEEN TO_DATE(:param2, 'DD/MM/rrrr') AND TO_DATE(:param3, 'DD/MM/rrrr')";
        string CollectionRegisterWithCustomer_Query() => @"SELECT M.COLLECTION_MST_ID, BATCH_NO,
                TO_CHAR(BATCH_DATE, 'DD/MM/YYYY') BATCH_DATE,
                BATCH_STATUS,
                M.BATCH_POSTING_STATUS,
                FN_UNIT_NAME(M.COMPANY_ID, M.UNIT_ID) COLLECTION_UNIT,
                D.REMARKS,
                CUSTOMER_CODE,
                FN_CUSTOMER_NAME(D.COMPANY_ID, CUSTOMER_ID) CUSTOMER_NAME,
                VOUCHER_NO,
                TO_CHAR(VOUCHER_DATE, 'DD/MM/YYYY') VOUCHER_DATE,
                FN_BRANCH_NAME(BANK_ID, BRANCH_ID) BRANCH_NAME,
                FN_BANK_NAME(BANK_ID) BANK_NAME,
                INVOICE_NO,
                COLLECTION_MODE,
                COLLECTION_AMT,
                TDS_AMT,
                MEMO_COST,
                NET_COLLECTION_AMT,
                D.ENTERED_DATE,
                D.COLLECTION_DTL_ID
                FROM COLLECTION_MST M, COLLECTION_DTL D
                WHERE M.COLLECTION_MST_ID = D.COLLECTION_MST_ID
                AND M.COLLECTION_MST_ID = :param1 
                ORDER BY D.ENTERED_DATE DESC, D.COLLECTION_DTL_ID ASC ";


        public DataTable DailySalesRegister(ReportParams reportParameters)
        {
            var query = @"  SELECT A.INVOICE_NO,
         TO_CHAR (A.INVOICE_DATE, 'DD/MM/YY') INVOICE_DATE,
         D.INVOICE_TYPE_NAME INVOICE_TYPE_CODE,
         A.MARKET_ID,
         B.MARKET_CODE,
         B.MARKET_NAME,
         A.CUSTOMER_ID,
         A.CUSTOMER_CODE,
         C.CUSTOMER_NAME,
         A.TOTAL_AMOUNT,
         A.CUSTOMER_DISC_AMOUNT,
         A.CUSTOMER_ADD1_DISC_AMOUNT,
         A.CUSTOMER_ADD2_DISC_AMOUNT,
         A.CUSTOMER_PRODUCT_DISC_AMOUNT,
         A.BONUS_PRICE_DISC_AMOUNT,
         A.PROD_BONUS_PRICE_DISC_AMOUNT,
         A.LOADING_CHARGE_AMOUNT,
         NVL(A.INVOICE_ADJUSTMENT_AMOUNT,0) INVOICE_ADJUSTMENT_AMOUNT,
         NVL(A.INVOICE_DISCOUNT_AMOUNT,0) INVOICE_DISCOUNT_AMOUNT,
         CASE
            WHEN A.INVOICE_TYPE_CODE = 'I0001'
            THEN
               NVL (A.NET_INVOICE_AMOUNT, 0)
            ELSE
               0
         END
         NET_INVOICE_AMOUNT,
         A.TDS_AMOUNT,
         CASE
            WHEN A.INVOICE_TYPE_CODE = 'I0001'
            THEN
               NVL (A.NET_INVOICE_AMOUNT, 0) + NVL (A.TDS_AMOUNT, 0)
            ELSE
               0
         END
            TOTAL_PAYABLE,
         FN_UNIT_NAME (A.COMPANY_ID, A.INVOICE_UNIT_ID) INVOICE_UNIT
    FROM INVOICE_MST A,
         MARKET_INFO B,
         CUSTOMER_INFO C,
         INVOICE_TYPE_INFO D
   WHERE     A.MARKET_ID = B.MARKET_ID
         AND A.CUSTOMER_ID = C.CUSTOMER_ID
         AND A.INVOICE_TYPE_CODE = D.INVOICE_TYPE_CODE(+)
         AND A.COMPANY_ID = :param1
         AND TRUNC (INVOICE_DATE) = TRUNC (TO_DATE ( :param2, 'DD/MM/RRRR'))";

            var parameters = new List<string> { reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_FROM.ToString() };

            if (reportParameters.UNIT_ID != "ALL")
            {
                query += " And A.INVOICE_UNIT_ID = " + reportParameters.UNIT_ID;
            }
            query += " ORDER BY A.INVOICE_UNIT_ID, A.MST_ID DESC";
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }

        public DataTable CollectionRegister(ReportParams reportParameters)
        {
            var query = "";
            var parameters = new List<string>();
            if (String.IsNullOrWhiteSpace(reportParameters.MST_ID) || reportParameters.MST_ID == "undefined")
            {
                query = CollectionRegister_query();
                parameters = new List<string> { reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_FROM.ToString(), reportParameters.DATE_TO.ToString() };
                //if (reportParameters.UNIT_ID != "ALL")
                //{
                //    query += " And M.UNIT_ID = " + reportParameters.UNIT_ID;
                //}

                if (reportParameters.CUSTOMER_ID[0] != '0')
                {
                    query += " AND D.CUSTOMER_ID IN (";
                    var arr = reportParameters.CUSTOMER_ID.Split(',');
                    var ids = "";
                    foreach (var no in arr)
                    {
                        ids += "'" + no + "',";
                    }
                    query += ids.Substring(0, ids.Length - 1) + ") ";

                }
                query += " ORDER BY M.COLLECTION_MST_ID, D.COLLECTION_DTL_ID";
            }
            else
            {
                query = CollectionRegisterWithCustomer_Query();
                parameters = new List<string> { reportParameters.MST_ID };
            }
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }

        #region Invoice wise return
        public DataTable InvoiceWiseReturn(ReportParams reportParameters)
        {
            var query = @"SELECT RETURN_NO,
                   TO_CHAR(RETURN_DATE, 'DD/MM/YY') RETURN_DATE,
                   FN_UNIT_NAME(A.COMPANY_ID, RETURN_UNIT_ID) RETURN_UNIT,
                   A.RETURN_TYPE,
                   A.INVOICE_NO,
                   TO_CHAR(INVOICE_DATE, 'DD/MM/YYYY') INVOICE_DATE,
                   FN_CUSTOMER_NAME(A.COMPANY_ID, CUSTOMER_ID) CUSTOMER_NAME,
                   A.MARKET_ID,
                   M.MARKET_NAME,
                   FN_SKU_NAME(A.COMPANY_ID, SKU_ID) SKU_NAME,
                   SKU_CODE,
                   UNIT_TP,
                   INVOICE_QTY,
                   B.RETURN_QTY
            FROM RETURN_MST A, RETURN_DTL B, MARKET_INFO M
            WHERE A.MST_ID=B.MST_ID
            AND   A.MARKET_ID(+) = M.MARKET_ID
            AND   A.COMPANY_ID = :param1
            AND   A.MST_ID=:param2";

            var parameters = new List<string> { reportParameters.COMPANY_ID.ToString(), reportParameters.MST_ID };

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }

        public DataTable ReturnProductBonus(ReportParams reportParameters)
        {
            var query = @"SELECT R.*, I.BONUS_QTY FROM
                (
                SELECT DISTINCT
                           b.INVOICE_NO,
                           C.SKU_ID,
                           C.SKU_CODE,
                           FN_SKU_NAME(C.COMPANY_ID, C.SKU_ID) SKU_NAME,
                           C.UNIT_TP,
                           SUM(NVL(C.RETURN_BONUS_QTY,0))RETURN_BONUS_QTY
                    FROM  RETURN_DTL B, RETURN_BONUS C
                    WHERE B.DTL_ID=C.DTL_ID
                    AND   B.COMPANY_ID = :param1
                    AND   B.MST_ID=:param2
                    GROUP BY B.MST_ID,C.SKU_ID,C.SKU_CODE,C.UNIT_TP,B.INVOICE_QTY, C.COMPANY_ID,b.INVOICE_NO
                ) R,
                (
                SELECT A.INVOICE_NO,C.SKU_ID,C.BONUS_QTY
                FROM INVOICE_MST A, INVOICE_DTL B,INVOICE_BONUS C
                WHERE A.MST_ID=B.MST_ID
                AND   B.DTL_ID=C.DTL_ID
                ) I
                WHERE R.INVOICE_NO = I.INVOICE_NO
                AND R.SKU_ID = I.SKU_ID";

            var parameters = new List<string> { reportParameters.COMPANY_ID.ToString(), reportParameters.MST_ID };

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }

        public DataTable ReturnComboBonus(ReportParams reportParameters)
        {
            var query = @"SELECT R.*, I.BONUS_QTY FROM
                (
                SELECT SKU_ID,
                SKU_CODE,
                FN_SKU_NAME(A.COMPANY_ID, SKU_ID) SKU_NAME,
                UNIT_TP,
                B.INVOICE_NO,
                SUM(RETURN_BONUS_QTY) RETURN_BONUS_QTY
                FROM RETURN_COMBO_BONUS A, RETURN_MST B
                WHERE A.MST_ID = :param1
                AND A.MST_ID = B.MST_ID
                GROUP BY SKU_CODE, UNIT_TP, A.COMPANY_ID, SKU_ID, B.INVOICE_NO
                ) R, (
                SELECT A.INVOICE_NO, B.SKU_ID, B.BONUS_QTY
                FROM INVOICE_MST A, INVOICE_COMBO_BONUS B
                WHERE A.MST_ID=B.MST_ID
                ) I
                WHERE R.INVOICE_NO = I.INVOICE_NO
                AND R.SKU_ID = I.SKU_ID";

            var parameters = new List<string> { reportParameters.MST_ID };

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }

        public DataTable ReturnComboGiftBonus(ReportParams reportParameters)
        {
            var query = @"SELECT R.*, I.GIFT_QTY FROM
                (
                SELECT
                GIFT_ITEM_ID,
                FN_GIFT_NAME(A.COMPANY_ID, GIFT_ITEM_ID) GIFT_NAME,
                UNIT_TP,
                B.INVOICE_NO,
                SUM(GIFT_RETURN_QTY) GIFT_RETURN_QTY
                FROM RETURN_COMBO_GIFT A, RETURN_MST B
                WHERE A.MST_ID = :param1
                AND A.MST_ID = B.MST_ID
                GROUP BY UNIT_TP, A.COMPANY_ID, GIFT_ITEM_ID, B.INVOICE_NO
                ) R, (
                SELECT A.INVOICE_NO, B.GIFT_ITEM_ID, B.GIFT_QTY
                FROM INVOICE_MST A, INVOICE_COMBO_GIFT B
                WHERE A.MST_ID=B.MST_ID
                ) I
                WHERE R.INVOICE_NO = I.INVOICE_NO
                AND R.GIFT_ITEM_ID = I.GIFT_ITEM_ID
                ";

            var parameters = new List<string> { reportParameters.MST_ID };

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }
        public DataTable ReturnGiftBonus(ReportParams reportParameters)
        {
            var query = @"SELECT R.*, I.GIFT_QTY FROM
                (
                SELECT
                B.GIFT_ITEM_ID,
                FN_GIFT_NAME(A.COMPANY_ID, B.GIFT_ITEM_ID) GIFT_NAME,
                B.UNIT_TP,
                A.INVOICE_NO,
                SUM(B.GIFT_RETURN_QTY) GIFT_RETURN_QTY
                FROM RETURN_DTL A, RETURN_GIFT B
                WHERE A.MST_ID = :param1
                AND A.DTL_ID = B.DTL_ID
                GROUP BY B.UNIT_TP, A.COMPANY_ID, GIFT_ITEM_ID, A.INVOICE_NO
                ) R, (
                SELECT A.INVOICE_NO, BG.GIFT_ITEM_ID, BG.GIFT_QTY
                FROM INVOICE_MST A, INVOICE_GIFT BG
                WHERE A.MST_ID=BG.DTL_ID
                ) I
                WHERE R.INVOICE_NO = I.INVOICE_NO
                AND R.GIFT_ITEM_ID = I.GIFT_ITEM_ID";

            var parameters = new List<string> { reportParameters.MST_ID };

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }
        #endregion

        #region Invoice wise return
        public DataTable DateWiseReturn(ReportParams reportParameters)
        {
            var query = @"SELECT 
                   FN_SKU_NAME(A.COMPANY_ID, SKU_ID) SKU_NAME,
                   SKU_CODE,
                   UNIT_TP,
                   SUM(A.RETURN_QTY) RETURN_QTY
            FROM RETURN_DTL A, RETURN_MST B
            WHERE A.MST_ID = B.MST_ID
            AND A.COMPANY_ID = :param1";

            if (reportParameters.UNIT_ID != "ALL")
            {
                query += " AND B.RETURN_UNIT_ID = " + reportParameters.UNIT_ID;
            }

            query += @" AND TRUNC(B.RETURN_DATE) BETWEEN TO_DATE(:param2, 'DD/MM/YYYY') AND TO_DATE(:param3, 'DD/MM/YYYY')
            GROUP BY A.COMPANY_ID, SKU_ID, SKU_CODE, UNIT_TP";

            var parameters = new List<string> { reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO };

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }

        public DataTable DateWiseReturnProductBonus(ReportParams reportParameters)
        {
            var query = @"SELECT DISTINCT
                   C.SKU_ID,
                   C.SKU_CODE,
                   FN_SKU_NAME(C.COMPANY_ID, C.SKU_ID) SKU_NAME,
                   C.UNIT_TP,
                   SUM(NVL(C.RETURN_BONUS_QTY,0))RETURN_BONUS_QTY
            FROM  RETURN_MST A, RETURN_DTL B, RETURN_BONUS C
            WHERE A.MST_ID = B.MST_ID
            AND   B.DTL_ID=C.DTL_ID
            AND   B.COMPANY_ID = :param1
            AND TRUNC(A.RETURN_DATE) BETWEEN TO_DATE(:param2, 'DD/MM/YYYY') AND TO_DATE(:param3, 'DD/MM/YYYY')";

            if (reportParameters.UNIT_ID != "ALL")
            {
                query += " AND A.RETURN_UNIT_ID = " + reportParameters.UNIT_ID;
            }

            query += @" GROUP BY B.MST_ID, C.SKU_ID, C.SKU_CODE, C.UNIT_TP, C.COMPANY_ID";

            var parameters = new List<string> { reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO };

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }

        public DataTable DateWiseReturnComboBonus(ReportParams reportParameters)
        {
            var query = @"SELECT SKU_ID,
                SKU_CODE,
                FN_SKU_NAME(A.COMPANY_ID, SKU_ID) SKU_NAME,
                UNIT_TP,
                SUM(RETURN_BONUS_QTY) RETURN_BONUS_QTY
                FROM RETURN_COMBO_BONUS A, RETURN_MST B
                WHERE A.MST_ID = B.MST_ID
                AND   B.COMPANY_ID = :param1
                AND TRUNC(B.RETURN_DATE) BETWEEN TO_DATE(:param2, 'DD/MM/YYYY') AND TO_DATE(:param3, 'DD/MM/YYYY')";

            if (reportParameters.UNIT_ID != "ALL")
            {
                query += " AND B.RETURN_UNIT_ID = " + reportParameters.UNIT_ID;
            }

            query += @" GROUP BY SKU_CODE, UNIT_TP, A.COMPANY_ID, SKU_ID";

            var parameters = new List<string> { reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO };

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }

        public DataTable DateWiseReturnComboGiftBonus(ReportParams reportParameters)
        {
            var query = @"SELECT
                GIFT_ITEM_ID,
                FN_GIFT_NAME(A.COMPANY_ID, GIFT_ITEM_ID) GIFT_NAME,
                UNIT_TP,
                SUM(GIFT_RETURN_QTY) GIFT_RETURN_QTY
                FROM RETURN_COMBO_GIFT A, RETURN_MST B
                WHERE A.COMPANY_ID = :param1
                AND A.MST_ID = B.MST_ID
                AND TRUNC(B.RETURN_DATE) BETWEEN TO_DATE(:param2, 'DD/MM/YYYY') AND TO_DATE(:param3, 'DD/MM/YYYY')";

            if (reportParameters.UNIT_ID != "ALL")
            {
                query += " AND B.RETURN_UNIT_ID = " + reportParameters.UNIT_ID;
            }

            query += @" GROUP BY UNIT_TP, A.COMPANY_ID, GIFT_ITEM_ID";

            var parameters = new List<string> { reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO };

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }
        public DataTable DateWiseReturnGiftBonus(ReportParams reportParameters)
        {
            var query = @"SELECT
                B.GIFT_ITEM_ID,
                FN_GIFT_NAME(A.COMPANY_ID, B.GIFT_ITEM_ID) GIFT_NAME,
                B.UNIT_TP,
                SUM(B.GIFT_RETURN_QTY) GIFT_RETURN_QTY
                FROM RETURN_DTL A, RETURN_GIFT B, RETURN_MST C
                WHERE A.DTL_ID = B.DTL_ID
                AND A.MST_ID = C.MST_ID
                AND C.COMPANY_ID = :param1
                AND TRUNC(C.RETURN_DATE) BETWEEN TO_DATE(:param2, 'DD/MM/YYYY') AND TO_DATE(:param3, 'DD/MM/YYYY')";

            if (reportParameters.UNIT_ID != "ALL")
            {
                query += " AND C.RETURN_UNIT_ID = " + reportParameters.UNIT_ID;
            }

            query += @" GROUP BY B.UNIT_TP, A.COMPANY_ID, GIFT_ITEM_ID";

            var parameters = new List<string> { reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO };

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }
        #endregion

        public DataTable GetCustWiseSalesCollRetStmtRpt(ReportParams reportParameters)
        {
            string Query = @"begin  :param1 := FN_CUST_WISE_SALES_STMT_RPT(TO_DATE(:param2, 'DD/MM/YYYY'), TO_DATE(:param3, 'DD/MM/YYYY'),:param4, :param5, :param6); end;";

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.CUSTOMER_ID == "0" ? "" : reportParameters.CUSTOMER_ID, reportParameters.UNIT_ID == "ALL" ? "" : reportParameters.UNIT_ID, reportParameters.COMPANY_ID.ToString() }));


        }  
        public DataTable GetMonthWiseSalesCollRtnSmry(ReportParams reportParameters)
        {
            string Query = @"begin  :param1 := FN_MONTH_WISE_SALES_COLL_RET(TO_DATE(:param2, 'DD/MM/YYYY'), TO_DATE(:param3, 'DD/MM/YYYY'),:param4, :param5, :param6); end;";

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.CUSTOMER_ID == "0" ? "" : reportParameters.CUSTOMER_ID, reportParameters.UNIT_ID == "ALL" ? "" : reportParameters.UNIT_ID, reportParameters.COMPANY_ID.ToString() }));


        }
        public DataTable GetSkuWiseSalesReturnRpt(ReportParams reportParameters)
        {
            string Query = @"begin  :param1 := FN_SKU_WISE_SALES_RETURN_RPT(TO_DATE(:param2, 'DD/MM/YYYY'), TO_DATE(:param3, 'DD/MM/YYYY'),:param4, :param5, :param6); end;";

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.CUSTOMER_ID=="0"?"": reportParameters.CUSTOMER_ID, reportParameters.UNIT_ID == "ALL" ? "" : reportParameters.UNIT_ID, reportParameters.COMPANY_ID.ToString() }));


        }
        public DataTable GetSalesReturnReport(ReportParams reportParameters)
        {
            string Query = @"begin  :param1 := FN_SALES_RETURN_REPORT(TO_DATE(:param2, 'DD/MM/YYYY'), TO_DATE(:param3, 'DD/MM/YYYY'),:param4, :param5, :param6); end;";

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.CUSTOMER_ID=="0"?"": reportParameters.CUSTOMER_ID, reportParameters.UNIT_ID == "ALL" ? "" : reportParameters.UNIT_ID, reportParameters.COMPANY_ID.ToString() }));


        }
        public DataTable GetCustomerCollectionReport(ReportParams reportParameters)
        {
            string Query = @"begin  :param1 := FN_DATE_WISE_COLLECTION(TO_DATE(:param2, 'DD/MM/YYYY'), TO_DATE(:param3, 'DD/MM/YYYY'),:param4, :param5, :param6); end;";

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.CUSTOMER_ID=="0"?"": reportParameters.CUSTOMER_ID, reportParameters.UNIT_ID== "ALL"?"": reportParameters.UNIT_ID, reportParameters.COMPANY_ID.ToString() }));


        }
        public DataTable GetAccountsSalesReportDatewise(ReportParams reportParameters)
        {
            string Query = @"begin  :param1 := FN_DATE_WISE_SALES_REPORT_ACC(TO_DATE(:param2, 'DD/MM/YYYY'), TO_DATE(:param3, 'DD/MM/YYYY'),:param4, :param5); end;";

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO,reportParameters.UNIT_ID == "ALL" ? "" : reportParameters.UNIT_ID, reportParameters.COMPANY_ID.ToString() }));
        }
        public DataTable GetSalesTrendAnalysisData(ReportParams reportParameters)
        {
            string Query = @"begin  :param1 := FN_SALES_TREND_ANALYSIS_REP(TO_DATE(:param2, 'DD/MM/YYYY'), TO_DATE(:param3, 'DD/MM/YYYY'),:param4); end;";

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.UNIT_ID }));
        }

        public DataTable GetCustWiseRemainingBnsRpt(ReportParams reportParameters)
        {
            if (reportParameters.CUSTOMER_CODE == null || reportParameters.CUSTOMER_CODE == "null" || reportParameters.CUSTOMER_CODE == "undefined" || reportParameters.CUSTOMER_CODE == "" || reportParameters.CUSTOMER_CODE == "''" || reportParameters.CUSTOMER_CODE == " ")
            {
                reportParameters.CUSTOMER_CODE = "ALL";
            }
            string Query = @"begin  :param1 := FN_CUST_WISE_REMAINING_BNS_RPT(TO_DATE(:param2, 'DD/MM/YYYY'), TO_DATE(:param3, 'DD/MM/YYYY'), :param4, :param5,:param6); end;";
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO,reportParameters.CUSTOMER_CODE, reportParameters.UNIT_ID, reportParameters.COMPANY_ID.ToString() }));
        }
        public DataTable GetCustWiseRemainingBnsSummaryRpt(ReportParams reportParameters)
        {
            if (reportParameters.CUSTOMER_CODE == null || reportParameters.CUSTOMER_CODE == "null" || reportParameters.CUSTOMER_CODE == "undefined" || reportParameters.CUSTOMER_CODE == "" || reportParameters.CUSTOMER_CODE == "''" || reportParameters.CUSTOMER_CODE == " ")
            {
                reportParameters.CUSTOMER_CODE = "ALL";
            }
            string Query = @"begin  :param1 := FN_CUST_REM_BNS_SUMMARY_RPT(TO_DATE(:param2, 'DD/MM/YYYY'), TO_DATE(:param3, 'DD/MM/YYYY'), :param4, :param5,:param6); end;";
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.CUSTOMER_CODE, reportParameters.UNIT_ID, reportParameters.COMPANY_ID.ToString() }));
        }
    }
}