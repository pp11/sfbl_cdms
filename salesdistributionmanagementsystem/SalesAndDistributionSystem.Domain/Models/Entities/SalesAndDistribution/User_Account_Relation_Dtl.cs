using SalesAndDistributionSystem.Domain.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class User_Account_Relation_Dtl
    {
        
        public int USER_ACCOUNT_DTL_ID { get; set; }
        public string USER_TYPE { get; set; }
        public int USER_ACCOUNT_MST_ID { get; set; }
        public string EFFECT_END_DATE { get; set; }
        public string EFFECT_START_DATE { get; set; }
        public int ACCOUNT_ID { get; set; }
        public string ACCOUNT_CODE { get; set; }

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
        public string ACCOUNT_NAME { get; set; }

    }

    
   
}
