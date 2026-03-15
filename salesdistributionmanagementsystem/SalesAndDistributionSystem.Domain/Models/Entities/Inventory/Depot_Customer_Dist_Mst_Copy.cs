using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.Inventory
{
    public class Depot_Customer_Dist_Mst_Copy
    {
        public int MST_ID { get; set; }
        public string DISTRIBUTION_NO { get; set; }
        public string DISTRIBUTION_DATE { get; set; }
        ////-------------------------
        //public int DIST_ROUTE_ID { get; set; }
        public string VEHICLE_NO { get; set; }
        public string VEHICLE_DESCRIPTION { get; set; }
        ////public decimal VEHICLE_TOTAL_VOLUME { get; set; }
        ////public decimal VEHICLE_TOTAL_WEIGHT { get; set; }
        public string DRIVER_ID { get; set; } //problem
        public string DRIVER_PHONE { get; set; }
        public string DISTRIBUTION_BY { get; set; }
        public string STATUS { get; set; }
        public int COMPANY_ID { get; set; }
        ////public int INVOICE_UNIT_ID { get; set; }
        ////public string REMARKS { get; set; }


        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }
        public decimal TOTAL_VOLUMN { get; set; }
        public decimal TOTAL_WEIGHT { get; set; }
        public int CONFIRMED { get; set; }

        //Helper Attribute--------
        public string MST_ID_ENCRYPTED { get; set; }
        public string WEIGHT_UNIT { get; set; }
        public string VOLUME_UNIT { get; set; }
        public string DRIVER_NAME { get; set; }
        public int ROW_NO { get; set; }
        public List<Depot_Customer_Dist_Invoice_Copy> Invoices { get; set; }
    }
}
