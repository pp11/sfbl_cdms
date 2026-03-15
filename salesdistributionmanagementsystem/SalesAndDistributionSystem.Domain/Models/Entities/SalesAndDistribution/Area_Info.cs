using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Area_Info
    {
        public int ROW_NO { get; set; }

        public int AREA_ID { get; set; }
        public string AREA_NAME { get; set; }
        public string AREA_CODE { get; set; }
        public string AREA_STATUS { get; set; }
        public string AREA_ADDRESS { get; set; }
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
