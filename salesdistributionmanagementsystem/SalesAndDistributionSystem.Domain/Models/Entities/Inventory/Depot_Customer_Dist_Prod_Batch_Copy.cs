using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.Inventory
{
    public class Depot_Customer_Dist_Prod_Batch_Copy
    {
        public int DEPOT_BATCH_ID { get; set; }
        public int DEPOT_PRODUCT_ID { get; set; }
        public int MST_ID { get; set; }
        public int SKU_ID { get; set; }
        public string SKU_CODE { get; set; }
        public int UNIT_TP { get; set; }
        public int BATCH_ID { get; set; }
        public string BATCH_NO { get; set; }
        public int DISTRIBUTION_INVOICE_QTY { get; set; }
        public int DISTRIBUTION_BONUS_QTY { get; set; }
        public int COMPANY_ID { get; set; }
        public int INVOICE_UNIT_ID { get; set; }

        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }

        //Helper Attribute--------
        public int ROW_NO { get; set; }
    }
}
