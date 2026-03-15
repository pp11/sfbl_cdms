using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Gift_Item_Receiving
    {
       
        public int RECEIVE_ID { get; set; }
        public int RECEIVED_BY_ID { get; set; }
        public int UNIT_ID { get; set; }
        public int GIFT_ITEM_ID { get; set; }
        public int SUPPLIER_ID { get; set; }
        public string RECEIVE_STATUS { get; set; }
        public string BATCH_NO { get; set; }
        public string CHALLAN_NO { get; set; }
        public string CHALLAN_DATE { get; set; }
        public string RECEIVE_DATE { get; set; }
        public int BATCH_ID { get; set; }
        public decimal GIFT_ITEM_PRICE { get; set; }
        public decimal RECEIVE_QTY { get; set; }
        public decimal RECEIVE_AMOUNT { get; set; }

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
        public string GIFT_ITEM_NAME { get; set; }

    }
}
