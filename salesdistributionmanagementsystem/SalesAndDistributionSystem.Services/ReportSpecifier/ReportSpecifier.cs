using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Services.ReportSpecifier
{
    public class ReportSpecifier : IReportSpecifier 
    {
  
        public List<ReportDataSpecify> GetReportList() =>
          new List<ReportDataSpecify>
          {
                new ReportDataSpecify { ReportId = 1, ReportArea = "SalesAndDistribution/Master/Product", ReportName = "ProductInfoReport", ReportDocumentTitle = "Product Information Report",ReportHeaderTitle = "Product Information Report For Test", IsCSV = true,IsPDF = true, IsPreview = true },
                new ReportDataSpecify { ReportId = 2, ReportArea = "SalesAndDistribution/Master/DivisionRegionRelationReport", ReportName = "DivisionRegionRelationReport", ReportDocumentTitle = "Division Region Relation Information Report",ReportHeaderTitle = "Division Region Relation Information Report", IsCSV = true, IsPDF = true, IsPreview = true }

          };

    }
}
