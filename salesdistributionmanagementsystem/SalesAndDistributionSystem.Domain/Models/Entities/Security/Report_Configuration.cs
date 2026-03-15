using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.Security
{
    public class Report_Configuration
    {
        public int REPORT_ID { get; set; }
        public string REPORT_NAME { get; set; }
        public string REPORT_TITLE { get; set; }

        public string STATUS { get; set; }
        public string HAS_CSV { get; set; }
        public string HAS_PDF { get; set; }
        public string HAS_PREVIEW { get; set; }

        public int MENU_ID { get; set; }
        public int ORDER_BY_SLNO { get; set; }
        public int COMPANY_ID { get; set; }


        public string ENTERED_TERMINAL { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_BY { get; set; }

        public string UPDATED_TERMINAL { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }

    }
}
