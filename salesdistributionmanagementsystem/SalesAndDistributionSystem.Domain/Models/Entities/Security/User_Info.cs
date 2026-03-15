using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.TableModels.User
{
    public class User_Info
    {

        public int USER_ID { get; set; }
        public string USER_NAME { get; set; }
        public int COMPANY_ID { get; set; }
        public int UNIT_ID { get; set; }
        public string USER_TYPE { get; set; }
        public string EMPLOYEE_ID { get; set; }

        public string UNIQUEACCESSKEY { get; set; }

        public string USER_PASSWORD { get; set; }
        public string EMAIL { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_BY { get; set; }

        public string UPDATED_TERMINAL { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public string STATUS { get; set; }


    }
}

