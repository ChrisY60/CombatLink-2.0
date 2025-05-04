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
    public class LikeRepository : ILikeRepository
    {
        private readonly string _connectionString;

        public LikeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> AddLike(Like like)
        {
            string query = @"INSERT INTO Likes (LikerUserId, LikedUserId, TimeOfLike) 
                             VALUES (@LikerUserId, @LikedUserId, @TimeOfLike)";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@LikerUserId", like.LikerUserId);
            command.Parameters.AddWithValue("@LikedUserId", like.LikedUserId);
            command.Parameters.AddWithValue("@TimeOfLike", like.TimeOfLike);

            int result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        public async Task<IEnumerable<Like>> GetLikesByUserId(int likedUserId)
        {
            string query = @"SELECT LikerUserId, LikedUserId, TimeOfLike 
                             FROM Likes 
                             WHERE LikedUserId = @LikedUserId";

            List<Like> likes = new List<Like>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@LikedUserId", likedUserId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                likes.Add(new Like
                {
                    LikerUserId = reader.GetInt32(0),
                    LikedUserId = reader.GetInt32(1),
                    TimeOfLike = reader.GetDateTime(2)
                });
            }

            return likes;
        }

        public async Task<bool> HasUserLiked(int likerUserId, int likedUserId)
        {
            string query = @"SELECT COUNT(*) 
                             FROM Likes 
                             WHERE LikerUserId = @LikerUserId AND LikedUserId = @LikedUserId";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@LikerUserId", likerUserId);
            command.Parameters.AddWithValue("@LikedUserId", likedUserId);

            int count = (int)await command.ExecuteScalarAsync();
            return count > 0;
        }
    }

}
