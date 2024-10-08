using ECommerceAPI.Models;
using ECommerceAPI.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;


namespace ECommerceAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly MongoDbContext _context;
        private readonly IProductService _productService;

        public OrderService(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<Order> GetOrderByIdAsync(string orderId)
        {
            return await _context.Orders.Find(o => o.OrderId == orderId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders.Find(_ => true).ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(string customerId)
        {
            return await _context.Orders.Find(o => o.CustomerId == customerId).ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByVendorIdAsync(string vendorId)
        {
            return await _context.Orders.Find(o => o.Products.Any(p => p.VendorId == vendorId)).ToListAsync();
        }

        public async Task CreateOrderAsync(Order order)
        {
            // Validate each product
            foreach (var orderProduct in order.Products)
            {
                var product = await _productService.GetProductByIdAsync(orderProduct.ProductId);
                if (product == null || !product.IsActive)
                    throw new Exception($"Product with ID {orderProduct.ProductId} is not available.");

                if (product.StockQuantity < orderProduct.Quantity)
                    throw new Exception($"Insufficient stock for product {product.Name}.");

                // Optionally, reduce stock quantity
                product.StockQuantity -= orderProduct.Quantity;
                await _productService.UpdateProductAsync(product);
            }

            order.CreatedAt = System.DateTime.UtcNow;
            order.UpdatedAt = System.DateTime.UtcNow;
            order.Status = OrderStatus.Processing.ToString();
            foreach (var product in order.Products)
            {
                product.DeliveryStatus = DeliveryStatus.Processing.ToString();
            }
            await _context.Orders.InsertOneAsync(order);
        }

        public async Task UpdateOrderStatusAsync(string orderId, OrderStatus newStatus)
        {
            var filter = Builders<Order>.Filter.Eq(o => o.OrderId, orderId);
            var update = Builders<Order>.Update
                .Set(o => o.Status, newStatus.ToString())
                .Set(o => o.UpdatedAt, System.DateTime.UtcNow);

            await _context.Orders.UpdateOneAsync(filter, update);
        }

        public async Task UpdateProductDeliveryStatusAsync(string orderId, string productId, DeliveryStatus newDeliveryStatus)
        {
            var filter = Builders<Order>.Filter.And(
                Builders<Order>.Filter.Eq(o => o.OrderId, orderId),
                Builders<Order>.Filter.ElemMatch(o => o.Products, p => p.ProductId == productId)
            );

            var update = Builders<Order>.Update
                .Set("Products.$.DeliveryStatus", newDeliveryStatus.ToString())
                .Set(o => o.UpdatedAt, System.DateTime.UtcNow);

            await _context.Orders.UpdateOneAsync(filter, update);

            // After updating, check if all products are delivered
            var order = await GetOrderByIdAsync(orderId);
            if (order != null && order.Products.All(p => p.DeliveryStatus == DeliveryStatus.Delivered.ToString()))
            {
                await UpdateOrderStatusAsync(orderId, OrderStatus.Delivered);
            }
            else if (order != null && order.Products.Any(p => p.DeliveryStatus == DeliveryStatus.Delivered.ToString()))
            {
                await UpdateOrderStatusAsync(orderId, OrderStatus.PartiallyDelivered);
            }
        }

        public async Task CancelOrderAsync(string orderId, string cancellationNote)
        {
            var filter = Builders<Order>.Filter.Eq(o => o.OrderId, orderId);
            var update = Builders<Order>.Update
                .Set(o => o.Status, OrderStatus.Cancelled.ToString())
                .Set(o => o.CancellationNote, cancellationNote)
                .Set(o => o.UpdatedAt, System.DateTime.UtcNow);

            await _context.Orders.UpdateOneAsync(filter, update);

            // Optionally, you can add logic to notify vendors/customers about the cancellation
        }



        public async Task DeleteOrderAsync(string orderId)
        {
            var filter = Builders<Order>.Filter.Eq(o => o.OrderId, orderId);
            await _context.Orders.DeleteOneAsync(filter);
        }
    }
}
