using CombatLink.Domain.IRepositories;
using CombatLink.Domain.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CombatLink.Infrastructure.Repositories
{
    public class ChatMessageRepository : IChatMessageRepository
    {
        private readonly string _connectionString;

        public ChatMessageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> SendMessageAsync(ChatMessage message)
        {
            string query = @"
                INSERT INTO ChatMessages (MatchId, SenderId, MessageContent, TimeSent, SeenByReceiver)
                VALUES (@MatchId, @SenderId, @MessageContent, @TimeSent, @SeenByReceiver)";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MatchId", message.RelatedMatch.Id);
            command.Parameters.AddWithValue("@SenderId", message.Sender.Id);
            command.Parameters.AddWithValue("@MessageContent", message.MessageContent);
            command.Parameters.AddWithValue("@TimeSent", message.TimeSent);
            command.Parameters.AddWithValue("@SeenByReceiver", message.SeenByReceiver);

            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteMessageAsync(int messageId)
        {
            string query = "DELETE FROM ChatMessages WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", messageId);

            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<ChatMessage>> GetMessagesBetweenUsersAsync(int userId1, int userId2)
        {
            var messages = new List<ChatMessage>();

            string query = @"
                SELECT cm.Id, cm.MatchId, cm.SenderId, cm.MessageContent, cm.TimeSent, cm.SeenByReceiver
                FROM ChatMessages cm
                INNER JOIN Matches m ON cm.MatchId = m.Id
                WHERE (m.User1Id = @User1 AND m.User2Id = @User2)
                   OR (m.User1Id = @User2 AND m.User2Id = @User1)
                ORDER BY cm.TimeSent";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@User1", userId1);
            command.Parameters.AddWithValue("@User2", userId2);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                messages.Add(new ChatMessage
                {
                    Id = reader.GetInt32(0),
                    RelatedMatch = new Match { Id = reader.GetInt32(1) },
                    Sender = new User { Id = reader.GetInt32(2) },
                    MessageContent = reader.GetString(3),
                    TimeSent = reader.GetDateTime(4),
                    SeenByReceiver = reader.GetBoolean(5)
                });
            }

            return messages;
        }

        public async Task<IEnumerable<ChatMessage>> GetMessagesForUserAsync(int userId)
        {
            var messages = new List<ChatMessage>();

            string query = @"
                SELECT cm.Id, cm.MatchId, cm.SenderId, cm.MessageContent, cm.TimeSent, cm.SeenByReceiver
                FROM ChatMessages cm
                INNER JOIN Matches m ON cm.MatchId = m.Id
                WHERE m.User1Id = @UserId OR m.User2Id = @UserId
                ORDER BY cm.TimeSent";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                messages.Add(new ChatMessage
                {
                    Id = reader.GetInt32(0),
                    RelatedMatch = new Match { Id = reader.GetInt32(1) },
                    Sender = new User { Id = reader.GetInt32(2) },
                    MessageContent = reader.GetString(3),
                    TimeSent = reader.GetDateTime(4),
                    SeenByReceiver = reader.GetBoolean(5)
                });
            }

            return messages;
        }

        public async Task<ChatMessage?> GetMessageByIdAsync(int messageId)
        {
            string query = @"
                SELECT Id, MatchId, SenderId, MessageContent, TimeSent, SeenByReceiver
                FROM ChatMessages
                WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", messageId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new ChatMessage
                {
                    Id = reader.GetInt32(0),
                    RelatedMatch = new Match { Id = reader.GetInt32(1) },
                    Sender = new User { Id = reader.GetInt32(2) },
                    MessageContent = reader.GetString(3),
                    TimeSent = reader.GetDateTime(4),
                    SeenByReceiver = reader.GetBoolean(5)
                };
            }

            return null;
        }
        //prenapishi q
        public async Task<IEnumerable<ChatSummary>> GetChatSummariesForUserAsync(int userId)
        {
            var summaries = new List<ChatSummary>();

            string query = @"
                SELECT 
                    CASE 
                        WHEN m.User1Id = @UserId THEN m.User2Id 
                        ELSE m.User1Id 
                    END AS PartnerId,
                    MAX(cm.TimeSent) AS LastTime,
                    MAX(cm.MessageContent) AS LastMessage,
                    SUM(CASE WHEN cm.SenderId <> @UserId AND cm.SeenByReceiver = 0 THEN 1 ELSE 0 END) AS Unread
                FROM ChatMessages cm
                INNER JOIN Matches m ON cm.MatchId = m.Id
                WHERE m.User1Id = @UserId OR m.User2Id = @UserId
                GROUP BY CASE 
                    WHEN m.User1Id = @UserId THEN m.User2Id 
                    ELSE m.User1Id 
                END";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                summaries.Add(new ChatSummary
                {
                    User = new User { Id = reader.GetInt32(0) },
                    LastMessage = reader.GetString(2),
                    UnreadCount = reader.GetInt32(3)
                });
            }

            return summaries;
        }

        public async Task<IEnumerable<ChatMessage>> GetMessagesForMatchIdAsync(int matchId)
        {
            var messages = new List<ChatMessage>();

            string query = @"
                SELECT Id, MatchId, SenderId, MessageContent, TimeSent, SeenByReceiver
                FROM ChatMessages
                WHERE MatchId = @MatchId
                ORDER BY TimeSent";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MatchId", matchId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                messages.Add(new ChatMessage
                {
                    Id = reader.GetInt32(0),
                    RelatedMatch = new Match { Id = reader.GetInt32(1) },
                    Sender = new User { Id = reader.GetInt32(2) },
                    MessageContent = reader.GetString(3),
                    TimeSent = reader.GetDateTime(4),
                    SeenByReceiver = reader.GetBoolean(5)
                });
            }

            return messages;
        }

    }
}
