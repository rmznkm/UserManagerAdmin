using EventBus.Storage;
using ManagementAPI.Model;
using ManagementAPI.Models;

namespace ManagementAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbStorage dbStorage;

        public UserRepository(IDbStorage dbStorage)
        {
            this.dbStorage = dbStorage;
        }

        public async Task AddUserRegisterRequestAsync(Guid userRegisterRequestId, UserRegisterRequest userRegisterRequest)
        {
            var insertSql = "INSERT INTO public.userapprovalrequest (id, first_name, last_name, updated_at, record_status) VALUES (@id, @first_name, @last_name, @updated_at, @record_status)";
            await dbStorage.ExecuteAsync(insertSql, new
            {
                @id = userRegisterRequestId,
                @first_name = userRegisterRequest.FirstName,
                @last_name = userRegisterRequest.LastName,
                @record_status = "W",
                @updated_at = DateTime.Now
            });
        }

        public async Task UpdateAsApprovedAsync(Guid userRegisterRequestId)
        {
            var updateSql = "UPDATE public.userapprovalrequest SET record_status = @record_status, updated_at= @updated_at  WHERE id = @id";
            await dbStorage.ExecuteAsync(updateSql, new
            {
                @id = userRegisterRequestId,
                @record_status = "A",
                @updated_at= DateTime.Now
            });
        }

        public async Task<IEnumerable<ApproveWaitingUser>> GetApproveWaitingUsersAsync()
        {
            var selectSql = "SELECT id, first_name, last_name, updated_at FROM public.userapprovalrequest WHERE record_status = @record_status";
            var reader = await dbStorage.ExecuteReaderAsync(selectSql, new { @record_status = "W" });
            var result = new List<ApproveWaitingUser>();
            while (reader.Read())
            {
                var temp = new ApproveWaitingUser
                {
                    Id = (Guid)reader["id"],
                    FirstName = (string)reader["first_name"],
                    LastName = (string)reader["last_name"],
                    UpdatedAt = (DateTime)reader["updated_at"]
                };
                result.Add(temp);
            }
            return result;
        }
    }
}
