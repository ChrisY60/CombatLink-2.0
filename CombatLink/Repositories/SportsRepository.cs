using CombatLinkMVC.Models;
using CombatLink.Repositories.IRepositories;
using Microsoft.Data.SqlClient;
namespace CombatLink.Repositories
{
    public class SportsRepository : ISportRepository
    {
        private readonly string _connectionString;

        public SportsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> CreateSport(Sport sport)
        {
            string query = "INSERT INTO Sports (Name) VALUES (@Name)";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", sport.Name);

            int result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        public async Task<Sport?> GetSportById(int sportId)
        {
            string query = "SELECT Id, Name FROM Sports WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", sportId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Sport
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                };
            }

            return null;
        }

        public async Task<IEnumerable<Sport>> GetAllSports()
        {
            var sports = new List<Sport>();
            string query = "SELECT Id, Name FROM Sports";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                sports.Add(new Sport
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                });
            }

            return sports;
        }

        public async Task<IEnumerable<Sport>> GetSportsByUserId(int userId)
        {
            var sports = new List<Sport>();
            string query = @"SELECT s.Id, s.Name 
                             FROM Sports s
                             INNER JOIN Sports_Users us ON s.Id = us.SportId
                             WHERE us.UserId = @UserId";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                sports.Add(new Sport
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                });
            }

            return sports;
        }

        public async Task<IEnumerable<User>> GetUsersBySportId(int sportId)
        {
            var users = new List<User>();
            string query = @"SELECT u.Id, u.Email 
                             FROM Users u
                             INNER JOIN UserSport us ON u.Id = us.UserId
                             WHERE us.SportId = @SportId";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@SportId", sportId);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(new User
                {
                    Id = reader.GetInt32(0),
                    Email = reader.GetString(1)
                });
            }

            return users;
        }
    }
}
