using Intranet.Core.DTOs.Hub;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Intranet.Hubs
{
    [Authorize]
    public class SignalRHub : Hub
    {
        // front end connection
        //  const connection = new signalR.HubConnectionBuilder()
        //.withUrl(`${apiUrl}/hubs/app`, {
        //  accessTokenFactory: () => localStorage.getItem('access_token')!
        //})
        //.withAutomaticReconnect()
        //.build();

        public static List<HubUserDto> _connections = new List<HubUserDto>();
        public SignalRHub()
        {
        }

        public static bool IsUserConnected(int userId) =>
            _connections.Any(c => c.UserId == userId);

        public override async Task OnConnectedAsync()
        {
            try
            {
                var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    await Clients.Caller.SendAsync("OnError", "Unauthorized: User ID not found.");
                    Context.Abort();
                    return;
                }

                var email = Context.User?.FindFirst(ClaimTypes.Email)?.Value;
                if (email == null)
                {
                    await Clients.Caller.SendAsync("OnError", "Unauthorized: Email not found.");
                    Context.Abort();
                    return;
                }

                // Add user connection info to the connection list
                _connections.Add(new HubUserDto
                {
                    UserId = Convert.ToInt32(userId),
                    Email = email,
                    ConnectionId = Context.ConnectionId
                });

                // Send success confirmation to the client with user's role
                await Clients.Caller.SendAsync("OnConnected", $"Connected Successfully.");

                // Call base method after your custom logic
                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                // Send error message to client and abort connection on any unexpected failure
                await Clients.Caller.SendAsync("OnError", $"Error during connection: {ex.Message}");
                Context.Abort();
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                // var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // remove user from live users list
                // _connections.RemoveAll(x => x.UserId == Convert.ToInt32(userId));
                _connections.RemoveAll(x => x.ConnectionId == Context.ConnectionId);

            }
            catch (Exception ex)
            {
                Clients.Caller.SendAsync("onError", "OnDisconnected: " + ex.Message);
            }

            return base.OnDisconnectedAsync(exception);
        }
    }
}
