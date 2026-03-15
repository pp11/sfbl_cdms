using SalesAndDistributionSystem.Domain.Models.ReportModels.Common.ProductReports;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace SalesAndDistributionSystem.Reporting.Areas.Inventory.Data
{
    public class GiftItemReport
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


        public DataTable GiftReceive(ReportParams reportParameters)
        {
            string query = @"SELECT RECEIVE_ID,
                   TO_CHAR(RECEIVE_DATE, 'DD/MM/YY') RECEIVE_DATE,
                   CHALLAN_NO,
                   TO_CHAR(CHALLAN_DATE, 'DD/MM/YY') CHALLAN_DATE,
                   F.UNIT_ID,
                   FN_UNIT_NAME(F.COMPANY_ID, F.UNIT_ID) UNIT_NAME,
                   BATCH_NO,
                   RECEIVE_QTY,
                   RECEIVE_AMOUNT,
                   GIFT_ITEM_PRICE,
                   G.GIFT_ITEM_NAME SKU_NAME,
                   FN_EMPLOYEE_ID(RECEIVED_BY_ID) EMPLOYEE_CODE,
                   FN_EMPLOYEE_NAME(RECEIVED_BY_ID) RECEIVED_BY_NAME,
                   S.SUPPLIER_NAME
            FROM GIFT_ITEM_RECEIVING F
            LEFT JOIN GIFT_ITEM_INFO G ON F.GIFT_ITEM_ID = G.GIFT_ITEM_ID
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

        public DataTable GiftReceiveRegister(ReportParams reportParameters)
        {
            var query = @"SELECT ROWNUM as SL, R.UNIT_ID,
               FN_UNIT_NAME(R.COMPANY_ID,R.UNIT_ID) UNIT_NAME,
               RECEIVE_ID RECEIVE_SLNO,
               G.GIFT_ITEM_NAME SKU_NAME,
               TO_CHAR(RECEIVE_DATE, 'DD/MM/RRRR') RECEIVE_DATE,
               SUPPLIER_ID,
               FN_SUPPLIER_NAME(R.COMPANY_ID,SUPPLIER_ID) SUPPLIER_NAME,
               CHALLAN_NO,
               CHALLAN_DATE,
               BATCH_NO,
               GIFT_ITEM_PRICE UNIT_TP,
               RECEIVE_QTY,
               RECEIVE_AMOUNT,
               RECEIVED_BY_ID,
               FN_EMPLOYEE_ID(RECEIVED_BY_ID) EMPPLOYEE_ID,
               FN_EMPLOYEE_NAME(RECEIVED_BY_ID) RECEIVED_BY
        FROM GIFT_ITEM_RECEIVING R
        LEFT JOIN GIFT_ITEM_INFO G ON R.GIFT_ITEM_ID = G.GIFT_ITEM_ID
        WHERE  R.COMPANY_ID=:param1
        AND    TRUNC(RECEIVE_DATE) BETWEEN TRUNC(TO_DATE(:param2,'DD/MM/RRRR')) AND TRUNC(TO_DATE(:param3,'DD/MM/RRRR'))";

            var parameters = new List<string> { reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_FROM, reportParameters.DATE_TO };

            if (reportParameters.UNIT_ID != "ALL")
            {
                query += " And R.UNIT_ID = " + reportParameters.UNIT_ID;
            }

            if (!string.IsNullOrWhiteSpace(reportParameters.GIFT_ITEM_ID)
                && reportParameters.GIFT_ITEM_ID != "undefined")
            {
                query += " AND R.GIFT_ITEM_ID IN (";
                var arr = reportParameters.GIFT_ITEM_ID.Split(',');
                var ids = "";
                foreach (var no in arr)
                {
                    ids += "'" + no + "',";
                }
                query += ids.Substring(0, ids.Length - 1) + ") ";
            }

            return commonServices.GetDataTable(ConfigurationManager.ConnectionStrings[reportParameters.DB].ConnectionString, query, commonServices.AddParameter(parameters.ToArray()));
        }

        public DataTable GiftStockRegister(ReportParams reportParameters)
        {
            var query = @"SELECT ROWNUM as SL,
                           TO_CHAR(STOCK_DATE, 'DD/MM/RRRR') STOCK_DATE,
                           UNIT_ID,
                           FN_UNIT_NAME(COMPANY_ID,UNIT_ID) UNIT_NAME,
                           GIFT_ITEM_ID,
                           FN_GIFT_ITEM_NAME(COMPANY_ID,GIFT_ITEM_ID) GIFT_ITEM_NAME,
                           CLOSING_STOCK_QTY STOCK_QTY
                    FROM GIFT_DATE_WISE_STOCK A
                    WHERE COMPANY_ID=:param1
                    AND   TRUNC(STOCK_DATE)= (SELECT TRUNC(MAX(STOCK_DATE)) 
                            FROM GIFT_DATE_WISE_STOCK 
                            WHERE COMPANY_ID=A.COMPANY_ID 
                            AND UNIT_ID=A.UNIT_ID 
                            AND GIFT_ITEM_ID=A.GIFT_ITEM_ID
                            AND TRUNC(STOCK_DATE) <=TRUNC(TO_DATE(:param2,'DD/MM/RRRR'))
                          )";

            var parameters = new List<string> { reportParameters.COMPANY_ID.ToString(), reportParameters.DATE_TO };

            if (reportParameters.UNIT_ID != "ALL")
            {
                query += " And UNIT_ID = " + reportParameters.UNIT_ID;
            }

            if (!string.IsNullOrWhiteSpace(reportParameters.GIFT_ITEM_ID)
                && reportParameters.GIFT_ITEM_ID != "undefined")
            {
                query += " AND GIFT_ITEM_ID IN (";
                var arr = reportParameters.GIFT_ITEM_ID.Split(',');
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