using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using TeleNeuro.API.Services;

namespace TeleNeuro.API.Hubs
{
    [Authorize]
    public class NotificationHub : Hub<INotify>
    {
        public static readonly ConcurrentDictionary<int, ConcurrentBag<string>> Connections = new();
        private readonly IUserManagerService _userManagerService;

        public NotificationHub(IUserManagerService userManagerService)
        {
            _userManagerService = userManagerService;
        }

        public override Task OnConnectedAsync()
        {
            try
            {
                if (Connections.ContainsKey(_userManagerService.UserId))
                {
                    Connections[_userManagerService.UserId].Add(Context.ConnectionId);
                }
                else
                {
                    Connections.TryAdd(_userManagerService.UserId, new ConcurrentBag<string> { Context.ConnectionId });
                }
            }
            catch
            {
                //TODO
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                if (Connections.ContainsKey(_userManagerService.UserId))
                {
                    Connections.TryRemove(_userManagerService.UserId, out _);
                }
            }
            catch
            {
                //TODO
            }

            return base.OnDisconnectedAsync(exception);
        }
    }
}
