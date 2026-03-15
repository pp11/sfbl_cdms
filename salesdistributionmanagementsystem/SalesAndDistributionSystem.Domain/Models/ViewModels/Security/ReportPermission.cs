using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.ViewModels.Security
{
    public class ReportPermission
    {
        public int REPORT_ID { get; set; }

        public string REPORT_NAME { get; set; }
        public string REPORT_TITLE { get; set; }

        public int MENU_ID { get; set; }
        public string MENU_NAME { get; set; }
        public string MENU_ACTION { get; set; }

        public int ORDER_BY_SLNO { get; set; }
        public string PDF_PERMISSION { get; set; }
        public string PREVIEW_PERMISSION { get; set; }
        public string CSV_PERMISSION { get; set; }


        //Helper Attribute
        public string ReportIdEncrypt { get; set; }


    }
}
