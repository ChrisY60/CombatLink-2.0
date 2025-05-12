using CombatLink.Domain.IRepositories;
using CombatLink.Domain.IServices;
using CombatLink.Domain.Models;

namespace CombatLink.Application.Services
{
    public class ChatMessageService : IChatMessageService
    {
        private readonly IChatMessageRepository _chatRepository;

        public ChatMessageService(IChatMessageRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task<bool> SendMessageAsync(ChatMessage message)
        {
            return await _chatRepository.SendMessageAsync(message);
        }

        public async Task<bool> DeleteMessageAsync(int messageId)
        {
            return await _chatRepository.DeleteMessageAsync(messageId);
        }

        public async Task<IEnumerable<ChatMessage>> GetMessagesBetweenUsersAsync(int userId1, int userId2)
        {
            return await _chatRepository.GetMessagesBetweenUsersAsync(userId1, userId2);
        }

        public async Task<IEnumerable<ChatMessage>> GetMessagesForUserAsync(int userId)
        {
            return await _chatRepository.GetMessagesForUserAsync(userId);
        }

        public async Task<ChatMessage?> GetMessageByIdAsync(int messageId)
        {
            return await _chatRepository.GetMessageByIdAsync(messageId);
        }

        public async Task<IEnumerable<ChatSummary>> GetChatSummariesForUserAsync(int userId)
        {
            return await _chatRepository.GetChatSummariesForUserAsync(userId);
        }
    }
}
