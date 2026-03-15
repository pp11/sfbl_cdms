using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.Inventory
{
    public class DEPOT_DISPATCH_MST : Base
    {
        public int MST_ID { get; set; }
        public string MST_ID_ENCRYPTED { get; set; }
        public string DISPATCH_NO { get; set; }
        public string DISPATCH_DATE { get; set; }
        public string VEHICLE_NO { get; set; }
                      
        public string VEHICLE_DESCRIPTION { get; set; }
        public double VECHILE_VOLUME { get; set; }
        public double VECHILE_WEIGHT { get; set; }
        public double DISPATCH_VOLUME { get; set; }
        public double DISPATCH_WEIGHT { get; set; }
        public string DRIVER_ID { get; set; }
        public string DISPATCH_BY { get; set; }
        public int DISPATCH_UNIT_ID { get; set; }
     
        public string REMARKS { get; set; }
        public string DISPATCH_TYPE { get; set; }
       
      
        public string q { get; set; }

        public List<DEPOT_DISPATCH_REQUISITION> requisitionIssueDtlList { get; set; }
    }
}
