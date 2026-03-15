using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Customer_SKU_Price_Mst
    {
        public int CUSTOMER_PRICE_MSTID { get; set; }
        public string CUSTOMER_PRICE_MSTID_ENCRYPTED { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_ID { get; set; }
        public List<string> CUSTOMER_IDs { get; set; }


        public string CUSTOMER_NAME { get; set; }
        public string ENTRY_DATE { get; set; }
        public string EFFECT_START_DATE { get; set; }
        public string EFFECT_END_DATE { get; set; }
        public int COMPANY_ID { get; set; }
        public int UNIT_ID { get; set; }
        public string CUSTOMER_STATUS { get; set; }
        public string REMARKS { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_TERMINAL { get; set; }
        public string UPDATED_DATE { get; set; }
        public string CUSTOMER_TYPE_ID { get; set; }
        public string q { get; set; }
        public int ROW_NO { get; set; }
        public List<Customer_SKU_Price_Dtl> customerSkuPriceList { get; set; }
        public string Mode { get; set; }
    }

}
