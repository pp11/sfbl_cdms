using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class SkuLoadingCharge
    {
        public int LOADING_CHARGE_ID { get; set; }
        public string SKU_CODE { get; set; }
        public string SKU_NAME { get; set; }
        public int SKU_ID { get; set; }

        public string PACK_SIZE { get; set; }
         
        public string ENTRY_DATE { get; set; }
        public string EFFECT_START_DATE { get; set; }
        public string EFFECT_END_DATE { get; set; }
        public decimal SHIPPER_QTY { get; set; }
        public decimal LOADING_CHARGE { get; set; }       
        public int UNIT_ID { get; set; }
        public int COMPANY_ID { get; set; }
        public string STATUS { get; set; }
        public string REMARKS { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }
    }
}
