using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.ViewModels.Security
{
    public class UserMenuConfigView
    {
        public int ROW_NO { get; set; }

        public int MENU_ID { get; set; }
        public string MENU_NAME { get; set; }
        public string HREF { get; set; }
        public int PARENT_MENU_ID { get; set; }
        public string PARENT_MENU_NAME { get; set; }
        public string PARENT_MENU_HREF { get; set; }
        public int MODULE_ID { get; set; }
        public string MODULE_NAME { get; set; }
        public int USER_CONFIG_ID { get; set; }
        public int USER_ID { get; set; }
        public string LIST_VIEW { get; set; }
        public string ADD_PERMISSION { get; set; }
        public string EDIT_PERMISSION { get; set; }
        public string DELETE_PERMISSION { get; set; }
        public string DETAIL_VIEW { get; set; }
        public string DOWNLOAD_PERMISSION { get; set; }
        public string CONFIRM_PERMISSION { get; set; }
        public string SEQUENCE { get; set; }

    }
}
