using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.Inventory
{
    public class Depot_Customer_Dist_Invoice_Copy
    {
        public int DEPOT_INV_ID { get; set; }
        public int MST_ID { get; set; }
        public string INVOICE_NO { get; set; }
        public string INVOICE_DATE { get; set; }
        public int INVOICE_UNIT_ID { get; set; }
        public int CUSTOMER_ID { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public int DIST_ROUTE_ID { get; set; }
        public string STATUS { get; set; }
        public int COMPANY_ID { get; set; }


        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }


        //Helper Attribute--------
        public int ROW_NO { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public List<Depot_Customer_Dist_Product_Copy> Products { get; set; }
        public List<Depot_Customer_Dist_Gift_Batch_Copy> Gift_Batches { get; set; }
    }
}
