using CombatLink.Domain.IRepositories;
using CombatLink.Domain.Models;
using Microsoft.Data.SqlClient;

namespace CombatLink.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> RegisterUserAsync(string email, string passwordHash)
        {
            string query = "INSERT INTO Users (Email, PasswordHash) " +
                           "VALUES (@Email, @PasswordHash)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("email", email);
                    command.Parameters.AddWithValue("PasswordHash", passwordHash);
                    var result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }

        public async Task<string?> GetPasswordHashByEmail(string email)
        {
            string query = "SELECT PasswordHash FROM Users WHERE Email = @Email";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    var result = await command.ExecuteScalarAsync();
                    return result != null ? Convert.ToString(result) : null;
                }
            }
        }

        public async Task<bool> UpdateUserProfile(int userId, string firstName, string lastName, DateTime dateOfBirth, decimal weight, decimal height, int monthsOfExperience, string? profilePictureUrl = null)
        {
            string query = "UPDATE Users SET FirstName = @FirstName, LastName = @LastName, DateOfBirth = @DateOfBirth, " +
                           "Weight = @Weight, Height = @Height, MonthsOfExperience = @MonthsOfExperience, ProfilePictureURL = @ProfilePictureURL " +
                           "WHERE Id = @UserId";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);
                    command.Parameters.AddWithValue("@Weight", weight);
                    command.Parameters.AddWithValue("@Height", height);
                    command.Parameters.AddWithValue("@MonthsOfExperience", monthsOfExperience);
                    command.Parameters.AddWithValue("@ProfilePictureURL", (object?)profilePictureUrl ?? DBNull.Value);

                    var result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }

        public async Task<User?> GetUserById(int userId)
        {
            string query = "SELECT Id, FirstName, LastName, DateOfBirth, Weight, Height, MonthsOfExperience, ProfilePictureURL, IsVerified FROM Users WHERE Id = @UserId";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new User
                            {
                                Id = (int)reader["Id"],
                                FirstName = reader["FirstName"] as string,
                                LastName = reader["LastName"] as string,
                                DateOfBirth = reader["DateOfBirth"] as DateTime?,
                                Weight = reader["Weight"] as decimal?,
                                Height = reader["Height"] as decimal?,
                                MonthsOfExperience = reader["MonthsOfExperience"] as int?,
                                ProfilePictureURL = reader["ProfilePictureURL"] as string,
                                IsVerified = (bool)reader["IsVerified"]
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<int?> GetUserIdByEmail(string email)
        {
            string query = "SELECT Id FROM Users WHERE email = @userEmail";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userEmail", email);
                    var result = await command.ExecuteScalarAsync();
                    return result != null ? Convert.ToInt32(result) : null;
                }
            }
        }

        public async Task<bool> AddSportToUser(Sport sport, User user)
        {
            string query = "INSERT INTO Sports_Users (UserId, SportId) VALUES (@UserId, @SportId)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", user.Id);
                    command.Parameters.AddWithValue("@SportId", sport.Id);

                    var result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }

        public async Task<bool> RemoveSportFromUser(int userId, int sportId)
        {
            string query = "DELETE FROM Sports_Users WHERE UserId = @UserId AND SportId = @SportId";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@SportId", sportId);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<bool> AddLanguageToUser(Language language, User user)
        {
            string query = "INSERT INTO Languages_Users (UserId, LanguageId) VALUES (@UserId, @LanguageId)";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", user.Id);
            command.Parameters.AddWithValue("@LanguageId", language.Id);

            var result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        public async Task<bool> RemoveLanguageFromUser(int userId, int languageId)
        {
            string query = "DELETE FROM Languages_Users WHERE UserId = @UserId AND LanguageId = @LanguageId";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@LanguageId", languageId);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            string query = "SELECT Id, Email, FirstName, LastName, DateOfBirth, Weight, Height, MonthsOfExperience, ProfilePictureURL, IsVerified FROM Users";
            var users = new List<User>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var user = new User
                {
                    Id = (int)reader["Id"],
                    Email = reader["Email"] as string,
                    FirstName = reader["FirstName"] as string,
                    LastName = reader["LastName"] as string,
                    DateOfBirth = reader["DateOfBirth"] as DateTime?,
                    Weight = reader["Weight"] as decimal?,
                    Height = reader["Height"] as decimal?,
                    MonthsOfExperience = reader["MonthsOfExperience"] as int?,
                    ProfilePictureURL = reader["ProfilePictureURL"] as string,
                    IsVerified = (bool)reader["IsVerified"]
                };

                users.Add(user);
            }

            return users;
        }

        public async Task<bool> SetUserVerifiedAsync(int userId, bool isVerified)
        {
            string query = "UPDATE Users SET IsVerified = @IsVerified WHERE Id = @UserId";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@IsVerified", isVerified);
                    var result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }
    }
}