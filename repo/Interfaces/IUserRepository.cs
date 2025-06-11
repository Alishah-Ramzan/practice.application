using Repo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repo.Interfaces
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task<User?> GetByUsernameAsync(string username);
        Task<List<User>> GetAllUsersAsync();
        Task UpdateUserAsync(User user);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);

    }
}
