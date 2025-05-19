using CombatLink.Domain.IServices;
using CombatLink.Domain.Models;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace CombatLink.Web.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatMessageService _chatMessageService;

        public ChatHub(IChatMessageService chatMessageService)
        {
            _chatMessageService = chatMessageService;
        }

        public override async Task OnConnectedAsync()
        {
            Debug.WriteLine($"Client connected: {Context.ConnectionId}");
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public async Task SendMessageToMatch(string matchId, string senderId, string message)
        {
            var sender = new
            {
                id = senderId
            };
            ChatMessage messageObj = new ChatMessage
            {
                RelatedMatch = new Match { Id = int.Parse(matchId) },
                Sender = new User { Id = int.Parse(senderId) },
                MessageContent = message,
                TimeSent = DateTime.UtcNow
            };
            await _chatMessageService.SendMessageAsync(messageObj);
            await Clients.Group(matchId).SendAsync("ReceiveMessage", sender, message, DateTime.UtcNow.ToString("o"));
        }

        public async Task JoinMatchGroup(string matchId)
        {
            try
            {
                Console.WriteLine($"[JoinMatchGroup] Joining matchId: {matchId}, ConnectionId: {Context.ConnectionId}");

                if (string.IsNullOrWhiteSpace(matchId))
                {
                    Console.WriteLine("[JoinMatchGroup] matchId is null or empty!");
                    throw new ArgumentException("Match ID cannot be null or empty.");
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, matchId);
                Console.WriteLine("[JoinMatchGroup] Successfully joined group.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[JoinMatchGroup] ERROR: {ex.Message}");
                throw;
            }

        }

        public async Task LeaveMatchGroup(string matchId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, matchId);
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
