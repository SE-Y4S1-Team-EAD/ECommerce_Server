using ECommerceAPI.Data;
using ECommerceAPI.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IMongoCollection<Notification> _notificationCollection;

        public NotificationService(MongoDbContext context)
        {
            _notificationCollection = context.Notifications;
        }

        public async Task<Notification> CreateNotificationAsync(Notification notification)
        {
            await _notificationCollection.InsertOneAsync(notification);
            return notification;
        }

        public async Task<Notification> GetNotificationByIdAsync(string notificationId)
        {
            return await _notificationCollection.Find(n => n.NotificationId == notificationId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(string userId)
        {
            return await _notificationCollection.Find(n => n.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetAllNotificationsAsync()
        {
            return await _notificationCollection.Find(_ => true).ToListAsync();
        }

        public async Task UpdateNotificationAsync(Notification notification)
        {
            var filter = Builders<Notification>.Filter.Eq(n => n.NotificationId, notification.NotificationId);
            await _notificationCollection.ReplaceOneAsync(filter, notification);
        }

        public async Task DeleteNotificationAsync(string notificationId)
        {
            var filter = Builders<Notification>.Filter.Eq(n => n.NotificationId, notificationId);
            await _notificationCollection.DeleteOneAsync(filter);
        }
    }
}
