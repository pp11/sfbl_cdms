using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class COLLECTION_REVERSE
    {
        public int COLL_REVERSE_ID { get; set; }
        public int COLLECTION_DTL_ID { get; set; }
        public string BATCH_NO { get; set; }
        public string BATCH_DATE { get; set; }
        public int CUSTOMER_ID { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string VOUCHER_NO { get; set; }
        public string VOUCHER_DATE { get; set; }
        public string REVERSE_DATE { get; set; }
        public int BANK_ID { get; set; }
        public int BRANCH_ID { get; set; }
        public string COLLECTION_MODE { get; set; }
        public decimal COLLECTION_AMT { get; set; }
        public decimal TDS_AMT { get; set; }
        public decimal MEMO_COST { get; set; }
        public decimal NET_COLLECTION_AMT { get; set; }
        public string REMARKS { get; set; }
        public int COMPANY_ID { get; set; }
        public int UNIT_ID { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }
    }
}
