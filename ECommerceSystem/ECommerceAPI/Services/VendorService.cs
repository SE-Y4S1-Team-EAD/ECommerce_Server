using ECommerceAPI.Models;
using ECommerceAPI.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ECommerceAPI.Services
{
    public class VendorService : IVendorService
    {
        private readonly MongoDbContext _context;

        public VendorService(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<Vendor> GetVendorByIdAsync(string vendorId)
        {
            
            return await _context.Vendors.Find(v => v.VendorId == vendorId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Vendor>> GetAllVendorsAsync()
        {
            return await _context.Vendors.Find(_ => true).ToListAsync();
        }

        public async Task CreateVendorAsync(CreateVendorDto vendorDto)
        {
            var vendor = new Vendor
            {
                VendorId = ObjectId.GenerateNewId().ToString(), // Ensure VendorId is generated
                UserId = vendorDto.UserId,
                Name = vendorDto.Name,
                CreatedAt = System.DateTime.UtcNow,
                UpdatedAt = System.DateTime.UtcNow
            };
            await _context.Vendors.InsertOneAsync(vendor);
        }

        public async Task UpdateVendorAsync(Vendor vendor)
        {
            var filter = Builders<Vendor>.Filter.Eq(v => v.VendorId, vendor.VendorId);
            vendor.UpdatedAt = System.DateTime.UtcNow;
            await _context.Vendors.ReplaceOneAsync(filter, vendor);
        }

        public async Task DeleteVendorAsync(string vendorId)
        {
            var filter = Builders<Vendor>.Filter.Eq(v => v.VendorId, vendorId);
            await _context.Vendors.DeleteOneAsync(filter);
        }

        public async Task AddRankingAsync(string vendorId, int ranking)
        {
            var filter = Builders<Vendor>.Filter.Eq(v => v.VendorId, vendorId);
            var update = Builders<Vendor>.Update
                .Push(v => v.Rankings, ranking)
                .Set(v => v.UpdatedAt, System.DateTime.UtcNow);

            await _context.Vendors.UpdateOneAsync(filter, update);
            await CalculateAverageRankingAsync(vendorId);
        }

        public async Task AddCommentAsync(string vendorId, Comment comment)
        {
            var filter = Builders<Vendor>.Filter.Eq(v => v.VendorId, vendorId);
            var update = Builders<Vendor>.Update
                .Push(v => v.Comments, comment)
                .Set(v => v.UpdatedAt, System.DateTime.UtcNow);

            await _context.Vendors.UpdateOneAsync(filter, update);
        }

        public async Task<Vendor> CalculateAverageRankingAsync(string vendorId)
        {
            var vendor = await GetVendorByIdAsync(vendorId);
            if (vendor != null && vendor.Rankings.Any())
            {
                vendor.AverageRanking = vendor.Rankings.Average();
                await UpdateVendorAsync(vendor);
            }
            return vendor;
        }
    }
}
