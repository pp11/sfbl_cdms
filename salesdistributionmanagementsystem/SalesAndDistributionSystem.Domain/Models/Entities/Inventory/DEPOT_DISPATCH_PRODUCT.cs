using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.Inventory
{
    public class DEPOT_DISPATCH_PRODUCT
    {

        public int DISPATCH_PRODUCT_ID { get; set; }
        public int DISPATCH_REQ_ID { get; set; }
        public int MST_ID { get; set; }

        public string SKU_NAME { get; set; }
        public int SKU_ID { get; set; }
        public double UNIT_TP { get; set; }
  
        public string SKU_CODE { get; set; }
        public string REQUISITION_NO { get; set; }
        public string DISPATCH_NO { get; set; }
        public string ISSUE_NO { get; set; }

        public int SHIPPER_QTY { get; set; }
        public int NO_OF_SHIPPER { get; set; }
        public int LOOSE_QTY { get; set; }
        public double SHIPPER_WEIGHT { get; set; }
        public double SHIPPER_VOLUME { get; set; }
        public double LOOSE_WEIGHT { get; set; }
        public double LOOSE_VOLUME { get; set; }
        public double TOTAL_WEIGHT { get; set; }
        public double TOTAL_SHIPPER_WEIGHT { get; set; }
        public double TOTAL_VOLUME { get; set; }
        public double TOTAL_SHIPPER_VOLUME { get; set; }
        public double PER_PACK_WEIGHT { get; set; }
        public double PER_PACK_VOLUME { get; set; }


        public int PENDING_DISPATCH_QTY { get; set; }
        public int ISSUE_QTY { get; set; }
        public int DISPATCH_QTY { get; set; }
        public int TOTALDISQTY { get; set; }
        public double ISSUE_AMOUNT { get; set; }
        public double DISTRIBUTION_AMOUNT { get; set; }
  

        public int DISPATCH_UNIT_ID { get; set; }
        public int DISPATCH_UNIT_NAME { get; set; }
        public int COMPANY_ID { get; set; }
        public double DISPATCH_AMOUNT { get; set; }

        public string REMARKS { get; set; }

        public int ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public int UPDATED_BY { get; set; }
        public string UPDATED_TERMINAL { get; set; }
        public string UPDATED_DATE { get; set; }
        public string DISPATCH_DATE { get; set; }
        public string DISPATCH_DATE_FORMATED { get; set; }

        public int ROW_NO { get; set; }
    }
}
