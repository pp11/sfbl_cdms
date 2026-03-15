using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.Target
{
    public class CUSOTMER_TARGET_MST
    {
        public CUSOTMER_TARGET_MST()
        {
            TARGET_DTLs = new HashSet<CUSTOMER_TARGET_DTL>();
        }

        public int MST_ID { get; set; }
        public string MST_ID_ENCRYPTED { get; set; }
        public string YEAR { get; set; }
        public string MONTH_CODE { get; set; }
        public string CUSTOMER_ID { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public int COMPANY_ID { get; set; }
        public int UNIT_ID { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }

        public ICollection<CUSTOMER_TARGET_DTL> TARGET_DTLs { get; set; }
    }
}
