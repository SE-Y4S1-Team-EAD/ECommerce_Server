using ECommerceAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Services
{
    public interface IInventoryService
    {
        Task<Inventory> GetInventoryByProductIdAsync(string productId);
        Task<IEnumerable<Inventory>> GetAllInventoriesAsync();
        Task CreateInventoryAsync(Inventory inventory);
        Task UpdateInventoryAsync(Inventory inventory);
        Task DeleteInventoryAsync(string inventoryId);
    }
}
