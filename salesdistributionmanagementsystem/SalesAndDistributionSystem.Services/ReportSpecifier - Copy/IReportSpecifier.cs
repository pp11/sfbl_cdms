using SalesAndDistributionSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Services.ReportSpecifier
{
    public interface IReportSpecifier
    {
        List<ReportDataSpecify> GetReportList();
    }
}
