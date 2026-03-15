using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.Inventory
{
    public class DEPOT_REQ_DISTRIBUTION_PRODUCT
    {
        public int DEPOT_REQ_PRODUCT_ID { get; set; }
        public int DEPOT_REQ_ID { get; set; }
        public int MST_ID { get; set; }
        public string ISSUE_NO { get; set; }
        public string ISSUE_DATE { get; set; }
        public int ISSUE_UNIT_ID { get; set; }

        public string DISTRIBUTION_DATE { get; set; }
        public string SKU_NAME { get; set; }
        public int SKU_ID { get; set; }
        public string SKU_CODE { get; set; }
        public double UNIT_TP { get; set; }

        public int BATCH_ID { get; set; }
        public string BATCH_NO { get; set; }


        public int ISSUE_QTY { get; set; }
        public int DISTRIBUTION_QTY { get; set; }
        public double ISSUE_AMOUNT { get; set; }
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
