using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Bonus_Declare_Product
    {
        public int BASE_PRODUCT_ID { get; set; }
        public int BONUS_MST_ID { get; set; }
        public int BONUS_DECLARE_ID { get; set; }
        public int BRAND_ID { get; set; }
        public int CATEGORY_ID { get; set; }
        public int GROUP_ID { get; set; }
        public string SKU_CODE { get; set; }
        public string PACK_SIZE { get; set; }
        public string GROUP_NAME { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string BASE_PRODUCT_NAME { get; set; }
        public string BRAND_NAME { get; set; }
        public string SKU_NAME { get; set; }

        public int SKU_ID { get; set; }
        public string STATUS { get; set; }

       
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }


        //Helper Attribute--------
        public int ROW_NO { get; set; }

    }
}
