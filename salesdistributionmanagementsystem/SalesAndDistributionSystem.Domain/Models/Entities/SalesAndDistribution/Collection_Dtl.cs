using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Collection_Dtl
    {
        public int COLLECTION_DTL_ID { get; set; }

        public int COLLECTION_MST_ID { get; set; }
        public string BANK_ID { get; set; }
        public string BRANCH_ID { get; set; }

        public decimal COLLECTION_AMT { get; set; }
        public string COLLECTION_MODE { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public int CUSTOMER_ID { get; set; }
        public string INVOICE_NO { get; set; }

        public decimal MEMO_COST { get; set; }
        public decimal NET_COLLECTION_AMT { get; set; }
        public decimal TDS_AMT { get; set; }
        public int UNIT_ID { get; set; }

        public string VOUCHER_NO { get; set; }
        public string VOUCHER_DATE { get; set; }
        public string REMARKS { get; set; }

        public int COMPANY_ID { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }
        public string VERSION_NO { get; set; }

        //Helper Attributes
        public int ROW_NO { get; set; }

    }
}
