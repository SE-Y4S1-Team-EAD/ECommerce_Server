using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models
{
    public class Notification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string NotificationId { get; set; }

        [BsonElement("UserId")]
        [Required]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } // Reference to the User receiving the notification

        [BsonElement("Message")]
        [Required]
        public string Message { get; set; }

        [BsonElement("Type")]
        [Required]
        public string Type { get; set; } // e.g., "Order Update", "Promotion", "Reminder"

        [BsonElement("IsRead")]
        public bool IsRead { get; set; } = false; // Indicates whether the notification has been read

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; } // Add this line
    }
}
