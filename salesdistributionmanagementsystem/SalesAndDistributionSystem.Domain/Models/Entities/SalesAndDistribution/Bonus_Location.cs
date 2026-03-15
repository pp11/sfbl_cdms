using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Bonus_Location
    {
        
        public int BONUS_LOCATION_ID { get; set; }
        public int BONUS_MST_ID { get; set; }

        public string LOCATION_CODE { get; set; }
        public int LOCATION_ID { get; set; }
        public string LOCATION_TYPE { get; set; }
        public string STATUS { get; set; }

       
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }


        //Helper Attribute--------
        public int ROW_NO { get; set; }
        public string LOCATION_NAME { get; set; }

    }
}
