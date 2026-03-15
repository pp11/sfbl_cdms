using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Customer_Route_Relation
    {
        public int ID { get; set; }
        public string ROUTE_ID { get; set; }
        public int CUSTOMER_ID { get; set; }
        public int SL_NO { get; set; }
    }
}
