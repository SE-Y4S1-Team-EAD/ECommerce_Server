using MongoDB.Driver;
using ECommerceModels.Settings;
using ECommerceAPI.Models;

namespace ECommerceAPI.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(DatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<Product> Products => _database.GetCollection<Product>("Products");
        public IMongoCollection<Order> Orders => _database.GetCollection<Order>("Orders");
        public IMongoCollection<Inventory> Inventories => _database.GetCollection<Inventory>("Inventories");
        public IMongoCollection<Vendor> Vendors => _database.GetCollection<Vendor>("Vendors");
        public IMongoCollection<Notification> Notifications => _database.GetCollection<Notification>("Notifications");
    }
}
