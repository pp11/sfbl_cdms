using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.TableModels.User
{
    public class User_Log : Base
    {

        public int USER_ID { get; set; }
        public int COMPANY_ID { get; set; }
        public string ACTIVITY_TABLE { get; set;}
        public string ACTIVITY_TYPE { get; set; }
        public string DTL { get; set; }
        public string LOCATION { get; set; }
        public string LOG_DATE { get; set; }
        public int LOG_ID { get; set; }

        public int UNIT_ID { get; set; }
        public int TRANSACTION_ID { get; set; }
        public string PAGE_REF { get; set; }
        public string USER_TERMINAL { get; set; }

       
       

    }
}
