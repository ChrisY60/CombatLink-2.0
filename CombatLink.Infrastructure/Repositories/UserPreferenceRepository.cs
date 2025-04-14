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
    public class UserPreferenceRepository : IUserPreferenceRepository
    {
        private readonly string _connectionString;
        public UserPreferenceRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<bool> AddAsync(UserPreference preference)
        {
            string query = @"INSERT INTO UserPreference 
                            (UserId, WeightMin, WeightMax, HeightMin, HeightMax, ExperienceMin, ExperienceMax)
                            VALUES (@UserId, @WeightMin, @WeightMax, @HeightMin, @HeightMax, @ExperienceMin, @ExperienceMax)";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", preference.RelatedUser.Id);
            command.Parameters.AddWithValue("@WeightMin", (object?)preference.WeightMin ?? DBNull.Value);
            command.Parameters.AddWithValue("@WeightMax", (object?)preference.WeightMax ?? DBNull.Value);
            command.Parameters.AddWithValue("@HeightMin", (object?)preference.HeightMin ?? DBNull.Value);
            command.Parameters.AddWithValue("@HeightMax", (object?)preference.HeightMax ?? DBNull.Value);
            command.Parameters.AddWithValue("@ExperienceMin", (object?)preference.ExperienceMin ?? DBNull.Value);
            command.Parameters.AddWithValue("@ExperienceMax", (object?)preference.ExperienceMax ?? DBNull.Value);

            return await command.ExecuteNonQueryAsync() > 0;
        }



        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "DELETE FROM UserPreference WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<UserPreference?> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM UserPreference WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new UserPreference
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    RelatedUser = new User { Id = reader.GetInt32(reader.GetOrdinal("UserId")) },
                    WeightMin = reader["WeightMin"] as decimal?,
                    WeightMax = reader["WeightMax"] as decimal?,
                    HeightMin = reader["HeightMin"] as decimal?,
                    HeightMax = reader["HeightMax"] as decimal?,
                    ExperienceMin = reader["ExperienceMin"] as int?,
                    ExperienceMax = reader["ExperienceMax"] as int?
                };
            }

            return null;
        }
        public async Task<UserPreference?> GetByUserIdAsync(int userId)
        {
            const string query = "SELECT * FROM UserPreference WHERE UserId = @UserId";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var up = new UserPreference
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    RelatedUser = new User { Id = reader.GetInt32(reader.GetOrdinal("UserId")) },
                    WeightMin = reader["WeightMin"] as decimal?,
                    WeightMax = reader["WeightMax"] as decimal?,
                    HeightMin = reader["HeightMin"] as decimal?,
                    HeightMax = reader["HeightMax"] as decimal?,
                    ExperienceMin = reader["ExperienceMin"] as int?,
                    ExperienceMax = reader["ExperienceMax"] as int?
                };
                return up;
            }

            return null;
        }

        public async Task<bool> UpdateAsync(UserPreference preference)
        {
            const string query = @"UPDATE UserPreference SET
                WeightMin = @WeightMin,
                WeightMax = @WeightMax,
                HeightMin = @HeightMin,
                HeightMax = @HeightMax,
                ExperienceMin = @ExperienceMin,
                ExperienceMax = @ExperienceMax
                WHERE UserId = @UserId";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserId", preference.RelatedUser.Id);
            command.Parameters.AddWithValue("@WeightMin", (object?)preference.WeightMin ?? DBNull.Value);
            command.Parameters.AddWithValue("@WeightMax", (object?)preference.WeightMax ?? DBNull.Value);
            command.Parameters.AddWithValue("@HeightMin", (object?)preference.HeightMin ?? DBNull.Value);
            command.Parameters.AddWithValue("@HeightMax", (object?)preference.HeightMax ?? DBNull.Value);
            command.Parameters.AddWithValue("@ExperienceMin", (object?)preference.ExperienceMin ?? DBNull.Value);
            command.Parameters.AddWithValue("@ExperienceMax", (object?)preference.ExperienceMax ?? DBNull.Value);

            return await command.ExecuteNonQueryAsync() > 0;
        }
    
    }
}
