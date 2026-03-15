using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.Inventory
{
    public class DEPOT_REQ_DISTRIBUTION_MST
    {
        public int MST_ID { get; set; }
        public string MST_ID_ENCRYPTED { get; set; }
        public string DISTRIBUTION_NO { get; set; }
        public string DISTRIBUTION_DATE { get; set; }
        public string VEHICLE_NO { get; set; }
        public string VEHICLE_DESCRIPTION { get; set; }
        public int VEHICLE_TOTAL_VOLUME { get; set; }
        public int VEHICLE_TOTAL_WEIGHT { get; set; }
        public string DRIVER_ID { get; set; }
        public string DISTRIBUTION_BY { get; set; }
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

        public List<DEPOT_REQ_DISTRIBUTION_REQ> requisitionIssueDtlList { get; set; }



    }
}
