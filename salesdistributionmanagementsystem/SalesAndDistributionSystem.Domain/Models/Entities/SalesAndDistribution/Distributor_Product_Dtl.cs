using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Distributor_Product_Dtl
    {
        public int DTL_ID { get; set; }
        public int MST_ID { get; set; }
        public string SKU_ID { get; set; }
        public string SKU_CODE { get; set; }
        //Helper
        public List<string> UNIT_ID { get; set; }
        public string UNIT_NAME { get; set; }


        public int ROW_NO { get; set; }

    }
}
