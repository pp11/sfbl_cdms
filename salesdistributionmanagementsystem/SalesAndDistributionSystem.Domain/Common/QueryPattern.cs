using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Common
{
   
    public class QueryPattern
    {

        /// <summary>
        /// SQL Statements
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Parameter List
        /// </summary>
        public List<Dictionary<string, string>> Parametes { get; set; } = new List<Dictionary<string, string>>();
    }
}
