using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.TableModels.Security
{
    public class Module_Info
    {
        public int MODULE_ID { get; set; }
        public string MODULE_NAME { get; set; }
        public string STATUS { get; set; }
        public int ORDER_BY_NO { get; set; }
        public int COMPANY_ID { get; set; }

        public string ENTERED_TERMINAL { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_BY { get; set; }

        public string UPDATED_TERMINAL { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }


    }

}
