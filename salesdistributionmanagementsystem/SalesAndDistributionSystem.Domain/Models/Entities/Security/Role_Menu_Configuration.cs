using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.TableModels.Security
{
    public  class Role_Menu_Configuration
    {
        public int ID { get; set; }
        public int ROLE_ID { get; set; }
        public int MENU_ID { get; set; }
        public string LIST_VIEW { get; set; }
        public string ADD_PERMISSION { get; set; }
        public string EDIT_PERMISSION { get; set; }
        public string DELETE_PERMISSION { get; set; }
        public string DETAIL_VIEW { get; set; }
        public string DOWNLOAD_PERMISSION { get; set; }
        public string CONFIRM_PERMISSION { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }
        public int COMPANY_ID { get; set; }
        public int ROLE_CONFIG_ID { get; set; }

        


    }
}
