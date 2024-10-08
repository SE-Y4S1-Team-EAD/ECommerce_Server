using ECommerceAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Services
{
    public interface INotificationService
    {
        Task<Notification> CreateNotificationAsync(Notification notification);
        Task<Notification> GetNotificationByIdAsync(string notificationId);
        Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(string userId);
        Task<IEnumerable<Notification>> GetAllNotificationsAsync();
        Task UpdateNotificationAsync(Notification notification);
        Task DeleteNotificationAsync(string notificationId);
    }
}
