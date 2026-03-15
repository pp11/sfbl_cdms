using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.TableModels.Security
{
    public class EMPLOYEE_INFO
    {
        public int ID { get; set; }
        public int EMPLOYEE_ID { get; set; }
        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_NAME { get; set; }
        public string EMPLOYEE_STATUS { get; set; }
        public int COMPANY_ID { get; set; }
        public int UNIT_ID { get; set; }


    }
}
