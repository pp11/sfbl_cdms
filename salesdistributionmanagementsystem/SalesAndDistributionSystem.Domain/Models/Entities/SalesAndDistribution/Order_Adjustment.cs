using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Order_Adjustment
    {
       public int ID { get; set; }

        public int ADJUSTMENT_ID { get; set; }
        public int ORDER_MST_ID { get; set; }
        public string ORDER_NO { get; set; }
        public int ORDER_UNIT_ID { get; set; }
        public decimal ADJUSTMENT_AMOUNT { get; set; }

        public string REMARKS { get; set; }
        public int COMPANY_ID { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }

        //Helper Attribute
        public int ROW_NO { get; set; }

        public string MONTH_NUMBER { get; set; }
        public string YEAR_NUMBER { get; set; }

    }
}
