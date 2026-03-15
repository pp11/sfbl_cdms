using Microsoft.Reporting.WebForms;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Oracle.ManagedDataAccess.Client;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.ReportModels.Common.ProductReports;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SalesAndDistributionSystem.Reporting.Areas.Target.Data
{
    public class TargetReport
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
        public DataTable TargetData(ReportParams reportParameters)
        {
            var query = @"SELECT A.MST_ID,
       A.CUSTOMER_CODE,
       C.CUSTOMER_NAME,
       A.MONTH_CODE,
       TO_CHAR (TO_DATE (A.MONTH_CODE, 'MM'), 'MONTH') AS MONTH_NAME,
       A.YEAR,
       B.SKU_CODE,
       B.SKU_ID,
       FN_SKU_NAME (A.COMPANY_ID, B.SKU_ID) SKU_NAME,
       FN_PACK_SIZE (A.COMPANY_ID, B.SKU_ID) PACK_SIZE,
       B.UNIT_TP,
       B.MRP,
       B.AVG_PER_DAY_TARGET_QTY,
       B.PREVIOUS_TARGET_QTY,
       B.TARGET_QTY,
       B.TARGET_VALUE,
       B.DISCOUNT_VALUE,
       B.NET_VALUE,
       C.TERRITORY_CODE,
       C.TERRITORY_ID,
       C.TERRITORY_NAME,
       C.AREA_CODE,
       C.AREA_ID,
       C.AREA_NAME,
       C.REGION_NAME,
       C.REGION_ID,
       C.REGION_CODE,
       C.DIVISION_CODE,
       C.DIVISION_ID,
       C.DIVISION_NAME,
       C.MARKET_CODE,
       C.MARKET_ID,
       C.MARKET_NAME
  FROM CUSTOMER_TARGET_MST A,
       CUSTOMER_TARGET_DTL B,
       VW_LOCATION_CUSTOMER_RELATION C
 WHERE     A.MST_ID = B.MST_ID
       AND C.CUSTOMER_CODE = A.CUSTOMER_CODE
       AND C.CUSTOMER_ID = A.CUSTOMER_ID
       AND A.COMPANY_ID = :param1
       AND A.YEAR = :param2
       AND A.MONTH_CODE = :param3
       AND C.CUSTOMER_CODE= NVL(:param4,C.CUSTOMER_CODE)";

            if (!string.IsNullOrEmpty(reportParameters.DIVISION_ID) && reportParameters.DIVISION_ID != "ALL" && reportParameters.DIVISION_ID != "undefined")
            {
                query += " AND C.DIVISION_ID = " + reportParameters.DIVISION_ID;
            }
            if (!string.IsNullOrEmpty(reportParameters.REGION_ID) && reportParameters.REGION_ID != "ALL" && reportParameters.REGION_ID != "undefined")
            {
                query += " AND C.REGION_ID = " + reportParameters.REGION_ID;
            }
            if (!string.IsNullOrEmpty(reportParameters.AREA_ID) && reportParameters.AREA_ID != "ALL" && reportParameters.AREA_ID != "undefined")
            {
                query += " AND C.AREA_ID = " + reportParameters.AREA_ID;
            }
            if (!string.IsNullOrEmpty(reportParameters.TERRITORY_ID) && reportParameters.TERRITORY_ID != "ALL" && reportParameters.TERRITORY_ID != "undefined")
            {
                query += " AND C.TERRITORY_ID = " + reportParameters.TERRITORY_ID;
            }
            if (!string.IsNullOrEmpty(reportParameters.MARKET_ID) && reportParameters.MARKET_ID != "ALL" && reportParameters.MARKET_ID != "undefined")
            {
                query += " AND C.MARKET_ID = " + reportParameters.MARKET_ID;
            }

            var parameters = new List<string> {
                reportParameters.COMPANY_ID.ToString(),
                reportParameters.YEAR,
                reportParameters.MONTH_CODE,
                reportParameters.CUSTOMER_CODE
                };

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }
        public DataTable GetInMarketSales(ReportParams reportParameters)
        {
            string Query = @"SELECT M.MST_ID,
       TO_CHAR(M.ENTRY_DATE,'DD/MM/YYYY') ENTRY_DATE,
       M.YEAR,
       M.MONTH_CODE,
       M.MARKET_ID,
       I.MARKET_NAME,
       M.MARKET_CODE,
       M.COMPANY_ID,
       M.UNIT_ID,
       M.ENTERED_BY,
       M.ENTERED_DATE,
       D.DTL_ID,
       D.SKU_ID,
       D.SKU_CODE,
       P.SKU_NAME,
       D.UNIT_TP,
       D.MRP,
       D.SALES_QTY,
       D.SALES_VALUE
    FROM IN_MARKET_SALES_MST M, IN_MARKET_SALES_DTL D, PRODUCT_INFO P, MARKET_INFO I 
    WHERE     D.MST_ID = M.MST_ID
    AND P.SKU_CODE = D.SKU_CODE
    AND M.MARKET_ID = I.MARKET_ID
    AND M.COMPANY_ID = :param1
    AND M.MARKET_ID = NVL ( :param2, M.MARKET_ID)
    AND M.MONTH_CODE = NVL ( :param3, M.MONTH_CODE)
    AND M.YEAR = NVL ( :param4, M.YEAR)";
            if (reportParameters.MST_ID != "undefined" && !String.IsNullOrWhiteSpace(reportParameters.MST_ID))
            {
                Query = Query + " and M.MST_ID IN(" + reportParameters.MST_ID + ")";
            }

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { reportParameters.COMPANY_ID.ToString(), reportParameters.MARKET_ID, reportParameters.MONTH_CODE, reportParameters.YEAR }));

        }

        
        public DataTable TargetDetail(ReportParams reportParameters)
        {
            //Update_Customer_Stock(reportParameters.DB); 

            //string Query = @" begin  :param1 := FN_TARGET_SUMMARY_VIEW_RPT( :param2, :param3, :param4); end;";


            string Query = @" begin  :param1 := FN_TARGET_DETAIL_VIEW_RPT( :param2, :param3, :param4,:param5, :param6, :param7,:param8, :param9); end;";

            reportParameters.DIVISION_ID = reportParameters.DIVISION_ID == "undefined" ? "0" : reportParameters.DIVISION_ID;
            reportParameters.REGION_ID = reportParameters.REGION_ID == "undefined" ? "0" : reportParameters.REGION_ID;
            reportParameters.AREA_ID = reportParameters.AREA_ID == "undefined" ? "0" : reportParameters.AREA_ID;
            reportParameters.MARKET_ID = reportParameters.MARKET_ID == "undefined" ? "0" : reportParameters.MARKET_ID;
            reportParameters.CUSTOMER_ID = reportParameters.CUSTOMER_ID == "undefined" ? "0" : reportParameters.CUSTOMER_ID;


            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.COMPANY_ID.ToString(), reportParameters.YEAR, reportParameters.MONTH_CODE, reportParameters.DIVISION_ID, reportParameters.REGION_ID, reportParameters.AREA_ID, reportParameters.MARKET_ID, reportParameters.CUSTOMER_ID }));

        }
        public DataTable TargetSummary(ReportParams reportParameters)
        {
            //Update_Customer_Stock(reportParameters.DB); 

            //string Query = @" begin  :param1 := FN_TARGET_SUMMARY_VIEW_RPT( :param2, :param3, :param4); end;";


             string Query = @" begin  :param1 := FN_TARGET_SUMMARY_VIEW_RPT( :param2, :param3, :param4,:param5, :param6, :param7,:param8, :param9); end;";

            reportParameters.DIVISION_ID = reportParameters.DIVISION_ID == "undefined" ? "0" : reportParameters.DIVISION_ID;
            reportParameters.REGION_ID = reportParameters.REGION_ID == "undefined" ? "0" : reportParameters.REGION_ID;
            reportParameters.AREA_ID = reportParameters.AREA_ID == "undefined" ? "0" : reportParameters.AREA_ID;
            reportParameters.MARKET_ID = reportParameters.MARKET_ID == "undefined" ? "0" : reportParameters.MARKET_ID;
            reportParameters.CUSTOMER_ID = reportParameters.CUSTOMER_ID == "undefined" ? "0" : reportParameters.CUSTOMER_ID;


            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.COMPANY_ID.ToString(), reportParameters.YEAR, reportParameters.MONTH_CODE,reportParameters.DIVISION_ID,reportParameters.REGION_ID,reportParameters.AREA_ID,reportParameters.MARKET_ID,reportParameters.CUSTOMER_ID }));

        }
        public  string Update_Customer_Stock(string db)
        {
      
             List<QueryPattern> listOfQuery = new List<QueryPattern>();
             try
             {
                string truncate = @"TRUNCATE TABLE TEMP_CUSTOMER_STOCK";

                string Update_query = @"INSERT INTO TEMP_CUSTOMER_STOCK( CUSTOMER_CODE,PRODUCT_CODE,STOCK) 
                select customer_code,PRODUCT_CODE, SUM(NVL(passed_qty,0)) CUSTOMER_STOCK
                from   batch_wise_stock@dblink_spa_stl.squaregroup.com
                GROUP BY customer_code, PRODUCT_CODE";



                listOfQuery.Add(commonServices.AddQuery(truncate, commonServices.AddParameter(new string[] { })));
                listOfQuery.Add(commonServices.AddQuery( Update_query, commonServices.AddParameter(new string[] { })));
                commonServices.SaveChanges(ConfigurationManager.ConnectionStrings[db].ConnectionString, listOfQuery);
                
                return "1";
             }
             catch (Exception ex)
             {
                return ex.Message;
             }
            
        }
    }
}