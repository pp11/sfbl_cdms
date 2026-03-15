using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.Target
{
    public class IN_MARKET_SALES_MST: Base
    {
   
        public int MST_ID { get; set; }
        public string MST_ID_ENCRYPTED { get; set; }
        public string ENTRY_DATE { get; set; }
        public string YEAR { get; set; }
        public string MONTH_CODE { get; set; }
        public string MONTH_NAME { get; set; }
        public string MARKET_ID { get; set; }
        public string MARKET_NAME { get; set; }
        public string MARKET_CODE { get; set; }
        public string q { get; set; }

        public List<IN_MARKET_SALES_DTL> IN_MARKET_SALES_DTL { get ; set ; }



    }
}


