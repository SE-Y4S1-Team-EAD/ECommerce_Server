using ECommerceAPI.Models;
using ECommerceAPI.Data;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using ECommerceAPI.Controllers;

namespace ECommerceAPI.Services
{
    public class UserService : IUserService
    {
        private readonly MongoDbContext _context;

        public UserService(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
   
            return await _context.Users.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.Find(_ => true).ToListAsync();
        }

        public async Task CreateUserAsync(UserDetails userDetails)
        {
            if (userDetails == null)
            {
                throw new ArgumentNullException(nameof(userDetails));
            }

            // Generate a new ID if not provided
            if (string.IsNullOrEmpty(userDetails.Id))
            {
                userDetails.Id = ObjectId.GenerateNewId().ToString(); // Generate a new ObjectId
            }

            // Create a new User object or directly use userDetails if it's the correct type
            var user = new User
            {
                Id = userDetails.Id, // Ensure the ID is set
                Username = userDetails.Username,
                Email = userDetails.Email,
                Nic = userDetails.Nic,
                PasswordHash = userDetails.PasswordHash,
                Role = userDetails.Role,
                IsActive = userDetails.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            // Insert the new user into the MongoDB collection
            await _context.Users.InsertOneAsync(user);
        }


        public async Task UpdateUserAsync(User user)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
            await _context.Users.ReplaceOneAsync(filter, user);
        }

        public async Task UpdateUserAsyncStatus(User user)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
            var update = Builders<User>.Update.Set(u => u.IsActive, user.IsActive);
            await _context.Users.UpdateOneAsync(filter, update);
        }

        public async Task DeleteUserAsync(string id)
        {

            await _context.Users.DeleteOneAsync(u => u.Id == id);
        }

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            // Add authentication logic here
            return await _context.Users.Find(u => u.Email == email && u.PasswordHash == password).FirstOrDefaultAsync();
        }
    }
}
