using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Bonus_Mst
    {

        public int BONUS_MST_ID { get; set; }
        public string BONUS_NAME { get; set; }
        public int COMPANY_ID { get; set; }
        public string EFFECT_END_DATE { get; set; }
        public string EFFECT_START_DATE { get; set; }
        public string ENTRY_DATE { get; set; }
        public string LOCATION_TYPE { get; set; }
        public string REMARKS { get; set; }
        public string STATUS { get; set; }
        public int UNIT_ID { get; set; }

        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }


        //Helper Attribute--------
        public int ROW_NO { get; set; }
        public List<Bonus_Declare_Product> BonusDeclareProduct { get; set; }
        public List<Bonus_Dtl> Bonus_Dtls { get; set; }
        public List<Bonus_Location> Bonus_Locations { get; set; }
        public string q { get; set; }
        public string BONUS_MST_ID_ENCRYPTED  { get; set; }

        
    }
}
