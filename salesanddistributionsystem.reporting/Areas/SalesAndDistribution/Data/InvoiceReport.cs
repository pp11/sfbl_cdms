using Oracle.ManagedDataAccess.Client;
using SalesAndDistributionSystem.Domain.Models.ReportModels.Common.ProductReports;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SalesAndDistributionSystem.Reporting.Areas.SalesAndDistribution.Data
{
    public class InvoiceReport
    {
        private readonly CommonServices commonServices = new CommonServices();

        //-----------------------Query Part--------------------------------
        public string GetPageHeaderReport_Query() => @"Select COMPANY_NAME,COMPANY_ADDRESS, :param1 as PREVIEW from VW_REPORT_HEADER_DATA where COMPANY_ID = :param2 and ROWNUM =1";
        private string GetInvoiceReport_Query() => @"begin  :param1 := FN_INVOICE_REPORT(TO_DATE(:param2, 'DD/MM/YYYY'), :param3, :param4, :param5 , :param6,  :param7, :param8, :param9, :param10 , :param11, :param12, :param13); end;";
        private string GetOrderReport_Query() => @"begin  :param1 := FN_ORDER_REPORT(TO_DATE(:param2, 'DD/MM/YYYY'), :param3, :param4, :param5 , :param6,  :param7, :param8, :param9, :param10 , :param11, :param12, :param13); end;";
        private string GetDeliverySlipReport_Query() => @" begin  :param1 := FN_delivery_slip_REPORT( TO_DATE(:param2, 'DD/MM/YYYY'), :param3, :param4, :param5 , :param6,  :param7, :param8, :param9, :param10 , :param11, :param12); end;";
        private string GeInvoiceWiseDeliverySlip_Query() => @" begin  :param1 := FN_INV_WISE_DELIVERY_SLIP_RPT( TO_DATE(:param2, 'DD/MM/YYYY'), :param3, :param4, :param5 , :param6,  :param7, :param8, :param9, :param10 , :param11, :param12); end;";
        private string GetCustomerOutstandingReport_Query() => @" begin  :param1 := FN_CUSTOMER_OUTSTANDING_REPORT( TO_DATE(:param2, 'DD/MM/YYYY'), :param3, :param4, :param5 , :param6,  :param7, :param8, :param9); end;";
        private string GetCustomerOutstandingReportV2_Query() => @" begin  :param1 := FN_CUSTOMER_OUTSTANDING_RPT_V2( TO_DATE(:param2, 'DD/MM/YYYY'), :param3, :param4, :param5 , :param6,  :param7, :param8); end;";

        private string GetCustOutstandingDtlReport_Query() => @" begin  :param1 := FN_CUST_OUTSTANDING_RPT_DTL( TO_DATE(:param2, 'DD/MM/YYYY'), :param3, :param4, :param5 , :param6,  :param7, :param8, :param9); end;";
        private string GetCustOutstandingDtlReportV2_Query() => @" begin  :param1 := FN_CUST_OUTSTANDING_RPT_DTL_V2( TO_DATE(:param2, 'DD/MM/YYYY'), :param3, :param4, :param5 , :param6,  :param7, :param8, :param9); end;";

        private string GetCustomerOutstandingWithInvoiceReport_Query() => @" begin  :param1 := FN_CUST_INV_OUTSTANDING_REPORT( TO_DATE(:param2, 'DD/MM/YYYY'), :param3, :param4, :param5 , :param6,  :param7, :param8, :param9); end;";
        private string GetCustomerOutstandingWithInvoiceReportV2_Query() => @" begin  :param1 := FN_CUST_INV_OUTSTANDING_RPT_V2( TO_DATE(:param2, 'DD/MM/YYYY'), :param3, :param4, :param5 , :param6,  :param7, :param8); end;";
        private string GetSalesRegisterReport_Query() => @"begin  :param1 := FN_SALES_REGISTER_REPORT(TO_DATE(:param2, 'DD/MM/YYYY'), TO_DATE(:param3, 'DD/MM/YYYY'), :param4, :param5); end;";
        private string GetDistributorLadgerReport_Query() => @"Select SLNO, INVOICE_DATE, INVOICE_NO, INVOICE_UNIT_ID, UNIT_NAME, CUSTOMER_CODE, CUSTOMER_ID, CUSTOMER_NAME, CUSTOMER_ADDRESS, VOUCHER_DATE, VOUCHER_NO, BANK_ID, BRANCH_ID, BANK_NAME, BRANCH_NAME, INVOICE_AMT, TDS_AMT, COLLECTION_INV_AMT, COLLECTION_TDS_AMT, RETURN_INV_AMT, RETURN_TDS_AMT, OPENING_INV_AMT, OPENING_TDS_AMT, CLOSING_INV_AMT, CLOSING_TDS_AMT from table(FN_CUSTOMER_LEDGER_REPORT( TO_DATE(:param1, 'DD/MM/YYYY'), TO_DATE(:param2, 'DD/MM/YYYY'), :param3, :param4, :param5))";
        private string GetDistributorLadgerReportV2_Query() => @"Select SLNO, INVOICE_DATE, INVOICE_NO, INVOICE_UNIT_ID, UNIT_NAME, CUSTOMER_CODE, CUSTOMER_ID, CUSTOMER_NAME, CUSTOMER_ADDRESS, VOUCHER_DATE, VOUCHER_NO, BANK_ID, BRANCH_ID, BANK_NAME, BRANCH_NAME, INVOICE_AMT, TDS_AMT, COLLECTION_INV_AMT, COLLECTION_TDS_AMT, RETURN_INV_AMT, RETURN_TDS_AMT, OPENING_INV_AMT, OPENING_TDS_AMT, CLOSING_INV_AMT, CLOSING_TDS_AMT from table(FN_CUSTOMER_LEDGER_REPORT_V2( TO_DATE(:param1, 'DD/MM/YYYY'), TO_DATE(:param2, 'DD/MM/YYYY'), :param3, :param4))";

        private string GetDistributorLadgerWLocationReport_Query() => @"SELECT A.SLNO, A.INVOICE_DATE, A.INVOICE_NO, A.INVOICE_UNIT_ID, A.UNIT_NAME, A.CUSTOMER_CODE, A.CUSTOMER_ID, A.CUSTOMER_NAME, A.CUSTOMER_ADDRESS, A.VOUCHER_DATE, A.VOUCHER_NO, A.BANK_ID, A.BRANCH_ID, A.BANK_NAME, A.BRANCH_NAME, A.INVOICE_AMT, A.TDS_AMT, A.COLLECTION_INV_AMT, A.COLLECTION_TDS_AMT, A.RETURN_INV_AMT, A.RETURN_TDS_AMT, A.OPENING_INV_AMT, A.OPENING_TDS_AMT, A.CLOSING_INV_AMT, A.CLOSING_TDS_AMT FROM TABLE (FN_CUSTOMER_LEDGER_REPORT (TO_DATE ( :param1, 'DD/MM/YYYY'), TO_DATE ( :param2, 'DD/MM/YYYY'), :param3, :param4, :param5)) A, VW_LOCATION_CUSTOMER_RELATION R WHERE A.CUSTOMER_ID= R.CUSTOMER_ID AND A.CUSTOMER_CODE= R.CUSTOMER_CODE AND R.DIVISION_ID = NVL(:param6,R.DIVISION_ID) AND R.REGION_ID= NVL(:param7,R.REGION_ID) AND R.AREA_ID= NVL(:param8,R.AREA_ID) AND R.TERRITORY_ID= NVL(:param9,R.TERRITORY_ID) AND R.MARKET_ID= NVL(:param10,R.MARKET_ID)";
          private string GetDistributorLadgerWLocationReportV2_Query() => @"SELECT A.SLNO, A.INVOICE_DATE, A.INVOICE_NO, A.INVOICE_UNIT_ID, A.UNIT_NAME, A.CUSTOMER_CODE, A.CUSTOMER_ID, A.CUSTOMER_NAME, A.CUSTOMER_ADDRESS, A.VOUCHER_DATE, A.VOUCHER_NO, A.BANK_ID, A.BRANCH_ID, A.BANK_NAME, A.BRANCH_NAME, A.INVOICE_AMT, A.TDS_AMT, A.COLLECTION_INV_AMT, A.COLLECTION_TDS_AMT, A.RETURN_INV_AMT, A.RETURN_TDS_AMT, A.OPENING_INV_AMT, A.OPENING_TDS_AMT, A.CLOSING_INV_AMT, A.CLOSING_TDS_AMT FROM TABLE (FN_CUSTOMER_LEDGER_REPORT_V2 (TO_DATE ( :param1, 'DD/MM/YYYY'), TO_DATE ( :param2, 'DD/MM/YYYY'), :param3, :param4)) A, VW_LOCATION_CUSTOMER_RELATION R WHERE A.CUSTOMER_ID= R.CUSTOMER_ID AND A.CUSTOMER_CODE= R.CUSTOMER_CODE AND R.DIVISION_ID = NVL(:param5,R.DIVISION_ID) AND R.REGION_ID= NVL(:param6,R.REGION_ID) AND R.AREA_ID= NVL(:param7,R.AREA_ID) AND R.TERRITORY_ID= NVL(:param8,R.TERRITORY_ID) AND R.MARKET_ID= NVL(:param9,R.MARKET_ID)";

        private string GetUnitWiseDistributorLadgerReport_Query() => @"Select SLNO, INVOICE_DATE, INVOICE_NO, INVOICE_UNIT_ID, UNIT_NAME, CUSTOMER_CODE, CUSTOMER_ID, CUSTOMER_NAME, CUSTOMER_ADDRESS, VOUCHER_DATE, VOUCHER_NO, BANK_ID, BRANCH_ID, BANK_NAME, BRANCH_NAME, INVOICE_AMT, TDS_AMT, COLLECTION_INV_AMT, COLLECTION_TDS_AMT, RETURN_INV_AMT, RETURN_TDS_AMT, OPENING_INV_AMT, OPENING_TDS_AMT, CLOSING_INV_AMT, CLOSING_TDS_AMT from table(FN_UNIT_WISE_CUST_LEDGER_RPT(TO_DATE(:param1, 'DD/MM/YYYY'), TO_DATE(:param2, 'DD/MM/YYYY'), :param3, :param4, :param5))";
        private string GetUnitWiseDistributorLadgerWLocationReport_Query() => @"SELECT A.SLNO, A.INVOICE_DATE, A.INVOICE_NO, A.INVOICE_UNIT_ID, A.UNIT_NAME, A.CUSTOMER_CODE, A.CUSTOMER_ID, A.CUSTOMER_NAME, A.CUSTOMER_ADDRESS, A.VOUCHER_DATE, A.VOUCHER_NO, A.BANK_ID, A.BRANCH_ID, A.BANK_NAME, A.BRANCH_NAME, A.INVOICE_AMT, A.TDS_AMT, A.COLLECTION_INV_AMT, A.COLLECTION_TDS_AMT, A.RETURN_INV_AMT, A.RETURN_TDS_AMT, A.OPENING_INV_AMT, A.OPENING_TDS_AMT, A.CLOSING_INV_AMT, A.CLOSING_TDS_AMT FROM TABLE (FN_UNIT_WISE_CUST_LEDGER_RPT (TO_DATE ( :param1, 'DD/MM/YYYY'), TO_DATE ( :param2, 'DD/MM/YYYY'), :param3, :param4, :param5)) A, VW_LOCATION_CUSTOMER_RELATION R WHERE A.CUSTOMER_ID= R.CUSTOMER_ID AND A.CUSTOMER_CODE= R.CUSTOMER_CODE AND R.DIVISION_ID = NVL(:param6,R.DIVISION_ID) AND R.REGION_ID= NVL(:param7,R.REGION_ID) AND R.AREA_ID= NVL(:param8,R.AREA_ID) AND R.TERRITORY_ID= NVL(:param9,R.TERRITORY_ID) AND R.MARKET_ID= NVL(:param10,R.MARKET_ID)";

        private string GetUnitName_Query() => @"SELECT C.COMPANY_NAME,C.UNIT_NAME FROM COMPANY_INFO C WHERE C.COMPANY_ID=:param1 AND C.UNIT_ID=:param2";

        //-------------------Execution Part------------------------
        public DataTable GetPageHeaderReport(ReportParams reportParameters)
        {
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, GetPageHeaderReport_Query(), commonServices.AddParameter(new string[] { reportParameters.PREVIEW, reportParameters.COMPANY_ID.ToString() }));

        }
        
        public DataTable GetOrderReport(ReportParams reportParameters)
        {
            string Query = GetOrderReport_Query();
            //if (reportParameters.MST_ID != "undefined" && !String.IsNullOrWhiteSpace(reportParameters.MST_ID))
            //{
            //    Query = Query + " and M.MST_ID IN(" + reportParameters.MST_ID + ")";
            //}
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.INVOICE_DATE, reportParameters.CUSTOMER_ID, reportParameters.DIVISION_ID, reportParameters.REGION_ID, reportParameters.AREA_ID, reportParameters.TERRITORY_ID, reportParameters.MARKET_ID, reportParameters.UNIT_ID, reportParameters.COMPANY_ID.ToString(), reportParameters.INVOICE_NO_FROM, reportParameters.INVOICE_NO_TO, reportParameters.PREVIEW == "NO" ? "PRINT" : "PREVIEW" }));

        }
        
        public DataTable GetInvoiceReport(ReportParams reportParameters)
        {
            string Query = GetInvoiceReport_Query();
            //if (reportParameters.MST_ID != "undefined" && !String.IsNullOrWhiteSpace(reportParameters.MST_ID))
            //{
            //    Query = Query + " and M.MST_ID IN(" + reportParameters.MST_ID + ")";
            //}

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.INVOICE_DATE, reportParameters.CUSTOMER_ID, reportParameters.DIVISION_ID, reportParameters.REGION_ID, reportParameters.AREA_ID, reportParameters.TERRITORY_ID, reportParameters.MARKET_ID, reportParameters.UNIT_ID, reportParameters.COMPANY_ID.ToString(), reportParameters.INVOICE_NO_FROM, reportParameters.INVOICE_NO_TO, reportParameters.PREVIEW == "NO" ? "PRINT" : "PREVIEW" }));
        }
        
        public DataTable GetTpWiseInvoiceReport(ReportParams reportParameters)
        {

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, @"begin  :param1 := FN_TP_WISE_INVOICE_REPORT(TO_DATE(:param2, 'DD/MM/YYYY'), :param3, :param4, :param5 , :param6,  :param7, :param8, :param9, :param10 , :param11, :param12, :param13); end;", commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.INVOICE_DATE, reportParameters.CUSTOMER_ID, reportParameters.DIVISION_ID, reportParameters.REGION_ID, reportParameters.AREA_ID, reportParameters.TERRITORY_ID, reportParameters.MARKET_ID, reportParameters.UNIT_ID, reportParameters.COMPANY_ID.ToString(), reportParameters.INVOICE_NO_FROM, reportParameters.INVOICE_NO_TO, reportParameters.PREVIEW == "NO" ? "PRINT" : "PREVIEW" }));

        }
        
        public DataTable GetTpWiseInvoiceSubReport(ReportParams reportParameters)
        {
            try
            {
                return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, @"SELECT INVOICE_NO,SKU_ID,UNIT_TP,INVOICE_QTY,BONUS_STATUS
FROM   VW_INVOICE_TP_DETAILS
WHERE  INVOICE_NO = :param1
       AND SKU_ID = :param2
       AND BONUS_STATUS = :param3", commonServices.AddParameter(new string[] { reportParameters.INVOICE_NO, reportParameters.SKU_ID, reportParameters.BONUS_STATUS }));
            }
            catch (Exception)
            {

                throw;
            }


        }
        
        public DataTable GetDeliverySlipReport(ReportParams reportParameters)
        {
            string Query = GetDeliverySlipReport_Query();
            if (reportParameters.MST_ID != "undefined" && !String.IsNullOrWhiteSpace(reportParameters.MST_ID))
            {
                Query = Query + " and M.MST_ID IN(" + reportParameters.MST_ID + ")";
            }

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.INVOICE_DATE, reportParameters.CUSTOMER_ID, reportParameters.DIVISION_ID, reportParameters.REGION_ID, reportParameters.AREA_ID, reportParameters.TERRITORY_ID, reportParameters.MARKET_ID, reportParameters.UNIT_ID, reportParameters.COMPANY_ID.ToString(), reportParameters.INVOICE_NO_FROM, reportParameters.INVOICE_NO_TO }));
        }
        
        public DataTable InvoiceWiseDeliverySlip(ReportParams reportParameters)
        {
            string Query = GeInvoiceWiseDeliverySlip_Query();
            if (reportParameters.MST_ID != "undefined" && !String.IsNullOrWhiteSpace(reportParameters.MST_ID))
            {
                Query = Query + " and M.MST_ID IN(" + reportParameters.MST_ID + ")";
            }

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.INVOICE_DATE, reportParameters.CUSTOMER_ID, reportParameters.DIVISION_ID, reportParameters.REGION_ID, reportParameters.AREA_ID, reportParameters.TERRITORY_ID, reportParameters.MARKET_ID, reportParameters.UNIT_ID, reportParameters.COMPANY_ID.ToString(), reportParameters.INVOICE_NO_FROM, reportParameters.INVOICE_NO_TO }));
        }
        
        public DataTable GetDistributorLadgerReport(ReportParams reportParameters)
        {
           
            if (string.Concat(reportParameters.DIVISION_ID, reportParameters.REGION_ID, reportParameters.AREA_ID, reportParameters.TERRITORY_ID, reportParameters.MARKET_ID).Length == 0)
            {
                string Query = GetDistributorLadgerReport_Query();
                return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.CUSTOMER_CODE, reportParameters.UNIT_ID, reportParameters.COMPANY_ID.ToString() }));
            }
            else {
                string Query = GetDistributorLadgerWLocationReport_Query();
                return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.CUSTOMER_CODE, reportParameters.UNIT_ID, reportParameters.COMPANY_ID.ToString(), reportParameters.DIVISION_ID, reportParameters.REGION_ID, reportParameters.AREA_ID, reportParameters.TERRITORY_ID, reportParameters.MARKET_ID }));
            }

        }
        public DataTable GetDistributorLadgerReportV2(ReportParams reportParameters)
        {

            if (string.Concat(reportParameters.DIVISION_ID, reportParameters.REGION_ID, reportParameters.AREA_ID, reportParameters.TERRITORY_ID, reportParameters.MARKET_ID).Length == 0)
            {
                string Query = GetDistributorLadgerReportV2_Query();
                return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.CUSTOMER_CODE, reportParameters.COMPANY_ID.ToString() }));
            }
            else
            {
                string Query = GetDistributorLadgerWLocationReportV2_Query();
                return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.CUSTOMER_CODE, reportParameters.COMPANY_ID.ToString(), reportParameters.DIVISION_ID, reportParameters.REGION_ID, reportParameters.AREA_ID, reportParameters.TERRITORY_ID, reportParameters.MARKET_ID }));
            }

        }
        
        public DataTable GetSalesRegisterReport(ReportParams reportParameters)
        {
            string Query = GetSalesRegisterReport_Query();
            if (reportParameters.MST_ID != "undefined" && !String.IsNullOrWhiteSpace(reportParameters.MST_ID))
            {
                Query = Query + " and M.MST_ID IN(" + reportParameters.MST_ID + ")";
            }

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.COMPANY_ID.ToString(), reportParameters.UNIT_ID }));
        }
        
        public DataTable GetUnitWiseDistributorLadgerReport(ReportParams reportParameters)
        {
            if (string.Concat(reportParameters.DIVISION_ID, reportParameters.REGION_ID, reportParameters.AREA_ID, reportParameters.TERRITORY_ID, reportParameters.MARKET_ID).Length == 0)
            {
                string Query = GetUnitWiseDistributorLadgerReport_Query();
                return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.CUSTOMER_CODE, reportParameters.UNIT_ID, reportParameters.COMPANY_ID.ToString() }));

            }
            else
            {
                string Query = GetUnitWiseDistributorLadgerWLocationReport_Query();
                return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.CUSTOMER_CODE, reportParameters.UNIT_ID, reportParameters.COMPANY_ID.ToString(), reportParameters.DIVISION_ID, reportParameters.REGION_ID, reportParameters.AREA_ID, reportParameters.TERRITORY_ID, reportParameters.MARKET_ID }));

            }

        }
        
        public DataTable GetCustomerOutstandingReport(ReportParams reportParameters)
        {
            string Query = GetCustomerOutstandingReport_Query();
            if (reportParameters.MST_ID != "undefined" && !String.IsNullOrWhiteSpace(reportParameters.MST_ID))
            {
                Query = Query + " and M.MST_ID IN(" + reportParameters.MST_ID + ")";
            }

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.INVOICE_DATE, reportParameters.CUSTOMER_ID, reportParameters.DIVISION_ID, reportParameters.REGION_ID, reportParameters.AREA_ID, reportParameters.TERRITORY_ID, reportParameters.UNIT_ID, reportParameters.COMPANY_ID.ToString() }));
        }
        public DataTable GetCustomerOutstandingReportV2(ReportParams reportParameters)
        {
            string Query = GetCustomerOutstandingReportV2_Query();
            if (reportParameters.MST_ID != "undefined" && !String.IsNullOrWhiteSpace(reportParameters.MST_ID))
            {
                Query = Query + " and M.MST_ID IN(" + reportParameters.MST_ID + ")";
            }

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.INVOICE_DATE, reportParameters.CUSTOMER_ID, reportParameters.DIVISION_ID, reportParameters.REGION_ID, reportParameters.AREA_ID, reportParameters.TERRITORY_ID, reportParameters.COMPANY_ID.ToString() }));
        }

        public DataTable GetCustOutstandingDtlReport(ReportParams reportParameters)
        {
            string Query = GetCustOutstandingDtlReport_Query();
            if (reportParameters.MST_ID != "undefined" && !String.IsNullOrWhiteSpace(reportParameters.MST_ID))
            {
                Query = Query + " and M.MST_ID IN(" + reportParameters.MST_ID + ")";
            }

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.INVOICE_DATE, reportParameters.CUSTOMER_ID, reportParameters.DIVISION_ID, reportParameters.REGION_ID, reportParameters.AREA_ID, reportParameters.TERRITORY_ID, reportParameters.UNIT_ID, reportParameters.COMPANY_ID.ToString() }));
        }
        public DataTable GetCustOutstandingDtlReportV2(ReportParams reportParameters)
        {
            string Query = GetCustOutstandingDtlReportV2_Query();
            if (reportParameters.MST_ID != "undefined" && !String.IsNullOrWhiteSpace(reportParameters.MST_ID))
            {
                Query = Query + " and M.MST_ID IN(" + reportParameters.MST_ID + ")";
            }

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.INVOICE_DATE, reportParameters.CUSTOMER_ID, reportParameters.DIVISION_ID, reportParameters.REGION_ID, reportParameters.AREA_ID, reportParameters.TERRITORY_ID, reportParameters.UNIT_ID, reportParameters.COMPANY_ID.ToString() }));
        }
        public DataTable GetCustOutstandingDtlReportV2CK(ReportParams reportParameters)
        {
            string Query = @" begin  :param1 := FN_CUST_OUTSTANDING_UPTO( TO_DATE(:param2, 'DD/MM/YYYY'),TO_DATE(:param3, 'DD/MM/YYYY'), :param4, :param5 , :param6,  :param7, :param8, :param9, :param10); end;";

            if (reportParameters.MST_ID != "undefined" && !String.IsNullOrWhiteSpace(reportParameters.MST_ID))
            {
                Query = Query + " and M.MST_ID IN(" + reportParameters.MST_ID + ")";
            }

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.CUSTOMER_ID, reportParameters.DIVISION_ID, reportParameters.REGION_ID, reportParameters.AREA_ID, reportParameters.TERRITORY_ID, reportParameters.UNIT_ID, reportParameters.COMPANY_ID.ToString() }));
        }
        public DataTable GetCustomerOutstandingWithInvoiceReport(ReportParams reportParameters)
        {
            string Query = GetCustomerOutstandingWithInvoiceReport_Query();
            if (reportParameters.MST_ID != "undefined" && !String.IsNullOrWhiteSpace(reportParameters.MST_ID))
            {
                Query = Query + " and M.MST_ID IN(" + reportParameters.MST_ID + ")";
            }

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.INVOICE_DATE, reportParameters.CUSTOMER_ID, reportParameters.DIVISION_ID, reportParameters.REGION_ID, reportParameters.AREA_ID, reportParameters.TERRITORY_ID, reportParameters.UNIT_ID, reportParameters.COMPANY_ID.ToString() }));
        }
        public DataTable GetCustomerOutstandingWithInvoiceReportV2(ReportParams reportParameters)
        {
            string Query = GetCustomerOutstandingWithInvoiceReportV2_Query();
            if (reportParameters.MST_ID != "undefined" && !String.IsNullOrWhiteSpace(reportParameters.MST_ID))
            {
                Query = Query + " and M.MST_ID IN(" + reportParameters.MST_ID + ")";
            }

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.INVOICE_DATE, reportParameters.CUSTOMER_ID, reportParameters.DIVISION_ID, reportParameters.REGION_ID, reportParameters.AREA_ID, reportParameters.TERRITORY_ID, reportParameters.COMPANY_ID.ToString() }));
        }
        public DataTable GetUnitName(ReportParams reportParameters)
        {
            string Query = GetUnitName_Query();
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { reportParameters.COMPANY_ID.ToString(), reportParameters.UNIT_ID }));
        }
        public DataTable OrderVsInvoice(ReportParams reportParameters)
        {
            string Query = @"begin  :param1 := FN_ORDER_VS_INVOICE(TO_DATE(:param2, 'DD/MM/YYYY'), TO_DATE(:param3, 'DD/MM/YYYY'), :param4, :param5,:param6,:param7,:param8,:param9,:param10,:param11); end;";
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.CUSTOMER_ID, reportParameters.DIVISION_ID,reportParameters.REGION_ID,reportParameters.AREA_ID, reportParameters.TERRITORY_ID,reportParameters.MARKET_ID ,reportParameters.UNIT_ID, reportParameters.COMPANY_ID.ToString() }));
        }

    }
}