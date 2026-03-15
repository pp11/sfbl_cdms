using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.ViewModels.Inventory
{
    public class PendingDeliveryInvoiceWithSKU
    {
        public string ORDER_NO { get; set; }
        public int ORDER_MST_ID { get; set; }
        public int MST_ID { get; set; }
        public string INVOICE_NO { get; set; }
        public int SKU_ID { get; set; }
        public string SKU_CODE { get; set; }
        public string SKU_NAME { get; set; }
        public string PACK_SIZE { get; set; }
        public decimal SHIPPER_QTY { get; set; }
        public decimal INVOICE_QTY { get; set; }
        public decimal BONUS_QTY { get; set; }
        public decimal TOTAL_QTY { get; set; }
        public int COMPANY_ID { get; set; }
        public int INVOICE_UNIT_ID { get; set; }
        public decimal PER_SHIPPER_WEIGHT { get; set; }

        public string SHIPPER_WEIGHT_UNIT { get; set; }
        public decimal PER_SHIPPER_VOLUME { get; set; }
        public string SHIPPER_VOLUME_UNIT { get; set; }
        public decimal PACK_VALUE { get; set; }
        public string PACK_UNIT { get; set; }
        public decimal WEIGHT_PER_PACK { get; set; }
        public string WEIGHT_UNIT { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public int CUSTOMER_ID { get; set; }
        public string CUSTOMER_CODE { get; set; }




    }
}
