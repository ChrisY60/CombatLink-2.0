using CombatLink.Repositories.IRepositories;
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
    }
}
