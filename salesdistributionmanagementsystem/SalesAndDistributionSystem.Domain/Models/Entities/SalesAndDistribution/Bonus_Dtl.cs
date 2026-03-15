using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Bonus_Dtl
    {    
        public int BONUS_DTL_ID { get; set; }
        public int BONUS_MST_ID { get; set; }

        public decimal BONUS_QTY { get; set; }
        public string BONUS_SKU_CODE { get; set; }
        public int BONUS_SKU_ID { get; set; }
        public string BONUS_SLAB_TYPE { get; set; }
        public string BONUS_TYPE { get; set; }
        public string CALCULATION_TYPE { get; set; }
        public decimal DISCOUNT_PCT { get; set; }
        public decimal DISCOUNT_VAL { get; set; }
        public int GIFT_ITEM_ID { get; set; }
        public decimal GIFT_QTY { get; set; }
        public decimal SLAB_QTY { get; set; }
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
