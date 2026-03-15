using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalesAndDistributionSystem.Reporting.Models
{
    public class Invoice
    {
        public string COMPANY_ID { get; set; }
        public string INVOICE_UNIT_ID { get; set; }
        public string CUSTOMER_ID { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string CUSTOMER_ADDRESS { get; set; }
        public string MARKET_CODE { get; set; }
        public string INVOICE_NO { get; set; }
        public string INVOICE_DATE { get; set; }
        public string INVOICE_TYPE_NAME { get; set; }
        public string SKU_ID { get; set; }
        public string SKU_NAME { get; set; }
        public string SKU_CODE { get; set; }
        public string PACK_SIZE { get; set; }
        public string INVOICE_QTY { get; set; }
        public string RETURN_INVOICE_QTY { get; set; }
        public string NET_INVOICE_QTY { get; set; }
        public string DISTRIBUTION_INVOICE_QTY { get; set; }
        public string PENDING_INVOICE_DIST_QTY { get; set; }
        public string BONUS_QTY { get; set; }
        public string RETURN_BONUS_QTY { get; set; }
        public string NET_BONUS_QTY { get; set; }
        public string DISTRIBUTION_BONUS_QTY { get; set; }
        public string PENDING_BONUS_DIST_QTY { get; set; }
    }
}