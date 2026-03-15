using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class User_Account_Relation_Mst
    {
        
        public int USER_ID { get; set; }
        public string USER_CODE { get; set; }
        public string REMARKS { get; set; }
        public int COMPANY_ID { get; set; }
        public int USER_ACCOUNT_MST_ID { get; set; }
        public string USER_TYPE { get; set; }
        public string EFFECT_END_DATE { get; set; }
        public string EFFECT_START_DATE { get; set; }

        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }
        public List<User_Account_Relation_Dtl> user_account_relation_Dtls { get; set; }


        //------Helper Attributes--------------
        public int ROW_NO { get; set; }
        public string USER_NAME { get; set; }
        public string USER_ACCOUNT_MST_ID_ENCRYPTED { get; set; }
        public string q { get; set; }

    }
}
