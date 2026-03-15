using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Fg_Reciving_From_Production
    {
        public int BATCH_ID { get; set; }
        public string BATCH_NO { get; set; }
        
        public int CHECKED_BY_ID { get; set; }

        public string CHECKED_BY_DATE { get; set; }
        public int COMPANY_ID { get; set; }
        public int RECEIVED_BY_ID { get; set; }
        public int RECEIVE_ID { get; set; }

        public string EXPIRY_DATE { get; set; }
        public string MFG_DATE { get; set; }
        public string PACK_SIZE { get; set; }
        public int SKU_ID  { get; set; }
        public int UNIT_ID { get; set; }

        public string RECEIVE_DATE { get; set; }
        public string RECEIVE_STATUS { get; set; }
        public decimal RECEIVE_AMOUNT { get; set; }
        public decimal RECEIVE_QTY { get; set; }
        public string RECEIVE_STOCK_TYPE { get; set; }
        public string REMARKS { get; set; }
        public decimal SHIPPER_QTY { get; set; }
        public string SKU_CODE { get; set; }
        public string TRANSFER_DATE { get; set; }
        public string TRANSFER_NOTE_NO { get; set; }
        public string TRANSFER_TYPE { get; set; }
        public decimal? UNIT_TP { get; set; }
        public decimal MRP { get; set; }
        public decimal TRANSFER_QTY { get; set; }

        
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }
        public string BATCH_PRICE_REVIEW_STATUS { get; set; }


        //Helper Attributes
        public int ROW_NO { get; set; }

        public string IsChecked { get; set; }
        public string RECEIVE_ID_ENCRYPTED { get; set; }

        public string RECEIVED_BY_NAME { get; set; }
        public string CHECKED_BY_NAME { get; set; }
        public string SKU_NAME { get; set; }

        
    }
}
