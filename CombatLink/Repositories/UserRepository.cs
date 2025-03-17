using CombatLink.Repositories.IRepositories;
using CombatLinkMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;

namespace CombatLink.Repositories
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
                    if (result > 0)
                    {
                        return true;
                    }else
                    {
                        return false;
                    }
                }
            }
        }

        public async Task<int?> LogInUserAsync(string email, string passwordHash)
        {
            string query = "SELECT Id FROM Users " +
                           "WHERE Email = @Email AND PasswordHash = @PasswordHash";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using(var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@PasswordHash", passwordHash);
                    var result = await command.ExecuteScalarAsync();
                    if(result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                }
                return null;
            }
        }
        public async Task<bool> UpdateUserProfile(int userId, string firstName, string lastName, DateTime dateOfBirth, decimal weight, decimal height, int monthsOfExperience)
        {
            string query = "UPDATE Users " +
                           "SET FirstName = @FirstName, LastName = @LastName, DateOfBirth = @DateOfBirth, " +
                           "Weight = @Weight, Height = @Height, MonthsOfExperience = @MonthsOfExperience " +
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

                    var result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }

        public async Task<User?> GetUserById(int userId)
        {
            string query = "SELECT FirstName, LastName, DateOfBirth, Weight, Height, MonthsOfExperience FROM Users WHERE Id = @UserId";

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
                                FirstName = reader["FirstName"] as string,
                                LastName = reader["LastName"] as string,
                                DateOfBirth = reader["DateOfBirth"] as DateTime?,
                                Weight = reader["Weight"] as decimal?,
                                Height = reader["Height"] as decimal?,
                                MonthsOfExperience = reader["MonthsOfExperience"] as int?
                            };
                        }
                    }
                }
            }
            return null;
        }





    }
}
