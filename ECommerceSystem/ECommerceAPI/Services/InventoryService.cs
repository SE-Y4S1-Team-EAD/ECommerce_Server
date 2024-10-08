using ECommerceAPI.Data;
using ECommerceAPI.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly MongoDbContext _context;
        private readonly IProductService _productService; // Inject product service
        private readonly INotificationService _notificationService;

        public InventoryService(MongoDbContext context, IProductService productService, INotificationService notificationService)
        {
            _context = context;
            _productService = productService; // Initialize product service
            _notificationService = notificationService;
        }

        public async Task<Inventory> GetInventoryByProductIdAsync(string productId)
        {
            return await _context.Inventories.Find(i => i.ProductId == productId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Inventory>> GetAllInventoriesAsync()
        {
            return await _context.Inventories.Find(_ => true).ToListAsync();
        }

        public async Task CreateInventoryAsync(Inventory inventory)
        {
            var product = await _productService.GetProductByIdAsync(inventory.ProductId);
            if (product != null)
            {
                // Check if the product has enough stock to fulfill the inventory addition
                if (inventory.StockLevel > product.StockQuantity)
                {
                    // Throw an exception to signal insufficient stock
                    throw new InvalidOperationException("Insufficient product stock to add to inventory.");
                }

                // Update the product stock
                product.StockQuantity -= inventory.StockLevel; // Decrease product stock
                await _productService.UpdateProductAsync(product); // Update product
            }

            await _context.Inventories.InsertOneAsync(inventory);
        }

        public async Task UpdateInventoryAsync(Inventory inventory)
        {
            var existingInventory = await GetInventoryByProductIdAsync(inventory.ProductId);
            if (existingInventory != null)
            {
                // Calculate the difference in stock level
                int stockDifference = inventory.StockLevel - existingInventory.StockLevel;

                // Update the product stock
                var product = await _productService.GetProductByIdAsync(inventory.ProductId);
                if (product != null)
                {
                    // Check if the product stock can handle the adjustment
                    if (stockDifference > product.StockQuantity)
                    {
                        // Throw an exception to signal insufficient stock
                        throw new InvalidOperationException("Insufficient product stock to update inventory.");
                    }

                    // Adjust product stock based on the difference
                    product.StockQuantity -= stockDifference; // Decrease or increase product stock
                    await _productService.UpdateProductAsync(product); // Update product
                }

                existingInventory.StockLevel = inventory.StockLevel;
                existingInventory.LastUpdated = DateTime.UtcNow;
                existingInventory.LowStockThreshold = inventory.LowStockThreshold;

                // Check if the stock is below or equal to the low stock threshold
                if (existingInventory.StockLevel <= existingInventory.LowStockThreshold)
                {
                    existingInventory.IsLowStock = true;

                    // Send a notification to the user/admin about low stock
                    var notification = new Notification
                    {
                        UserId = "66f8f0699f7e5a57c096b5fa", // Specify the user receiving the notification
                        Message = $"Product {product.Name} has low stock: {existingInventory.StockLevel} units remaining.",
                        Type = "Low Stock Alert",
                        IsRead = false,
                        Timestamp = DateTime.UtcNow
                    };

                    await _notificationService.CreateNotificationAsync(notification);
                }

                await _context.Inventories.ReplaceOneAsync(i => i.InventoryId == existingInventory.InventoryId, existingInventory);
            }
        }

        public async Task DeleteInventoryAsync(string inventoryId)
        {
            var inventory = await _context.Inventories.Find(i => i.InventoryId == inventoryId).FirstOrDefaultAsync();
            if (inventory != null)
            {
                var product = await _productService.GetProductByIdAsync(inventory.ProductId);
                if (product != null)
                {
                    product.StockQuantity += inventory.StockLevel; // Restore product stock
                    await _productService.UpdateProductAsync(product); // Update product
                }

                await _context.Inventories.DeleteOneAsync(i => i.InventoryId == inventoryId);
            }
        }
    }
}
