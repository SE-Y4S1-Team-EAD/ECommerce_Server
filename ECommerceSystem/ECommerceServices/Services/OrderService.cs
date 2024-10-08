// ECommerceServices/Services/OrderService.cs
using ECommerceModels.Models;
using ECommerceModels.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceServices.Services
{
    public class OrderService
    {
        private readonly IMongoCollection<Order> _orders;

        public OrderService(IOptions<DatabaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _orders = database.GetCollection<Order>("Orders");
        }

        public async Task<List<Order>> GetAsync() =>
            await _orders.Find(order => true).ToListAsync();

        public async Task<Order> GetByIdAsync(string id) =>
            await _orders.Find<Order>(order => order.OrderId == id).FirstOrDefaultAsync();

        public async Task<Order> CreateAsync(Order order)
        {
            await _orders.InsertOneAsync(order);
            return order;
        }

        public async Task UpdateAsync(string id, Order orderIn) =>
            await _orders.ReplaceOneAsync(order => order.OrderId == id, orderIn);

        public async Task DeleteAsync(string id) =>
            await _orders.DeleteOneAsync(order => order.OrderId == id);
    }
}
