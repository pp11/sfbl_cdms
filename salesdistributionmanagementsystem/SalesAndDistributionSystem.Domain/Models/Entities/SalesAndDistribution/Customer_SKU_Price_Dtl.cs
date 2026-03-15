using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Customer_SKU_Price_Dtl
    {
        public int CUSTOMER_PRICE_DTLID { get; set; }
        public int CUSTOMER_PRICE_MSTID { get; set; }
        public int SKU_ID { get; set; }
        public string SKU_CODE { get; set; }
        public string SKU_NAME { get; set; }
        public string PACK_SIZE { get; set; }
        public int GROUP_ID { get; set; }
        public int CATEGORY_ID { get; set; }
        public int BRAND_ID { get; set; }
        public int BASE_PRODUCT_ID { get; set; }
        public string PRICE_FLAG { get; set; }
        public decimal SKU_PRICE { get; set; }
        public string COMMISSION_FLAG { get; set; }
        public string COMMISSION_TYPE { get; set; }
        public decimal COMMISSION_VALUE { get; set; }
        public decimal ADD_COMMISSION1 { get; set; }
        public decimal ADD_COMMISSION2 { get; set; }
        public string STATUS { get; set; }
        public int COMPANY_ID { get; set; }
        public int UNIT_ID { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }
        public int ROW_NO { get; set; }

    }
    public class Price_Dtl_Param
    {
        public string GROUP_ID { get; set; }
        public string db { get; set; }
        public int COMPANY_ID { get; set; }
        public string BRAND_ID { get; set; }
        public string CATEGORY_ID { get; set; }
        public string BASE_PRODUCT_ID { get; set; }
        public List<string> PRODUCT_ID { get; set; }
    }
}
