using CombatLink.Domain.IRepositories;
using CombatLink.Domain.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

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
                (ChallengerUserId, ChallengedUserId, TimeProposed, SportId, Longitude, Latitude, TimeOfSession, Status)
                VALUES 
                (@ChallengerUserId, @ChallengedUserId, @TimeProposed, @SportId, @Longitude, @Latitude, @TimeOfSession, @Status)";

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
            command.Parameters.AddWithValue("@Status", (int)proposal.Status);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<SparringSessionProposal?> GetByIdAsync(int id)
        {
            const string query = @"
                SELECT Id, ChallengerUserId, ChallengedUserId, TimeProposed, SportId, Longitude, Latitude, TimeOfSession, Status
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
                    Status = (ProposalStatus)reader.GetInt32(8)
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
                    Status = @Status
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
            command.Parameters.AddWithValue("@Status", (int)proposal.Status);

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
                SELECT Id, ChallengerUserId, ChallengedUserId, TimeProposed, SportId, Longitude, Latitude, TimeOfSession, Status
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
                    Status = (ProposalStatus)reader.GetInt32(8)
                });
            }

            return proposals;
        }

        public async Task<IEnumerable<SparringSessionProposal>> GetByTwoUserIdsAsync(int userId1, int userId2)
        {
            var proposals = new List<SparringSessionProposal>();

            const string query = @"
                SELECT Id, ChallengerUserId, ChallengedUserId, TimeProposed, SportId, Longitude, Latitude, TimeOfSession, Status
                FROM SparringSessionProposals
                WHERE 
                    (ChallengerUserId = @UserId1 AND ChallengedUserId = @UserId2)
                    OR
                    (ChallengerUserId = @UserId2 AND ChallengedUserId = @UserId1)
                ORDER BY TimeProposed ASC";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId1", userId1);
            command.Parameters.AddWithValue("@UserId2", userId2);

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
                    Status = (ProposalStatus)reader.GetInt32(8)
                });
            }

            return proposals;
        }

        public async Task<IEnumerable<SparringSessionProposal>> GetUpcommingSparringsForUserId(int userId)
        {
            var proposals = new List<SparringSessionProposal>();

            const string query = @"
        SELECT Id, ChallengerUserId, ChallengedUserId, TimeProposed, SportId, Longitude, Latitude, TimeOfSession, Status
        FROM SparringSessionProposals
        WHERE 
            (ChallengerUserId = @UserId OR ChallengedUserId = @UserId)
            AND Status = @AcceptedStatus
            AND TimeOfSession > GETUTCDATE()
        ORDER BY TimeOfSession ASC";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@AcceptedStatus", (int)ProposalStatus.Accepted);

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
                    Status = (ProposalStatus)reader.GetInt32(8)
                });
            }

            return proposals;
        }

        public async Task<IEnumerable<SparringSessionProposal>> GetCompletedSparringSessionsForUserId(int userId)
        {
            var proposals = new List<SparringSessionProposal>();

            const string query = @"
        SELECT Id, ChallengerUserId, ChallengedUserId, TimeProposed, SportId, Longitude, Latitude, TimeOfSession, Status
        FROM SparringSessionProposals
        WHERE 
            (ChallengerUserId = @UserId OR ChallengedUserId = @UserId)
            AND Status = @AcceptedStatus
            AND TimeOfSession <= GETUTCDATE()
        ORDER BY TimeOfSession DESC";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@AcceptedStatus", (int)ProposalStatus.Accepted);

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
                    Status = (ProposalStatus)reader.GetInt32(8)
                });
            }

            return proposals;
        }


    }
}
