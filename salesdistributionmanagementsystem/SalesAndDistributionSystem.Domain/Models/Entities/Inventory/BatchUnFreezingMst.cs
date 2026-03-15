using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.Inventory
{
    public class BatchUnFreezingMst
    {
        public int ROW_NO { get; set; }
        public int MST_ID { get; set; }
        public string ENTRY_DATE{ get; set; }
        public int COMPANY_ID { get; set; }
        public int UNIT_ID { get; set; }
        public int SKU_ID { get; set; }
        public string SKU_CODE { get; set; }       
        public string REMARKS { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }

        public List<BatchUnFreezingDtl> batchFreezingDtlList { get; set; }

    }
}
