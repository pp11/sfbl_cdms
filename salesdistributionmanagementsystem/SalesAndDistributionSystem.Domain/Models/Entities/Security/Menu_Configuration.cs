using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.TableModels.Security
{
    public class Menu_Configuration
    {
        public int MENU_ID { get; set; }
        public string MENU_NAME { get; set; }
        public int MODULE_ID { get; set; }
        public string CONTROLLER { get; set; }
        public string AREA { get; set; }

        public string ACTION { get; set; }
        public string HREF { get; set; }
        public string STATUS { get; set; }
        public string MENU_SHOW { get; set; }

        public int PARENT_MENU_ID { get; set; }
        public int ORDER_BY_SLNO { get; set; }
        public int COMPANY_ID { get; set; }

        public string ENTERED_TERMINAL { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_BY { get; set; }

        public string UPDATED_TERMINAL { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }

    }
}
