using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class CustomerCommission
    {
        public int COMMISSION_ID { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string CUSTOMER_ID { get; set; }
        public string ENTRY_DATE { get; set; }
        public string EFFECT_START_DATE { get; set; }
        public string EFFECT_END_DATE { get; set; }        
        public string COMMISSION_TYPE { get; set; }
        public decimal COMMISSION_VALUE { get; set; }
        public decimal ADD_COMMISSION1 { get; set; }
        public decimal ADD_COMMISSION2 { get; set; }
        public int UNIT_ID { get; set; }
        public int COMPANY_ID { get; set; }
        public string STATUS { get; set; }
        public string REMARKS { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }

    }
}
