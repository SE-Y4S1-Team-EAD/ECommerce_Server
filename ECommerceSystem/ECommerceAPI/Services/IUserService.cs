using ECommerceAPI.Controllers;
using ECommerceAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Services
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(string id);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task CreateUserAsync(UserDetails userDetails);
        Task UpdateUserAsync(User user);
        Task UpdateUserAsyncStatus(User user);
        Task DeleteUserAsync(string id);
        Task<User> AuthenticateAsync(string email, string password);
    }
}
