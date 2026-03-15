using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.Inventory
{
    public class RefurbishmentFinalizeMaster
    {
        public int MST_SLNO { get; set; }
        public string FINALIZE_DATE { get; set; }
        public string FINALIZE_SHIFT { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CLAIM_NO { get; set; }
        public string RECEIVE_CATEGORY { get; set; }
        public decimal TOTAL_AMOUNT { get; set; }
        public string REMARKS { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }
        public string APPROVED_STATUS { get; set; }
        public string APPROVED_BY { get; set; }
        public string APPROVED_DATE { get; set; }
        public string APPROVED_TERMINAL { get; set; }
        public string COMPANY_ID { get; set; }
        public string UNIT_ID { get; set; }
        public List<RefurbishmentFinalizeReceive> RcvDetails { get; set; }
        public List<RefurbishmentFinalizeDetail> Details { get; set; }

    }
}
