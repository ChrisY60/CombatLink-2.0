using CombatLink.Domain.IRepositories;
using CombatLink.Domain.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatLink.Infrastructure.Repositories
{
    public class MatchRepository : IMatchRepository
    {
        private readonly string _connectionString;

        public MatchRepository (string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> AddMatch(Match match)
        {
            string query = @"INSERT INTO Matches (User1Id, User2Id, TimeOfMatch) 
                             VALUES (@User1Id, @User2Id, @TimeOfMatch)";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@User1Id", match.User1Id);
            command.Parameters.AddWithValue("@User2Id", match.User2Id);
            command.Parameters.AddWithValue("@TimeOfMatch", match.TimeOfMatch);

            int result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        public async Task<IEnumerable<Match>> GetMatchesByUserId(int userId)
        {
            List<Match> matches = new List<Match>();

            string query = @"SELECT Id, User1Id, User2Id, TimeOfMatch
                             FROM Matches
                             WHERE User1Id = @UserId OR User2Id = @UserId";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                matches.Add(new Match
                {
                    Id = reader.GetInt32(0),
                    User1Id = reader.GetInt32(1),
                    User2Id = reader.GetInt32(2),
                    TimeOfMatch = reader.GetDateTime(3)
                });
            }

            return matches;
        }

        public async Task<bool> MatchExists(int user1Id, int user2Id)
        {
            string query = @"SELECT COUNT(*) 
                             FROM Matches 
                             WHERE (User1Id = @User1Id AND User2Id = @User2Id)
                                OR (User1Id = @User2Id AND User2Id = @User1Id)";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@User1Id", user1Id);
            command.Parameters.AddWithValue("@User2Id", user2Id);

            int count = (int)await command.ExecuteScalarAsync();
            return count > 0;
        }

        public async Task<Match?> GetMatchById(int matchId)
        {
            string query = @"SELECT Id, User1Id, User2Id, TimeOfMatch FROM Matches WHERE Id = @MatchId";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MatchId", matchId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Match
                {
                    Id = reader.GetInt32(0),
                    User1Id = reader.GetInt32(1),
                    User2Id = reader.GetInt32(2),
                    TimeOfMatch = reader.GetDateTime(3)
                };
            }

            return null;
        }


    }
}
