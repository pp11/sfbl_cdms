using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Territory_Market_Mst
    {

        public int TERRITORY_ID { get; set; }
        public string TERRITORY_CODE { get; set; }
        public string REMARKS { get; set; }
        public int COMPANY_ID { get; set; }
        public int TERRITORY_MARKET_MST_ID { get; set; }
        public string TERRITORY_MARKET_MST_STATUS { get; set; }
        public string EFFECT_END_DATE { get; set; }
        public string EFFECT_START_DATE { get; set; }

        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }
        public List<Territory_Market_Dtl> territory_Market_Dtls { get; set; }


        //------Helper Attributes--------------
        public int ROW_NO { get; set; }
        public string TERRITORY_NAME { get; set; }
        public string TERRITORY_MARKET_MST_ID_ENCRYPTED { get; set; }
        public string q { get; set; }

    }
}
