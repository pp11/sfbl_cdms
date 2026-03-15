using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.Inventory
{
    public class Depot_Customer_Dist_Product_Copy
    {
        public int DEPOT_PRODUCT_ID { get; set; }
        public int DEPOT_INV_ID { get; set; }
        public int MST_ID { get; set; }
        public int SKU_ID { get; set; }
        public string SKU_CODE { get; set; }
        public int UNIT_TP { get; set; }
        public int INVOICE_QTY { get; set; }
        public int BONUS_QTY { get; set; }
        public int DISTRIBUTION_QTY { get; set; }
        public int DISTRIBUTION_BONUS_QTY { get; set; }
        public int TOTAL_DISTRIBUTION_QTY { get; set; }
        public int SHIPPER_QTY { get; set; }
        public int NO_OF_SHIPPER { get; set; }
        public int LOOSE_QTY { get; set; }
        public decimal? SHIPPER_WEIGHT { get; set; }
        public decimal? SHIPPER_VOLUMN { get; set; }
        public decimal? LOOSE_WEIGHT { get; set; }
        public decimal? LOOSE_VOLUMN { get; set; }
        public decimal? TOTAL_WEIGHT { get; set; }
        public decimal? TOTAL_VOLUMN { get; set; }
        public int COMPANY_ID { get; set; }
        public int INVOICE_UNIT_ID { get; set; }
        public decimal? PENDING_INVOICE_DIST_QTY { get; set; }
        public decimal? PENDING_BONUS_DIST_QTY { get; set; }

        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }

        //Helper Attribute--------
        public string SKU_NAME { get; set; }
        public string PACK_SIZE { get; set; }
        public decimal? WEIGHT_PER_PACK { get; set; }
        public string SHIPPER_VOLUME_UNIT { get; set; }
        public string SHIPPER_WEIGHT_UNIT { get; set; }
        public decimal? PER_SHIPPER_WEIGHT { get; set; }
        public decimal? PER_SHIPPER_VOLUME { get; set; }
        public string SKU_TOTAL_VOLUME { get; set; }
        public string SKU_TOTAL_WEIGHT { get; set; }
        public decimal? PACK_VALUE { get; set; }
        public int ROW_NO { get; set; }
       
        public List<Depot_Customer_Dist_Prod_Batch> Batches { get; set; }
    }
}
