using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.Inventory
{
    public class RefurbishmentReceivingDtl
    {
        public int DTL_SLNO { get; set; }
        public int MST_SLNO { get; set; }
        public string REFURBISHMENT_PRODUCT_STATUS { get; set; }
        public string PRODUCT_CODE { get; set; }
        public decimal CLAIM_QTY { get; set; }
        public decimal RECEIVED_QTY { get; set; }
        public decimal DISPUTE_QTY { get; set; }
        public decimal TRADE_PRICE { get; set; }
        public decimal REVISED_PRICE { get; set; }
        public string EXPIRY_DATE { get; set; }
        public string REMARKS { get; set; }
        public string ENTERED_BY { get; set; }
        public Nullable<DateTime> ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<DateTime> UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }
        public string BATCH_NO { get; set; }
        public string LOT_NO { get; set; }

        
    }
}
