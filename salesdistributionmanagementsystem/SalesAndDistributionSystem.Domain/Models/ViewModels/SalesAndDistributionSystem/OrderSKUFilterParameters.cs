using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem
{
    public class OrderSKUFilterParameters : Base
    {
        public string DATE_FROM { get; set; } = "01/01/2020";
        public string DATE_TO { get; set; } = "01/01/2040";
        public string DIVISION_ID { get; set; }
        public string REGION_ID { get; set; }
        public string AREA_ID { get; set; }
        public string TERRITORY_ID { get; set; }
        public string CUSTOMER_ID { get; set; }
        public string ORDER_STATUS { get; set; }
        public string ORDER_TYPE { get; set; }
        public string ORDER_ENTRY_TYPE { get; set; }
        public string IsDistributor { get; set; }
        public string IsOSM { get; set; }

        public string CUSTOMER_CODE { get; set; }
        




    }
}
