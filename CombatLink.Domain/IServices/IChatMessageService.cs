using CombatLink.Domain.Models;

namespace CombatLink.Domain.IServices
{
    public interface IChatMessageService
    {
        Task<bool> SendMessageAsync(ChatMessage message);
        Task<bool> DeleteMessageAsync(int messageId);
        Task<IEnumerable<ChatMessage>> GetMessagesBetweenUsersAsync(int userId1, int userId2);
        Task<IEnumerable<ChatMessage>> GetMessagesForMatchId(int matchId);
        Task<IEnumerable<ChatMessage>> GetMessagesForUserAsync(int userId);
        Task<ChatMessage?> GetMessageByIdAsync(int messageId);
        Task<IEnumerable<ChatSummary>> GetChatSummariesForUserAsync(int userId);
    }
}
