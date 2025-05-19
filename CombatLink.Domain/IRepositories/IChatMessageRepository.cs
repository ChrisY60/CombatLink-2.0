using CombatLink.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CombatLink.Domain.IRepositories
{
    public interface IChatMessageRepository
    {
        Task<bool> SendMessageAsync(ChatMessage message);
        Task<bool> DeleteMessageAsync(int messageId);
        Task<IEnumerable<ChatMessage>> GetMessagesBetweenUsersAsync(int userId1, int userId2);
        Task<IEnumerable<ChatMessage>> GetMessagesForMatchIdAsync(int matchId);
        Task<IEnumerable<ChatMessage>> GetMessagesForUserAsync(int userId);
        Task<ChatMessage?> GetMessageByIdAsync(int messageId);
        Task<IEnumerable<ChatSummary>> GetChatSummariesForUserAsync(int userId);

    }
}
