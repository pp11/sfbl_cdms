using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.Inventory
{
    public class DEPOT_DISPATCH_PRODUCT_BATCH
    { 
        public int DISPATCH_PROD_BATCH_ID { get; set; }
        public int DISPATCH_PRODUCT_ID { get; set; }
        public int MST_ID { get; set; }
        public string REQUISITION_NO { get; set; }

        public int DISPATCH_UNIT_ID { get; set; }

        public string DISPATCH_DATE { get; set; }
        public string SKU_NAME { get; set; }
        public int SKU_ID { get; set; }
        public string SKU_CODE { get; set; }
        public double UNIT_TP { get; set; }

        public int BATCH_ID { get; set; }
        public string BATCH_NO { get; set; }

        public int DISPATCH_QTY { get; set; }
        public double DISPATCH_AMOUNT { get; set; }
        public double DISTRIBUTION_AMOUNT { get; set; }
        public string STATUS { get; set; }
        public int COMPANY_ID { get; set; }

        public string REMARKS { get; set; }

        public int ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public int UPDATED_BY { get; set; }
        public string UPDATED_TERMINAL { get; set; }
        public string UPDATED_DATE { get; set; }

        public int ROW_NO { get; set; }

    }
}
