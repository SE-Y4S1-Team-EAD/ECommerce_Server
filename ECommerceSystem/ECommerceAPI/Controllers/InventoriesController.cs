using ECommerceAPI.Models;
using ECommerceAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetInventoryByProductId(string productId)
        {
            var inventory = await _inventoryService.GetInventoryByProductIdAsync(productId);
            if (inventory == null) return NotFound();
            return Ok(inventory);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInventories()
        {
            var inventories = await _inventoryService.GetAllInventoriesAsync();
            return Ok(inventories);
        }

        [HttpPost]
        public async Task<IActionResult> CreateInventory([FromBody] Inventory inventory)
        {
            await _inventoryService.CreateInventoryAsync(inventory);
            return CreatedAtAction(nameof(GetInventoryByProductId), new { productId = inventory.ProductId }, inventory);
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateInventory(string productId, [FromBody] Inventory inventory)
        {
            inventory.ProductId = productId; // Ensure productId is set
            await _inventoryService.UpdateInventoryAsync(inventory);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventory(string id)
        {
            await _inventoryService.DeleteInventoryAsync(id);
            return NoContent();
        }
    }
}
