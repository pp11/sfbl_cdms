using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.TableModels.Security
{
    public class Role_User_Configuration
    {
        public int ID { get; set; }
        public int ROW_NO { get; set; }

        public int COMPANY_ID { get; set; }
        public int ROLE_ID { get; set; }
        public int USER_ID { get; set; }
        public int USER_CONFIG_ID { get; set; }
        public string ROLE_NAME { get; set; }

        public string IS_PERMITTED { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string PERMITTED_BY { get; set; }
        public string PERMITE_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }


    }
}
