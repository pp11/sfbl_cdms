using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.Inventory
{
    public class RefurbishmentReceivingMst
    {
        public int MST_SLNO { get; set; }
        public string CLAIM_NO { get; set; }
        public String RECEIVE_DATE { get; set; }
        public string RECEIVE_SHIFT { get; set; }
        public string RECEIVE_CATEGORY { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_TYPE { get; set; }
        public string CHALLAN_NUMBER { get; set; }
        public String CHALLAN_DATE { get; set; }
        public decimal SENDING_CARTON_QTY { get; set; }
        public decimal SENDING_BAG_QTY { get; set; }
        public decimal SENDING_TOTAL_AMOUNT { get; set; }
        public decimal RECEIVE_CARTON_QTY { get; set; }
        public decimal RECEIVE_BAG_QTY { get; set; }
        public decimal RECEIVE_TOTAL_AMOUNT { get; set; }
        public string TSO_CODE { get; set; }
        public string TSO_NAME { get; set; }
        public string TSO_CONTACT_NO { get; set; }
        public string DRIVER_CODE { get; set; }
        public string DRIVER_NAME { get; set; }
        public string DRIVER_CONTACT_NO { get; set; }
        public string VEHICLE_NO { get; set; }
        public string RECEIVED_BY { get; set; }
        public string REMARKS { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }
        public string APPROVED_STATUS { get; set; }
        public string APPROVED_BY { get; set; }
        public string APPROVED_DATE { get; set; }
        public string UNIT_ID { get; set; }
        public string COMPANY_ID { get; set; }
        public List<RefurbishmentReceivingDtl> Details { get; set; }

    }
}
