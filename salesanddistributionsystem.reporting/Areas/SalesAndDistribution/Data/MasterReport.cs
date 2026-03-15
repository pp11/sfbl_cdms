using Microsoft.Reporting.WebForms;
using Oracle.ManagedDataAccess.Client;
using SalesAndDistributionSystem.Domain.Models.ReportModels.Common.ProductReports;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace SalesAndDistributionSystem.Reporting.Areas.SalesAndDistribution.Data
{
    public class MasterReport
    {
        private readonly CommonServices commonServices = new CommonServices();

        //---------------Query Part--------------------------------
        private string GetPageHeaderReport_Query() => @"SELECT COMPANY_NAME, COMPANY_ADDRESS
  FROM VW_REPORT_HEADER_DATA
 WHERE COMPANY_ID = :param1 AND ROWNUM = 1";
        private string GetProductReport_Query() => @"SELECT ROW_NUMBER () OVER (ORDER BY P.SKU_ID ASC) AS Serial,
       SKU_ID,
       SKU_CODE,
       SKU_NAME,
       SKU_NAME_BANGLA,
       PACK_SIZE,
       PRODUCT_TYPE_ID,
       PRIMARY_PRODUCT_ID,
       BRAND_ID,
       CATEGORY_ID,
       BASE_PRODUCT_ID,
       GROUP_ID,
       PRODUCT_SEASON_ID,
       QTY_PER_PACK,
       WEIGHT_PER_PACK,
       WEIGHT_UNIT,
       SHIPPER_QTY,
       SHIPPER_WEIGHT,
       SHIPPER_WEIGHT_UNIT,
       SHIPPER_VOLUME,
       SHIPPER_VOLUME_UNIT,
       PACK_UNIT,
       PACK_VALUE,
       STORAGE_ID,
       FONT_COLOR,
       COMPANY_ID,
       UNIT_ID,
       PRODUCT_STATUS,
       REMARKS
  FROM PRODUCT_INFO P";
        //private string GetProductBounusReport_Query() => @"begin  :param1 := fn_product_bonus_report(:param2); end;";
        private string GetProductBounusReport_Query() => @"begin  :param1 := fn_SKU_bonus_report(TO_DATE(:param2, 'DD/MM/YYYY'), TO_DATE(:param3, 'DD/MM/YYYY'),:param4); end;";       
        private string GetProductBounusReportLine_Query() => @"begin  :param1 := FN_DATE_WISE_PRODUCT_BONUS(TO_DATE(:param2, 'DD/MM/YYYY'), TO_DATE(:param3, 'DD/MM/YYYY'),:param4); end;";
        private string GetComboBounusReport_Query() => @"begin  :param1 := fn_combo_bonus_report(:param2); end;";
        private string GetProductPriceInformationReport_Query() => @"select *from VW_PRODUCT_PRICE P";
        //private string GetCustomerInfoReport_Query() => @"select *from VW_LOCATION_FOR_ORDER O ORDER BY O.DIVISION_ID, O.REGION_ID, O.AREA_ID, O.TERRITORY_ID, O.MARKET_ID, O.CUSTOMER_ID";        
        private string GetCustomerRelationReport_Query() => @"Select *from VW_LOCATION_CUSTOMER_RELATION O 
WHERE O.DIVISION_ID=NVL(:param1,O.DIVISION_ID) AND O.REGION_ID=NVL(:param2,O.REGION_ID) AND O.AREA_ID=NVL(:param3,O.AREA_ID)
 AND O.TERRITORY_ID=NVL(:param4,O.TERRITORY_ID) AND O.MARKET_ID=NVL(:param5,O.MARKET_ID) AND O.CUSTOMER_ID=NVL(:param6,O.CUSTOMER_ID)
ORDER BY O.DIVISION_ID, O.REGION_ID, O.AREA_ID, O.TERRITORY_ID, O.CUSTOMER_ID, O.MARKET_ID";
        private string GetCustomerPriceReport_Query() => @"SELECT M.CUSTOMER_CODE,
         M.CUSTOMER_ID,
         M.CUSTOMER_PRICE_MSTID,
         D.CUSTOMER_PRICE_DTLID,
         TO_DATE (M.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE,
         TO_DATE (M.EFFECT_START_DATE, 'DD/MM/YYYY') EFFECT_START_DATE,
         M.ENTERED_DATE,
         M.REMARKS,
         M.STATUS,
         D.ADD_COMMISSION1,
         D.ADD_COMMISSION2,
         D.COMMISSION_FLAG,
         D.COMMISSION_TYPE,
         D.COMMISSION_VALUE,
         D.CUSTOMER_PRICE_DTLID,
         D.PRICE_FLAG,
         D.SKU_CODE,
         FN_SKU_NAME (D.COMPANY_ID, D.SKU_ID) SKU_NAME,
         FN_PACK_SIZE (D.COMPANY_ID, D.SKU_ID) PACK_SIZE,
         D.SKU_PRICE,
         D.STATUS AS EXPR1,
         U.COMPANY_NAME,
         U.UNIT_NAME,
         R.CUSTOMER_NAME,
         I.CUSTOMER_TYPE_NAME
    FROM CUSTOMER_SKU_PRICE_MST M
         INNER JOIN CUSTOMER_SKU_PRICE_DTL D ON D.CUSTOMER_PRICE_MSTID = M.CUSTOMER_PRICE_MSTID
         INNER JOIN CUSTOMER_INFO R ON R.CUSTOMER_ID = M.CUSTOMER_ID
         --INNER JOIN PRODUCT_INFO P ON P.SKU_ID = D.SKU_ID
         INNER JOIN STL_ERP_SCS.COMPANY_INFO U  ON U.UNIT_ID = M.UNIT_ID AND U.COMPANY_ID = M.COMPANY_ID
         LEFT OUTER JOIN CUSTOMER_TYPE_INFO I ON I.CUSTOMER_TYPE_ID = R.CUSTOMER_TYPE_ID
   WHERE M.COMPANY_ID = :param1
         AND M.UNIT_ID = NVL ( :param2, M.UNIT_ID)
         AND M.CUSTOMER_ID = NVL ( :param3, M.CUSTOMER_ID)
   ORDER BY M.CUSTOMER_PRICE_MSTID, D.CUSTOMER_PRICE_DTLID";
        private string GetCreditPolicyReport_Query() => @"SELECT COMPANY_ID,
       FN_COMPANY_NAME (COMPANY_ID) COMPANY_NAME,
       UNIT_ID,
       FN_UNIT_NAME (COMPANY_ID, UNIT_ID) UNIT_NAME,
       CUSTOMER_ID,
       CUSTOMER_CODE,
       FN_CUSTOMER_NAME (COMPANY_ID, CUSTOMER_ID) CUSTOMER_NAME,
       FN_CUSTOMER_ADDRESS (COMPANY_ID, CUSTOMER_ID) CUSTOMER_ADDRESS,
       EFFECT_START_DATE,
       EFFECT_END_DATE,
       CREDIT_LIMIT,
       CREDIT_DAYS,
       STATUS
 FROM CREDIT_INFO
 WHERE COMPANY_ID = :param1 AND UNIT_ID = NVL(:param2,UNIT_ID)";
        private string GetCustomerInfoReport_Query() => @"SELECT C.CUSTOMER_CODE, C.CUSTOMER_NAME, C.PROPRIETOR_NAME, C.CUSTOMER_ADDRESS, C.CUSTOMER_CONTACT, C.BIN_NO, C.TIN_NO, D.DIST_ROUTE_NAME ROUTE_ID, C.CUSTOMER_STATUS  FROM CUSTOMER_INFO C, DISTRIBUTION_ROUTE_INFO D
WHERE C.ROUTE_ID=D.DIST_ROUTE_ID(+)
AND C.COMPANY_ID=NVL(:param1,C.COMPANY_ID) AND C.CUSTOMER_ID=NVL(:param2,C.CUSTOMER_ID)
ORDER BY C.COMPANY_ID, C.UNIT_ID, C.CUSTOMER_NAME";
        private string GetLocationReport_Query() => @"SELECT DISTINCT  O.DIVISION_ID, O.DIVISION_CODE, 
   O.DIVISION_NAME, O.REGION_ID, O.REGION_CODE, 
   O.REGION_NAME, O.AREA_ID, O.AREA_CODE, 
   O.AREA_NAME, O.TERRITORY_ID, O.TERRITORY_CODE, 
   O.TERRITORY_NAME, O.MARKET_ID, O.MARKET_CODE, 
   O.MARKET_NAME
   from VW_LOCATION_RELATION O 
WHERE O.DIVISION_ID=NVL(:param1,O.DIVISION_ID) AND O.REGION_ID=NVL(:param2,O.REGION_ID) AND O.AREA_ID=NVL(:param3,O.AREA_ID)
 AND O.TERRITORY_ID=NVL(:param4,O.TERRITORY_ID) AND O.MARKET_ID=NVL(:param5,O.MARKET_ID)";
        //private string GetProductBounusReport_Query() => @"select A.PRODUCT_BONUS_MST_ID,
        //                   A.BONUS_NAME,
        //                   A.EFFECT_START_DATE,
        //                   A.EFFECT_END_DATE,
        //                   A.LOCATION_TYPE,       
        //                   A.SKU_CODE DEC_SKU_CODE,
        //                   FN_SKU_NAME(A.COMPANY_ID,A.SKU_ID) DEC_SKU_NAME,
        //                   FN_PACK_SIZE(A.COMPANY_ID,A.SKU_ID)DEC_PACK_SIZE,
        //                   A.STATUS,
        //                   A.REMARKS,

        //                   B.LOCATION_CODE,
        //                   FN_LOCATION_NAME(A.COMPANY_ID,A.LOCATION_TYPE,B.LOCATION_ID) LOCATION_NAME, 
        //                   B.STATUS LOCATION_STATUS,      
        //                   C.PRODUCT_BONUS_DTL_ID,
        //                   C.PRODUCT_BONUS_SLAB_TYPE,
        //                   C.SLAB_QTY,
        //                   C.PRODUCT_BONUS_TYPE,       
        //                   C.PRODUCT_BONUS_SKU_CODE BONUS_SKU_CODE,
        //                   FN_SKU_NAME(A.COMPANY_ID,C.PRODUCT_BONUS_SKU_ID)BONUS_SKU_NAME,
        //                   FN_PACK_SIZE(A.COMPANY_ID,C.PRODUCT_BONUS_SKU_ID)BONUS_PACK_SIZE,       
        //                   C.CALCULATION_TYPE,
        //                   C.PRODUCT_BONUS_QTY,
        //                   C.DISCOUNT_PCT,
        //                   C.DISCOUNT_VAL,
        //                   GIFT_ITEM_NAME(A.COMPANY_ID,C.GIFT_ITEM_ID)GIFT_ITEM_NAME,
        //                   C.GIFT_QTY
        //            from PRODUCT_BONUS_MST A, PRODUCT_BONUS_LOCATION  B, PRODUCT_BONUS_DTL C
        //            WHERE A.PRODUCT_BONUS_MST_ID=B.PRODUCT_BONUS_MST_ID
        //            AND   A.PRODUCT_BONUS_MST_ID=C.PRODUCT_BONUS_MST_ID
        //            ";


        //private string GetComboBounusReport_Query() => @"select A.BONUS_MST_ID,
        //                   A.BONUS_NAME,
        //                   A.EFFECT_START_DATE,
        //                   A.EFFECT_END_DATE,
        //                   A.LOCATION_TYPE,       
        //                   A.STATUS,
        //                   A.REMARKS,
        //                   A.ENTRY_DATE,
        //                   B.BONUS_LOCATION_ID,
        //                   B.LOCATION_CODE,
        //                   FN_LOCATION_NAME(A.COMPANY_ID,A.LOCATION_TYPE,B.LOCATION_ID) LOCATION_NAME, 
        //                   B.STATUS LOCATION_STATUS,      
        //                   C.BONUS_DTL_ID,
        //                   C.BONUS_SLAB_TYPE,
        //                   C.SLAB_QTY,
        //                   C.BONUS_TYPE,       
        //                   C.BONUS_SKU_CODE,
        //                   FN_SKU_NAME(A.COMPANY_ID,C.BONUS_SKU_ID) BONUS_SKU_NAME,
        //                   FN_PACK_SIZE(A.COMPANY_ID,C.BONUS_SKU_ID) BONUS_PACK_SIZE,       
        //                   C.CALCULATION_TYPE,
        //                   C.BONUS_QTY,
        //                   C.DISCOUNT_PCT,
        //                   C.DISCOUNT_VAL,
        //                   GIFT_ITEM_NAME(A.COMPANY_ID,C.GIFT_ITEM_ID) GIFT_ITEM_NAME,
        //                   C.GIFT_QTY,
        //                   D.BONUS_DECLARE_ID,
        //                   D.SKU_CODE DEC_SKU_CODE,
        //                   FN_SKU_NAME(A.COMPANY_ID,D.SKU_ID) DEC_SKU_NAME,
        //                   FN_PACK_SIZE(A.COMPANY_ID,D.SKU_ID) DEC_PACK_SIZE,
        //                   D.GROUP_ID,
        //                   D.BRAND_ID,
        //                   D.BASE_PRODUCT_ID,
        //                   D.STATUS DEC_STATUS
        //            from BONUS_MST A, BONUS_LOCATION  B, BONUS_DTL C, BONUS_DECLARE_PRODUCT D
        //            WHERE A.BONUS_MST_ID=B.BONUS_MST_ID
        //            AND   A.BONUS_MST_ID=C.BONUS_MST_ID AND D.BONUS_MST_ID=A.BONUS_MST_ID";
        //------Execution Part------------------------
        public DataTable GetProductReport()
        {
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString, GetProductReport_Query(), commonServices.AddParameter(new string[] { }));
        }
        public DataTable GetComboBounusReport(ReportParams reportParameters)
        {
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString, GetComboBounusReport_Query(), commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.PRODUCT_BONUS_MST_ID }));
        }
        public DataTable GetProductBounusReport(ReportParams reportParameters)
        {
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, GetProductBounusReport_Query(), commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.SKU_ID == "ALL"?"":reportParameters.SKU_ID}));
        } 
        public DataTable GetProductBounusLineReport(ReportParams reportParameters)
        {
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, GetProductBounusReportLine_Query(), commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO, reportParameters.SKU_ID == "ALL"?"":reportParameters.SKU_ID}));
        }
        public DataTable GetProductPriceInformationReport(ReportParams reportParameters)
        {
            string Query = GetProductPriceInformationReport_Query() + " WHERE P.COMPANY_ID=" + reportParameters.COMPANY_ID;

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
            if (reportParameters.SKU_ID != "ALL" && reportParameters.SKU_ID != null && reportParameters.SKU_ID != "" && reportParameters.SKU_ID != "undefined")
            {
                Query = Query + " AND p.SKU_ID in (" + reportParameters.SKU_ID + ")";
            }
            Query = Query + " Order By p.SKU_NAME,p.SKU_CODE";
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, Query, commonServices.AddParameter(new string[] { }));
        }
        public DataTable GetCustomerRelationReport(ReportParams reportParameters)
        {
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, GetCustomerRelationReport_Query(), commonServices.AddParameter(new string[] { reportParameters.DIVISION_ID, reportParameters.REGION_ID, reportParameters.AREA_ID, reportParameters.TERRITORY_ID, reportParameters.MARKET_ID, reportParameters.CUSTOMER_ID }));
        }
        public DataTable GetCustomerInfoReport(ReportParams reportParameters)
        {
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, GetCustomerInfoReport_Query(), commonServices.AddParameter(new string[] { reportParameters.COMPANY_ID.ToString(),  reportParameters.CUSTOMER_ID }));
        }
        public DataTable GetCreditPolicyReport(ReportParams reportParameters)
        {
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, GetCreditPolicyReport_Query(), commonServices.AddParameter(new string[] { reportParameters.COMPANY_ID.ToString(), reportParameters.UNIT_ID }));
        }
        public DataTable GetCustomerPriceReport(ReportParams reportParameters)
        {
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, GetCustomerPriceReport_Query(), commonServices.AddParameter(new string[] { reportParameters.COMPANY_ID.ToString(), reportParameters.UNIT_ID, reportParameters.CUSTOMER_ID }));
        }
        public DataTable GetLocationReport(ReportParams reportParameters)
        {
            try
            {
                return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, GetLocationReport_Query(), commonServices.AddParameter(new string[] { reportParameters.DIVISION_ID, reportParameters.REGION_ID, reportParameters.AREA_ID, reportParameters.TERRITORY_ID, reportParameters.MARKET_ID }));
            }
            catch (Exception)
            {

                throw;
            }

        }
        public DataTable GetPageHeaderReport(ReportParams reportParameters)
        {
            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, GetPageHeaderReport_Query(), commonServices.AddParameter(new string[] { reportParameters.COMPANY_ID.ToString() }));

        }
    }
}