using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Common
{
    public class ReportDataGenerator
    {
        public string DocumentTitle { get; set; }
        public string HtmlContentData { get; set; }
        public string PageOrientation { get; set; }
        public string PaperKind { get; set; }
        public string ColorMode { get; set; }

        public int MarginTop { get; set; }

    }
}
