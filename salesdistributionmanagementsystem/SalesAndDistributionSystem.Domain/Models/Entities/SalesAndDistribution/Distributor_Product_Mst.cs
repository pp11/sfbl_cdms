using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Distributor_Product_Mst
    {
        public int ROW_NO { get; set; }
        public int MST_ID { get; set; }
        public string DISTRIBUTOR_PRODUCT_TYPE { get; set; }
        public List<Distributor_Product_Dtl> distributor_Product_Dtls { get; set; }
        public string MST_ID_ENCRYPTED { get; set; }
        public string q { get; set; }
    }
}
