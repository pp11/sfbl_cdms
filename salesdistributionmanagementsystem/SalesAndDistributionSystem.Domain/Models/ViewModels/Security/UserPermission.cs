using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.ViewModels.Security
{
    public class UserPermission
    {
        public string Controller_Name { get; set; }
        public string Action_Name { get; set; }

        public string LIST_VIEW { get; set; }

        public string ADD_PERMISSION { get; set; }
        public string EDIT_PERMISSION { get; set; }
        public string DELETE_PERMISSION { get; set; }
        public string DETAIL_VIEW { get; set; }
        public string DOWNLOAD_PERMISSION { get; set; }
    }
}
