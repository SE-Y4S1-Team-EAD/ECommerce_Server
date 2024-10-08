using ECommerceAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Services
{
    public interface IOrderService
    {
        Task<Order> GetOrderByIdAsync(string orderId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(string customerId);
        Task<IEnumerable<Order>> GetOrdersByVendorIdAsync(string vendorId);
        Task CreateOrderAsync(Order order);
        Task UpdateOrderStatusAsync(string orderId, OrderStatus newStatus);
        Task UpdateProductDeliveryStatusAsync(string orderId, string productId, DeliveryStatus newDeliveryStatus);
        Task CancelOrderAsync(string orderId, string cancellationNote);
        Task DeleteOrderAsync(string orderId);
    }
}
