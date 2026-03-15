using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Region_Info
    {
        public int ROW_NO { get; set; }

        public int REGION_ID { get; set; }
        public string REGION_NAME { get; set; }
        public string REGION_CODE { get; set; }
        public string REGION_STATUS { get; set; }
        public string REGION_ADDRESS { get; set; }
        public string REMARKS { get; set; }
        public int COMPANY_ID { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }

    }
}
