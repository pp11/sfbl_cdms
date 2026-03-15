using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem
{
    public class Process_data : Base
    {
        public string INVOICE_NO { get; set; }

        public int SKU_ID { get; set; }
        public string SKU_CODE { get; set; }
        public int RETURN_QTY { get; set; }
    }
}
