using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class DEBIT_CREDIT_ADJ
    {
        public int ID { get; set; }
        public string ADJUSTMENT_NO { get; set; }
        public int CUSTOMER_ID { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public decimal ADJUSTMENT_AMOUNT { get; set; }
        public int COMPANY_ID { get; set; }
        public string REMARKS { get; set; }
        public string POSTING_STATUS { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string POSTED_BY { get; set; }
        public string POSTED_DATE { get; set; }
        public string POSTED_TERMINAL { get; set; }
    }
}
