using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class SKU_DEPOT_RELATION
    {
    
   
        public int SKU_DEPO_ID { get; set; }
        public int SKU_ID { get; set; }
        public string SKU_CODE { get; set; }
        public int DEPOT_ID { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public int COMPANY_ID { get; set; }

        //Helper Attributes
        public int ROW_NO { get; set; }

    }
}
