using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ECommerceAPI.Models
{
    public class Inventory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string InventoryId { get; set; }

        [BsonElement("ProductId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; }

        [BsonElement("StockLevel")]
        public int StockLevel { get; set; }

        [BsonElement("LowStockThreshold")]
        public int LowStockThreshold { get; set; } = 10; // Default threshold

        [BsonElement("LastUpdated")]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        [BsonElement("IsLowStock")]
        public bool IsLowStock { get; set; } = false;

        // Additional fields as needed
    }
}
