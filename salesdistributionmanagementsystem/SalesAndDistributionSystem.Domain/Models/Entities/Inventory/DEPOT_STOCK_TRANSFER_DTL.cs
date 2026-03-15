using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.Inventory
{
    public class DEPOT_STOCK_TRANSFER_DTL
    {
        public int MST_ID { get; set; }
        public int DTL_ID { get; set; }
        public string SKU_ID { get; set; }
        public string SKU_NAME { get; set; }
        public string SKU_CODE { get; set; }
        public double UNIT_TP { get; set; }
        public decimal TRANSFER_QTY { get; set; }
        public decimal STOCK_QTY { get; set; }
        public decimal TRANSFER_AMOUNT { get; set; }
        public string STATUS { get; set; }

        public int COMPANY_ID { get; set; }

        public string REMARKS { get; set; }
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
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_TERMINAL { get; set; }
        public string UPDATED_DATE { get; set; }
        public string q { get; set; }
        public int ROW_NO { get; set; }
    }
}
