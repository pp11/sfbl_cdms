using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class AdjustmentMst
    {
        public int COMPANY_ID { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UNIT_ID { get; set; }
        public List<AdjustmentDtl> Adjustment { get; set; }
    }

    public class AdjustmentDtl
    {
        public int CUSTOMER_ID { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string EFFECT_START_DATE { get; set; }
        public string EFFECT_END_DATE { get; set; }
        public string REMARKS { get; set; }
        public Decimal AMOUNT { get; set; }

    }
}
