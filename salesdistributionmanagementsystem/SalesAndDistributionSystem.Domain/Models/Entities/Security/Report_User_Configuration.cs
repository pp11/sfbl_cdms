using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.Security
{
    public class Report_User_Configuration
    {
        public int ID { get; set; }
        public int USER_ID { get; set; }
        public int REPORT_ID { get; set; }
        public string PDF_PERMISSION { get; set; }
        public string CSV_PERMISSION { get; set; }
        public string PREVIEW_PERMISSION { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }
        public int COMPANY_ID { get; set; }
    }
}
