using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.ReportModels.ProductReports
{
    public class ProductReport
    {
        public int BASE_PRODUCT_ID { get; set; }
        public int PRIMARY_PRODUCT_ID { get; set; }
        public int PRODUCT_SEASON_ID { get; set; }
        public int PRODUCT_TYPE_ID { get; set; }
        public int BRAND_ID { get; set; }
        public int STORAGE_ID { get; set; }
        public int SKU_ID { get; set; }
        public int UNIT_ID { get; set; }
        public int CATEGORY_ID { get; set; }
        public int COMPANY_ID { get; set; }
        public int GROUP_ID { get; set; }

        public string SKU_CODE { get; set; }
        public string SKU_NAME { get; set; }
        public string SKU_NAME_BANGLA { get; set; }
        public string FONT_COLOR { get; set; }
        public string PACK_SIZE { get; set; }
        public string PACK_UNIT { get; set; }
        public int PACK_VALUE { get; set; }
        public string PRODUCT_STATUS { get; set; }
        public decimal QTY_PER_PACK { get; set; }
        public decimal SHIPPER_QTY { get; set; }
        public string REMARKS { get; set; }
        public decimal SHIPPER_VOLUME { get; set; }
        public string SHIPPER_VOLUME_UNIT { get; set; }
        public decimal SHIPPER_WEIGHT { get; set; }
        public string SHIPPER_WEIGHT_UNIT { get; set; }
        public string WEIGHT_UNIT { get; set; }
        public decimal WEIGHT_PER_PACK { get; set; }


        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }


        //Helper Attributes
        public int ROW_NO { get; set; }

    }
}
