using ECommerceAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Services
{
    public interface IVendorService
    {
        Task<Vendor> GetVendorByIdAsync(string vendorId);
        Task<IEnumerable<Vendor>> GetAllVendorsAsync();
        Task CreateVendorAsync(CreateVendorDto vendorDto);
        Task UpdateVendorAsync(Vendor vendor);
        Task DeleteVendorAsync(string vendorId);
        Task AddRankingAsync(string vendorId, int ranking);
        Task AddCommentAsync(string vendorId, Comment comment);
        Task<Vendor> CalculateAverageRankingAsync(string vendorId);
    }
}
