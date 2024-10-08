// ECommerceServices/Services/ProductService.cs
using ECommerceModels.Models;
using ECommerceModels.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceServices.Services
{
    public class ProductService
    {
        private readonly IMongoCollection<Product> _products;

        public ProductService(IOptions<DatabaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _products = database.GetCollection<Product>("Products");
        }

        public async Task<List<Product>> GetAsync() =>
            await _products.Find(product => true).ToListAsync();

        public async Task<Product> GetByIdAsync(string id) =>
            await _products.Find<Product>(product => product.ProductId == id).FirstOrDefaultAsync();

        public async Task<Product> CreateAsync(Product product)
        {
            await _products.InsertOneAsync(product);
            return product;
        }

        public async Task UpdateAsync(string id, Product productIn) =>
            await _products.ReplaceOneAsync(product => product.ProductId == id, productIn);

        public async Task DeleteAsync(string id) =>
            await _products.DeleteOneAsync(product => product.ProductId == id);
    }
}
