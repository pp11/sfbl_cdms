using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.Target
{
    public class CUSTOMER_TARGET_DTL
    {
        public int DTL_ID { get; set; }
        public int MST_ID { get; set; }
        public string SKU_ID { get; set; }
        public string SKU_CODE { get; set; }
        public decimal? UNIT_TP { get; set; }
        public decimal? MRP { get; set; }
        public decimal? AVG_PER_DAY_TARGET_QTY { get; set; }
        public decimal PREVIOUS_TARGET_QTY { get; set; }
        public decimal TARGET_QTY { get; set; }
        public decimal TARGET_VALUE { get; set; }
        public decimal? DISCOUNT_VALUE { get; set; }
        public decimal NET_VALUE { get; set; }
        public int COMPANY_ID { get; set; }
        public int UNIT_ID { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }
        public string PACK_SIZE { get; set; }

        //Extra
        public int ROW_NO { get; set; }
        //public string MARKET_CODE { get; set; }
        //public string MARKET_ID { get; set; }
        public string CUSTOMER_ID { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_NAME { get; set; }
        //public string MARKET_NAME { get; set; }
        public string SKU_NAME { get; set; }
    }
}
