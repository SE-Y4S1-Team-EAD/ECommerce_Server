using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string OrderId { get; set; }

        [BsonElement("CustomerId")]
        [Required]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CustomerId { get; set; }

        [BsonElement("Products")]
        [Required]
        public List<OrderProduct> Products { get; set; } = new List<OrderProduct>();

        [BsonElement("Status")]
        [Required]
        [EnumDataType(typeof(OrderStatus))]
        public string Status { get; set; } // e.g., "Processing", "Delivered", "Cancelled", "Partially Delivered"

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("CancellationNote")]
        public string CancellationNote { get; set; }
    }

    public class OrderProduct
    {
        [BsonElement("ProductId")]
        [Required]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; }

        [BsonElement("Quantity")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [BsonElement("VendorId")]
        [Required]
        [BsonRepresentation(BsonType.ObjectId)]
        public string VendorId { get; set; }

        [BsonElement("DeliveryStatus")]
        [EnumDataType(typeof(DeliveryStatus))]
        public string DeliveryStatus { get; set; } // e.g., "Processing", "Ready", "Delivered"
    }

    public enum OrderStatus
    {
        Processing,
        PartiallyDelivered,
        Delivered,
        Cancelled
    }

    public enum DeliveryStatus
    {
        Processing,
        Ready,
        Delivered
    }
}
