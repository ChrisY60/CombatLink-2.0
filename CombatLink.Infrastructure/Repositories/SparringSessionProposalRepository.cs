using CombatLink.Domain.IRepositories;
using CombatLink.Domain.Models;
using Microsoft.Data.SqlClient;
namespace CombatLink.Infrastructure.Repositories
{
    public class SparringSessionProposalRepository : ISparringSessionProposalRepository
    {
        private readonly string _connectionString;

        public SparringSessionProposalRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> AddAsync(SparringSessionProposal proposal)
        {
            const string query = @"
                INSERT INTO SparringSessionProposals 
                (ChallengerUserId, ChallengedUserId, TimeProposed, SportId, Longitude, Latitude, TimeOfSession, IsAccepted)
                VALUES 
                (@ChallengerUserId, @ChallengedUserId, @TimeProposed, @SportId, @Longitude, @Latitude, @TimeOfSession, @IsAccepted)";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ChallengerUserId", proposal.ChallengerUserId);
            command.Parameters.AddWithValue("@ChallengedUserId", proposal.ChallengedUserId);
            command.Parameters.AddWithValue("@TimeProposed", proposal.TimeProposed);
            command.Parameters.AddWithValue("@SportId", proposal.SportId);
            command.Parameters.AddWithValue("@Longitude", proposal.Longtitude);
            command.Parameters.AddWithValue("@Latitude", proposal.Latitude);
            command.Parameters.AddWithValue("@TimeOfSession", proposal.TimeOfSession);
            command.Parameters.AddWithValue("@IsAccepted", proposal.IsAccepted);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<SparringSessionProposal?> GetByIdAsync(int id)
        {
            const string query = @"
                SELECT Id, ChallengerUserId, ChallengedUserId, TimeProposed, SportId, Longitude, Latitude, TimeOfSession, IsAccepted
                FROM SparringSessionProposals
                WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new SparringSessionProposal
                {
                    Id = reader.GetInt32(0),
                    ChallengerUserId = reader.GetInt32(1),
                    ChallengedUserId = reader.GetInt32(2),
                    TimeProposed = reader.GetDateTime(3),
                    SportId = reader.GetInt32(4),
                    Longtitude = reader.GetDecimal(5),
                    Latitude = reader.GetDecimal(6),
                    TimeOfSession = reader.GetDateTime(7),
                    IsAccepted = reader.GetBoolean(8)
                };
            }

            return null;
        }

        public async Task<bool> UpdateAsync(SparringSessionProposal proposal)
        {
            const string query = @"
                UPDATE SparringSessionProposals SET
                    ChallengerUserId = @ChallengerUserId,
                    ChallengedUserId = @ChallengedUserId,
                    TimeProposed = @TimeProposed,
                    SportId = @SportId,
                    Longitude = @Longitude,
                    Latitude = @Latitude,
                    TimeOfSession = @TimeOfSession,
                    IsAccepted = @IsAccepted
                WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", proposal.Id);
            command.Parameters.AddWithValue("@ChallengerUserId", proposal.ChallengerUserId);
            command.Parameters.AddWithValue("@ChallengedUserId", proposal.ChallengedUserId);
            command.Parameters.AddWithValue("@TimeProposed", proposal.TimeProposed);
            command.Parameters.AddWithValue("@SportId", proposal.SportId);
            command.Parameters.AddWithValue("@Longitude", proposal.Longtitude);
            command.Parameters.AddWithValue("@Latitude", proposal.Latitude);
            command.Parameters.AddWithValue("@TimeOfSession", proposal.TimeOfSession);
            command.Parameters.AddWithValue("@IsAccepted", proposal.IsAccepted);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "DELETE FROM SparringSessionProposals WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<IEnumerable<SparringSessionProposal>> GetByUserIdAsync(int userId)
        {
            var proposals = new List<SparringSessionProposal>();

            const string query = @"
                SELECT Id, ChallengerUserId, ChallengedUserId, TimeProposed, SportId, Longitude, Latitude, TimeOfSession, IsAccepted
                FROM SparringSessionProposals
                WHERE ChallengerUserId = @UserId OR ChallengedUserId = @UserId
                ORDER BY TimeProposed DESC";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                proposals.Add(new SparringSessionProposal
                {
                    Id = reader.GetInt32(0),
                    ChallengerUserId = reader.GetInt32(1),
                    ChallengedUserId = reader.GetInt32(2),
                    TimeProposed = reader.GetDateTime(3),
                    SportId = reader.GetInt32(4),
                    Longtitude = reader.GetDecimal(5),
                    Latitude = reader.GetDecimal(6),
                    TimeOfSession = reader.GetDateTime(7),
                    IsAccepted = reader.GetBoolean(8)
                });
            }

            return proposals;
        }

    }
}
