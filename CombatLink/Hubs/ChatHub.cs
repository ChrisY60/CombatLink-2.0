using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace CombatLink.Web.Hubs
{
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            Debug.WriteLine($"Client connected: {Context.ConnectionId}");
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public async Task SendMessage(string user, string message)
        {
            Debug.WriteLine("V huba sme");
            try
            {
                Console.WriteLine($"Message from {user}: {message}");
                await Clients.All.SendAsync("ReceiveMessage", user, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendMessage: {ex.Message}");
                throw;
            }
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
            if (exception != null)
            {
                Console.WriteLine($"Disconnected with error: {exception.Message}");
            }

            await base.OnDisconnectedAsync(exception);
        }

    }
}
