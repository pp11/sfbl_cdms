using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Collection_Mst
    {
        public int COLLECTION_MST_ID { get; set; }

        public int UNIT_ID { get;set;}
        public string BATCH_NO { get;set;}
        public string BATCH_DATE { get; set; }
        public string BATCH_STATUS { get; set; }
        public string BATCH_POSTING_STATUS { get; set; }

        public int COMPANY_ID { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }
        public string VERSION_NO { get; set; }

        public List<Collection_Dtl> Collection_Dtls { get; set; }

        //Helper Attributes
        public int ROW_NO { get; set; }
        public string COLLECTION_MST_ID_ENCRYPTED { get; set; }
        public string q { get; set; }
        public string APPROVED_DATE { get; set; }

    }
}
