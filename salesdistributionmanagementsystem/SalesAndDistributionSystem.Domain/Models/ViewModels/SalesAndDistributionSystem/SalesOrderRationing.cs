using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem
{
    public class SalesOrderRationing : Base
    {
     
        public int ORDER_MST_ID { get; set; } 
        public string ORDER_DATE { get; set; } 
        public string ORDER_NO { get; set; }
        public int CUSTOMER_ID { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public int SKU_ID { get; set; }
        public string SKU_CODE { get; set; }
        public string SKU_NAME { get; set; }
        public string PACK_SIZE { get; set; }
        public decimal STOCK_QTY { get; set; }
        public decimal ORDER_QTY { get; set; }
        public decimal SUGGESTED_LIFTING { get; set; }
        public decimal ORDER_AMOUNT { get; set; }
        public decimal REVISED_QTY { get; set; }
        public int ADJ_TYPE { get; set; }
        public decimal ADJ_AMT { get; set; }

        
    }
}
