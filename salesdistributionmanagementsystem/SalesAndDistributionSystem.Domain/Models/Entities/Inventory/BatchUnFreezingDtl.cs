using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.Inventory
{
    public class BatchUnFreezingDtl
    {

        public int DTL_ID { get; set; }
        public int MST_ID { get; set; }
        public int COMPANY_ID { get; set; }
        public int UNIT_ID { get; set; }
        public int SKU_ID { get; set; }
        public string SKU_CODE { get; set; }
        public string SKU_NAME { get; set; }
        public string PACK_SIZE { get; set; }
        public int BATCH_ID { get; set; }
        public string BATCH_NO { get; set; }
        public int UNIT_TP { get; set; }
       
        public int STOCK_QTY { get; set; }
        public int BATCH_QTY { get; set; }
        public int BATCH_FREEZE_QTY { get; set; }
        public int BATCH_UN_FREEZE_QTY { get; set; }

        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }

    }
}
