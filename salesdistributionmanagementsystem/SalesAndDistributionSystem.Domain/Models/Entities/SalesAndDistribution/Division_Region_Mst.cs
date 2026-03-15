using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Division_Region_Mst
    {

        public int DIVISION_ID { get; set; }
        public string DIVISION_CODE { get; set; }
        public string REMARKS { get; set; }
        public int COMPANY_ID { get; set; }
        public int DIVISION_REGION_MST_ID { get; set; }
        public string DIVISION_REGION_MST_STATUS { get; set; }
        public string EFFECT_END_DATE { get; set; }
        public string EFFECT_START_DATE { get; set; }

        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }
        public List<Division_Region_Dtl> division_Region_Dtls { get; set; }


        //------Helper Attributes--------------
        public int ROW_NO { get; set; }
        public string DIVISION_NAME { get; set; }
        public string DIVISION_REGION_MST_ID_ENCRYPTED { get; set; }
        public string q { get; set; }

    }
}
