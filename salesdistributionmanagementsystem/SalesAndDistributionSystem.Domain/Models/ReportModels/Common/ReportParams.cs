using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.ReportModels.Common.ProductReports
{
    public class ReportParams
    {
        public string PREVIEW { get; set; }
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }
        public string DATE_RANGE_SELECTION_TEXT { get; set; }
        public string SKU_ID { get; set; }
        public string BRAND_ID { get; set; }
        public string CATEGORY_ID { get; set; }
        public string BASE_PRODUCT_ID { get; set; }
        public string GIFT_ITEM_ID { get; set; }
        public string BATCH_NO { get; set; }
        public string PRIMARY_PRODUCT_ID { get; set; }
        public string PRODUCT_SEASON_ID { get; set; }
        public string PRODUCT_TYPE_ID { get; set; }
        public string MST_ID { get; set; }
        public string REQUISITION_NO { get; set; }
        public string Type { get; set; }
        public string UNIT_ID { get; set; }
        public string UNIT_NAME { get; set; }
        public string GROUP_ID { get; set; }
        public int COMPANY_ID { get; set; }
        public string TRANSFER_NOTE_NO { get; set; }
        public string CHALLAN_NO { get; set; }
        public int REPORT_ID{ get; set; }
        public string DB { get; set; }
        public string DB_SECURITY { get; set; }
        public string SECRET_KEY { get; set; }
        public string USER_NAME { get; set; }
        public string HEADER_SELECTION { get; set; }
        public string REPORT_EXTENSION { get; set; }
        public string DISPATCH_NO { get; set; }
        public string YEAR { get; set; }
        public string MONTH_CODE { get; set; }
        public string DOT_PRINTER { get; set; }
        public string QUERY { get; set; }
        public string PRODUCT_STATUS { get; set; }
        public string INVOICE_STATUS { get; set; }

        //--------------------------INVOICE REPORT PARAM----------------------------
        public string INVOICE_DATE { get; set; }
        public string DIVISION_ID { get; set; }
        public string DIVISION_CODE { get; set; }
        public string REGION_ID { get; set; }
        public string REGION_CODE { get; set; }
        public string AREA_ID { get; set; }
        public string AREA_CODE { get; set; }
        public string TERRITORY_ID { get; set; }
        public string TERRITORY_CODE { get; set; }
        public string MARKET_ID { get; set; }
        public string CUSTOMER_ID { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string INVOICE_NO_FROM { get; set; }
        public string INVOICE_NO_TO { get; set; }
        public string INVOICE_NO { get; set; }

        //--------------------------MASTER REPORT----------------------------
        public string PRODUCT_BONUS_MST_ID { get; set; }


    }
}
