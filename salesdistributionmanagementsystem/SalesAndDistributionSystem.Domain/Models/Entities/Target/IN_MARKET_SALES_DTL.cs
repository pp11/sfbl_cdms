using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.Target
{
    public class IN_MARKET_SALES_DTL
    {
        public string DTL_ID { get; set; }
        public string MST_ID { get; set; }
        public string SKU_ID { get; set; }
        public string SKU_CODE { get; set; }
        public Decimal UNIT_TP { get; set; }
        public string PACK_SIZE { get; set; }
        public Decimal MRP { get; set; }
        public Decimal SALES_QTY { get; set; }
        public Decimal SALES_VALUE { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }
        public string q { get; set; }
        public int ROW_NO { get; set; }

        public string ENTRY_DATE { get; set; }
        public string YEAR { get; set; }
        public string MONTH_CODE { get; set; }
        public string MARKET_ID { get; set; }
        public string MARKET_CODE { get; set; }

    }
}
