using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.ViewModels.Inventory
{
    public class CheckTransferQtyVsRcv
    {
        public string TRANSFER_NO { get; set; }
        public string SKU_ID { get; set; }
        public string SKU_CODE { get; set; }
        public double TRANS_RCV_QTY { get; set; }
        public double TRANSFER_QTY { get; set; }
    }
}
