using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class DEPOT_REQUISITION_ISSUE_MST : Base
    {
        public int MST_ID { get; set; }
        public string MST_ID_ENCRYPTED { get; set; }
        public string REQUISITION_UNIT_ID { get; set; }
        public string ISSUE_UNIT_ID { get; set; }
        public string REQUISITION_UNIT_NAME { get; set; }
        public string ISSUE_NO { get; set; }
        public string ISSUE_DATE { get; set; }
        public string REQUISITION_NO { get; set; }
        public string REQUISITION_DATE { get; set; }
        public double REQUISITION_AMOUNT { get; set; }
        public double ISSUE_AMOUNT { get; set; }
        public double TOTAL_WEIGHT { get; set; }
        public double TOTAL_VOLUME { get; set; }
        public string ISSUE_BY { get; set; }
        public string STATUS { get; set; }

        public string REMARKS { get; set; }
       
        public string q { get; set; }
        public List<DEPOT_REQUISITION_ISSUE_DTL> requisitionIssueDtlList { get; set; }

    }
}
