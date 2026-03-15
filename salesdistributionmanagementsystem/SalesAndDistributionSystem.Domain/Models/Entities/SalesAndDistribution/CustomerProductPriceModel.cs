using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class CustomerProductPriceModel
    {
        public int COMPANY_ID { get; set; }
        public int CUSTOMER_PRICE_MSTID { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string EFFECT_START_DATE { get; set; }
        public string EFFECT_END_DATE { get; set; }
        public string CUSTOMER_STATUS { get; set; }
        public string REMARKS { get; set; }
        public string ENTRYDATE { get; set; }
        public string GROUP_ID { get; set; }
        public string BRAND_ID { get; set; }
        public string CATEGORY_ID { get; set; }
        public string BASE_PRODUCT_ID { get; set; }
        public string PRODUCT_ID { get; set; }
        public double SKU_PRICE { get; set; }
        public string COMMISSION_FLAG { get; set; }
        public string PRICE_FLAG { get; set; }
        public double COMMISSION_VALUE { get; set; }
        public string COMMISSION_TYPE { get; set; }
        public double ADD_COMMISSION { get; set; }
        public double ADDITIONAL_COMMISSION { get; set; }
        public double ADD_COMMISSION2 { get; set; }
        public List<Customer_SKU_Price_Mst> customerProductList{ get; set; }
    }
    
}
