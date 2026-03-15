using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Depot_Customer_Mst
    {
        public int MST_ID { get; set; }
        public int DEPOT_ID { get; set; }
        public string DEPOT_CODE { get; set; }
        public string DEPOT_NAME { get; set; }
        public string EFFECT_START_DATE { get; set; }
        public string EFFECT_END_DATE { get; set; }
        public string STATUS { get; set; }
        public int COMPANY_ID { get; set; }
        public string REMARKS { get; set; }

        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }

        //Helper Attribute--------
        public int ROW_NO { get; set; }
        public string MST_ID_ENCRYPTED { get; set; }
        public List<Depot_Customer_Dtl> Depot_Customer_Dtls { get; set; }
        public string q { get; set; }

    }
}


