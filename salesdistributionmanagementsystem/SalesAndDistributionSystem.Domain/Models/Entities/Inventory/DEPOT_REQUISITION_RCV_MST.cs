using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.Inventory
{
    public class DEPOT_REQUISITION_RCV_MST
    {
        public int MST_ID { get; set; }
        public string MST_ID_ENCRYPTED { get; set; }
        public int ISSUE_UNIT_ID { get; set; }
        public string ISSUE_UNIT_NAME { get; set; }
        public int REQUISITION_UNIT_ID { get; set; }

 
        public string REQUISITION_UNIT_NAME { get; set; }
        public string ISSUE_NO { get; set; }
        public string ISSUE_DATE { get; set; }
        public string REQUISITION_NO { get; set; }
    
        public string RECEIVE_NO { get; set; }
        public string DISPATCH_NO { get; set; }
        public string RECEIVE_DATE { get; set; }
   
        public double ISSUE_AMOUNT { get; set; }
        public double RECEIVE_AMOUNT { get; set; }

        public string RECEIVE_BY { get; set; }
        public string STATUS { get; set; }
        public int COMPANY_ID { get; set; }

        public string REMARKS { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }
        public string q { get; set; }
        public int ROW_NO { get; set; }
        public List<DEPOT_REQUISITION_RCV_DTL> requisitionRcvDtlList { get; set; }
    }
}
