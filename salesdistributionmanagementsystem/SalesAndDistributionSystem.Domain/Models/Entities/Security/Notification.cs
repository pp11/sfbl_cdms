using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.TableModels.Company
{
    public class Notification
    {
        public int ID { get; set; }

        public int NOTIFICATION_ID { get; set; }
        public int COMPANY_ID { get; set; }
        public int UNIT_ID { get; set; }
        public int USER_ID { get; set; }
        public int NOTIFICATION_POLICY_ID { get; set; }
        public string NOTIFICATION_TITLE { get; set; }

        public string NOTIFICATION_BODY { get; set; }
        public string NOTIFICATION_DATE { get; set; }
        public string STATUS { get; set; }

        public int ROWNUM { get; set; }
       



    }
}
