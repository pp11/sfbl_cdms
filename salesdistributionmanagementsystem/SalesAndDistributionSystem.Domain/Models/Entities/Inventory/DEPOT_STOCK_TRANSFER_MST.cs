using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.Inventory
{
    public class DEPOT_STOCK_TRANSFER_MST
    {
        public int MST_ID { get; set; }
        public string MST_ID_ENCRYPTED { get; set; }
        public string REF_NO { get; set; }
        public string REF_DATE { get; set; }
        public string TRANSFER_TYPE { get; set; }
        public string TRANSFER_NO { get; set; }
        public string DISPATCH_NO { get; set; }
        public string TRANSFER_DATE { get; set; }
        public string TRANSFER_UNIT_ID { get; set; }
        public string TRANSFER_UNIT_NAME { get; set; }
        public string TRANS_RCV_UNIT_ID { get; set; }
        public int STOCK_QTY { get; set; }

        public double TRANSFER_AMOUNT { get; set; }
        public string TRANSFER_BY { get; set; }
        public string STATUS { get; set; }

        public int COMPANY_ID { get; set; }

        public string REMARKS { get; set; }

        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_TERMINAL { get; set; }
        public string UPDATED_DATE { get; set; }
        public string q { get; set; }
        public int ROW_NO { get; set; }
        public double TOTAL_VOLUME { get; set; }
        public double TOTAL_WEIGHT { get; set; }
        public List<DEPOT_STOCK_TRANSFER_DTL> stockTransferDtlList { get; set; }
    }
}
