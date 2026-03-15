using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.ViewModels.Security
{
    public class PermittedMenu
    {
        public int MENU_ID { get; set; }
        public string MENU_NAME { get; set; }
        public int MODULE_ID { get; set; }
        public string AREA { get; set; }

        public string CONTROLLER { get; set; }
        public string ACTION { get; set; }
        public string MENU_SHOW { get; set; }

        public string HREF { get; set; }
        public int PARENT_MENU_ID { get; set; }
        public int ORDER_BY_SLNO { get; set; }
        public string LIST_VIEW { get; set; }
        public string ADD_PERMISSION { get; set; }
        public string EDIT_PERMISSION { get; set; }
        public string DELETE_PERMISSION { get; set; }
        public string CONFIRM_PERMISSION { get; set; }
        public string DETAIL_VIEW { get; set; }
        public string DOWNLOAD_PERMISSION { get; set; }
        public string USER_TYPE { get; set; }
    }
}
