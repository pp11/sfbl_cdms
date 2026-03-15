using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Customer_Info
    {
        public int CUSTOMER_ID { get; set; }
        public int UNIT_ID { get; set; }
        public int DB_LOCATION_ID { get; set; }
        public string DB_LOCATION_NAME { get; set; }
        public int CUSTOMER_TYPE_ID { get; set; }
        public string CUSTOMER_STATUS { get; set; }
        public string CUSTOMER_REMARKS { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_CONTACT { get; set; }
        public string CUSTOMER_EMAIL { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string CUSTOMER_NAME_BANGLA { get; set; }

        public int COMPANY_ID { get; set; }
        public string CONTACT_PERSON_NAME { get; set; }
        public string CONTACT_PERSON_NO { get; set; }
        public string CUSTOMER_ADDRESS { get; set; }
        public string CUSTOMER_ADDRESS_BANGLA { get; set; }
        public string BIN_NO { get; set; }
        public string CLOSING_DATE { get; set; }

        public string DELIVERY_ADDRESS { get; set; }
        public string DELIVERY_ADDRESS_BANGLA { get; set; }
        public string OPENING_DATE { get; set; }
        public int PRICE_TYPE_ID { get; set; }
        public string PROPRIETOR_NAME { get; set; }
        public decimal SECURITY_MONEY { get; set; }
        public string TDS_FLAG { get; set; }
        public string TIN_NO { get; set; }
        public string TRADE_LICENSE_NO { get; set; }
        public string VAT_REG_NO { get; set; }
        //public int ROUTE_ID { get; set; }
        //public int SERIAL_NO { get; set; }

        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }
        public string DISTRIBUTOR_PRODUCT_TYPE { get; set; }
        public decimal SUGGEST_PERCENT { get; set; }
        public decimal MAXIMUM_PERCENT { get; set; }
        public decimal MINIMUM_PERCENT { get; set; }


        public List<Customer_Route_Relation> Customer_Route_Relations { get; set; }
        //Helper Attributes
        public int ROW_NO { get; set; }

    }
}
