using ECommerceAPI.Models;
using ECommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;

        public OrdersController(IOrderService orderService, IUserService userService, IProductService productService)
        {
            _orderService = orderService;
            _userService = userService;
            _productService = productService;
        }

        // POST: api/orders
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customerId = GetCurrentUserId();
            var products = createOrderDto.Products.Select(p => new OrderProduct
            {
                ProductId = p.ProductId,
                Quantity = p.Quantity,
                VendorId = p.VendorId,
                DeliveryStatus = p.DeliveryStatus
            }).ToList();

            var order = new Order
            {
                CustomerId = customerId,
                Products = products,
                Status = OrderStatus.Processing.ToString(),
                CreatedAt = System.DateTime.UtcNow,
                UpdatedAt = System.DateTime.UtcNow
            };

            await _orderService.CreateOrderAsync(order);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
        }

        // GET: api/orders
        [Authorize(Roles = "Administrator,CSR,Vendor,Customer")]
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = GetCurrentUserId();

            IEnumerable<Order> orders;

            if (userRole == "Administrator" || userRole == "CSR")
            {
                orders = await _orderService.GetAllOrdersAsync();
            }
            else if (userRole == "Customer")
            {
                orders = await _orderService.GetOrdersByCustomerIdAsync(userId);
            }
            else if (userRole == "Vendor")
            {
                orders = await _orderService.GetOrdersByVendorIdAsync(userId);
            }
            else
            {
                return Forbid();
            }

            return Ok(orders);
        }

        // GET: api/orders/{id}
        [Authorize(Roles = "Administrator,CSR,Vendor,Customer")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(string id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = GetCurrentUserId();

            if (userRole == "Customer" && order.CustomerId != userId)
                return Forbid();

            if (userRole == "Vendor" && !order.Products.Any(p => p.VendorId == userId))
                return Forbid();

            return Ok(order);
        }

        // PUT: api/orders/{id}/status
        [Authorize(Roles = "Administrator,CSR")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(string id, [FromBody] UpdateOrderStatusDto updateOrderStatusDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            // Validate status transition
            if (!Enum.TryParse<OrderStatus>(updateOrderStatusDto.Status, out var newStatus))
                return BadRequest(new { message = "Invalid order status." });

            await _orderService.UpdateOrderStatusAsync(id, newStatus);
            return NoContent();
        }

        // PUT: api/orders/{id}/products/{productId}/delivery-status
        [Authorize(Roles = "Vendor,Administrator,CSR")]
        [HttpPut("{id}/products/{productId}/delivery-status")]
        public async Task<IActionResult> UpdateProductDeliveryStatus(string id, string productId, [FromBody] UpdateDeliveryStatusDto updateDeliveryStatusDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = GetCurrentUserId();

            if (userRole == "Vendor")
            {
                // Ensure the vendor is associated with the product
                if (!order.Products.Any(p => p.ProductId == productId && p.VendorId == userId))
                    return Forbid();
            }

            if (!Enum.TryParse<DeliveryStatus>(updateDeliveryStatusDto.DeliveryStatus, out var newDeliveryStatus))
                return BadRequest(new { message = "Invalid delivery status." });

            await _orderService.UpdateProductDeliveryStatusAsync(id, productId, newDeliveryStatus);
            return NoContent();
        }

        // DELETE: api/orders/{id}
        [Authorize(Roles = "Customer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelOrder(string id, [FromBody] CancelOrderDto cancelOrderDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            var userId = GetCurrentUserId();
            if (order.CustomerId != userId)
                return Forbid();

            if (order.Status == OrderStatus.Delivered.ToString() || order.Status == OrderStatus.PartiallyDelivered.ToString())
                return BadRequest(new { message = "Cannot cancel order after dispatch." });

            await _orderService.CancelOrderAsync(id, cancelOrderDto.CancellationNote);
            return NoContent();
        }

        // Helper method to get current user's ID from JWT claims
        private string GetCurrentUserId()
        {
            return User.FindFirst("id")?.Value;
        }

        public class CreateOrderDto
        {
            [Required]
            public string CustomerId { get; set; }

            [Required]
            public List<OrderProductDto> Products { get; set; } = new List<OrderProductDto>();

            [Required]
            [StringLength(50, MinimumLength = 3)]
            public string Status { get; set; } // e.g., "Processing", "Delivered", "Cancelled", "Partially Delivered"

            // Optional fields
            public string CancellationNote { get; set; }
        }

        public class OrderProductDto
        {
            [Required]
            public string ProductId { get; set; }

            [Required]
            [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
            public int Quantity { get; set; }

            [Required]
            public string VendorId { get; set; }

            public string DeliveryStatus { get; set; } // e.g., "Processing", "Ready", "Delivered"
        }

        public class UpdateOrderStatusDto
        {
            [Required]
            public string Status { get; set; } // e.g., "Processing", "Delivered", "Cancelled", "Partially Delivered"
        }

        public class UpdateDeliveryStatusDto
        {
            [Required]
            public string DeliveryStatus { get; set; } // e.g., "Processing", "Ready", "Delivered"
        }

        public class CancelOrderDto
        {
            public string CancellationNote { get; set; }
        }
    }
}
