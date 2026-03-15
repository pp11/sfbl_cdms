using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class BATCH_PRICE_MST
    {
        public string MST_ID_ENCRYPTED { get; set; }
        public int MST_ID { get; set; }
        public int COMPANY_ID { get; set; }
        public int UNIT_ID { get; set; }
        public string SKU_CODE { get; set; }
        public string SKU_ID { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string ENTRY_DATE { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_TERMINAL { get; set; }
        public List<BATCH_PRICE_DTL> BATCH_PRICE_DTL_LIST { get; set; }
        //Helper Property
        public List<string> UNIT_ID_MULTIPLE { get; set; }
        
        [NotMapped]
        public string BATCH_NO { get; set; }


    }

}
