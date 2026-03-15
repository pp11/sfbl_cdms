using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.Inventory
{
    public class RefurbishmentFinalizeReceive
    {
        public int DTL_SLNO { get; set; }
        public int MST_SLNO { get; set; }
        public string PRODUCT_CODE { get; set; }
        public decimal TRADE_PRICE { get; set; }
        public decimal REVISED_PRICE { get; set; }
        public decimal PROD_QTY { get; set; }
        public decimal RECEIVED_QTY { get; set; }
        public decimal AMOUNT { get; set; }
        public decimal VALUE { get; set; }
        public string REMARKS { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }
    }
}
