using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Product_Type_Info
    {

        public int PRODUCT_TYPE_ID { get;set;}
        public string PRODUCT_TYPE_NAME { get;set;}
        public string PRODUCT_TYPE_CODE { get; set; }
        public string STATUS { get; set; }
        public string REMARKS { get; set; }
        public int COMPANY_ID { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }


        //Helper Attributes
        public int ROW_NO { get; set; }

    }
}
