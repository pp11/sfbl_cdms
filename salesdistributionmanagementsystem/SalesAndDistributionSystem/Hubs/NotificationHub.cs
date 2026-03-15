using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Hubs
{
    public class NotificationHub : Hub
    {

        public async Task SendMessage(List<string> User_list, string message)
        {
           
                await Clients.All.SendAsync("ReceiveMessageHandler", User_list, message);


        }
    }
}
