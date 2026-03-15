using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using SalesAndDistributionSystem.Services.Business.User;
using SalesAndDistributionSystem.Common;
using System.Text.Json;
using Newtonsoft.Json;
using SalesAndDistributionSystem.Services.Common;

namespace SalesAndDistributionSystem.Hubs
{
    public class UserHub : Hub
    {
        private readonly IUserManager _userManager;
        private readonly ICommonServices _commonServices;

        public UserHub(IUserManager userManager, ICommonServices commonServices)
        {
            _userManager = userManager;
            _commonServices = commonServices;
        }
        private static readonly Dictionary<string, List<(int UserId, string OrderId)>> UserOrderViews = new Dictionary<string, List<(int UserId, string OrderId)>>();

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (UserOrderViews.ContainsKey(Context.ConnectionId))
            {
                UserOrderViews.Remove(Context.ConnectionId);
            }
            await base.OnDisconnectedAsync(exception);
        }


        public async Task<string> NewWindowLoaded(int userId, string orderId)
        {
            try
            {
                orderId = _commonServices.Decrypt(orderId);
                int workingUserId = 0;
                if (orderId != "0")
                {
                    bool orderIdExists = UserOrderViews.Any(kvp => kvp.Value.Any(order => order.OrderId == orderId));
                    if (orderIdExists)
                    {
                        foreach (var kvp in UserOrderViews)
                        {
                            List<(int UserId, string OrderId)> orders = kvp.Value;
                            foreach (var order in orders)
                            {
                                if (order.OrderId == orderId)
                                {
                                    workingUserId = order.UserId;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!UserOrderViews.ContainsKey(Context.ConnectionId))
                        {
                            UserOrderViews[Context.ConnectionId] = new List<(int, string)>();
                        }
                        UserOrderViews[Context.ConnectionId].Add((userId, orderId));
                    }
                }
                await Clients.All.SendAsync("updateTotalViews",0);
                var obj = _userManager.GetUserByUserId(GetDbConnectionString(), workingUserId);
                return JsonConvert.SerializeObject(new { userId = workingUserId, userInfo = obj });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { userId = 0, userInfo = ex.InnerException });
            }
        }
        private string GetDbConnectionString(int Company_Id = 1)
        {
            ServiceProvider _serve = new ServiceProvider();
            string claim = _serve.GetConnectionString(Company_Id.ToString(), "Security");
            if (claim != null)
            {
                return claim;
            }
            else
            {
                return string.Empty;
            }

        }
    }
}
