using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models
{
    public class Inventory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string InventoryId { get; set; }

        [BsonElement("ProductId")]
        [Required]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; }

        [BsonElement("StockLevel")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock level cannot be negative.")]
        public int StockLevel { get; set; }

        [BsonElement("LowStockThreshold")]
        [Range(1, int.MaxValue, ErrorMessage = "Threshold must be at least 1.")]
        public int LowStockThreshold { get; set; }

        [BsonElement("IsLowStock")]
        public bool IsLowStock { get; set; } = false;

        [BsonElement("LastUpdated")]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        [BsonElement("ReservedStock")]
        [Range(0, int.MaxValue, ErrorMessage = "Reserved stock cannot be negative.")]
        public int ReservedStock { get; set; } = 0;
    }
}
