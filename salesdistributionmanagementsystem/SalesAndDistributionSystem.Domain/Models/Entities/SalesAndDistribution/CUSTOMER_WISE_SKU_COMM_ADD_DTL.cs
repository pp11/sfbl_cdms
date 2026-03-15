using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class CUSTOMER_WISE_SKU_COMM_ADD_DTL
    {
        public int DTL_ID { get; set; }
        public int MST_ID { get; set; }
        public string UNIT_ID { get; set; }
        public int CUSTOMER_ID { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string SKU_ID { get; set; }
        public string SKU_CODE { get; set; }
        public string PRICE_FLAG { get; set; }
        public decimal SKU_PRICE { get; set; }
        public string COMMISSION_FLAG { get; set; }
        public string COMMISSION_TYPE { get; set; }
        public string COMMISSION_VALUE { get; set; }
        public string ADD_COMMISSION1 { get; set; }
        public string ADD_COMMISSION2 { get; set; }
        public int COMPANY_ID { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }
    }
}
