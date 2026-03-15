using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    internal class AdjustmentInfo
    {
        public int ADJUSTMENT_ID { get; set; }
        public string ADJUSTMENT_NAME { get; set; }
        public string ADJUSTMENT_STATUS { get; set; }
        public string COMPANY_ID { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }
    }
}
