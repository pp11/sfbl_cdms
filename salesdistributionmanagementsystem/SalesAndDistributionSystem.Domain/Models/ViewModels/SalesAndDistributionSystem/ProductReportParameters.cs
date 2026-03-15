using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem
{
    public class ProductReportParameters
    {
        public string DATE_FROM { get; set; } = "'01/01/2020'";

        public string DATE_TO { get; set; } = "'01/01/2040'";
        public string DateRangeSelectionText { get; set; }

        public string SKU_ID { get; set; }

        public string BRAND_ID { get; set; }
        public string CATEGORY_ID { get; set; }
        public string BASE_PRODUCT_ID { get; set; }
        public string PRIMARY_PRODUCT_ID { get; set; }
        public string PRODUCT_SEASON_ID { get; set; }
        public string PRODUCT_TYPE_ID { get; set; }

        public string UNIT_ID { get; set; }
        public string GROUP_ID { get; set; }
        public int COMPANY_ID { get; set; }

    }
}
