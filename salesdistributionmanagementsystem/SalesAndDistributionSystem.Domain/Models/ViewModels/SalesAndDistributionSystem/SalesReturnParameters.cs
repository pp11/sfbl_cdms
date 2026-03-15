using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem
{
    public class SalesReturnParameters : Base
    {
        public string INVOICE_NO { get; set; }
        public string INVOICE_DATE { get; set; }
        public int RETURN_UNIT_ID { get; set; }
        public int CUSTOMER_ID { get; set; }

        public string CUSTOMER_CODE { get; set; }

    }
}
