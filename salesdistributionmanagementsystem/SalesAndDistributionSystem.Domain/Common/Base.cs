using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Base
    {
        public int ROW_NO { get; set; }
        public int COMPANY_ID { get; set; }
        public int UNIT_ID { get; set; }

        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }
        public string db_sales { get; set; }
        public string db_security { get; set; }
        public string USER_TYPE { get; set; }


    }
}
