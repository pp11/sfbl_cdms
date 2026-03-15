using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.Inventory
{
    public class Stock_Adjustment
    {


        public int ROW_NO { get; set; }
        public int ADJUSTMENT_ID { get; set; }
        public string ADJUSTMENT_DATE { get; set; }
        public int UNIT_ID { get; set; }
        public string UNIT_NAME { get; set; }
        public string STOCK_TYPE { get; set; }
        public string ADJUSTMENT_TYPE { get; set; }
        public int SKU_ID { get; set; }
        public string SKU_CODE { get; set; }
        public string SKU_NAME { get; set; }
        public int UNIT_TP { get; set; }
        public int BATCH_ID { get; set; }
        public string BATCH_NO { get; set; }
        public int ADJUSTMENT_QTY { get; set; }
        public int ADJUSTMENT_AMOUNT { get; set; }
        public string REMARKS { get; set; }
        public string STATUS { get; set; }
        public int COMPANY_ID { get; set; }
        public string COMPANY_NAME { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_BY_NAME { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_NAME { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }


    }
}
