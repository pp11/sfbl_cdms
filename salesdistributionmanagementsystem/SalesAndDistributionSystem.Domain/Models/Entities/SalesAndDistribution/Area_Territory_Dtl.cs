using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Area_Territory_Dtl
    {

        public int AREA_TERRITORY_DTL_ID { get; set; }
        public string AREA_TERRITORY_DTL_STATUS { get; set; }
        public int AREA_TERRITORY_MST_ID { get; set; }
        public string EFFECT_END_DATE { get; set; }
        public string EFFECT_START_DATE { get; set; }
        public int TERRITORY_ID { get; set; }
        public string TERRITORY_CODE { get; set; }
        public string REMARKS { get; set; }
        public int COMPANY_ID { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }


        //Helper Data Attributes

        public int ROW_NO { get; set; }
        public string IS_Deleted { get; set; }
        public string TERRITORY_NAME { get; set; }

    }
}
