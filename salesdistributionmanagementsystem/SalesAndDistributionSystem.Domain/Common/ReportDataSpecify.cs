using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Common
{
    public class ReportDataSpecify
    {
        public int ReportId { get; set; }
        public string ReportIdEncrypt { get; set; }
        public string ReportHeaderTitle { get; set; }
        public string ReportDocumentTitle { get; set; }

        public string ReportName { get; set; }
        public string ReportArea { get; set; }
        public bool IsPDF { get; set; }
        public bool IsDoc { get; set; }
        public bool IsCSV { get; set; }
        public bool IsPreview { get; set; }

    }
}
