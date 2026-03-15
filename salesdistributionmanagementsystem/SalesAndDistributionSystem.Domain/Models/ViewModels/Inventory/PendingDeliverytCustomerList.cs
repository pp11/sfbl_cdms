using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.ViewModels.Inventory
{
    public class PendingDeliverytCustomerList
    {
        public int COMPANY_ID { get; set; }
        public int INVOICE_UNIT_ID { get; set; }
        public int CUSTOMER_ID { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string MARKET_CODE { get; set; }
        public int ROUTE_ID { get; set; }
        public string INVOICE_NO { get; set; }
        public string INVOICE_DATE { get; set; }
        public int SERIAL_NO { get; set; }
        public string NET_INVOICE_AMOUNT { get; set; }
        public string NET_INVOICE_QTY { get; set; }


    }
}
