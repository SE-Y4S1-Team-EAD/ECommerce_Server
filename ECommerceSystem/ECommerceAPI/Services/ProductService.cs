using ECommerceAPI.Models;
using ECommerceAPI.Data;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace ECommerceAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly MongoDbContext _context;

        public ProductService(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<Product> GetProductByIdAsync(string id)
        {

            return await _context.Products.Find(p => p.ProductId == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products.Find(_ => true).ToListAsync();
        }

        public async Task CreateProductAsync(Product product)
        {
            await _context.Products.InsertOneAsync(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.ProductId, product.ProductId);
            await _context.Products.ReplaceOneAsync(filter, product);
        }

        public async Task DeleteProductAsync(string id)
        {
            await _context.Products.DeleteOneAsync(p => p.ProductId == id);
        }
    }
}
