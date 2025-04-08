using CombatLink.Repositories.IRepositories;
using CombatLinkMVC.Models;
using Microsoft.Data.SqlClient;

namespace CombatLink.Repositories
{
    public class LanguageRepository : ILanguageRepository
    {
        private readonly string _connectionString;

        public LanguageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> CreateLanguage(Language language)
        {
            string query = "INSERT INTO Languages (Name) VALUES (@Name)";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", language.Name);

            int result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        public async Task<IEnumerable<Language>> GetAllLanguages()
        {
            string query = "SELECT Id, Name FROM Languages";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);

            using var reader = await command.ExecuteReaderAsync();
            List<Language> languages = new List<Language>();
            while(await reader.ReadAsync())
            {
                languages.Add( new Language
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                });
            }

            return languages;
        }

        public async Task<Language> GetLanguageById(int languageId)
        {
            string query = "SELECT Id, Name FROM Languages WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", languageId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Language
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                };
            }

            return null;
        }

        public async Task<IEnumerable<Language>> GetLanguagesByUserId(int userId)
        {
            List<Language> languages = new List<Language>();
            string query = @"SELECT l.Id, l.Name 
                             FROM Languages l
                             INNER JOIN Languages_Users lu ON l.Id = lu.LanguageId
                             WHERE lu.UserId = @UserId";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                languages.Add(new Language
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                });
            }

            return languages;
        }

        public async Task<IEnumerable<User>> GetUsersByLanguageId(int languageId)
        {
            List<User> users = new List<User>();
            string query = @"SELECT u.Id, u.Email
                     FROM Users u
                     INNER JOIN Languages_Users lu ON u.Id = lu.UserId
                     WHERE lu.LanguageId = @LanguageId";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@LanguageId", languageId);

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
