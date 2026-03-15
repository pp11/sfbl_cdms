using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.ViewModels.Inventory
{
    public class CommissionDoneParams
    {
      public string Market_Code { get; set; }
      public string SKU_ID { get; set; }
      public string CUSTOMER_TYPE_ID { get; set; }
        public string CUSTOMER_STATUS { get; set; }

    }
}
