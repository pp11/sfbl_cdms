using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Models.ViewModels.Security;
using SalesAndDistributionSystem.Domain.Utility;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace SalesAndDistributionSystem.Models
{
    public class DBConnectionProvider
    {

        public string GetConnectionString(string companyName)
        {
            return "myconn";
        }
     
    }
}
