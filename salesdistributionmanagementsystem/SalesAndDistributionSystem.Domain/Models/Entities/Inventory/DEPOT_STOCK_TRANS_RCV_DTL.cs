using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.Inventory
{
    public class DEPOT_STOCK_TRANS_RCV_DTL
    {
        public int MST_ID { get; set; }
        public int DTL_ID { get; set; }
        public string SKU_ID { get; set; }
        public string SKU_NAME { get; set; }
        public string SKU_CODE { get; set; }
        public double UNIT_TP { get; set; }
        public int TRANSFER_QTY { get; set; }
        public int STOCK_QTY { get; set; }
        public double TRANSFER_AMOUNT { get; set; }
        public double TRANS_RCV_QTY { get; set; }
        public double TRANS_RCV_AMOUNT { get; set; }
        public string STATUS { get; set; }

        public int COMPANY_ID { get; set; }

        public string REMARKS { get; set; }

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
