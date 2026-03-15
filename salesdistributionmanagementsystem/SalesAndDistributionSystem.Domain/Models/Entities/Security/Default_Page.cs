using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.TableModels.Security
{
    public class Default_Page
    {
        public int ID { get; set; }
        public int COMPANY_ID { get; set; }
        public int USER_ID { get; set; }
        public string USER_NAME { get; set; }

        public int MENU_ID { get; set; }
        public string MENU_NAME { get; set; }
        
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }

        
        







    }
}
