using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Vehicle_Info
    {
        public int VEHICLE_ID { get; set; }
        public string VEHICLE_NO { get; set; }
        public string UNIT_ID { get; set; }
        public string VEHICLE_DESCRIPTION { get; set; }
        public decimal VEHICLE_TOTAL_VOLUME { get; set; }
        public string VOLUME_UNIT { get; set; }
        public decimal VEHICLE_TOTAL_WEIGHT { get; set; }
        public string WEIGHT_UNIT { get; set; }
        public string DRIVER_ID { get; set; }
        public string DRIVER_NAME { get; set; }
        public string STATUS { get; set; }
        public int COMPANY_ID { get; set; }
        public string REMARKS { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }

        //-------- Helper Column
        public int ROW_NO { get; set; }
        public string Measuring_Unit_Type { get; set; }
    }
}
