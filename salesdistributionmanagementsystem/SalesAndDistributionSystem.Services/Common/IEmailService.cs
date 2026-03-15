using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;

using MailKit.Security;
using SalesAndDistributionSystem.Domain.Common;

namespace SalesAndDistributionSystem.Services.Common
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailConfiguration mailRequest);
        string BodyReader(EmailConfiguration emailConfiguration,string Path);
    }
}
