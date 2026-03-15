using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Region_Area_Dtl
    {
   
        public int REGION_AREA_DTL_ID { get; set;}
        public string REGION_AREA_DTL_STATUS { get; set; }
        public int REGION_AREA_MST_ID { get; set; }
        public string EFFECT_END_DATE { get; set; }
        public string EFFECT_START_DATE { get; set; }
        public int AREA_ID { get; set; }
        public string AREA_CODE { get; set; }
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
        public string AREA_NAME { get; set; }

    }
}
